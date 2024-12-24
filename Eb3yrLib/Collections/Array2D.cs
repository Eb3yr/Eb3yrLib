using System.Collections;
using System.Runtime.CompilerServices;

namespace Eb3yrLib.Collections
{
	/// <summary>A 2D array implementation using a 1D backing array</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="lengthX">Length of the first dimension</param>
	/// <param name="lengthY">Length of the second dimension</param>
	/// <remarks> y dimension elements for a given x index are contiguous in the backing array. Accessing all y elements for a given x index performs like a jagged or 1D array, but accessing all of x for a given y performs moderately worse than a multidimensional array. Ideally enumerate manually. While it implements IEnumerable<T>, avoid LINQ where unecessary due to significant overhead on fast operations</remarks>
	public readonly struct Array2D<T> : IReadOnlyList<T>   // ReadOnly to avoid implementing everything else. It's still mutable (:
	{
		public Array2D(int lengthX, int lengthY)
		{
			values = new T[lengthX * lengthY];
			xBound = lengthX - 1;
			yBound = lengthY - 1;
		}
		public Array2D(int lengthX, int lengthY, T defaultValue) : this(lengthX, lengthY)
		{
			for (int i = 0; i < values.Length; i++)
				values[i] = defaultValue;
		}

		public Array2D(int length) : this(length, length) { }

		public Array2D(int length, T defaultValue) : this(length, length, defaultValue) { }

		private readonly int xBound;
		private readonly int yBound;
		private readonly T[] values;

		public int Count => values.Length;

		public T this[int index] => values[index];

		public T this[int x, int y]
		{
			// No bounds checks for a dramatic increase in performance. Went from ~30% slower than a multidim array to within error of jagged, and only ~3% slower than 1D access
			get => values[GetIndex(x, y)];
			set => values[GetIndex(x, y)] = value;
		}

		/// <summary>Returns the length of the array in the given dimension. Argument of -1 gives the product of each dimension's lengths</summary>
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
		private int GetIndex(int x, int y) => xBound * x + y;   // multiply by x instead of y so that all y vals are adjacent. Expecting nested for, with y inside the x loop. Might speed up.

		public IEnumerator<T> GetEnumerator() => values.AsEnumerable().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => values.GetEnumerator();
	}
}
