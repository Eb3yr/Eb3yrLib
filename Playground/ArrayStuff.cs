using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Playground
{
	/// <summary>Jagged array implementation of 2D arrays that are indexed as a 2D array would be and implements IEnumerable<T></summary>
	public sealed class Jagged2D<T> : IEnumerable<T>
	{
		/// <summary>Construct a jagged 2D array of dimensions m by n, indexed as [m, n]</summary>
		public Jagged2D(int m, int n)
		{
			this.m = m;
			this.n = n;
			jagged = new T[n][];
			for (int i = 0; i < m; i++)
				jagged[i] = new T[m];
		}

		public Jagged2D(T[][] arr)
		{
			// Check n matches

			m = arr.Length;
			n = arr[0].Length;
			jagged = arr;	// Should really be copying here but what the heck
		}
		
		// Cannot be reassigned. jagged's elements can still be mutated, so the arrays stored within it can also be reassigned. Do not do so lest you compromise their dimensions!
		private readonly T[][] jagged;
		public readonly int m;
		public readonly int n;

		public int Count => jagged.Length * jagged[0].Length;

		// Don't need to worry about concurrency, don't care
		public bool IsSynchronized => throw new NotImplementedException();
		public object SyncRoot => throw new NotImplementedException();

		public T this[int m, int n]
		{
			get => jagged[m][n];
			set => jagged[m][n] = value;
		}

		public IEnumerator<T> GetEnumerator() => new Jagged2DEnumerator<T>(this);
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	// About twice as slow as a 2D array enumeration
	public sealed class Jagged2DEnumerator<T>(Jagged2D<T> inJagged) : IEnumerator<T>
	{
		private Jagged2D<T> jagged = inJagged;
		private int _m = 0;
		private int _n = 0;

		private T _current = default!;
		public T Current => _current;

		object IEnumerator.Current => throw new NotImplementedException();

		// Get the jagged array's outer array layer's enumerator, enumerate through it, and for each iteration enumerate through the array contained within.
		// Or lets do a simpler implementation and index the array

		public void Dispose() { }

		public bool MoveNext()
		{
			if (_m < jagged.m)
			{
				if (_n < jagged.n)
				{
					_current = jagged[_m, _n];
					_n++;
					return true;
				}
				else
				{
					_m++;
					_n = 0;
					if (_m < jagged.m)
					{
						_current = jagged[_m, _n];
						return true;
					}
					_current = default!;
					return false;
				}
			}
			else
				_current = default!;

			return false;
		}

		public void Reset()
		{
			_m = 0;
			_n = 0;
		}
	}
}
