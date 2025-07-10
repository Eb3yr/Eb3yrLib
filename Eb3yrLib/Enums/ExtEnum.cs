using Microsoft.CSharp.RuntimeBinder;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Eb3yrLib.Enums
{
	public abstract class ExtEnum<TSelf, TValue>(TValue value) : IComparable<TSelf>, IEquatable<TSelf> where TValue : IComparable<TValue>, IEquatable<TValue> where TSelf : ExtEnum<TSelf, TValue>
	{
		private readonly TValue _value = value;
		public TValue Value { get => _value; }
		public int CompareTo(TSelf? other)
		{
			if (other is null) return 1;
			return _value.CompareTo(other._value);
		}

		public bool Equals(TSelf? other)
		{
			if (other is not null)
				return _value.Equals(other._value);

			return false;
		}

		public override bool Equals(object? obj) => Equals(obj as TSelf);

		public override int GetHashCode() => _value.GetHashCode();

		public static bool operator ==(ExtEnum<TSelf, TValue> left, ExtEnum<TSelf, TValue> right) => left.Equals(right);
		public static bool operator !=(ExtEnum<TSelf, TValue> left, ExtEnum<TSelf, TValue> right) => !left.Equals(right);

		// Both sides required: 
		public static bool operator ==(ExtEnum<TSelf, TValue> left, TValue right) => left._value.Equals(right);
		public static bool operator !=(ExtEnum<TSelf, TValue> left, TValue right) => !left._value.Equals(right);
		public static bool operator ==(TValue left, ExtEnum<TSelf, TValue> right) => right._value.Equals(left);
		public static bool operator !=(TValue left, ExtEnum<TSelf, TValue> right) => !right._value.Equals(left);

		// Bitwise ops. Only defining for ExtEnum<T, TSelf> left and right, not T left or right
		public static ExtEnum<TSelf, TValue> operator |(ExtEnum<TSelf, TValue> left, ExtEnum<TSelf, TValue> right)
		{
			throw new NotImplementedException("Finish implementing | and & with unmanaged types first");

			if (left.GetType() != right.GetType())
				ThrowHelpers.ThrowArgumentException($"Mismatched OR operator parameters. Type of left: {left.GetType()}, right: {right.GetType()}");

			if (System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
				ThrowHelpers.ThrowArgumentException($"Cannot do bitwise operator on managed type {typeof(TValue)} with {left.GetType()}");

			try
			{
				// This is utterly disgusting
				// Try replacing it with .NET 10 extension members where T supports the OR operator
				dynamic _l = left._value, _r = right._value;
				dynamic ret = left.GetType().GetConstructor(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, [typeof(TValue)])!.Invoke([_l | _r]);
				return ret;
			}
			catch (RuntimeBinderException)
			{
				Debug.WriteLine($"Tried to do a bitwise operation on a non-binary integer type {typeof(TValue)} with {left.GetType()}");
				Debug.WriteLine("How did you get here? Shouldn't be here with a managed type.");
				throw;
			}
		}

		// I wonder if this can be improved using extension members from .NET 10? They have static and instance methods and properties but no operators. But if I can define something like this (whether it's this release or the next):
		// public static TEnum operator &<TEnum, TSelf, TValue>(left, right) where TEnum : ExtEnum<TSelf, TValue>
		/*
		 * extension <TEnum, TSelf, TValue>(TEnum left, TEnum right)
		 * {
		 *		public static TEnum operator &(TEnum left, TEnum right) => blahblahblah;
		 * }
		 * 
		 * or alternatively use inheritance to have a bitwise derived class of ExtEnum that accepts unmanaged exclusively?
		 * 
		 * This feels extremely convoluted and like I should have two entirely separate base classes which get chosen as appropriate
		 * 
		 */
		public static ExtEnum<TSelf, TValue> operator &(ExtEnum<TSelf, TValue> left, ExtEnum<TSelf, TValue> right)
		{
			throw new NotImplementedException("Finish implementing | and & with unmanaged types first");

			if (left.GetType() != right.GetType())
				ThrowHelpers.ThrowArgumentException($"Mismatched AND operator parameters. Type of left: {left.GetType()}, right: {right.GetType()}");

			if (System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
				ThrowHelpers.ThrowArgumentException($"Cannot do bitwise operator on managed type {typeof(TValue)} with {left.GetType()}");

			try
			{
				dynamic _l = left._value, _r = right._value;
				dynamic ret = left.GetType().GetConstructor(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, [typeof(TValue)])!.Invoke([_l & _r]);

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
				Debug.WriteLine($"Tried to do a bitwise operation on a non-binary integer type {typeof(TValue)} with {left.GetType()}");
				Debug.WriteLine("How did you get here? Shouldn't be here with a managed type.");
				throw;
			}
		}

		public override string ToString() => $"{GetType()}`1[{typeof(TValue)}]";
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