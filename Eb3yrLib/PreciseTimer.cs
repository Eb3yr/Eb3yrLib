using System.Diagnostics;
using System.Runtime.InteropServices;
using Eb3yrLib.Extensions;

namespace Eb3yrLib
{
	public sealed class PreciseTimer : IDisposable
	{
		readonly Win32WaitableTimer timer;
		readonly Stopwatch stopwatch;
		readonly uint uCurrentResolution;

		public PreciseTimer()
		{
			timer = new();
			stopwatch = new();
			uCurrentResolution = Win32WaitableTimer.GetCurrentTimerResolution() / 10;
		}

		public bool Wait(long udelay)
		{
			stopwatch.Restart();
			long minusRes = udelay - uCurrentResolution;
			if (minusRes > 0)
				if (!timer.Wait(minusRes))
					return false;

			while (stopwatch.ElapsedMicroseconds < udelay)
				;

			return true;
		}

		public void Dispose()
		{
			timer.Dispose();
		}
	}

	public sealed partial class Win32WaitableTimer : IDisposable
	{
		private readonly Win32WaitableTimerHandle handle;

		public Win32WaitableTimer() => handle = NativeMethods.CreateWaitableTimerExW(0, null, 0x1 | 0x2, 0x1F0003 /*TIMER_ALL_ACCESS*/);

		/// <summary>Waits until the timer signals.</summary>
		/// <param name="udelay">Delay in microseconds.</param>
		/// <returns>Whether setting the timer succeeded and the timer signalled before timing out.</returns>
		public bool Wait(long udelay)
		{
			if (udelay > 0)
				udelay = -udelay;

			long timeoutMilli = 10 + udelay / 1000;
			udelay *= 10;   // Convert us into 100s of nanoseconds

			if (!NativeMethods.SetWaitableTimerEx(handle, ref udelay, 0, 0, 0, 0, 0))
				return false;

			uint res = NativeMethods.WaitForSingleObject(handle, (uint)timeoutMilli);

			return res switch
			{
				0x00000000 => true,    // object signalled
				0x00000102 => false,   // time-out interval elapsed without the object signalling
				0x00000080 => false,   // WAIT_ABANDONED	(should be unreachable)
				0xFFFFFFFF => false,    // function failed
				_ => throw new UnreachableException("How did we get here?")
			};
		}

		public static void PrintTimerResolution()
		{
			// In 100s of ns
			NativeMethods.NtQueryTimerResolution(out uint min, out uint max, out uint current);

			Console.WriteLine($"Min resolution: {min / 10_000f}ms\nMax resolution: {max / 10_000f}ms\nCurrent resolution: {current / 10_000f}ms");

		}

		public static uint GetCurrentTimerResolution()
		{
			NativeMethods.NtQueryTimerResolution(out _, out _, out uint current);
			return current;
		}

		public void Dispose() => handle.Dispose();

		private static partial class NativeMethods
		{
			// WaitableTimerExW lets us use the high precision timer. NtQueryTimerResolution gives us a current precision of 1ms, so we overshoot by <=1ms
			[LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
			public static partial Win32WaitableTimerHandle CreateWaitableTimerExW(nint lpTimerAttributes, string? lpTimerName, uint dwFlags, uint dwDesiredAccess);

			[LibraryImport("kernel32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static partial bool SetWaitableTimerEx(Win32WaitableTimerHandle handle, ref readonly long lpDueTime, int lperiod, nint pfnCompletionRoutine, nint lpArgToCompletionRoutine, nint WakeContext, uint Tolerabledelay);

			[LibraryImport("kernel32.dll", SetLastError = true)]
			public static partial uint WaitForSingleObject(Win32WaitableTimerHandle handle, uint dwMilliseconds /*Timeout interval*/);

			[LibraryImport("kernel32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static partial bool CloseHandle(nint handle);

			[LibraryImport("ntdll.dll", SetLastError = true)]
			public static partial void NtQueryTimerResolution(out uint MinimumResolution, out uint MaximumResolution, out uint CurrentResolution);
		}

		private sealed class Win32WaitableTimerHandle() : Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid(true)
		{
			protected override bool ReleaseHandle()
			{
				if (!IsInvalid)
					return NativeMethods.CloseHandle(handle);

				return true;
			}
		}
	}
}
