using System.Numerics;
using System.Runtime.CompilerServices;

namespace Eb3yrLib
{
	public static class HelperFuncs
	{
		public static T PushFromHalf<T>(T num, T exponent) where T : IFloatingPoint<T> => PushFrom(num, T.One / (T.One + T.One), exponent);
		public static T PushFrom<T>(T num, T from, T exponent) where T : INumber<T>
		{
			// Consider exponents, hell add a func param that lets the user define custom funcs to apply to the result. Do this in another overload
			T diff = num - from;
			int sign = T.Sign(diff);	// CopySign would be better, this just reminds me that T.Sign and T.CopySign exists
			diff = T.Abs(diff);
			

			throw new NotImplementedException("Go away red squiggly lines");
		}
		
	}
}
