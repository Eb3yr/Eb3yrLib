namespace Eb3yrLib.Mechanics.Statics
{
	// And switch z, y around in args and tuple returns! It's this way because I originally wrote it as Ix, Iy and replace tooled it, but it's likely to cause confusion later!

	/// <summary> A set of solutions for the second moments of area for various shapes.</summary>
	/// <remarks>Unless stated otherwise, assume the second moment of area is taken from the centroid of the shape</remarks>
	public static class SecondMoments
		{
			// https://en.wikipedia.org/wiki/List_of_second_moments_of_area

			public static (double Iz, double Iy, double J) Circle(double r)
			{
				double r2 = r * r;
				double val = double.Pi * 0.25 * (r2 * r2);
				return (val, val, val * 2);
			}

			/// <returns>Polar second moment of area J of the circular section</returns>
			public static double CirclePolar(double r)
			{
				double r2 = r * r;
				return double.Pi * 0.5 * r2 * r2;
			}

			public static (double Iz, double Iy, double J) HollowCircle(double rInner, double rOuter)
			{
				double rI2 = rInner * rInner;
				double rO2 = rOuter * rOuter;
				double val = double.Pi * 0.25 * (rI2 * rI2 - rO2 * rO2);
				return (val, val, val * 2);
			}

			/// <remarks>Symmetric about the z (horizontal right) azis.</remarks>
			public static double /*Iz*/ Sector(double r, double theta)
			{
				theta = double.Abs(theta) % 2 * double.Pi;
				double _r = r * r;
				return (theta - double.Sin(theta)) * _r * _r * 0.125;
			}

			public static (double Iz, double Iy) SemiCircle(double r)
			{
				double _r = r * r;
				return (
					(double.Pi * 0.125 - (8 / 9) * double.Pi) * _r * _r,
					double.Pi * 0.125 * _r * _r
					);
			}

			public static (double Iz, double Iy) Ellipse(double za, double yb)
			{
				double ab = double.Pi * 0.25 * za * yb;
				return (
					ab * yb * yb,
					ab * za * za
					);
			}

			public static (double Iz, double Iy) Rect(double zb, double yh)
			{
				double bh = zb * yh / 12;
				return (
					bh * yh * yh,
					bh * zb * zb
					);
			}

			public static (double Iz, double Iy) Square(double a)
			{
				double a2 = a * a;
				double I = a2 * a2 / 12;
				return (I, I);
			}

			/// <summary>An L-beam where both legs are equally long and thick.</summary>
			public static (double Iz, double Iy, double Iyz) EqualLegAngle(double L, double t)
			{				double L2 = L * L;	// These help reduce visual clutter and slightly optimise multiplications
				double t2 = t * t;
				double IzIy = (t * (5 * L2 - 5 * L * t + t2) * (L2 - L * t + t2)) / (12 * (2 * L - t));
				double Iyz = (L2 * t * (L - t) * (L - t)) / (4 * (t - 2 * L));
				return (IzIy, IzIy, Iyz);
			}
			
			/// <summary>Determine the second moment of area around a new azis</summary>
			/// <param name="I">Moment of inertia relative to the centroid</param>
			/// <param name="A">Area of the section</param>
			/// <param name="d">Perpendicular distance to the parallel azis</param>
			/// <returns>Moment of inertia relative to the given parallel azis</returns>
			public static double ParallelAxis(double I, double A, double d) => I + A * d * d;

			public static double ParallelAxis(double Iyz, double A, double dz, double dy) => Iyz + A * dz * dy;

			public static (double Iz, double Iy) ParallelAxis(double Iz, double Iy, double A, double dz, double dy) => (ParallelAxis(Iz, A, dy), ParallelAxis(Iy, A, dz));  // VERY deliberately swapped dy and dz. dy is the distance in the vertical axis that it's displaced by, which is the distance the parallel axis to z will be moved. Beware!

			public static (double Iz, double Iy, double Iyz) ParallelAxis(double Iz, double Iy, double Iyz, double A, double dz, double dy) => (ParallelAxis(Iz, A, dy), ParallelAxis(Iy, A, dz), ParallelAxis(Iyz, A, dz, dy));

			public static (double IzPrime, double IyPrime, double IyzPrime, double theta) PrincipalAxes(double Iz, double Iy, double Iyz)
			{
				double theta = double.Atan2(2 * Iyz, Iz - Iy);
				double twoTheta = 2 * theta;
				double sum = 0.5 * (Iz + Iy);
				double sub = 0.5 * (Iz - Iy) * double.Cos(twoTheta);
				double yz = Iyz * double.Sin(twoTheta);

				return (
					sum + sub + yz,
					sum - sub - yz,
					-0.5 * (Iz - Iy) * double.Sin(twoTheta) + Iyz * double.Cos(twoTheta),
					theta
					);
			}

			public static (double IMax, double IMin) PrincipalAxesMaxMin(double Iz, double Iy, double Iyz)
			{
				double sum = 0.5 * (Iz + Iy);
				double sqrt = double.Sqrt(0.25 * (Iz - Iy) * (Iz - Iy) + Iyz * Iyz);
				return (sum + sqrt, sum - sqrt);
			}
		}
}