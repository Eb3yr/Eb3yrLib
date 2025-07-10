using Eb3yrLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Numerics.Tensors;
using System.Text;
using System.Threading.Tasks;

namespace Eb3yrLib.Mathematics
{
    public static class Maths
    {
		/// <summary>A sum function that accepts start and stop indexes, an interval, and a function to sum</summary>
		/// <param name="start">Start index of the sum</param>
		/// <param name="stop">End index of the sum, inclusive</param>
		/// <param name="interval">Sum interval. One by default</param>
		/// <param name="sumFunction">Function inside the sum. n by default</param>
		/// <returns>The sum of sumFunction's results for each interval between start and stop</returns>
		public static T SumFunc<T>(int start, int stop, int interval = 1, Func<int, T>? sumFunction = null) where T : INumber<T>
		{
			Func<int, T> sumFunc = sumFunction is null ? (int x) => (T)Convert.ChangeType(x, typeof(T), System.Globalization.CultureInfo.InvariantCulture) : sumFunction;
			T d = T.Zero;
			for (int i = start; i <= stop; i += interval)
				d += sumFunc(i);

			return d;
		}

		public static T Sum<T>(ReadOnlySpan<T> span) where T : INumberBase<T>
		{
			if (span.Length < 16)
			{
				T sum = T.Zero;
				foreach (T t in span)
					sum += t;

				return sum;
			}

			return TensorPrimitives.Sum(span);
		}

		/// <summary>Gets f(x) at the given x position using linear interpolation for two inputted enumerables xx and fx of equal lengths. Does not accept unsorted inputs.</summary>
		/// <param name="x">The x position to find the interpolated value of f(x) from</param>
		/// <param name="xx">An enumerable of x numbers</param>
		/// <param name="fx">An enumerable of f(x) numbers</param>
		/// <returns>f(x) at the given x position</returns>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="ArithmeticException"></exception>
		public static T LerpSorted<T>(T x, IEnumerable<T> xx, IEnumerable<T> fx) where T : INumber<T>
		{
			using var xEnum = xx.GetEnumerator();
			using var fEnum = fx.GetEnumerator();
			xEnum.MoveNext();
			fEnum.MoveNext();
			T _x, _f, _xPrev = xEnum.Current, _fPrev = fEnum.Current;
			while (xEnum.MoveNext() && fEnum.MoveNext())
			{
				_x = xEnum.Current;
				if (_x >= x)
				{
					_f = fEnum.Current;
					T dydx = (_f - _fPrev) / (_x - _xPrev);
					return _fPrev + dydx * (x - _xPrev);
				}
				else
				{
					_xPrev = _x;
					_fPrev = fEnum.Current;
				}
			}
			throw new ArithmeticException("Unable to interpolate. x may be out of range, input enumerables may have less than two elements, or may have mismatched lengths");
		}

		/// <summary>
		/// Gets f(x) at the given x position using linear interpolation for two inputted enumerables xx and fx of equal lengths. Accepts both sorted and unsorted enumerables, so long as xx and fx indexes correspond to one another. 
		/// </summary>
		/// <param name="x">The x position to find the interpolated value of f(x) from</param>
		/// <param name="xx">An enumerable of x numbers</param>
		/// <param name="fx">An enumerable of f(x) numbers</param>
		/// <returns>f(x) at the given x position</returns>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="ArithmeticException"></exception>
		public static T Lerp<T>(T x, IEnumerable<T> xx, IEnumerable<T> fx) where T : INumber<T>
		{
			if (!xx.IsOrdered())
			{
				var _xx = xx.ToArray();
				var _fx = fx.ToArray();	
				Array.Sort(_xx, _fx);
				xx = _xx;
				fx = _fx;
			}

			return LerpSorted(x, xx, fx);
		}
	}
}
