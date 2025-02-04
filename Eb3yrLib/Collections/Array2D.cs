﻿using System.Collections;
using System.Runtime.CompilerServices;

namespace Eb3yrLib.Collections
{
	/// <summary>A 2D array implementation using a 1D backing array</summary>
	/// <remarks> y dimension elements for a given x index are contiguous in the backing array. Accessing all y elements for a given x index performs like a jagged or 1D array, but accessing all of x for a given y performs moderately worse than a multidimensional array. Ideally enumerate manually. While it implements IEnumerable<T>, avoid LINQ where unecessary due to significant overhead on fast operations</remarks>
	public readonly struct Array2D<T> : IReadOnlyList<T>   // ReadOnly to avoid implementing list-specific operators that don't apply to arrays. It's still mutable (:
	{
		/// <param name="lengthX">Length of the first dimension</param>
		/// <param name="lengthY">Length of the second dimension</param>
		public Array2D(int lengthX, int lengthY)
		{
			values = new T[lengthX * lengthY];
			xBound = lengthX - 1;
			yBound = lengthY - 1;
		}

		/// <param name="lengthX">Length of the first dimension</param>
		/// <param name="lengthY">Length of the second dimension</param>
		/// <param name="defaultValue">Default value to populate the array with</param>
		public Array2D(int lengthX, int lengthY, T defaultValue) : this(lengthX, lengthY)
		{
			for (int i = 0; i < values.Length; i++)
				values[i] = defaultValue;
		}

		/// <param name="length">Length of each dimension</param>
		public Array2D(int length) : this(length, length) { }

		/// <param name="length">Length of each dimension</param>
		/// <param name="defaultValue">Default value to populate the array with</param>
		public Array2D(int length, T defaultValue) : this(length, length, defaultValue) { }

		private readonly int xBound;
		private readonly int yBound;
		private readonly T[] values;

		public int Count => values.Length;

		public T this[int index] => values[index];

		public T this[int x, int y]
		{
			get => values[GetIndex(x, y)];
			set => values[GetIndex(x, y)] = value;
		}

		/// <summary>Returns the length of the array in the given dimension</summary>
		/// <param name="dimension">Which dimension to get the length of. -1 will give the product of each dimension's lengths (the length of the backing 1D array)</param>
		public int Length(int dimension = -1)
		{
			return dimension switch
			{
				0 => xBound + 1,
				1 => yBound + 1,
				-1 => values.Length,
				_ => throw new ArgumentOutOfRangeException(message: "dimension must be zero or one.", null)
			};
		}

		/// <summary>Gets the index by multiplying x by xBound and adding y. Means that y elements for a given x are contiguous, quicker in a nested for loop where the outer loop is x. Slightly slower than a multidimensional array otherwise. </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetIndex(int x, int y) => xBound * x + y;   // multiply by x instead of y so that all y vals are adjacent for ideal performance in nested for loops with y on the inside

		public IEnumerator<T> GetEnumerator() => values.AsEnumerable().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => values.GetEnumerator();
	}
}
