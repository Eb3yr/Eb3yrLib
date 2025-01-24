using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Eb3yrLib.Extensions
{
	public static class NumericIListExt
	{
		/// <summary>In-place arithmetic on an IList<TNumber></summary>
		/// <param name="arr"></param>
		/// <param name="multiplier"></param>
		public static void Mult<T, H>(this IList<T> arr, H multiplier) where T : IMultiplyOperators<T, H, T>
		{
			for (int i = 0; i < arr.Count; i++)
				arr[i] *= multiplier;
		}

		/// <summary>In-place arithmetic on an IList<TNumber></summary>
		/// <param name="arr"></param>
		/// <param name="divider"></param>
		public static void Div<T, H>(this IList<T> arr, H divider) where T : IDivisionOperators<T, H, T>
		{
			for (int i = 0; i < arr.Count; i++)
				arr[i] /= divider;
		}

		/// <summary>In-place arithmetic on an IList<TNumber></summary>
		/// <param name="arr"></param>
		/// <param name="adder"></param>
		public static void Add<T, H>(this IList<T> arr, H adder) where T : IAdditionOperators<T, H, T>
		{
			for (int i = 0; i < arr.Count; i++)
				arr[i] += adder;
		}

		/// <summary>In-place arithmetic on an IList<TNumber></summary>
		/// <param name="arr"></param>
		/// <param name="subtractor"></param>
		public static void Sub<T, H>(this IList<T> arr, H subtractor) where T : ISubtractionOperators<T, H, T>
		{
			for (int i = 0; i < arr.Count; i++)
				arr[i] -= subtractor;
		}
	}
}
