namespace Eb3yrLib.Mechanics.Statics
{
	public static class Bending
		{
			/// <remarks>Be wary of the sign of the result, consider whether it's appropriate to retain for your situation</remarks>
			public static double NeutralAxis(double Mz, double My, double Iz, double Iy, double Iyz)
			{
				return double.Atan2((Mz * Iyz - My * Iz), (Mz * Iy - My * Iyz));
			}

			/// <summary>Get the axial stress for the cross section at the given y and z coordinates, with the given section properties and bending moments</summary>
			public static double AxialStress(double z, double y, double Mz, double My, double Iz, double Iy, double Iyz)
			{
				return ((Mz * Iy - My * Iyz) * y + (My * Iz - Mz * Iyz) * z) / (Iy * Iz - Iyz * Iyz);
			}

			/// <summary>Get the z and y coefficients for the axial stress equation, in the form σxx = zCoeff * z + yCoeff * y</summary>
			public static (double zCoeff, double yCoeff) AxialStressCoefficients(double Mz, double My, double Iz, double Iy, double Iyz)
			{
				double div = Iy * Iz - Iyz * Iyz;
				double y = (Mz * Iy - My * Iyz) / div;
				double z = (My * Iz - Mz * Iyz) / div;
				return (z, y);
			}

			// These aren't really usable in the expected sense. Mz and My will be functions of x (axial distance) in reality, so the result of this function should itself be a function of x. However then we'd want some nice symbolics and symbolic integration in C#, which is problematic for all except simple functions (which, in fairness, most moment functions are going to be polynomial). Investigate using a symbolics library later when I have more time
			// Alternatively, indefinite integration that returns a function could be used, but I imagine it could be very hard to serialise
			public static double d2vdx2(double E, double Mz, double My, double Iz, double Iy, double Iyz)	// Deflection downwards
			{
				return -(Mz * Iy - My * Iyz) / (E * (Iy * Iz - Iyz * Iyz));	// The difference between v and w is that the numerator gets swapped about a bit
			}

			// And likewise for d^2w/dx^2
			public static double d2wdx2(double E, double Mz, double My, double Iz, double Iy, double Iyz)	// Deflection sideways
			{
				return -(My * Iz - Mz * Iyz) / (E * (Iy * Iz - Iyz * Iyz));
			}
		}
}