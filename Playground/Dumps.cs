using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Collections;

namespace Eb3yrLib
{
	public sealed class BoolOpsEnum<T> : IComparable, IComparable<BoolOpsEnum<T>>, IEquatable<BoolOpsEnum<T>> where T : INumber<T>
	{
		// Reformat so that it's just a Type that I can use with ExtEnum properly, instead of being its own enum. 
		// I wonder if I can implicitly cast from Func<T, T, bool> to BoolOpsEnum<T> to avoid a new(); ?

		readonly Func<T, T, bool> _op;
		public Func<T, T, bool> Value { get => _op; }
		readonly int _ID;
		static int tally = 0;

		private BoolOpsEnum(Func<T, T, bool> op)
		{
			_op = op;
			_ID = tally;
			tally++;
		}

		public static readonly BoolOpsEnum<T> Eq = new((T left, T right) => left == right);
		public static readonly BoolOpsEnum<T> Ineq = new((T left, T right) => left != right);
		public static readonly BoolOpsEnum<T> LessThan = new((T left, T right) => left < right);
		public static readonly BoolOpsEnum<T> GreaterThan = new((T left, T right) => left > right);
		public static readonly BoolOpsEnum<T> LessThanOrEq = new((T left, T right) => left <= right);
		public static readonly BoolOpsEnum<T> GreaterThanOrEq = new((T left, T right) => left >= right);

		public int CompareTo(object? obj) => CompareTo(obj as BoolOpsEnum<T>);

		public int CompareTo(BoolOpsEnum<T>? other)
		{
			if (other is null)
				return 1;
			return _ID.CompareTo(other._ID);
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as BoolOpsEnum<T>);
		}

		public bool Equals(BoolOpsEnum<T>? other)
		{
			return other is not null && _ID == other._ID;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(_ID, _op.GetHashCode());
		}
	}

	public struct Intermediary()
	{
		// An intermediary between System.Numerics.Vector2 and System.Drawing.Point that allows implicit casts to and from this intermediary, but unfortunately not to and from Vector2 and Point. 
		public Vector2 Value;
		public Intermediary(Vector2 vec2) : this() => Value = vec2;
		public static implicit operator Vector2(Intermediary i) => i.Value;
		public static implicit operator Intermediary(Vector2 vec2) => new(vec2);
		public static implicit operator Point(Intermediary i) => new((int)Math.Round(i.Value.X, MidpointRounding.AwayFromZero), (int)Math.Round(i.Value.Y, MidpointRounding.AwayFromZero));
		public static implicit operator Intermediary(Point p) => new(new Vector2(p.X, p.Y));

		public static void Demo()
		{
			static void LemmeVec(Vector2 vec)
			{
				Console.WriteLine(vec);
			}

			static void LemmePoint(Point p)
			{
				Console.WriteLine(p);
			}

			Vector2 myVec = new(1f, 5f);
			Intermediary inter = myVec;
			LemmeVec(inter);
			LemmePoint(inter);
		}
	}
}

namespace Playground
{
	// Stack and queue simultaneously. Why am I doing this? 
	// Supports enqueue, dequeue, push pop peek, isEmpty, isFull, etc. Go through Stack<T> and Queue<T> in docs to see what needs implementing.
	// Torn between calling this Stew or Quack
	public class Stew<T> : IEnumerable<T>, IReadOnlyCollection<T>, ICollection, IQueue<T>, IStack<T>
	{
		private List<T> _values;
		private int start;
		private int end;
		public Stew()
		{
			_values = [];
			// Should I use ICollection for values?
			// The problem with using a list is that it won't wrap around. I don't want to be shuffling everything every time something gets dequeued and the entire list needs to be moved, which is O(n), but I don't want to have discontinuous bits. Eg It wraps around, then the arrays gets expanded, and now you have discontinuous filling. Having to shuffle at this point sucks.
			// I am starting to realise why this is a stupid idea.
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
