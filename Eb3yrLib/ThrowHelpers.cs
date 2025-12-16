using System.Diagnostics.CodeAnalysis;

namespace Eb3yrLib
{
	/// <summary>
	/// A collection of functions to throw exceptions. Intended for use in high-performance cases
	/// </summary>
	public static class ThrowHelpers
	{
		[DoesNotReturn]
		public static void ThrowArgumentException(string? msg = null, string? paramName = null) => throw new ArgumentException(msg, paramName);

		[DoesNotReturn]
		internal static int ThrowArgumentOutOfRange(string? message = null, Exception? innerException = null) => throw new ArgumentOutOfRangeException(message, innerException);

		[DoesNotReturn]
		internal static void ThrowNotSupported(string? message = null, Exception? innerException = null) => throw new NotSupportedException(message, innerException);
		}
}
