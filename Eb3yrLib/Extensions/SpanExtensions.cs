using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Eb3yrLib.Extensions
{
	public static class SpanExtensions
	{
		extension<T>(ReadOnlySpan<T> span) where T : IComparable<T>
		{
			public bool IsOrdered(bool ascending = true)
			{
				if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
				{
					return IsOrderedImpl.Fallback(span, ascending);
				}

				// May be vectorisable. 512, 256, 128, remainder pattern?


				return IsOrderedImpl.Fallback(span, ascending);
				throw new NotImplementedException();
			}
		}
	}

	file static class IsOrderedImpl
	{
		public static bool Fallback<T>(ReadOnlySpan<T> span, bool ascending) where T : IComparable<T>
		{
            int compareVal = ascending ? 1 : -1;
            for (int i = 1; i < span.Length; i++)
            {
                if (span[i - 1].CompareTo(span[i]) == compareVal)
                    return false;
            }
            return true;
        }
	}
}
