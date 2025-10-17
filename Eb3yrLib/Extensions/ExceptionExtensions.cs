using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace Eb3yrLib.Extensions
{
	public static class ExceptionExtensions
	{
		extension(ArgumentException)
		{
			public static void ThrowIf([DoesNotReturnIf(true)] bool condition, string? msg = null, string? paramName = null)
			{
				if (condition) throw new ArgumentException(msg, paramName);
			}
		}
	}
}
