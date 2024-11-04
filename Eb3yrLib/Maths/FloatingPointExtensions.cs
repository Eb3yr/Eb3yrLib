using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eb3yrLib.Maths
{
	public static class FloatingPointExtensions
	{
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T InverseLerp<T>(this T a, T b, T value) where T : IFloatingPoint<T>
		{
			return (value - a) / (b - a);
		}

		/// <summary>Increments a floating point value by the smallest possible amount</summary>
		/// <returns>The next discretely greater value</returns>
		public static T NextAfter<T>(this T value) where T : IFloatingPoint<T>
		{
			if (!T.IsFinite(value))
				return value;

			throw new NotImplementedException();
			// https://stackoverflow.com/questions/70509323/is-it-possible-to-implement-nextafter-w-o-obtaining-a-binary-representation
		}

		/// <summary>Decrements a floating point value by the smallest possible amount</summary>
		/// <returns>The next discretely smaller value</returns>
		public static T NextBefore<T>(this T value) where T : IFloatingPoint<T>
		{
			throw new NotImplementedException();
		}
	}
}
