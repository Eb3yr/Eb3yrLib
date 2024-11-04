using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Eb3yrLib.Kinematics
{
	/// <summary>Static class that contains methods for inverse kinematics</summary>
	public static class Kinematics
	{
		/// <summary>
		/// An analytical solution to the inverse kinematics problem where there are two joints and one end effector.
		/// </summary>
		/// <param name="joints">IList of joint positions in 2D space, including the end effector</param>
		/// <param name="target">Target position</param>
		/// <returns>IList of new joint positions</returns>
		public static IList<Vector2> Analytical2D(IList<Vector2> joints, Vector2 target)
		{
			throw new NotImplementedException("Some funky behaviour, WIP.");
			if (joints.Count == 3)
			{
				float la = Vector2.Distance(joints[1], joints[0]);
				float lb = Vector2.Distance(joints[2], joints[1]);
				float at = Vector2.Distance(target, joints[0]);
				if (at == 0)	// avoid divide-by-zeroes if the target overlaps the root
				{
					joints[1] = joints[0] + la * Vector2.UnitX;
					joints[2] = target;
				}
				else if (at > la + lb)   // out of range
				{
					Debug.WriteLine("out of range");
					Vector2 unitAT = Vector2.Normalize(target - joints[0]);
					joints[1] = joints[0] + unitAT * la;
					joints[2] = joints[1] + unitAT * lb;
				}
				else
				{
					float alpha = float.Acos(0.5f * (la * la + lb * lb - at * at) / (la * lb)); // Range exiting [-1, 1] and giving NaN sometimes. Likewise for gamma.
					if (float.IsNaN(alpha))
						Debug.WriteLine("la: " + la + ", lb: " + lb + ", at: " + at + ", (la * lb): " + (la * lb) + ", top: " + (0.5f * (la * la + lb * lb - at * at)));
					// I have a feeling NaN issues might disappear if the issues allowing the chain to distort are fixed.
					Debug.WriteLine("alpha: " + alpha);
					float gamma = float.Acos((target.Y - joints[0].Y) / at) - 0.5f * (alpha - float.Pi);    // Simplification since we're finding the angle between the unit vertical and |AT|
					Debug.WriteLine("gamma: " + gamma);
					Debug.WriteLine("la: " + la);
					joints[1] = joints[0] + la * new Vector2(float.Cos(gamma), float.Sin(gamma));
					joints[2] = target;
				}
			}
			return joints;
		}

		/// <summary>
		/// Use the FABRIK (forward and backward reaching inverse kinematics) method to solve the inverse kinematics problem
		/// </summary>
		/// <param name="joints">IList of joint positions in 3D space, including the end effector</param>
		/// <param name="target">Target position</param>
		/// <param name="tolerance">Acceptable tolerance to end iterations and return</param>
		public static IList<Vector3> Fabrik(IList<Vector3> joints, Vector3 target, float tolerance = 0.1f)
		{
			float distRootToTarget = Vector3.Distance(joints[0], target);
			float[] di = new float[joints.Count - 1];  // Distance from index joint to next. Used to keep constant lengths between joints
			for (int i = 0; i < joints.Count - 1; i++)
				di[i] = Vector3.Distance(joints[i + 1], joints[i]);

			if (distRootToTarget > di.Sum())    // if unreachable
			{
				for (int i = 0; i < joints.Count - 1; i++)
				{
					float r = Vector3.Distance(joints[i], target);
					float lambda = di[i] / r;	// Cannot overlap target, no risk of divide by zero
					joints[i + 1] = (1f - lambda) * joints[i] + lambda * target;
				}
			}
			else
			{
				Vector3 b = joints[0];
				float dif_A;
				do
				{
					dif_A = Vector3.DistanceSquared(joints[joints.Count - 1], target);	// Micro-optimisation to avoid sqrt every iteration. Reflected in while condition
					// Forwards-reaching:
					joints[joints.Count - 1] = target;
					for (int i = joints.Count - 2; i > -1; i--)
					{
						float r = Vector3.Distance(joints[i + 1], joints[i]);
						if (r == 0) continue;
						float lambda = di[i] / r;
						joints[i] = (1f - lambda) * joints[i + 1] + lambda * joints[i];
					}
					// Backwards-reaching:
					joints[0] = b;
					for (int i = 0; i < joints.Count - 1; i++)
					{
						float r = Vector3.Distance(joints[i + 1], joints[i]);
						if (r == 0) continue;
						float lambda = di[i] / r;
						joints[i + 1] = (1f - lambda) * joints[i] + lambda * joints[i + 1];
					}
				} while (dif_A > tolerance * tolerance);
			}
			return joints;
		}

		/// <summary>Use the FABRIK (forward and backward reaching inverse kinematics) method to solve the inverse kinematics problem</summary>
		/// <param name="joints">IList of joint positions in 2D space, including the end effector</param>
		/// <param name="target">Target position</param>
		/// <param name="tolerance">Acceptable tolerance to end iterations and return</param>
		public static IList<Vector2> Fabrik2D(IList<Vector2> joints, Vector2 target, float tolerance = 0.1f)
		{
			float dist = Vector2.Distance(joints[0], target);
			float[] di = new float[joints.Count - 1];  // Distance from index joint to next
			for (int i = 0; i < joints.Count - 1; i++)
				di[i] = Vector2.Distance(joints[i + 1], joints[i]);

			if (dist > di.Sum())    // if unreachable
			{
				for (int i = 0; i < joints.Count - 1; i++)
				{
					float r = Vector2.Distance(joints[i], target);
					float lambda = di[i] / r;   // Cannot overlap target, no risk of divide by zero
					joints[i + 1] = (1f - lambda) * joints[i] + lambda * target;
				}
			}
			else
			{
				Vector2 b = joints[0];
				float dif_A;
				do
				{
					dif_A = Vector2.DistanceSquared(joints[joints.Count - 1], target);	// Micro-optimisation to avoid sqrt every iteration. Reflected in while condition
					// Forwards-reaching:
					joints[joints.Count - 1] = target;
					for (int i = joints.Count - 2; i > -1; i--)
					{
						float r = Vector2.Distance(joints[i + 1], joints[i]);
						if (r == 0) continue;
						float lambda = di[i] / r;
						joints[i] = (1f - lambda) * joints[i + 1] + lambda * joints[i];
					}
					// Backwards-reaching:
					joints[0] = b;
					for (int i = 0; i < joints.Count - 1; i++)
					{
						float r = Vector2.Distance(joints[i + 1], joints[i]);
						if (r == 0) continue;
						float lambda = di[i] / r;
						joints[i + 1] = (1f - lambda) * joints[i] + lambda * joints[i + 1];
					}
				} while (dif_A > tolerance * tolerance);
			}
			return joints;
		}
	}
}