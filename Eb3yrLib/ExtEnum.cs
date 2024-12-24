using Microsoft.CSharp.RuntimeBinder;
using System.Diagnostics;

namespace Eb3yrLib
{
	public abstract class ExtEnum<T>(T value) : IComparable, IComparable<ExtEnum<T>>, IEquatable<ExtEnum<T>> where T : IComparable, IComparable<T>, IEquatable<T>
	{
		private readonly T _value = value;
		public T Value { get => _value; }
		public int CompareTo(ExtEnum<T>? other)
		{
			if (other is null) return 1;
			return _value.CompareTo(other._value);
		}

		public int CompareTo(object? obj)
		{
			ArgumentNullException.ThrowIfNull(obj);
			if (obj is not T)
				throw new ArgumentException($"Type of parameter {obj.GetType()} does not match type {typeof(T)}");

			return CompareTo((ExtEnum<T>)obj);
		}

		public bool Equals(ExtEnum<T>? other)
		{
			if (other is null)
				return false;

			return _value.Equals(other._value);
		}

		public override bool Equals(object? obj) => Equals(obj as ExtEnum<T>);

		public override int GetHashCode() => _value.GetHashCode();

		public static bool operator ==(ExtEnum<T> left, ExtEnum<T> right) => left.Equals(right);
		public static bool operator !=(ExtEnum<T> left, ExtEnum<T> right) => !left.Equals(right);
		
		// Both sides required: 
		public static bool operator ==(ExtEnum<T> left, T right) => left._value.Equals(right);
		public static bool operator !=(ExtEnum<T> left, T right) => !left._value.Equals(right);
		public static bool operator ==(T left, ExtEnum<T> right) => right._value.Equals(left);
		public static bool operator !=(T left, ExtEnum<T> right) => !right._value.Equals(left);

		// Bitwise ops. Only defining for ExtEnum<T> left and right, not T left or right
		public static ExtEnum<T> operator |(ExtEnum<T> left, ExtEnum<T> right)
		{
			if (left.GetType() != right.GetType())
				throw new ArgumentException($"Mismatched OR operator parameters. Type of left: {left.GetType()}, right: {right.GetType()}");

			try
			{
				dynamic _l = left._value, _r = right._value;
				dynamic ret = left.GetType().GetConstructor(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, [typeof(T)])!.Invoke([_l | _r]);
				return ret;
			}
			catch (RuntimeBinderException)
			{
				Debug.WriteLine($"Tried to do a bitwise operation on a non-binary integer type {typeof(T)} with {left.GetType()}");
				throw;
			}
		}

		public static ExtEnum<T> operator &(ExtEnum<T> left, ExtEnum<T> right)
		{
			if (left.GetType() != right.GetType())
				throw new ArgumentException($"Mismatched AND operator parameters. Type of left: {left.GetType()}, right: {right.GetType()}");

			try
			{
				dynamic _l = left._value, _r = right._value;
				dynamic ret = left.GetType().GetConstructor(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, [typeof(T)])!.Invoke([_l & _r]);
				
				// We check if the result of & matches a member of the derived class, and return that member if so. Generally not necessary on bitwise OR, but likely to be useful for AND
				foreach (var fi in left.GetType().GetFields(System.Reflection.BindingFlags.Static))
				{
					dynamic f = fi.GetValue(null)!;
					if (f == ret)
						return f;
				}
				return ret;
			}
			catch (RuntimeBinderException)
			{
				Debug.WriteLine($"Tried to do a bitwise operation on a non-binary integer type {typeof(T)} with {left.GetType()}");
				throw;
			}
		}

		public override string ToString() => $"{GetType()}`1[{typeof(T)}]";
	}

	/* Example usage: 
	
	sealed class StrEnum : ExtEnum<string>
	{
		private StrEnum(string value) : base(value) { }
		public static readonly StrEnum One = new("One");
		public static readonly StrEnum Two = new("Two");
		public static readonly StrEnum Three = new("Three");
		public static readonly StrEnum Four = new("Four");
	}

	*/
}