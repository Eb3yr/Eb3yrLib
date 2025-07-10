using Eb3yrLib.Extensions;
using Eb3yrLib.Mathematics;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eb3yrLib.Aerodynamics
{
	public static class BoundaryLayer
	{
		/// <summary>Calculate the properties of a flat plate's boundary layer from a velocity profile</summary>
		/// <param name="u">Ordered list of velocities</param>
		/// <param name="y">Ordered list of distances to the plate</param>
		/// <param name="mu"></param>
		/// <param name="rho"></param>
		/// <returns>A tuple of freestream velocity, y position where U/Uinf = 0.99, wall shear stress, skin friction coefficient, displacement thickness, momentum thickness, and the ratio of displacement to momentum thickness</returns>
		/// <remarks>Mu can be given a nonsense value if only displacement or momentum thicknesses are desired</remarks>
		public static (double Uinf, double delta, double tau_w, double Cf, double dispt, double momt, double h) Properties(double[] u, double[] y, double mu, double rho = 1.225)
		{
			double Uinf = u.Max();
			var uOverUinf = u.Select(i => i / Uinf).ToArray();
			double delta = Maths.LerpSorted(0.99, uOverUinf, y);
			double tau_w = 1000 * mu * (u[1] - u[0]) / (y[1] - y[0]);
			double Cf = tau_w / (0.5 * rho * Uinf * Uinf);

			double[] ones = uOverUinf.Select(x => 1d - x).ToArray();

			double dispt = Mathematics.Integrate.Trapz(ones, y);

			for (int i = 0; i < ones.Length; i++)
				ones[i] *= uOverUinf[i];

			double momt = Mathematics.Integrate.Trapz(ones, y);
			double h = dispt / momt;

			return (Uinf, delta, tau_w, Cf, dispt, momt, h);
		}

		public static class PlateBLFlowProfiles
		{
			public static class Blasius
			{
				/// <summary>
				/// Blasius's proposed parabolic solution to the laminar boundary layer profile.
				/// </summary>
				/// <param name="yOverDelta">y/δ, the dimensionless ratio of distance from the surface to boundary layer thickness</param>
				/// <returns>U / U_infinity, the ratio of flow velocity at y to the freestream flow velocity</returns>
				public static double Laminar(double yOverDelta)
				{
					return 2d * yOverDelta - (yOverDelta * yOverDelta);
				}
				// Go find the more suitable name for these ratios
				public static double ThetaOverX(double reynolds_x) => 0.664d / double.Sqrt(reynolds_x); // momentum thickness / x
				public static double DeltaStarOverX(double reynolds_x) => 1.771d / double.Sqrt(reynolds_x); // displacement thickness / x
				public static double LocalSkinFricCoef(double reynolds_x) => ThetaOverX(reynolds_x);
			}
		}

		public static class LaminarPlate
		{

		}

		public static class TurbulentPlate
		{

		}

		public static class GeneralPlate
		{
			/// <summary>
			/// Calculate the total viscous drag coefficient C_F; not to be confused with the local viscous drag coefficient C_f. The same underlying equation as ViscousDragCoefficient(), but with dragPerUnitSpan in place of drag and chordLength in place of surfaceArea.
			/// </summary>
			/// <param name="dragPerUnitSpan">Drag per unit span of the plate, D'</param>
			/// <param name="density">Fluid density, ρ</param>
			/// <param name="freestreamVelocity">Velocity of the stream flow, U_∞</param>
			/// <param name="chordLength">Length of the plate, L or c</param>
			/// <returns>Viscous drag coefficient, C_F</returns>
			public static double ViscousDragCoefficient_UnitSpan(double dragPerUnitSpan, double density, double freestreamVelocity, double chordLength)
			{
				return dragPerUnitSpan / (0.5d * density * freestreamVelocity * freestreamVelocity * chordLength);  // 0.5d in denominator
			}
			/// <summary>
			/// Calculate the total viscous drag coefficient C_F; not to be confused with the local viscous drag coefficient C_f. The same underlying equation as ViscousDragCoefficient_UnitSpan(), but with drag in place of of dragPerUnitSpan and surfaceArea in place of chordLength.
			/// </summary>
			/// <param name="drag">Total drag of the plate, D</param>
			/// <param name="density">Fluid density, ρ</param>
			/// <param name="freestreamVelocity">Velocity of the stream flow, U_∞</param>
			/// <param name="surfaceArea">Surface area of the plate, s</param>
			/// <returns>Viscous drag coefficient, C_F</returns>
			public static double ViscousDragCoefficient(double drag, double density, double freestreamVelocity, double surfaceArea)
			{
				return drag / (0.5d * density * freestreamVelocity * freestreamVelocity * surfaceArea);  // 0.5d in denominator
			}

			/// <summary>
			/// Calculates the viscous (skin friction) drag of a plate using fluid density, flow velocity, surface area, and the viscous drag coefficient.
			/// </summary>
			/// <param name="density"></param>
			/// <param name="velocity"></param>
			/// <param name="surfaceArea"></param>
			/// <param name="viscousDragCoefficient"></param>
			/// <returns></returns>
			public static double Drag(double density, double velocity, double surfaceArea, double viscousDragCoefficient)
			{
				return 0.5d * density * velocity * velocity * surfaceArea * viscousDragCoefficient;
			}
		}
	}
}
