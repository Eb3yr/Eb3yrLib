using System.Collections;
using System.Diagnostics;
using System.Numerics;

namespace Eb3yrLib.Kinematics
{
	public class Chain
	{
		public Joint this[int i]
		{
			get => Joints[i];
		}
		public Joint[] Joints { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="count">Number of joints, excluding end effector</param>
		/// <param name="length"></param>
		/// <param name="rootOffset"></param>
		public Chain(int count, float length, Vector3 rootOffset = default)
		{
			Joints = new Joint[count + 1];
			for (int i = 0; i < count; i++)
			{
				//Joints[i].length = length;
				Joints[i].pos = rootOffset + new Vector3(i * length, 0, 0);
			}
			//Joints[count].length = 0;
			//Joints[count].pos = Joints[count - 1].pos + Joints[count - 1].LengthDirection;
			Joints[count].pos = Joints[count - 1].pos + (Joints[count - 1].pos - Joints[count - 2].pos);	// Testing prev line replacement
		}
		/// <summary>
		/// Use the triangulation method to move this chain towards the target
		/// </summary>
		public void Triangulate(Vector3 target)
		{
			// Something is borked and it doesn't properly handle the end effector?
			// Also sometimes direction becomes NaN, which cascades down the chain and ruins everything
			for (int i = 0; i < Joints.Length; i++)
			{
				Vector3 vecC = target - Joints[i].pos;
				float c = vecC.Length();
				vecC /= c;
				float b = 0f;
				for (int j = i + 1; j < Joints.Length; j++)
					b += Joints[i].length;
		
				float a = Joints[i].length;
		
				Vector3 vecA;
				if (c >= a + b /*|| i == Joints.Length - 1*/ && c < float.Abs(a - b)) // OR flag to try and fix a bug that seems to occur when the else if triggers on the final joint
					vecA = vecC;
				else if (c < float.Abs(a - b))
					vecA = -vecC;
				else
				{
					if (i != 0)
						vecA = Joints[i - 1].dir;
					else
						vecA = new Vector3(1, 0, 0);
		
					float theta = float.Acos(Vector3.Dot(vecA, vecC)) - float.Acos((a * a + c * c - b * b) / (2f * a * c));
		
					Vector3 r;
					if (vecA == vecC || vecA == -vecC)
						r = new(0, 1, 0);
					else
						r = Vector3.Cross(vecA, vecC);
		
					Quaternion q = Quaternion.CreateFromAxisAngle(r / r.Length(), theta);
					vecA = Vector3.Transform(vecA, q);
				}
		
				Joints[i].dir = vecA;
				if (i != Joints.Length - 1)
					Joints[i + 1].pos = Joints[i].pos + Joints[i].LengthDirection;
			}
		}

		/// <summary>
		/// Use the FABRIK (forward and backward reaching inverse kinematics) method to move this chain towards the target
		/// </summary>
		public void Fabrik(Vector3 target, float tolerance = 1f /*Arbitrary, find a better default tolerance later*/)
		{
			float dist = Vector3.Distance(Joints[0].pos, target);
			float[] di = new float[Joints.Length - 1];  // Distance from index joint to next
			for (int i = 0; i < Joints.Length - 1; i++)
				di[i] = Vector3.Distance(Joints[i + 1].pos, Joints[i].pos);

			if (dist > di.Sum())
			{
				// Target is unreachable
				for (int i = 0; i < Joints.Length - 1; i++)
				{
					float r = Vector3.Distance(Joints[i].pos, target);
					float lambda = di[i] / r;
					Joints[i + 1].pos = (1f - lambda) * Joints[i].pos + lambda * target;
				}
			}
			else
			{
				// Target is reachable
				Vector3 b = Joints[0].pos;
				float dif_A;
				do	// Minor differences in indexing between the contents of each for loop.
				{
					dif_A = Vector3.DistanceSquared(Joints[Joints.Length - 1].pos, target);     // Micro-optimisation to avoid sqrt
					// Forwards-reaching:
					Joints[Joints.Length - 1].pos = target;
					for (int i = Joints.Length - 2; i > -1; i--)
					{
						float r = Vector3.Distance(Joints[i + 1].pos, Joints[i].pos);
						if (r == 0) continue;
						float lambda = di[i] / r;
						Joints[i].pos = (1f - lambda) * Joints[i + 1].pos + lambda * Joints[i].pos;
					}
					// Backwards-reaching:
					Joints[0].pos = b;
					for (int i = 0; i < Joints.Length - 1; i++)
					{
						float r = Vector3.Distance(Joints[i + 1].pos, Joints[i].pos);   // Cannot cache as pos changes with each iteration of the for loop
						if (r == 0) continue;
						float lambda = di[i] / r;
						Joints[i + 1].pos = (1f - lambda) * Joints[i].pos + lambda * Joints[i + 1].pos;
					}
				} while (dif_A > tolerance * tolerance);    // Check whether distance between end effector and t is greater than a tolerance
			}
		}

		public void Analytical2D(Vector2 target)
		{
			Vector2[] inJoints = Joints.Select(j => new Vector2(j.pos.X, j.pos.Y)).ToArray();
			var outJoints = Kinematics.Analytical2D(inJoints, target);
			for (int i = 0; i < Joints.Length; i++)
			{
				Joints[i].pos.X = outJoints[i].X;
				Joints[i].pos.Y = outJoints[i].Y;
			}
		}
	}
}
