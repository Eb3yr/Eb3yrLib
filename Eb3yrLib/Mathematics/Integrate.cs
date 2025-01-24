using MathNet.Numerics;
using System.Numerics;
using Eb3yrLib.Extensions;

namespace Eb3yrLib.Mathematics
{
	public static class CustomIntegrate
	{
		/// <summary>Integrate a set of data points using the trapezium rule. Permits varying distances between x values and an unordered set of x values
		/// <param name="x">An array of x values</param>
		/// <param name="y">An array of f(x) values</param>
		/// <exception cref="ArgumentException"></exception>
		public static double Trapz(double[] x, double[] y)
		{
			if (x.Length != y.Length)
				throw new ArgumentException("Mismatched array lengths");

			if (x.Length < 2)
				throw new ArgumentException("Arrays must be of length two or greater");

			if (!x.IsOrderedAscending())
				Array.Sort(x, y);   // Keep x sorted and re-arrange y so that it matches the new arrangement of x

			double result = 0;
			for (int i = 0; i < x.Length - 1; i++)	// I could do this with IEnumerable because I'm just iterating each one
				result += 0.5 * (y[i + 1] + y[i]) * (x[i + 1] - x[i]);	// 0.5(a+b)h
			
			return result;
		}

		/// <summary>Integrate a set of data points using the trapezium rule. Permits varying distances between x values and an unordered set of x values</summary>
		/// <param name="x">An ordered enumerable of x values of length >= 2</param>
		/// <param name="y">An enumerable of f(x) values of length >= 2</param>
		/// <exception cref="ArgumentException"></exception>
		public static T Trapz<T>(IEnumerable<T> x, IEnumerable<T> y, bool allowUnsorted = false) where T : IFloatingPoint<T>
		{
			if (!x.IsOrderedAscending())
			{
				if (allowUnsorted)
					x.Sort(y);
				else throw new ArgumentException("x is not sorted in ascending order");
			}

			T result = T.Zero;
			T half = (T)Convert.ChangeType(0.5, typeof(T), System.Globalization.CultureInfo.InvariantCulture);

			var xEnum = x.GetEnumerator();
			var yEnum = y.GetEnumerator();
			xEnum.MoveNext();
			yEnum.MoveNext();
			T xPrev = xEnum.Current;
			T yPrev = yEnum.Current;

			while (xEnum.MoveNext() && yEnum.MoveNext())
			{
				result += half * (yEnum.Current + yPrev) * (xEnum.Current - xPrev);
				xPrev = xEnum.Current;
				yPrev = yEnum.Current;
			}
			return result;
		}

		/// <summary>Integrate a set of data points by integrating the function returned by curve fitting the data points to a polynomial</summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="order">Order of the polynomial to fit to</param>
		public static double DiscreteCurveFitPoly(double[]x, double[]y, int order = 3)
		{
			Func<double, double> fx = Fit.PolynomialFunc(x, y, order);
			return Integrate.OnClosedInterval(fx, x.Min(), x.Max());
		}
	}
}
