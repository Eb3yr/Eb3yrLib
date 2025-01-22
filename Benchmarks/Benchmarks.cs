using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using Eb3yrLib;
using Eb3yrLib.Extensions;
using Eb3yrLib.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BenchmarkProj
{
	[MemoryDiagnoser(true)]
	public class Benchmarks
	{
		Quaternion q = new();
		Vector4 v4;
		public Benchmarks()
		{

		}

		[Benchmark(Baseline = true)]
		public void AsVector4()
		{
			for (int i = 0; i < 1_000; i++)
			{
				v4 = q.AsVector4();
			}
		}

		[Benchmark]
		public void CloneOfAsVector4AggressiveInlining()
		{
			for (int i = 0; i < 1_000; i++)
			{
				v4 = q.AsVector4AggressiveInlining();
			}
		}

		[Benchmark]
		public void CloneOfAsVector4NoInlining()
		{
			for (int i = 0; i < 1_000; i++)
			{
				v4 = q.AsVectorNoInlining();
			}
		}

		[Benchmark]
		public void CloneOfAsVector4NoAttribute()
		{
			for (int i = 0; i < 1_000; i++)
			{
				v4 = q.AsVector4NoAttribute();
			}
		}

		[Benchmark]
		public unsafe void MemoryMarshalReadWrite()
		{
			Span<byte> s = new byte[sizeof(Quaternion)];
			for (int i = 0; i < 1_000; i++)
			{
				MemoryMarshal.Write(s, q);
				v4 = MemoryMarshal.Read<Vector4>(s);
			}
		}

		[Benchmark]
		public unsafe void MemoryMarshalReadWriteStackAlloc()
		{
			Span<byte> s = stackalloc byte[sizeof(Quaternion)];
			for (int i = 0; i < 1_000; i++)
			{
				MemoryMarshal.Write(s, q);
				v4 = MemoryMarshal.Read<Vector4>(s);
			}
		}

		[Benchmark]
		public void CopyFields()
		{
			for (int i = 0; i < 1_000; i++)
			{
				v4 = new Vector4(q.X, q.Y, q.Z, q.W);
			}
		}

		[Benchmark]
		public unsafe void UnsafeBitCast()	// AsVector4 uses this
		{
			for (int i = 0; i < 1_000; i++)
			{
				v4 = Unsafe.BitCast<Quaternion, Vector4>(q);
			}
		}

		[Benchmark]
		public unsafe void UnsafeReadUnaligned()	// Used by Unsafe.BitCast, along with a check for sizeof(TFrom) == sizeof(TTo)
		{
			for (int i = 0; i < 1_000; i++)
			{
				v4 = Unsafe.ReadUnaligned<Vector4>(ref Unsafe.As<Quaternion, byte>(ref q));
			}
		}

		[Benchmark]
		public unsafe void UnsafeRead()
		{
			for (int i = 0; i < 1_000; i++)
			{
				v4 = Unsafe.Read<Vector4>(Unsafe.AsPointer(ref q));
			}
		}
	}

	// Clones of System.Numerics.Quaternion.Extensions Vector.AsVector4(this Quaternion value), without its Intrinsic 
	public static class VectorExt
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 AsVector4AggressiveInlining(this Quaternion value)
		{
#if MONO
            return Unsafe.As<Quaternion, Vector4>(ref value);
#else
			return Unsafe.BitCast<Quaternion, Vector4>(value);
#endif
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Vector4 AsVectorNoInlining(this Quaternion value)
		{
#if MONO
            return Unsafe.As<Quaternion, Vector4>(ref value);
#else
			return Unsafe.BitCast<Quaternion, Vector4>(value);
#endif
		}

		public static Vector4 AsVector4NoAttribute(this Quaternion value)
		{
#if MONO
            return Unsafe.As<Quaternion, Vector4>(ref value);
#else
			return Unsafe.BitCast<Quaternion, Vector4>(value);
#endif
		}
	}
}
