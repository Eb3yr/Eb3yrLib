using System.Numerics;

namespace Eb3yrLib.Extensions
{
	public static class ValueTupleExtensions
	{
		extension <T>((T, T) t1) where T : IFloatingPoint<T>
		{
			public (T, T) Add((T, T) t2) => (t1.Item1 +  t2.Item1, t1.Item2 + t2.Item2);
		}
	}
}
