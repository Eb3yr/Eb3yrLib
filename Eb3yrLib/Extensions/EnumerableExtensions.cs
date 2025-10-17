using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Eb3yrLib.Extensions
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> Range<T>(int start, int count, Func<int, T> populator)
		{
			for (int i = start; i < start + count; i++)
				yield return populator(i);
		}

		public static IEnumerable<T> Range<T>(int count, Func<T> populator)
		{
			for (int i = 0; i < count; i++)
				yield return populator();
		}

		public static string ToFormattedString<T>(this IEnumerable<T> enumerable, char delimiter = ',', bool braces = true)
		{
			System.Text.StringBuilder outStr = new();
			if (braces)
				outStr.Append('[');

			_ = outStr.AppendJoin(delimiter, enumerable);
			if (braces)
				outStr.Append(']');

			return outStr.ToString();
		}

		public static string ToFormattedString2D<T>(this IEnumerable<IEnumerable<T>> enumerable, char delimiter = ',', bool braces = true)
		{
			System.Text.StringBuilder outStr = new();
			if (braces)
				outStr.Append('[');

			bool nonZero = false;
			foreach (var a in enumerable)
			{
				nonZero = true;
				if (a != null)
					outStr.Append(a.ToFormattedString(delimiter, braces));

				outStr.Append(delimiter);
			}

			if (nonZero)
				outStr = outStr.Remove(outStr.Length - 1, 1);  // Remove last delimiter

			if (braces)
				outStr.Append(']');

			return outStr.ToString();
		}

		public static bool IsOrderedAscending<T>(this IEnumerable<T> enumerable) where T : IComparable<T>
		{
			using var enumerator = enumerable.GetEnumerator();
			if (!enumerator.MoveNext())
				return true;	// Length 0
			
			T prev = enumerator.Current;
			while (enumerator.MoveNext())
			{
				if (prev.CompareTo(enumerator.Current) > 0)
					return false;

				prev = enumerator.Current;
			}
			return true;
		}

		public static bool IsOrderedDescending<T>(this IEnumerable<T> enumerable) where T : IComparable<T>
		{
			using var enumerator = enumerable.GetEnumerator();
			if (!enumerator.MoveNext())
				return true;

			enumerator.MoveNext();
			T prev = enumerator.Current;
			while (enumerator.MoveNext())
			{
				if (prev.CompareTo(enumerator.Current) < 0)
					return false;

				prev = enumerator.Current;
			}
			return true;
		}

		public static bool IsOrdered<T>(this IEnumerable<T> enumerable) where T : IComparable<T>
		{
			return IsOrderedAscending(enumerable) || IsOrderedDescending(enumerable);
		}

		public static IEnumerable<T> Interweave<T>(this IEnumerable<T> enumerable, IEnumerable<T> other)
		{
			using var enum1 = enumerable.GetEnumerator();
			using var enum2 = other.GetEnumerator();
			bool enumMoveable;
			while (true)
			{
				enumMoveable = enum1.MoveNext();
				if (enumMoveable)
					yield return enum1.Current;

				if (enum2.MoveNext())
					yield return enum2.Current;
				else if (!enumMoveable)
					yield break;
			}
		}

		/// <summary>Convert a numeric enumerable to another numeric type using INumber<T>.CreateSaturating(TOther)</summary>
		public static IEnumerable<TOut> ToNum<T, TOut>(this IEnumerable<T> enumerable) where T : INumber<T> where TOut : INumber<TOut>
		{
			foreach (T e in enumerable)
				yield return TOut.CreateSaturating(e);
		}

		/// <summary>Convert a numeric enumerable to another numeric type using INumber<T>.CreateTruncating(TOther)</summary>
		public static IEnumerable<TOut> ToNumTruncating<T, TOut>(this IEnumerable<T> enumerable) where T : INumber<T> where TOut : INumber<TOut>
		{
			foreach (T e in enumerable)
				yield return TOut.CreateTruncating(e);
		}

		/// <summary>Convert a numeric enumerable to another numeric type using INumber<T>.CreateChecked(TOther)</summary>
		/// <exception cref="OverflowException"></exception>
		public static IEnumerable<TOut> ToNumChecked<T, TOut>(this IEnumerable<T> enumerable) where T : INumber<T> where TOut : INumber<TOut>
		{
			foreach (T e in enumerable)
				yield return TOut.CreateChecked(e);
		}

		/// <summary>Apply a condition to the elements of an enumerable and transform the element depending on whether it meets that condition or not using functions passed as arguments</summary>
		public static IEnumerable<TOut> WhereSuccessAndFail<T, TOut>(this IEnumerable<T> enumerable, Func<T, bool> condition, Func<T, TOut> success, Func<T, TOut> failure)
		{
			foreach (T e in enumerable)
				yield return condition(e) ? success(e) : failure(e);
		}

        /// <summary>Evaluates whether source contains all of the elements of other.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="other"></param>
        /// <param name="hasDuplicates">Set to true only if other contains no duplicates.</param>
        /// <returns>Whether source contains all the elements of other./returns>
        public unsafe static bool ContainsAll<T>(this IEnumerable<T> source, IEnumerable<T> other, bool hasDuplicates = true) where T : notnull
        {
			HashSet<T> kvp;
			
#pragma warning disable IDE0028, IDE0306 // Collection expression generates worse code in .NET 10 for the Span path and does not use the HashSet ctor
			if (TryGetSpan(null, other, out ReadOnlySpan<T> readOnlySpan) && !hasDuplicates)
			{
				kvp = new(readOnlySpan.Length);
				for (int i = 0; i < readOnlySpan.Length; i++)   // Foreach generates two extra locals: a copy of the span and a var to store *readOnlySpan[i] in
				{
					kvp.Add(readOnlySpan[i]);
				}
			}
			else
			{
				kvp = new(other);
			}
#pragma warning restore IDE0028, IDE0306

            foreach (T item in source)
			{
				if (kvp.Remove(item) && kvp.Count == 0)
				{
					return true;
				}
			}

            return false;
        }

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod)]
        private extern static bool TryGetSpan<TSource>([UnsafeAccessorType("System.Linq.Enumerable, System.Linq")] object? c, IEnumerable<TSource> source, out ReadOnlySpan<TSource> span);
    }
}
