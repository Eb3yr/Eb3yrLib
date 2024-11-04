using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground
{
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
			public static double ThetaOverX(double reynolds_x) => 0.664d / double.Sqrt(reynolds_x);
			public static double DeltaStarOverX(double reynolds_x) => 1.771d / double.Sqrt(reynolds_x);
			public static double LocalSkinFricCoef(double reynolds_x) => ThetaOverX(reynolds_x);
		}
	}

	public static class LaminarPlateEquations
	{

	}

	public static class TurbulentPlateEquations
	{

	}

	public static class GeneralPlateBLEquations
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
