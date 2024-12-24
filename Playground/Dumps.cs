using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Collections;
using Eb3yrLib.Aerodynamics;
using ScottPlot;

namespace Playground
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

	public static class StrExt
	{
		public static string Jumble(this string str)
		{
			Random rng = new();
			char[] chars = str.ToCharArray();
			return new string(chars.OrderBy((char c) => rng.NextSingle() - 0.5f).ToArray());
		}
	}

	public static class CollectionExtensions
	{
		public static ICollection<T> Jumble<T>(this ICollection<T> collection)
		{
			Random rng = new();
			return [.. collection.OrderBy((T item) => rng.NextSingle() - 0.5f)];
		}
	}

	public static class Plotting
	{
		public static RootedCoordinateVector[] CoordVec((double x, double z)[,] coords, (double u, double w)[,] vecs, double maskAbove = 20)
		{
			var coordVecs = new RootedCoordinateVector[coords.Length];
			int i = 0;
			var cEnum = coords.GetEnumerator();
			var vEnum = vecs.GetEnumerator();

			while (cEnum.MoveNext() && vEnum.MoveNext())
			{
				var (x, z) = ((double x, double z))cEnum.Current;
				var (u, w) = ((double u, double w))vEnum.Current;
				if (double.Abs(double.Min(u, w)) > maskAbove)
					u = w = 0;

				coordVecs[i] = new(new Coordinates(x, z), new Vector2((float)u, (float)w));
				i++;
			}
			return coordVecs;
		}

		public static Coordinates3d[,] Coord3D((double x, double z)[,] coords, double[,] stream, double maskAbove = 100)
		{
			var arr = new Coordinates3d[coords.GetLength(0), coords.GetLength(1)];
			for (int i = 0; i < arr.GetLength(0); i++)
				for (int j = 0; j < arr.GetLength(1); j++)
					arr[i, j] = new Coordinates3d(coords[i, j].x, coords[i, j].z, stream[i, j]);

			return arr;
		}

		public static void Main()
		{
			StreamFunction psi = (PotentialFlows.Vortex(6 * double.Pi, -1, 0) + PotentialFlows.Source(-8 * double.Pi, 2, 0) + PotentialFlows.Uniform(5, 0)).Sum();
			psi = PotentialFlows.Vortex(10, 0, 0);




			var plt = new Plot();
			var grid = PotentialFlows.CoordGrid(-4, 4, -2, 2, 0.02, 0.02);

			double gamma = -2.4 * double.Pi;
			psi = PotentialFlows.Uniform(4.7) + PotentialFlows.Source(1.6);
			psi = psi.Sum();

			Console.WriteLine(psi(2, 4.1) - psi(1, 2.5));

			var stream = PotentialFlows.StreamGrid(grid, psi);
			var c3d = Coord3D(grid, stream);  // This and stream show that I'm getting the correct stream values for x and z -> uniform depends solely on z
											  //plt.Add.Heatmap(c3d);	// No good, stinky, arranges based on array index, NOT on coordinate? Even though it still has coordinates? what
			var cl = plt.Add.ContourLines(c3d, 50); // : )
			cl.LabelStyle.IsVisible = false;
			cl.Colormap = new ScottPlot.Colormaps.Turbo();
			cl.LineWidth = 2;
			cl.LineStyle.Rounded = true;
			plt.SavePng("pltTest.png", 1600, 1200);

			var VelFunc = psi.GetVelocityFunc();
			Console.WriteLine(VelFunc(2.3, 0));
		}
	}
}
