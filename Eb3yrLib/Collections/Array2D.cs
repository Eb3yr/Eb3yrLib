using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Eb3yrLib.Collections
{
	/// <summary>A 2D fixed-length array implementation using a 1D backing array</summary>
	/// <remarks> Array2D is row-major, therefore for loops should index using a y-outer x-inner pattern.</remarks>
	/// 

	// Make sure the indexing matches that of Span2D<T> from CommunityToolkit.HighPerformance (NuGet). That can be initialised from a 1D array (the backing array here), a width, and a height. It's just a sanity check

	public sealed class Array2D<T> : IList<T>
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

		private readonly T[] values;
		private readonly int xBound;
		private readonly int yBound;

		public int Count => values.Length;

		public bool IsReadOnly => false;

		public T this[int index]
		{
			get => values[index];
			set => values[index] = value;
		}

		public T this[int x, int y]
		{
			get => values[GetIndex(x, y)];
			set => values[GetIndex(x, y)] = value;
		}

		/// <summary>Returns the length of the array in the given dimension</summary>
		/// <param name="dimension">Which dimension to get the length of. -1 will give the product of each dimension's lengths (the length of the backing 1D array)</param>
		public int Length(int dimension = 0)
		{
			return dimension switch
			{
				0 => xBound + 1,
				1 => yBound + 1,
				_ =>  ThrowHelpers.ThrowArgumentOutOfRange(message: "dimension must be zero or one.", null)
			};
		}

		/// <summary>Gets the index by multiplying x by xBound and adding y. Means that y elements for a given x are contiguous, quicker in a nested for loop where the outer loop is x. Slightly slower than a multidimensional array otherwise. </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndex(int x, int y) => yBound * y + x;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public (int x, int y) FromIndex(int index)
		{
			int y = index % yBound;
			return (index - y * yBound, y);
		}

		public IEnumerator<T> GetEnumerator() => values.AsEnumerable().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => values.GetEnumerator();

		public (int x, int y) IndexOf2D(T item) => FromIndex(Array.IndexOf(values, item));

		public int IndexOf(T item) => Array.IndexOf(values, item);

		public void Insert(int index, T item) => ThrowHelpers.ThrowNotSupported("Inserting items not supported on a fixed-length Array2D");

		public void RemoveAt(int index) => ThrowHelpers.ThrowNotSupported("Removing items not supported on a fixed-length Array2D");

		public void Add(T item) => ThrowHelpers.ThrowNotSupported("Adding items not supported on a fixed-length Array2D");

		public void Clear() => Array.Clear(values);

		public bool Contains(T item) => Array.IndexOf(values, item) is -1;

		public void CopyTo(T[] array, int arrayIndex) => values.CopyTo(array, arrayIndex);

		public bool Remove(T item) { ThrowHelpers.ThrowNotSupported("Removing items not supported on a fixed-length Array2D"); return false; }

		public Span<T> AsSpan() => values.AsSpan();
	}
}
