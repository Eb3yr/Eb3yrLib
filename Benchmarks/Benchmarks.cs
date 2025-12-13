using BenchmarkDotNet.Attributes;
using System;
using System.Numerics.Tensors;

namespace Benchmarks
{
	[MemoryDiagnoser(true)]
	public class Benchmarks
	{
		// See https://discord.com/channels/143867839282020352/143867839282020352/1449272708947902638 for motivation

		long[] _arr = null!;
		long[][] _arr2 = null!;

		public long[] _arrToAssign = null!;
		public long[][] _arr2ToAssign = null!;

        [Params(1_000_000_000 / 8)]
        public int length;

		[GlobalSetup]
		public void GlobalSetup()
		{
			Random rng = new(0);
			_arr = new long[length];
			for (int i = 0; i < length; i++)
				_arr[i] = rng.Next(-10_000_000, 10_000_000);

			int chunkSize = 48_000 / 8;	// 48kb per arr
			int count = length / chunkSize;
			int rem = length - count * chunkSize;

			_arr2 = new long[rem != 0 ? count + 1 : count][];

			for (int i = 0; i < count; i++)
				_arr2[i] = new long[chunkSize];

			if (rem != 0)
				_arr2[^1] = new long[rem];

			rng = new(0);
			foreach (long[] arr in _arr2)
			{
				for (int i = 0; i < arr.Length; i++)
					arr[i] = rng.Next(-10_000_000, 10_000_000);
			}

			long s1 = UnbatchedSumTensorPrims();
			long s2 = BatchedSumTensorPrims();

			int sumLengths = 0;
			foreach (long[] arr in _arr2)
				sumLengths += arr.Length;

			if (sumLengths != _arr.Length)
				throw new Exception($"Array length mismatch!\n\n_arr length: {_arr.Length}\nsumLengths: {sumLengths}");

			if (s1 != s2)
				throw new Exception($"Count mismatch!\n\ns1: {s1}\ns2: {s2}");
		}

		[Benchmark(Baseline = true)]
		public long UnbatchedSumTensorPrims()
		{
			return TensorPrimitives.Sum(_arr);
		}

		[Benchmark]
		public long BatchedSumTensorPrims()
		{
			long count = 0;
			foreach (long[] arr in _arr2)
				count += TensorPrimitives.Sum(arr);

			return count;
		}

		[Benchmark]
		public long UnbatchedSumAllocateInBenchTensorPrims()
		{
			_arrToAssign = new long[length];
			return TensorPrimitives.Sum(_arr);
		}

		[Benchmark]
		public long BatchedSumAllocateInBenchTensorPrims()
		{
			int chunkSize = 48_000 / 8; // 48kb per arr
			int lenCount = length / chunkSize;
			int rem = length - lenCount * chunkSize;

			_arr2ToAssign = new long[rem != 0 ? lenCount + 1 : lenCount][];

			for (int i = 0; i < lenCount; i++)
				_arr2ToAssign[i] = new long[chunkSize];

			if (rem != 0)
				_arr2ToAssign[^1] = new long[rem];

			long count = 0;
			foreach (long[] arr in _arr2)
				count += TensorPrimitives.Sum(arr);
			
			return count;
		}
	}
}
