using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using Eb3yrLib;
using Eb3yrLib.Extensions;
using Eb3yrLib.Maths;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BenchmarkProj
{
	[MemoryDiagnoser(true)]
	public class Benchmarks
	{
		List<int> ten;
		List<int> hundred;
		List<int> thousand;
		List<int> hundredThousand;


		public Benchmarks()
		{
			ten = Enumerable.Range(0, 10).ToList();
			hundred = Enumerable.Range(0, 100).ToList();
			thousand = Enumerable.Range(0, 1000).ToList();
			hundredThousand = Enumerable.Range(0, 100_000).ToList();
		}

		[Benchmark]
		public void RemoveCol10()
		{
			_ = RemoveAtCol(ten, 1);
		}

		[Benchmark]
		public void RemoveEnumerable10()
		{
			_ =RemoveAtEnumerable(ten, 1).ToList();
		}

		[Benchmark]
		public void RemoveWhere10()
		{
			_ = RemoveAtWhere(ten, 1).ToList();
		}

		[Benchmark]
		public void RemoveCol100()
		{
			_ = RemoveAtCol(hundred, 1).ToList();
		}

		[Benchmark]
		public void RemoveEnumerable100()
		{
			_ = RemoveAtEnumerable(hundred, 1).ToList();
		}

		[Benchmark]
		public void RemoveWhere100()
		{
			_ = RemoveAtWhere(hundred, 1).ToList();
		}

		[Benchmark]
		public void RemoveCol1000()
		{
			_ = RemoveAtCol(thousand, 1);
		}
		
		[Benchmark]
		public void RemoveEnumerable1000()
		{
			_ = RemoveAtEnumerable(thousand, 1).ToList();
		}
		
		[Benchmark]
		public void RemoveWhere1000()
		{
			_ = RemoveAtWhere(thousand, 1).ToList();
		}
		
		[Benchmark]
		public void RemoveCol100000()
		{
			_ = RemoveAtCol(hundredThousand, 1);
		}
		
		[Benchmark]
		public void RemoveEnumerable100000()
		{
			_ = RemoveAtEnumerable(hundredThousand, 1).ToList();
		}
		
		[Benchmark]
		public void RemoveWhere100000()
		{
			_ = RemoveAtWhere(hundredThousand, 1).ToList();
		}


		public static List<T> RemoveAtCol<T>(List<T> l, int i)
		{
			return [.. l[..i], .. l[(i + 1)..]];
		}

		public static IEnumerable<T> RemoveAtEnumerable<T>(IEnumerable<T> en, int i)
		{
			var e = en.GetEnumerator();
			int c = 0;
			while (e.MoveNext())
			{
				if (c == i)
					continue;

				c++;
				yield return e.Current;
			}
		}

		public static IEnumerable<T> RemoveAtWhere<T>(IEnumerable<T> en, int i)
		{
			return en.Where((_, index) => index != i);
		}
	}
}
