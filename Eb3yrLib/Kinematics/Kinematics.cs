using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eb3yrLib.Kinematics
{
	/// <summary>Static class that contains methods for inverse kinematics</summary>
	/// <remarks>Most methods have an in IList parameter. These are modified and behave as ref params. They exist because arrays only implement IList, not IList<T>, and laziness because we can't simply ref an array from a Chain into an IList<Vector2> method overload</remarks>
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
				if (at == 0)    // avoid divide-by-zeroes if the target overlaps the root
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

		/// <summary>Use the FABRIK method to solve the inverse kinematics problem</summary>
		/// <param name="joints">Joint positions in 2D space, including the end effector</param>
		/// <param name="target">Target position</param>
		/// <param name="tolerance">Acceptable tolerance to end iterations and return</param>
		public static unsafe void Fabrik2D(Span<Vector2> joints, Vector2 target, float tolerance = 0.1f)
		{
			throw new NotImplementedException("Go fix the shitty loop here. Vectorise it, and make it not unsafe shit. Span enumerator might turn out to be far faster than a for loop too.");
			// Distance from index joint to next
			Span<float> di = joints.Length < 128 ? stackalloc float[joints.Length - 1] : new float[joints.Length - 1];  // Minimise heap allocations for smaller chains
			// Avoid bounds checks
			fixed (float* ptr = di)
			fixed (Vector2* jptr = joints)
			{
				for (int i = 0; i < joints.Length - 1; i++)
					ptr[i] = Vector2.Distance(jptr[i + 1], jptr[i]);
			}

			Fabrik2D(joints, di, target, tolerance);
		}

		/// <summary>Use the FABRIK method to solve the inverse kinematics problem</summary>
		/// <param name="joints">Joint positions in 2D space, including the end effector</param>
		/// <param name="lengths">Length of each joint. The final joint should have a length of 0 or not exist</param>
		/// <param name="target">Target position</param>
		/// <param name="tolerance">Acceptable tolerance to end iterations and return</param>
		public static unsafe void Fabrik2D(Span<Vector2> joints, ReadOnlySpan<float> lengths, Vector2 target, float tolerance = 0.1f)
		{
			float dist = Vector2.Distance(joints[0], target);

			if (dist > TensorPrimitives.Sum(lengths))    // if unreachable
			{
				for (int i = 0; i < joints.Length - 1; i++)
				{
					float r = Vector2.Distance(joints[i], target);
					float lambda = lengths[i] / r;   // Cannot overlap target, no risk of divide by zero
					joints[i + 1] = (1f - lambda) * joints[i] + lambda * target;
				}
			}
			else
			{
				Vector2 b = joints[0];
				float dif_A;
				float toleranceSquared = tolerance * tolerance;
				do
				{
					dif_A = Vector2.DistanceSquared(joints[joints.Length - 1], target);
					// Forwards-reaching:
					joints[joints.Length - 1] = target;
					for (int i = joints.Length - 2; i > -1; i--)
					{
						float r = Vector2.Distance(joints[i + 1], joints[i]);
						if (r == 0) continue;
						float lambda = lengths[i] / r;
						joints[i] = (1f - lambda) * joints[i + 1] + lambda * joints[i];
					}
					// Backwards-reaching:
					joints[0] = b;
					for (int i = 0; i < joints.Length - 1; i++)
					{
						float r = Vector2.Distance(joints[i + 1], joints[i]);
						if (r == 0) continue;
						float lambda = lengths[i] / r;
						joints[i + 1] = (1f - lambda) * joints[i] + lambda * joints[i + 1];
					}
				} while (dif_A > toleranceSquared);	// Avoid sqrt(dif_A)
			}
		}

		/// <summary>Use the FABRIK method to solve the inverse kinematics problem</summary>
		/// <param name="chain">A class that implements abstract Chain</param>
		/// <param name="target">Target position</param>
		/// <param name="tolerance">Acceptable tolerance to end iterations and return</param>
		public static void Fabrik2D(Chain chain, Vector2 target, float tolerance = 0.1f)
		{
			Fabrik2D(chain.Joints, chain.Lengths, target, tolerance);
		}
	}

	file static unsafe class KinematicsHelpers
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void ArgumentThrowHelper(string? msg)
		{
			throw new ArgumentException(msg);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static float ManhattanDistance(Vector2 left, Vector2 right)
		{
			left = Vector2.Abs(right - left);
			return left.X + left.Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]  // Inlining is the only thing that permits this, due to the use of stackalloc. If it doesn't inline, things break. Pls inline (:
		internal static Span<float> Distances(Span<Vector2> joints)
		{
			throw new NotImplementedException("This is done idiotically and desperately needs a rewrite!");
			// Why are we returning a span that could be stackalloced? If it's stackalloced it WILL throw. Stupid!
			Span<float> di = joints.Length < 128 ? stackalloc float[joints.Length - 1] : new float[joints.Length - 1];  // Minimise heap allocations for smaller chains
																														// Avoid bounds checks
			fixed (float* ptr = di)
			fixed (Vector2* jptr = joints)
			{
				for (int i = 0; i < joints.Length - 1; i++)
					ptr[i] = Vector2.Distance(jptr[i + 1], jptr[i]);
			}
#pragma warning disable CS9080 // Use of variable in this context may expose referenced variables outside of their declaration scope
			return di;
#pragma warning restore CS9080 // Use of variable in this context may expose referenced variables outside of their declaration scope
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Span<float> ManhattanDistances(Span<Vector2> joints)
		{
			Span<float> di = joints.Length < 128 ? stackalloc float[joints.Length - 1] : new float[joints.Length - 1];  // Minimise heap allocations for smaller chains
			// Avoid bounds checks
			
			fixed (float* ptr = di)
			fixed (Vector2* jptr = joints)
			{
				for (int i = 0; i < joints.Length - 1; i++)
					ptr[i] = ManhattanDistance(jptr[i + 1], jptr[i]);
			}
#pragma warning disable CS9080 // Use of variable in this context may expose referenced variables outside of their declaration scope
			return di;
#pragma warning restore CS9080 // Use of variable in this context may expose referenced variables outside of their declaration scope
		}
	}
}