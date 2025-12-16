using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Playground
{
    public static partial class FindSetOfInts
    {
		public static void FindSyncTask(string path1, string path2, string pathWriteTo, int batchSize = int.MaxValue)
		{
			static TResult WaitResult<TResult>(ValueTask<TResult> task)
			{
				while (!task.IsCompleted) ; // Stinky spinwaits
				return task.Result;
			}
			static void Wait(ValueTask task)
			{
				while (!task.IsCompleted) ;
				return;
			}

			const int byteBufferLength = 4096;
			const bool useAsync = true;
			using FileStream fs1 = new(path1, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, byteBufferLength, useAsync);
			BitArrays bitArray = new(4_294_967_296L, batchSize);

			Memory<byte> buffer = new byte[byteBufferLength];
			Span<byte> buffer2 = byteBufferLength <= 4096 ? stackalloc byte[byteBufferLength] : new byte[byteBufferLength];
			const long addTo = (long)int.MaxValue + 1;
			ValueTask<int> intTask;

			long numPlusAddTo;
			int bytesRead;
			scoped Span<int> intSpan;
			bytesRead = WaitResult(fs1.ReadAsync(buffer));
			while (bytesRead > 0)
			{
				buffer.Span.CopyTo(buffer2);
				intTask = fs1.ReadAsync(buffer);
				intSpan = MemoryMarshal.Cast<byte, int>(buffer2);
				foreach (int i in intSpan)
				{
					numPlusAddTo = i + addTo;
					bitArray[numPlusAddTo] = true;
				}
				bytesRead = WaitResult(intTask);
			}
			fs1.Dispose();

			using FileStream fs2 = new(path2, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, byteBufferLength, useAsync);
			using FileStream fsOut = new(pathWriteTo, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, byteBufferLength, useAsync);
			List<int> listToWrite = new(byteBufferLength);
			Memory<byte> listMem = listToWrite.AsBytes(listToWrite.Capacity);
			ValueTask writerTask = ValueTask.CompletedTask;

			bytesRead = WaitResult(fs2.ReadAsync(buffer));
			while (bytesRead > 0)
			{
				buffer.Span.CopyTo(buffer2);
				intTask = fs2.ReadAsync(buffer);
				intSpan = MemoryMarshal.Cast<byte, int>(buffer2);
				foreach (int number in intSpan)
				{
					numPlusAddTo = number + addTo;
					if (bitArray[numPlusAddTo] == true)
					{
						listToWrite.Add(number);
						bitArray[numPlusAddTo] = false;
					}
				}

				Wait(writerTask);
				writerTask = fsOut.WriteAsync(listToWrite.Count < listToWrite.Capacity
					? listMem[..(listToWrite.Count * 4)]
					: listMem);

				listToWrite.Clear();
				bytesRead = WaitResult(intTask);
			}
			Wait(writerTask);
		}

		public static void RunSyncTask()
		{
			GC.Collect();
			var sw = Stopwatch.StartNew();
			FindSyncTask(Path1, Path2, PathOut, int.MaxValue);
			sw.Stop();
			Console.WriteLine($"FindSyncTask for batchSize = {int.MaxValue} ran in {sw.ElapsedMilliseconds / 1000}s");
			GC.Collect();
			sw.Restart();
			FindSyncTask(Path1, Path2, PathOut, 64_000);
			sw.Stop();
			Console.WriteLine($"FindSyncTask for batchSize = {64_000} ran in {sw.ElapsedMilliseconds / 1000}s");
		}
	}
}
