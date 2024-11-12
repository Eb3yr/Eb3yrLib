using System.Numerics;
using System.Runtime.CompilerServices;

namespace Eb3yrLib
{
	public static class HelperFuncs
	{
		public static T PushFromHalf<T>(T num, T exponent) where T : INumber<T> => PushFrom(num, (T)Convert.ChangeType(0.5d, typeof(T), System.Globalization.CultureInfo.InvariantCulture), exponent);
		public static T PushFrom<T>(T num, T from, T exponent) where T : INumber<T>
		{
			// Consider exponents, hell add a func param that lets the user define custom funcs to apply to the result. Do this in another overload
			T diff = num - from;
			bool positive = diff > T.Zero;
			diff = T.Abs(diff);


			throw new NotImplementedException("Go away red squiggly lines");
		}
		
	}
}
