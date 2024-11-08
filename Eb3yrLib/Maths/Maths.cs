using Eb3yrLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Eb3yrLib.Maths
{
    public static class Maths
    {
		/// <summary>A sum function that accepts start and stop indexes, an interval, and a function to sum</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="start">Start index of the sum</param>
		/// <param name="stop">End index of the sum, inclusive</param>
		/// <param name="interval">Sum interval. One by default</param>
		/// <param name="sumFunction">Function inside the sum. n by default</param>
		/// <returns>The sum of sumFunction's results for each interval between start and stop</returns>
		public static T Sum<T>(int start, int stop, int interval = 1, Func<int, T>? sumFunction = null) where T : INumber<T>
		{
			Func<int, T> sumFunc = sumFunction is null ? (int x) => (T)Convert.ChangeType(x, typeof(T)) : sumFunction;
			T d = T.Zero;
			for (int i = start; i <= stop; i += interval)
				d += sumFunc(i);

			return d;
		}

		/// <summary>
		/// Gets f(x) at the given x position using linear interpolation for two inputted arrays xx and fx of equal lengths. Accepts both sorted and unsorted arrays, so long as xx and fx indexes correspond to one another. 
		/// </summary>
		/// <param name="x">The x position to find the interpolated value of f(x) from</param>
		/// <param name="xx">A collection of x numbers</param>
		/// <param name="fx">A collection of f(x) numbers</param>
		/// <returns>f(x) at the given x position</returns>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="ArithmeticException"></exception>
		public static T LerpUnsorted<T>(T x, IList<T> xx, IList<T> fx) where T : INumber<T>
		{
			if (xx.Count != fx.Count)
				throw new ArgumentException("Arrays xx and fx must not have different lengths");

			if (xx.Count < 2)
				throw new ArgumentException("Arrays xx and fx must have lengths greater than one");

			// Concat xx, fx into one list that can be sorted on with respect to x. 
			List<(T x, T f)> xxfx = [];
			for (int i = 0; i < xx.Count; i++)
				xxfx.Add((xx[i], fx[i]));

			xxfx.Sort(((T x, T f) t1, (T x, T f) t2) => Comparer<T>.Default.Compare(t1.x, t2.x));

			for (int i = 1; i < xxfx.Count; i++)
			{
				if (xxfx[i].x >= x)
				{
					T dydx = (xxfx[i].f - xxfx[i - 1].f) / (xxfx[i].x - xxfx[i - 1].x);
					return xxfx[i - 1].f + dydx * (x - xxfx[i - 1].x);
				}
			}
			throw new ArithmeticException("Unable to interpolate. x may be out of range");
		}

		public static T LerpSorted<T>(T x, IList<T> xx, IList<T> fx) where T : INumber<T>
		{
			// Assume sorted
			if (xx.Count < 2)
				throw new ArgumentException("Arrays xx and fx must have lengths greater than one");

			for (int i = 1; i < (xx.Count >= fx.Count ? xx.Count : fx.Count); i++)
			{
				if (xx[i] >= x)
				{
					T dydx = (fx[i] - fx[i - 1]) / (xx[i] - xx[i - 1]);
					return fx[i - 1] + dydx * (x - xx[i - 1]);
				}
			}
			throw new ArithmeticException("Unable to interpolate. x may be out of range");
		}

		// Twice as fast as LerpSorted, now check unsorted implementations against each other
		public static T IEnumerableLerp<T>(T x, IEnumerable<T> xx, IEnumerable<T> fx) where T : INumber<T>
		{   // Assume xx and fx are sorted, handle case only in Unsorted, or split this off into a separate sorted() and have base Lerp() that decides
			// Can handle mismatched array lengths and 

			// No need for xxfx because I can enumerate both simultaneously and I don't need to sort them here
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

		// Have the ordered check and sorting if it isn't so. 
		public static T IEnumerableLerpUnsorted<T>(T x, IEnumerable<T> xx, IEnumerable<T> fx) where T : INumber<T> => throw new NotImplementedException();



		public static T BubbleSort<T>(IEnumerable<T> e)
		{
			// Store prev val as a local, compare to enumerator.Current, iterate through and bubble. Might need to store many
			// This is probably a shit idea but I'm pretty sure it can be done, albeit very scuffed, using an enumerator
			// The idea is to use a private local function to call until it no longer returns false (unsorted).
			// Do this using out vars - maybe return the enumerable to assign back to e every iteration, with an out boolean to control the do {} while() loop?
			throw new NotImplementedException();
		}
	}
}
