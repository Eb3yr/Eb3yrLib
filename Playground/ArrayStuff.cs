using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Playground
{
	// Could these get extended into n dimensions?

	/// <summary>
	/// Attempting a jagged array implementation of 2D arrays that are indexed as a 2D array would be. Enumerators and all that scares me
	/// </summary>
	public class Jagged2D<T> : ICollection
	{
		public Jagged2D(int x, int y)
		{
			// Consistent size of arrays within the array, like a 2D array, as regular jagged arrays can have varying sizes	
			// Which way round does it go again? Don't think it matters so long as x[0, 1] maps onto x[0][1]
			jagged = new T[y][];
			for (int i = 0; i < x; i++)
				jagged[i] = new T[x];
		}
		private T[][] jagged;

		public int Count => jagged.Length * jagged[0].Length;

		public bool IsSynchronized => throw new NotImplementedException();

		public object SyncRoot => throw new NotImplementedException();

		public T this[int x, int y]
		{
			get => jagged[x][y];
			set => jagged[x][y] = value;
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// An N-dimensional array implemented using a single array. 
	/// </summary>
	public class SingleArrNDims<T> : ICollection, ICloneable    // IStructuralEquatable and IStructuralComparable. Structural compares actual values, it's the opposite of comparing references.
	{
		readonly T[] _values;   // Reference cannot be altered, but individual elements can
		readonly int[] _dimensions; // Tracks length of each dimension for the sake of the indexer
		public SingleArrNDims(int size, params int[] additionalDimensionSizes)
		{
			_values = new T[size * additionalDimensionSizes.Aggregate(1, (product, next) => product * next)];
			_dimensions = new int[1 + additionalDimensionSizes.Length];
			_dimensions[0] = size;
			additionalDimensionSizes.CopyTo(_dimensions, 1);
		}

		public SingleArrNDims(T[] inValues, int[] inDimensions)
		{
			_values = inValues;
			_dimensions = inDimensions;
		}

		public T this[int index] { get => _values[index]; set => _values[index] = value; }

		// Considering allowing lesser numbers of arguments fetching a SingleArrNDims instance
		public T this[int index, params int[] indexes]
		{
			get
			{
				if (indexes.Length != _dimensions.Length - 1)
					throw new ArgumentException("Number of arguments does not match the number of dimensions of this array.");

				return _values[GetIndexFromArgs(index, indexes)];
			}
			set
			{
				if (indexes.Length != _dimensions.Length - 1)
					throw new ArgumentException("Number of arguments does not match the number of dimensions of this array.");

				_values[GetIndexFromArgs(index, indexes)] = value;
			}
		}

		public int Count => _values.Length;

		public int Rank => _dimensions.Length;

		public bool IsReadOnly => _values.IsReadOnly;

		public bool IsSynchronized => _values.IsSynchronized;

		public object SyncRoot => _values.SyncRoot;

		public object Clone() => _values.Clone();

		public void CopyTo(Array array, int index) => _values.CopyTo(array, index);

		public IEnumerator GetEnumerator() => _values.GetEnumerator();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetIndexFromArgs(int index, params int[] indexes)
		{
			// Maybe have an additional array field that caches the index ranges. 0 element is the product of all, 1st element is product of all but the topmost-level dimension length, etc.
			// Using this method I can just sum up the argument for each dimension multiplied by that dimension's width
			// Ezpz just a few integer multiplications and additions, shouldn't be *too* expensive. Compare caching vs generating on the fly.
			// Generating on the fly might be mean for iterators



			throw new NotImplementedException();
		}
	}
}
