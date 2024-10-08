using MathNet.Numerics;

namespace Eb3yrLib.Maths
{
	public static class FourierSeries
	{
		public static Func<double, double> GetFourierFunc(Func<double, double> func, int precision) => GetResult(func, precision).fourierFunc;
		public static FourierResult GetResult(Func<double, double> func, int precision)
		{
			double a0;
			double[] aa = new double[precision];
			double[] bb = new double[precision];
			double deltaMN;
			Func<double, double> fourierFunc;

			// from notes, I think correct?
			a0 = Integrate.OnClosedInterval(func, -double.Pi, double.Pi) / double.Pi;
			fourierFunc = (double x) => 0.5d * a0;

			double a, b, m, n;	// Do I need m, n? a and b are constants for each nth term, i in this case (should I change that????)
			for (int i = 0; i < precision; i++)
			{
				// Curry together a function here
				// eg fourierFunc = (double x) => fourierFunc(x) + newBitUsingX;
			}

			return new FourierResult(fourierFunc, a0, aa, bb);
		}
	}

	public record FourierResult(Func<double, double> fourierFunc, double a0, double[] aa, double[] bb)
	{
		public Func<double, double> fourierFunc = fourierFunc;
		public double a0 = a0;
		public double[] aa = aa;
		public double[] bb = bb;
	}
}
