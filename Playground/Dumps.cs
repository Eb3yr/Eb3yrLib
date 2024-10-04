using System.Drawing;
using System.Numerics;

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
