using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eb3yrLib
{
	/// <summary>
	/// A collection of functions to throw exceptions. Intended for use in high-performance, short functions to minimise function size
	/// </summary>
	public static class ThrowHelper
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
