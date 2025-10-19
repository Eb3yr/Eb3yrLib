using Eb3yrLib.Extensions;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Eb3yrLib.Mathematics
{
    public static class Maths
    {
        /// <summary>Map a linear interpolation of x between two sequential elements of xx onto a linear interpolation between the corresponding elements in fx.</summary>
        /// <param name="x">The x position to find the interpolated value of f(x) from.</param>
        /// <param name="xx">Values to map from, sorted.</param>
        /// <param name="fx">Values to map to, sorted.</param>
        /// <exception cref="ArgumentException">Mismatched span lengths; span lengths of less than two.</exception>
        /// <exception cref="ArgumentOutOfRangeException">x not within the range [xx[0], xx[^1].</exception>
        public static T Lerp<T>(T x, ReadOnlySpan<T> xx, ReadOnlySpan<T> fx) where T : IFloatingPointIeee754<T>
		{
            ArgumentException.ThrowIf(xx.Length != fx.Length, "Mismatched span lengths.");
			ArgumentException.ThrowIf(xx.Length < 2, "Cannot interpolate with less than two elements in span.");

            bool isAscending = xx[1] > xx[0];
            Debug.Assert(xx.IsOrdered(isAscending));
            Debug.Assert(fx.IsOrdered(isAscending));

            int idx = isAscending ? xx.BinarySearch(x)
                                  : xx.BinarySearch(x, Comparer<T>.Create((x, y) => -x.CompareTo(y)));
			if (idx < 0)
				idx = ~idx;
			else return fx[idx];

			if (isAscending)
			{
                ArgumentOutOfRangeException.ThrowIfLessThan(x, xx[0], nameof(x));
                ArgumentOutOfRangeException.ThrowIfGreaterThan(x, xx[^1], nameof(x));
                Debug.Assert(idx > 1 && idx < xx.Length - 1);
				return LerpMap(xx[idx - 1], xx[idx], fx[idx - 1], fx[idx], x);
			}
			else
			{
                ArgumentOutOfRangeException.ThrowIfGreaterThan(x, xx[0], nameof(x));
                ArgumentOutOfRangeException.ThrowIfLessThan(x, xx[^1], nameof(x));
                Debug.Assert(idx > -1 && idx < xx.Length - 2);
				return LerpMap(xx[idx + 1], xx[idx], fx[idx + 1], fx[idx], x);
			}
		}

        /// <summary>Maps a linear interpolation of value between fromA and fromB onto toA and toB.</summary>
        /// <param name="fromA">Lower bound of the first range.</param>
        /// <param name="fromB">Upper bound of the first range.</param>
        /// <param name="toA">Lower bound of the range to be re-mapped upon.</param>
        /// <param name="toB">Upper bound of the range to be re-mapped upon.</param>
        /// <param name="value">A number between fromA and fromB to re-map between toA and toB.</param>
        /// <returns>value re-mapped from the interval [fromA, fromB] to the interval [toA, toB].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T LerpMap<T>(T fromA, T fromB,  T toA, T toB, T value) where T : IFloatingPointIeee754<T>
		{
			Debug.Assert(fromA < fromB);
			Debug.Assert(toA < toB);
			return T.Lerp(toA, toB, InverseLerp(fromA, fromB, value));
		}

        /// <summary>Gets the weight which represents where value falls in the interval [a, b].</summary>
		/// <param name="a">Lower bound of the range.</param>
		/// <param name="b">Upper bound of the range.</param>
		/// <param name="value">A number between within the range [a, b].</param>
        /// <returns>The weighting for which Lerp(a, b, weight) == value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T InverseLerp<T>(T a, T b, T value) where T : IFloatingPointIeee754<T>
		{
			Debug.Assert(a < b);
			return (value - a) / (b - a);
		}
	}
}
