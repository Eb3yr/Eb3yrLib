using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Playground
{
    public static partial class FindSetOfInts
    {
		public static async ValueTask FindAsync(string path1, string path2, string pathWriteTo, int batchSize = int.MaxValue, bool useAsync = true)
		{
			const int byteBufferLength = 4096;
			//const bool useAsync = false; // Setting this to true drops throughput by 75%!
			using FileStream fs1 = new(path1, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, byteBufferLength, useAsync);
			BitArrays arr = new(4_294_967_296L, batchSize);

			Memory<byte> buffer = new byte[byteBufferLength];
			Memory<byte> buffer2 = new byte[byteBufferLength];
			const long addTo = (long)int.MaxValue + 1;
			ValueTask<int> intTask;

			long numPlusAddTo;
			int bytesRead;
			scoped Span<int> intSpan;
			bytesRead = await fs1.ReadAsync(buffer);
			while (bytesRead > 0)
			{
				buffer.CopyTo(buffer2);
				intTask = fs1.ReadAsync(buffer);
				intSpan = MemoryMarshal.Cast<byte, int>(buffer2.Span);
				foreach (int i in intSpan)
				{
					numPlusAddTo = i + addTo;
					arr[numPlusAddTo] = true;
				}
				bytesRead = await intTask;
			}
			fs1.Dispose();

			using FileStream fs2 = new(path2, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, byteBufferLength, useAsync);
			using FileStream fsOut = new(pathWriteTo, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, byteBufferLength, useAsync);
			List<int> listToWrite = new(byteBufferLength);
			Memory<byte> listMem = listToWrite.AsBytes(listToWrite.Capacity);
			ValueTask writerTask = ValueTask.CompletedTask;

			bytesRead = await fs2.ReadAsync(buffer);
			while (bytesRead > 0)
			{
				buffer.CopyTo(buffer2);
				intTask = fs2.ReadAsync(buffer);
				intSpan = MemoryMarshal.Cast<byte, int>(buffer2.Span);
				foreach (int number in intSpan)
				{
					numPlusAddTo = number + addTo;
					if (arr[numPlusAddTo] == true)
					{
						listToWrite.Add(number);
						arr[numPlusAddTo] = false;
					}
				}

				await writerTask;
				writerTask = fsOut.WriteAsync(listToWrite.Count < listToWrite.Capacity
					? listMem[..(listToWrite.Count * 4)]
					: listMem);

				listToWrite.Clear();
				bytesRead = await intTask;
			}
		}

		public static async ValueTask RunAsync()
		{
			GC.Collect();
			var sw = Stopwatch.StartNew();
			await FindAsync(Path1, Path2, PathOut, int.MaxValue);
			sw.Stop();
			Console.WriteLine($"FindAsync for batchSize = {int.MaxValue} ran in {sw.ElapsedMilliseconds / 1000}s");
			GC.Collect();
			sw.Restart();
			await FindAsync(Path1, Path2, PathOut, 64_000);
			sw.Stop();
			Console.WriteLine($"FindAsync for batchSize = {64_000} ran in {sw.ElapsedMilliseconds / 1000}s");
		}

		public static async ValueTask RunAsyncFalse()
		{
			GC.Collect();
			var sw = Stopwatch.StartNew();
			await FindAsync(Path1, Path2, PathOut, int.MaxValue, false);
			sw.Stop();
			Console.WriteLine($"FindAsync(useAsync = false) for batchSize = {int.MaxValue} ran in {sw.ElapsedMilliseconds / 1000}s");
			GC.Collect();
			sw.Restart();
			await FindAsync(Path1, Path2, PathOut, 64_000, false);
			sw.Stop();
			Console.WriteLine($"FindAsync(useAsync = false) for batchSize = {64_000} ran in {sw.ElapsedMilliseconds / 1000}s");
		}
	}
	}
}
