using Playground;
using Eb3yrLib;
using System.Diagnostics;
using System.Runtime.InteropServices;

//Stopwatch sw = new();
//int targetFps = 60;
//float interval = 1000f / targetFps;
//float mult = 0.5f;
//Console.WriteLine($"Interval: {interval}");
//Console.WriteLine($"Delay: {(int)(interval * mult)}ms");
//await Task.Delay(500);
//sw.Start();
//
//while (true)
//{
//	await Task.Delay((int)(interval * mult));
//
//	while (sw.ElapsedTicks < interval * 1_000)	// Interval in ms, ticks in 100ns
//	{
//		;	// no-op 
//	}
//	sw.Stop();
//	Console.WriteLine($"{float.Round(sw.ElapsedTicks / 10_000f, 0)}ms");
//	sw.Restart();
//}

//_ = TimingTest.NtQueryTimerResolution(out uint min, out uint max, out uint current);
//Console.WriteLine($"{min}, {max}, {current}");
//Console.WriteLine($"In ms: {min / 10_000f}, {max / 10_000f}, {current / 10_000f}");
//
//TimingTest.QueryTimerResolution(out min, out max, out current);

// uint min, max, current;
// TimingTest.NtTest(out min, out max, out current);
// Console.WriteLine($"{min / 10000f}, {max / 10000f}, {current / 10000f}");

static void NativeExceptionLogger(object sender, UnhandledExceptionEventArgs e)
{
	Console.WriteLine($"Native exception logged:\n{e.ExceptionObject}");	
}

AppDomain.CurrentDomain.UnhandledException += NativeExceptionLogger;

Console.WriteLine(CppDllTest.RetOne());
try
{
	CppDllTest.ThrowRuntime();
}
catch
{
	Console.WriteLine("Caught");
}

try
{
	CppDllTest.ThrowRuntime();
}
catch (System.Runtime.CompilerServices.RuntimeWrappedException ex)
{
	Console.WriteLine(ex.ToString());
}
catch (SEHException ex)
{
	// Gives no inner exception info, we just know that unmanaged code threw
	// Also doesn't cause unmanaged C++ exception destructors to get called - use the catch { } syntax instead 
	Console.WriteLine(ex.ToString());
}
//catch
//{
//	Console.WriteLine("Caught native exception");
//}

Console.WriteLine("Done");
Console.ReadLine();

namespace Playground
{
	public static partial class CppDllTest
	{
		[LibraryImport("TestDll")]	// I don't have to stick an extension on this - avoids conflicts on Linux where .so files are used instead of .dll
		public static partial int RetOne();

		[LibraryImport("TestDll")]
		public static partial void ThrowRuntime();
	}

	// The goal is to make a class that can get info about the timer for any OS
	// Would also like to have a class that utilises this to make as high precision timers as possible for each platform
	// Windows NT is a pain, Linux has some high level timers. x86 has rdtsc() to get the clock counts, but you'd still be burning up the CPU then anyway
	// That still leaves platforms like iOS
	public static partial class TimingTest	// Partial allows use of source gens
	{
		public static void QueryTimerResolution(out uint min, out uint max, out uint current)
		{
			try
			{
				if (OperatingSystem.IsWindows())
				{
					min = max = current = 0;
					NtQueryTimerResolution(out min, out max, out current);
				}
				else
				{
					Yippee();
					min = max = current = 0;
				}
			}
			catch (DllNotFoundException ex)
			{
				Console.WriteLine("Yippee oops");
				min = max = current = 0;
			}
		}

		public static int NtTest(out uint min, out uint max, out uint current) => NtQueryTimerResolution(out min, out max, out current);

		//[DllImport("ntdll.dll", SetLastError = true)]
		[LibraryImport("ntdll.dll", SetLastError = true)]
		private static partial int NtQueryTimerResolution(out uint min, out uint max, out uint current);

		// It only tries to load this the first time it gets invoked. What about if the platform doesn't allow it?
		[DllImport("Woah.dll")]
		public static extern int Yippee();
	}

	public static partial class HighPrecisionTimers
	{

		// !!!!!

		// STOP BEING STUPID
		// 
		// https://stackoverflow.com/questions/13397571/precise-thread-sleep-needed-max-1ms-error





		private static readonly Platform targetPlatform;
		private static readonly uint minResolution, maxResolution, currentResolution;   // Microseconds. min > max. We use the min resolution for sleeping before burning up the rest. 
		private static Stopwatch sw = new();

		static HighPrecisionTimers()
		{
			if (OperatingSystem.IsWindows())
			{
				try
				{
					PlatformTimers.NtQueryTimerResolution(out minResolution, out maxResolution, out currentResolution);
					targetPlatform = Platform.Windows;
					return;
				}
				catch (DllNotFoundException) { }
			}
			
			if (OperatingSystem.IsMacOS())
			{
				throw new PlatformNotSupportedException("Why on Earth would you use MacOS?");
			}
			
			if (OperatingSystem.IsLinux())
			{
				try
				{
					targetPlatform = Platform.Linux;
					// Do the resolutions need initialising? Regardless, we need to check if we can access the uDelay function
					return;
				}
				catch (DllNotFoundException) { }
			}

			targetPlatform = Platform.Fallback;
			minResolution = 20_000; // We're almost certainly gonna be burning up for the rest of our time, no sleeping
			maxResolution = 20_000;
			currentResolution = 20_000;
		}

		public static void DelayMilliseconds(int ms) => DelayMicroseconds(ms * 1000);

		public static void DelayMicroseconds(int us)
		{
			switch (targetPlatform)
			{
				case Platform.Windows:
					goto default;

				case Platform.Linux:
					Linux(us);	// Use that whole kernel uDelay thing
					break;

				default:
					Windows(us);
					break;
			}
		}

		private static void Windows(int us)
		{
			sw.Restart();
			if (us > minResolution)
			{
				int delay = (int)((us % minResolution) * minResolution);
				Thread.Sleep(delay);
				// Rubbish. Can sleep for more than just multiples. The assumption is sleep for at least us and at most us + resolution
			}

			// Burn up the remainder
			while (sw.ElapsedTicks < us * 10_000)
			{
				;
			}
		}

		private static void Linux(long us)
		{

		}

		private static partial class PlatformTimers
		{
			[LibraryImport("ntdll.dll", SetLastError = true)]
			public static partial int NtQueryTimerResolution(out uint min, out uint max, out uint current);
		}

		private enum Platform
		{
			Fallback,	// If everything goes tits up just go back to using Thread.Sleep and burning up the CPU for the remainder of the time
			Windows,
			Linux,
			MacOS,	// Ew
			Console	// Lol
		}
	}
}