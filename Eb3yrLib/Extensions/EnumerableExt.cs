using System.Numerics;

namespace Eb3yrLib.Extensions
{
	public static class EnumerableExt
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
			string outStr = "";
			bool nonZero = false;
			foreach (var a in enumerable)
			{
				nonZero = true;
				if (a != null) 
					outStr += a.ToString();

				outStr += delimiter;
			}

			if (nonZero)
				outStr = outStr.Remove(outStr.Length - 1);	// Remove last delimiter

			if (braces)
				outStr = "[" + outStr + "]";

			return outStr;
		}

		public static bool IsOrderedAscending<T>(this IEnumerable<T> enumerable) where T : IComparable<T>
		{
			using var enumerator = enumerable.GetEnumerator();
			if (!enumerator.MoveNext()) // Start the enumerator
				return true;	// Length 0? Current is undefined if MoveNext returns false and throws an exception. Safeguard just in case.
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
			enumerator.MoveNext();  // Start the enumerator
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

		/// <summary>Sorts an enumerable by converting it to an array and using Array.Sort</summary>
		public static IList<T> Sort<T>(this IEnumerable<T> enumerable, IComparer<T>? comparer = null) where T : IComparable<T>
		{
			T[] arr = enumerable.ToArray();
			comparer ??= Comparer<T>.Default;
			Array.Sort(arr, comparer);
			return arr;
		}

		/// <summary>Sorts a pair of enumerables based on the elements in the first enumerable</summary>
		/// <param name="enumerable">Enumerable to be sorted on</param>
		/// <param name="other">Enumerable that matches the sorting of the first enumerable</param>
		/// <param name="comparer">Optional comparer. If null the default comparer is used</param>
		public static ValueTuple<IList<T>, IList<T>> Sort<T>(this IEnumerable<T> enumerable, IEnumerable<T> other, IComparer<T>? comparer = null) where T : IComparable<T>
		{
			T[] arr = enumerable.ToArray();
			T[] otherArr = other.ToArray();
			comparer ??= Comparer<T>.Default;
			Array.Sort(arr, otherArr, comparer);
			return (arr, otherArr);
		}

		/// <summary>Convert a numeric enumerable to another numeric type using INumber<T>.CreateSaturating(TOther)</summary>
		public static IEnumerable<TOut> ToNum<T, TOut>(this IEnumerable<T> enumerable) where T : INumber<T> where TOut : INumber<TOut>
		{
			using IEnumerator<T> e = enumerable.GetEnumerator();
			while (e.MoveNext())
				yield return TOut.CreateSaturating(e.Current);
		}

		/// <summary>Convert a numeric enumerable to another numeric type using INumber<T>.CreateTruncating(TOther)</summary>
		public static IEnumerable<TOut> ToNumTruncating<T, TOut>(this IEnumerable<T> enumerable) where T : INumber<T> where TOut : INumber<TOut>
		{
			using IEnumerator<T> e = enumerable.GetEnumerator();
			while (e.MoveNext())
				yield return TOut.CreateTruncating(e.Current);
		}

		/// <summary>Convert a numeric enumerable to another numeric type using INumber<T>.CreateChecked(TOther)</summary>
		/// <exception cref="OverflowException"></exception>
		public static IEnumerable<TOut> ToNumChecked<T, TOut>(this IEnumerable<T> enumerable) where T : INumber<T> where TOut : INumber<TOut>
		{
			using IEnumerator<T> e = enumerable.GetEnumerator();
			while (e.MoveNext())
				yield return TOut.CreateChecked(e.Current);
		}

		/// <summary>Apply a condition to the elements of an enumerable and transform the element depending on whether it meets that condition or not using functions passed as arguments</summary>
		public static IEnumerable<TOut> WhereSuccessAndFail<T, TOut>(this IEnumerable<T> enumerable, Func<T, bool> condition, Func<T, TOut> success, Func<T, TOut> failure)
		{
			foreach (T e in enumerable)
				yield return condition(e) ? success(e) : failure(e);
		}

		/// <summary>Get a circular enumerator from an enumerable, which restarts the enumeration upon reaching the end</summary>
		public static CircularEnumerator<T> GetCircularEnumerator<T>(this IEnumerable<T> enumerable)
		{
			return new CircularEnumerator<T>(enumerable.GetEnumerator());
		}
	}
}
