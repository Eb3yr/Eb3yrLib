using System.Collections;
using System.Runtime.InteropServices;

namespace Eb3yrLib.Collections
{
	/// <summary>Encapsulates a <see cref="List{T}"/> that is sorted on the provided <see cref="Comparer{T}"/>, or the default comparer if none is provided.</summary>
	/// <typeparam name="T"></typeparam>
	public class SortedList<T> : IList<T>
	{
		private readonly List<T> _list;

		public Comparer<T> Comparer
		{
			get;
			set
			{
				field = value;
				_list.Sort(value);
			}
		}

		public SortedList(int capacity = 0)
		{
			_list = new(capacity);
			Comparer = Comparer<T>.Default;
		}

		public SortedList(List<T> list)
		{
			_list = new(list);
			_list.Sort();
			Comparer = Comparer<T>.Default;
		}

		public SortedList(Comparer<T> comparer) : this(0, comparer) { }
		
		public SortedList(int capacity, Comparer<T> comparer) : this(capacity)
		{
			Comparer = comparer;
		}

		public SortedList(List<T> list, Comparer<T> comparer)
		{
			_list = list;
			Comparer = comparer;
		}

		public T this[int index]
		{
			get => _list[index];
			set => throw new NotSupportedException("Use RemoveAt(int index) and Add(T item) instead");
		}

		public int Count => _list.Count;

		public bool IsReadOnly => false;

		public void Add(T item)
		{
			int res = _list.BinarySearch(item, Comparer);
			if (res < 0)
				_list.Insert(~res, item);
			else
				_list.Insert(res, item);
		}

		public void Clear() => _list.Clear();

		public bool Contains(T item) => _list.Contains(item);

		public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

		public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

		public int IndexOf(T item) => _list.IndexOf(item);

		/// <exception cref="NotSupportedException"></exception>
		public void Insert(int index, T item) => throw new NotSupportedException();

		public bool Remove(T item) => _list.Remove(item);

		public void RemoveAt(int index) => _list.RemoveAt(index);

		IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

		public List<T> AsList() => _list;	// Preferred over an operator to make it clearer that it returns the internal list

		public Span<T> AsSpan() => CollectionsMarshal.AsSpan(_list);
	}
}
