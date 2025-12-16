using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Playground
{
    public static partial class FindSetOfInts
    {
		public static void Find(string path1, string path2, string pathWriteTo, int batchSize = int.MaxValue)
		{
			const int byteBufferLength = 4096;
			using FileStream fs1 = new(path1, FileMode.Open);
			BitArrays bitArray = new(4_294_967_296L, batchSize);
			Span<byte> buffer = new byte[byteBufferLength];
			const long addTo = (long)int.MaxValue + 1;

			long numPlusAddTo;
			int bytesRead;
			scoped Span<int> intSpan;
			while (fs1.Position < fs1.Length)
			{
				bytesRead = fs1.Read(buffer);
				intSpan = MemoryMarshal.Cast<byte, int>(buffer);
				foreach (int i in intSpan)
				{
					numPlusAddTo = i + addTo;
					bitArray[numPlusAddTo] = true;
				}
			}
			fs1.Dispose();

			using FileStream fs2 = new(path2, FileMode.Open);
			using FileStream fsOut = new(pathWriteTo, FileMode.OpenOrCreate);
			List<int> listToWrite = new(byteBufferLength);

			while (fs2.Position < fs2.Length)
			{
				bytesRead = fs2.Read(buffer);
				intSpan = MemoryMarshal.Cast<byte, int>(buffer);
				foreach (int number in intSpan)
				{
					numPlusAddTo = number + addTo;
					if (bitArray[numPlusAddTo] == true)
					{
						listToWrite.Add(number);
						bitArray[numPlusAddTo] = false;
					}
				}
				fsOut.Write(MemoryMarshal.Cast<int, byte>(CollectionsMarshal.AsSpan(listToWrite)));
				listToWrite.Clear();
			}
		}

		public static void Run()
		{
			GC.Collect();
			var sw = Stopwatch.StartNew();
			Find(Path1, Path2, PathOut, int.MaxValue);
			sw.Stop();
			Console.WriteLine($"Find for batchSize = {int.MaxValue} ran in {sw.ElapsedMilliseconds / 1000}s");
			GC.Collect();
			sw.Restart();
			Find(Path1, Path2, PathOut, 64_000);
			sw.Stop();
			Console.WriteLine($"Find for batchSize = {64_000} ran in {sw.ElapsedMilliseconds / 1000}s");
		}
	}
}
