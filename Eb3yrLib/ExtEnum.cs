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

		public override bool Equals(object? o) => Equals(o as ExtEnum<T>);

		public override int GetHashCode() => _value.GetHashCode();

		public static bool operator ==(ExtEnum<T> left, ExtEnum<T> right) => left.Equals(right);
		public static bool operator !=(ExtEnum<T> left, ExtEnum<T> right) => !left.Equals(right);
		
		// Both sides required: 
		public static bool operator ==(ExtEnum<T> left, T right) => left._value.Equals(right);
		public static bool operator !=(ExtEnum<T> left, T right) => left._value.Equals(right);
		public static bool operator ==(T left, ExtEnum<T> right) => right._value.Equals(left);
		public static bool operator !=(T left, ExtEnum<T> right) => right._value.Equals(left);
	}

	/* Example usage: 
	
	sealed class StrEnum : ExtEnum<string>
	{
		private StrEnum(string value) : base(value) { }
		public static StrEnum One = new("One");
		public static StrEnum Two = new("Two");
		public static StrEnum Three = new("Three");
		public static StrEnum Four = new("Four");
	}

	*/
}