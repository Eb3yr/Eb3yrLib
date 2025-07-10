using System;
using System.Collections.Generic;
using System.Text;

namespace Eb3yrLib.Mechanics.Statics
{
	// Thick-walled cylinders and shrink fit
	public class ContinuumMechanics
	{
		/// <summary></summary>
		/// <param name="ri">Inner radius</param>
		/// <param name="ro">Outer radius</param>
		/// <param name="pi">Inner pressure</param>
		/// <param name="po">Outer pressure</param>
		/// <returns>The constant A used in σrr = A - B / (r^2) and σθθ = A + B / (r^2)</returns>
		public static double A(double ri, double ro, double pi, double po)
		{
			double ri2 = ri * ri;
			double ro2 = ro * ro;
			return (pi * ri2 - po * ro2) / (ro2 - ri2);
		}

		/// <summary></summary>
		/// <param name="ri">Inner radius</param>
		/// <param name="ro">Outer radius</param>
		/// <param name="pi">Inner pressure</param>
		/// <param name="po">Outer pressure</param>
		/// <returns>The constant B used in σrr = A - B / (r^2) and σθθ = A + B / (r^2)</returns>
		public static double B(double ri, double ro, double pi, double po)
		{
			double ri2 = ri * ri;
			double ro2 = ro * ro;
			return (pi - po) * ri2 * ro2 / (ro2 - ri2);
		}

		/// <param name="ri">Inner radius</param>
		/// <param name="ro">Outer radius</param>
		/// <returns>The ratio k = ro / ri</returns>
		public static double K(double ri, double ro) => ro / ri;

		///// <summary></summary>
		/// <param name="ri">Inner radius</param>
		/// <param name="ro">Outer radius</param>
		/// <param name="pi">Inner pressure</param>
		/// <param name="po">Outer pressure</param>
		/// <returns>Radial stress σrr as a function of radial position r</returns>
		//public static Func<double, double> RadialStress(double ri, double ro, double pi, double po)
		//{
		//	double _A = A(ri, ro, pi, po);
		//	double _B = B(ri, ro, pi, po);
		//	return (double r) => _A - (_B / (r * r));
		//}
		//
		///// <summary></summary>
		/// <param name="ri">Inner radius</param>
		/// <param name="ro">Outer radius</param>
		/// <param name="pi">Inner pressure</param>
		/// <param name="po">Outer pressure</param>
		/// <returns>Hoop stress σθθ as a function of radial position r</returns>
		//public static Func<double, double> HoopStress(double ri, double ro, double pi, double po)
		//{
		//	double _A = A(ri, ro, pi, po);
		//	double _B = B(ri, ro, pi, po);
		//	return (double r) => _A + (_B / (r * r));
		//}

		// THESE VARY DEPENDING ON THE VERSION BEING USED

		public static class AllPressureCylinder
		{

		}

		public static class InternalPressureCylinder
		{

		}

		public static class ShrinkFit
		{

		}
	}
}
