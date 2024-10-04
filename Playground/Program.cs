using Eb3yrLib;
using Playground;
using System.Collections;
using System.Diagnostics;
using System.Numerics;



Console.WriteLine("Done");
Console.ReadLine();


namespace Playground
{
	// Stack and queue simultaneously. Why am I doing this? 
	// Supports enqueue, dequeue, push pop peek, isEmpty, isFull, etc. Go through Stack<T> and Queue<T> in docs to see what needs implementing.
	// Torn between calling this Stew or Quack
	public class Stew<T> : IEnumerable<T>, IReadOnlyCollection<T>, ICollection, IQueue<T>, IStack<T>
	{
		private List<T> _values;
		Stew()
		{
			_values = [];
		}

		public int Count => _values.Count;

		public bool IsSynchronized => throw new NotImplementedException();

		public object SyncRoot => throw new NotImplementedException();

		public void CopyTo(Array array, int index)
		{
			for (int i = index; i < _values.Count + index; i++)
				array.SetValue(_values[i - index], i);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _values.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public T Dequeue()
		{
			throw new NotImplementedException();
		}

		public void Enqueue(T value)
		{
			throw new NotImplementedException();
		}

		public T Peek()
		{
			throw new NotImplementedException();
		}

		public T Pop()
		{
			throw new NotImplementedException();
		}

		public void Push(T value)
		{
			throw new NotImplementedException();
		}

		public bool TryDequeue(out T value)
		{
			if (_values.Count == 0)
			{
				value = default!;
				return false;
			}
			try
			{
				value = Dequeue();
				return true;
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
				value = default!;
				return false;
			}
		}

		public bool TryEnqueue(T value)
		{
			try
			{
				Enqueue(value);
				return true;
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
				return false;
			}
		}

		public bool TryPeek(out T value)
		{
			throw new NotImplementedException();
		}

		public bool TryPop(out T value)
		{
			throw new NotImplementedException();
		}

		public bool TryPush(T item)
		{
			throw new NotImplementedException();
		}
		public void Clear()
		{
			_values.Clear();
		}

		public bool Contains(T value)
		{
			return _values.Contains(value);
		}
	}

	public interface IQueue<T>
	{
		public abstract void Enqueue(T value);
		public abstract T Dequeue();
		public abstract bool TryEnqueue(T value);
		public abstract bool TryDequeue(out T value);
		public abstract bool Contains(T value);
		public abstract void Clear();
	}

	public interface IStack<T>
	{
		public abstract void Push(T value);
		public abstract T Pop();
		public abstract T Peek();
		public abstract bool TryPush(T item);
		public abstract bool TryPop(out T value);
		public abstract bool TryPeek(out T value);
		public abstract bool Contains(T value);
		public abstract void Clear();
	}

	// Prio queue? Screw that
}
