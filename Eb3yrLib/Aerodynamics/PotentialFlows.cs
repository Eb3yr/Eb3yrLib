using MathNet.Numerics;

namespace Eb3yrLib.Aerodynamics
{
	/// <summary>A streamfunction that accepts cartesian x-z coordinates and returns a magnitude. When adding multiple to one delegate, call .Sum() on that delegate after or else only the result of the last-added streamfunction will be returned</summary>
	public delegate double StreamFunction(double x, double z);

	/// <summary>Contains stream functions and methods to use them for potential flow theory.</summary>
	/// <remarks>StreamFunction is a delegate type that represents a function with parameters x and z. Dotnet defines a delegate with multiple delegates added to it to return only the result of the last delegate if they have a return type. It will not return the sum of all of the delegates. For this reason, always call .Sum() on a StreamFunction composed of multiple functions before using it. </remarks>
	public static class PotentialFlows
	{
		const double oneOver2Pi = (0.5 / double.Pi);

		public static StreamFunction Uniform(double Uinf, double alpha = 0) // radians
		{
			return (x, z) => Uinf * double.Cos(alpha) * z - Uinf * double.Sin(alpha) * x;
		}

		/// <summary>Creates a source with the negative of the given strength Q</summary>
		public static StreamFunction Sink(double Q, double x0 = 0, double z0 = 0) => Source(-Q, x0, z0);
		public static StreamFunction Source(double Q, double x0 = 0, double z0 = 0)		// Q = area flow rate (volumetric flow rate per unit length)
		{
			return (x, z) => Q * oneOver2Pi * double.Atan2(z - z0, x - x0);
		}

		public static StreamFunction Doublet(double kappa, double x0 = 0, double z0 = 0)
		{
			return (x, z) =>
			{
				double xDiff = x - x0;
				double zDiff = z - z0;
				return -kappa * oneOver2Pi * zDiff / (xDiff * xDiff + zDiff * zDiff);
			};
		}

		public static StreamFunction Vortex(double gamma, double x0 = 0, double z0 = 0)		// positive gamma = CW rotation
		{
			return (x, z) => gamma * oneOver2Pi * double.Log(Radius(x - x0, z - z0));
		}

		public static StreamFunction Rankine(double Uinf, double lambda, double b, double x0 = 0, double z0 = 0)
		{
			// Would like to do an extension with angle alpha, but I'm really not sure quite how that'll work
			return (Uniform(Uinf) + Source(lambda, x0 + b, z0) + Sink(lambda, x0 - b, z0)).Sum();	// Verify Source and Sink are offset correctly
		}

		public static StreamFunction Cylinder(double Uinf, double kappa, double gamma = 0, double x0 = 0, double z0 = 0)
		{
			StreamFunction sf = Uniform(Uinf);
			sf += Doublet(kappa, x0, z0);
			if (gamma != 0)	// If gamma = 0 then no lift, can remove vortex
				sf += Vortex(gamma, x0, z0);

			sf.Sum();
			return sf;
		}

		#region other
		private static double Radius(double x, double z) => double.Sqrt(x * x + z * z);

		/// <summary>Get the cartesian velocity function from a cartesian stream function</summary>
		public static Func<double, double, (double u, double w)> GetVelocityFunc(this StreamFunction func)
		{
			var _f = new Func<double, double, double>(func);
			var dfdx = Differentiate.FirstPartialDerivative2Func(_f, 0);
			var dfdz = Differentiate.FirstPartialDerivative2Func(_f, 1);
			return (x, z) => (dfdz(x, z), -dfdx(x, z));	// u = dphi/dz, w = -dphi/dx. 
		}

		public static (double x, double z)[,] CoordGrid(double xMin, double xMax, double zMin, double zMax, double dx, double dz)
		{
			int iMax = (int)double.Ceiling((xMax - xMin) / dx);
			int jMax = (int)double.Ceiling((zMax - zMin) / dz);
			(double x, double z)[,] arr = new (double x, double z)[iMax + 1, jMax + 1];

			for (int i = 0; i <= iMax; i++)
				for (int j = 0; j <= jMax; j++)
					arr[i, j] = (xMin + i * dx, zMin + j * dz);

			return arr;
		}

		public static double[,] StreamGrid((double x, double z)[,] meshGrid, StreamFunction func)
		{
			double[,] stream = new double[meshGrid.GetLength(0), meshGrid.GetLength(1)];
			for (int i = 0; i < meshGrid.GetLength(0); i++)
			{
				for (int j = 0; j < meshGrid.GetLength(1); j++)
				{
					(double x, double z) val = meshGrid[i, j];
					stream[i, j] = func(val.x, val.z);
				}
			}
			return stream;
		}

		public static (double u, double w)[,] VelocityGrid((double x, double z)[,] meshGrid, StreamFunction func)
		{
			var velFunc = GetVelocityFunc(func);
			var velGrid = new (double u, double w)[meshGrid.GetLength(0), meshGrid.GetLength(1)];

			(double x, double z) val;
			for (int i = 0; i < meshGrid.GetLength(0); i++)
				for (int j = 0; j < meshGrid.GetLength(1); j++)
				{
					val = meshGrid[i, j];
					velGrid[i, j] = (velFunc(val.x, val.z));
				}

			return velGrid;
		}

		/// <summary>Combine the functions within the invocation list of the given StreamFunction such that invoking the returned StreamFunction will return the sum of each function added to that delegate, rather than the result of invoking only the last member of the invocation list. This must be invoked on a stream function delegate with more than one function added to the invocation list.</summary>
		public static StreamFunction Sum(this StreamFunction sf)
		{
			return (double x, double z) =>
			{
				double sum = 0;
				foreach (StreamFunction s in (sf.GetInvocationList() as StreamFunction[])!)
					sum += s(x, z);

				return sum;
			};
		}
		#endregion
	}
}
