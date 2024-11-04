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
		public static T LinearInterpolate<T>(T x, IList<T> xx, IList<T> fx) where T : INumber<T>
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
				if (xxfx[i].x > x)
				{
					T dydx = (xxfx[i].f - xxfx[i - 1].f) / (xxfx[i].x - xxfx[i - 1].x);
					return xxfx[i - 1].f + dydx * (x - xxfx[i - 1].x);
				}
			}

			throw new ArithmeticException("Unable to interpolate. x may be out of range");
		}



	}
}
