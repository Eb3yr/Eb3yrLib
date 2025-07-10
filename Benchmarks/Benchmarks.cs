using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
//using Eb3yrLib;
//using Playground;
using System.Threading.Tasks;

namespace Benchmarks
{
	[MemoryDiagnoser(true)]
	public unsafe class Benchmarks
	{
		private Vector128<float> _vec128;
		// Attempting to stop overzealous optimisations?
		public Vector128<float> Vec128
		{
			[MethodImpl(MethodImplOptions.NoInlining & MethodImplOptions.NoOptimization)]
			get => _vec128;
			set => _vec128 = value;
		}
		private Vector256<float> _vec256;
		public Vector256<float> Vec256
		{
			[MethodImpl(MethodImplOptions.NoInlining & MethodImplOptions.NoOptimization)]
			get => _vec256;
			set => _vec256 = value;
		}

		public Vector128<float> DumpVec128;
		public Vector256<float> DumpVec256;

		[GlobalSetup]
		public void GlobalSetup()
		{
			Random rng = new(0);
			Vec128 = Vector128.Create<float>([rng.NextSingle(), rng.NextSingle(), rng.NextSingle(), rng.NextSingle()]);
			Vec256 = Vector256.Create<float>([rng.NextSingle(), rng.NextSingle(), rng.NextSingle(), rng.NextSingle(), rng.NextSingle(), rng.NextSingle(), rng.NextSingle(), rng.NextSingle()]);

			
			Console.WriteLine("SSE: " + Sse.IsSupported);
			Console.WriteLine("SSE2: " + Sse2.IsSupported);
			Console.WriteLine("SSE3: " + Sse3.IsSupported);
			Console.WriteLine("AVX: " + Avx.IsSupported);
			Console.WriteLine("AVX2: " + Avx2.IsSupported);
			Console.WriteLine("Vec128: " + Vector128.IsHardwareAccelerated + "," + Vector128<float>.IsSupported);
			Console.WriteLine("Vec256: " + Vector256.IsHardwareAccelerated + "," + Vector256<float>.IsSupported);
		}

		// All using 128 floats

		[Benchmark(Baseline = true)]
		public void SseReciprocal128()
		{
			for (int i = 0; i < 1000; i++) DumpVec128 =
			Sse.Reciprocal(Vec128);
		}

		[Benchmark]
		public void Sse2Reciprocal128()
		{
			for (int i = 0; i < 1000; i++) DumpVec128 =
			Sse2.Reciprocal(Vec128);
		}

		[Benchmark]
		public void Sse3Reciprocal128()
		{
			for (int i = 0; i < 1000; i++) DumpVec128 =
			Sse3.Reciprocal(Vec128);
		}

		[Benchmark]
		public void AvxReciprocal256()
		{
			for (int i = 0; i < 1000; i++) DumpVec256 =
			Avx.Reciprocal(Vec256);
		}

		[Benchmark]
		public void Avx2Reciprocal256()
		{
			for (int i = 0; i < 1000; i++) DumpVec256 =
			Avx2.Reciprocal(Vec256);
		}

		[Benchmark]
		public void Vector128OneOverVec()
		{
			for (int i = 0; i < 1000; i++) DumpVec128 =
			Vector128<float>.One / Vec128;
		}

		[Benchmark]
		public void Vector256OneOverVec()
		{
			for (int i = 0; i < 1000; i++) DumpVec256 =
			Vector256<float>.One / Vec256;
		}
	}
}
