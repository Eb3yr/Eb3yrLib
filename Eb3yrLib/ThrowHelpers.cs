using System.Diagnostics.CodeAnalysis;

namespace Eb3yrLib
{
	/// <summary>
	/// A collection of functions to throw exceptions. Intended for use in high-performance cases
	/// </summary>
	public static class ThrowHelpers
	{
		[DoesNotReturn]
		public static void ThrowArgumentException(string? msg = null, string? paramName = null)
		{
			throw new ArgumentException(msg, paramName);
		}

		[DoesNotReturn]
		public static void ThrowNotSupportedException(string? msg = null)
		{
			throw new NotSupportedException(msg);
		}
	}
}
