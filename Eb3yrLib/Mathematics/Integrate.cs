using MathNet.Numerics;
using System.Numerics;
using Eb3yrLib.Extensions;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using System.Numerics.Tensors;

namespace Eb3yrLib.Mathematics
{
	public static class Integrate
	{
		/// <summary>Integrate a set of data points using the trapezium rule. Permits varying distances between x values and an unordered set of x values
		/// <param name="x">A span of x values</param>
		/// <param name="y">A span of f(x) values</param>
		/// <exception cref="ArgumentException">Spans have mismatched lengths or lengths of less than two</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe T Trapz<T>(T[] x, T[] y) where T : unmanaged, INumberBase<T> => Trapz(new ReadOnlySpan<T>(x), new ReadOnlySpan<T>(y));

		/// <summary>Integrate a set of data points using the trapezium rule. Permits varying distances between x values and an unordered set of x values
		/// <param name="x">A span of x values</param>
		/// <param name="y">A span of f(x) values</param>
		/// <exception cref="ArgumentException">Spans have mismatched lengths or lengths of less than two</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe T Trapz<T>(Span<T> x, Span<T> y) where T : unmanaged, INumberBase<T> => Trapz((ReadOnlySpan<T>)x, (ReadOnlySpan<T>)y);

		/// <summary>Integrate a set of data points using the trapezium rule. Permits varying distances between x values and an unordered set of x values
		/// <param name="x">A span of x values</param>
		/// <param name="y">A span of f(x) values</param>
		/// <exception cref="ArgumentException">Spans have mismatched lengths or lengths of less than two</exception>
		public static unsafe T Trapz<T>(ReadOnlySpan<T> x, ReadOnlySpan<T> y) where T : unmanaged, INumberBase<T>
		{
			IntegrationHelpers.ThrowIfNotMatchOrLessThanTwo(x, y);

			// Use the unrolled loop implementation if hardware acceleration for type T isn't supported
			if (!Vector.IsHardwareAccelerated || !Vector<T>.IsSupported)
				return TrapzSoftwareFallback(x, y);

			T result = T.Zero;

			// TensorPrimitives implementation is only faster above lengths of ~2048-2560, use Vector<T> otherwise
			if (x.Length < 2560)
			{
				fixed (T* xPtr = x)
				fixed (T* yPtr = y)
				{
					int i = 0;
					int remainder = (x.Length - 1) / Vector<T>.Count;
					while (remainder > 0)
					{
						result += Vector.Sum(
							Vector.Multiply(
								Vector.Add(Vector.Load(yPtr + i), Vector.Load(yPtr + i + 1)),
								Vector.Subtract(Vector.Load(xPtr + i + 1), Vector.Load(xPtr + i))
							)
						);

						remainder -= Vector<T>.Count;
						i += Vector<T>.Count;
					}

					while (i < x.Length - 1)
						result += (yPtr[i + 1] + yPtr[i]) * (xPtr[i + 1] - xPtr[i++]);
				}
			}
			else
			{
				// Avoid heap allocation by re-using a smaller stackalloc destination array
				const int stackLim = 1024;   // Multiple of 16 for vector loading
				int remainder = x.Length;
				int minOfLength = int.Min(x.Length - 1, stackLim);
				Span<T> dest = stackalloc T[minOfLength];
				Span<T> xDiff = stackalloc T[minOfLength];

				int offset = 0;
				while (remainder > stackLim)
				{
					TensorPrimitives.Subtract(x.Slice(1 + offset, stackLim), x.Slice(offset, stackLim), xDiff);
					TensorPrimitives.Add(y.Slice(1 + offset, stackLim), y.Slice(offset, stackLim), dest);
					TensorPrimitives.Multiply(dest, xDiff, dest);
					result += TensorPrimitives.Sum<T>(dest);

					offset += stackLim;
					remainder -= stackLim;
				}

				// We slice the destination spans so that the lengths aren't mismatched
				TensorPrimitives.Subtract(x.Slice(1 + offset, remainder - 1), x.Slice(offset, remainder - 1), xDiff.Slice(0, remainder - 1));
				TensorPrimitives.Add(y.Slice(1 + offset, remainder - 1), y.Slice(offset, remainder - 1), dest.Slice(0, remainder - 1));
				TensorPrimitives.Multiply(dest.Slice(0, remainder - 1), xDiff.Slice(0, remainder - 1), dest.Slice(0, remainder - 1));
				result += TensorPrimitives.Sum<T>(dest.Slice(0, remainder - 1));
			}

			return result / (T.One + T.One);
		}

		/// <summary>Fallback if vectoristaion is unsupported</summary>
		private static unsafe T TrapzSoftwareFallback<T>(ReadOnlySpan<T> x, ReadOnlySpan<T> y) where T : unmanaged, INumberBase<T>
		{
			T result = T.Zero;

			fixed (T* xPtr = x)
			fixed (T* yPtr = y)
			{
				int i = 0;
				int remainder = (x.Length - 1) / 4;

				while (remainder > 0)
				{
					result += (yPtr[i + 1] + yPtr[i]) * (xPtr[i + 1] - xPtr[i++]);
					result += (yPtr[i + 1] + yPtr[i]) * (xPtr[i + 1] - xPtr[i++]);
					result += (yPtr[i + 1] + yPtr[i]) * (xPtr[i + 1] - xPtr[i++]);
					result += (yPtr[i + 1] + yPtr[i]) * (xPtr[i + 1] - xPtr[i++]);
					remainder -= 4;
				}

				while (i < x.Length - 1)
					result += (yPtr[i + 1] + yPtr[i]) * (xPtr[i + 1] - xPtr[i++]);
			}
			result /= (T.One + T.One);

			return result;
		}
	}

	file static class IntegrationHelpers
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ThrowIfNotMatchOrLessThanTwo<T>(ReadOnlySpan<T> x, ReadOnlySpan<T> y)
		{
			if (x.Length != y.Length)
				ThrowHelper.ThrowArgumentException($"Mismatched span lengths. x length: {x.Length}, y length: {x.Length}");

			if (x.Length < 2)
				ThrowHelper.ThrowArgumentException($"Spans must be of length two or greater. x length: {x.Length}, y length: {x.Length}");

			return;
		}
	}
}