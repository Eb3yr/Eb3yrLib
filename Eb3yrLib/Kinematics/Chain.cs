﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Eb3yrLib.Kinematics
{
	public abstract class Chain	// Should implement IList. Support proper insertions and all, where the collection is of type ValueTuple<float, Vector2>
	{
		/// <summary>Length of each joint. The final joint should have a length of 0 or not exist</summary>
		public float[] Lengths;	// Store this so that it doesn't have to be calculated at the start of every completion of the FABRIK algorithm

		public Vector2[] Joints;

		public Chain(int count)
		{
			Lengths = new float[count];
			Joints = new Vector2[count];
		}

		public Chain(float[] lengths) : this(lengths.Length)
		{
			lengths.CopyTo(Lengths, 0);
		}

		public Chain(float[] lengths, Vector2[] joints) : this(lengths)
		{
			joints.CopyTo(Joints, 0);
		}
	}

	public abstract class NewChain
	{

	}

	public interface IChain
	{
		public abstract Vector2[] JointPos { get; }
	}
}
