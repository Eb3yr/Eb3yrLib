using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Benchmarks
{
	[MemoryDiagnoser(true)]
	public class Benchmarks
	{
		string str;
	
		public char value => str[str.Length - 1];   // Most possible work to reach, excl. duplicates
	
		[Params(8, 64, 2048, 16384)]
		public int Length;
	
		[GlobalSetup]
		public void GlobalSetup()
		{
			Span<char> cc = new char[Length];
			for (ushort i = 0; i < Length; i++)
				cc[i] = (char)i;
	
			// Add some duplicates
			//Random rng = new(0);
			//for (int j = 0; j < Length / 32 + rng.Next(2); j++)
			//	cc[rng.Next(0, Length)] = (char)rng.Next(0, Length);
	
			str = new(cc);
			Debug.Assert(IndexOf() == IndexOfSimd(), "Mismatched results");
		}
	
		[Benchmark(Baseline = true)]
		public int IndexOf()
		{
			return str.IndexOf(value);
		}
	
		[Benchmark]
		public unsafe int IndexOfSimd()
		{
			int i = 0;
			fixed (char* ptr = str)
			{
				if (str.Length > 16)
				{
					Vector<ushort> vec;
					Vector<ushort> values = Vector.Create<ushort>(value);
					for (; i < str.Length - Vector<ushort>.Count; i += Vector<ushort>.Count)
					{
						vec = Unsafe.Read<Vector<ushort>>(ptr + i);
						if (Vector.EqualsAny(vec, values))
						{
							int max = i + Vector<ushort>.Count;
							while (i < max)
							{
								if (ptr[i++] == value ||
									ptr[i++] == value ||
									ptr[i++] == value ||
									ptr[i++] == value)
									return i - 1;
							}
						}
					}
				}

				int remaining = str.Length - i;
				while (remaining > 0)
				{
					switch (remaining & 3)
					{
						case 0:
							if (ptr[i++] == value) return i - 1;
							goto case 3;
						case 3:
							if (ptr[i++] == value) return i - 1;
							goto case 2;
						case 2:
							if (ptr[i++] == value) return i - 1;
							goto case 1;
						case 1:
							if (ptr[i++] == value) return i - 1;
							break;
					}
					remaining -= 4;
				}
			}
			return -1;
		}
	}
}
