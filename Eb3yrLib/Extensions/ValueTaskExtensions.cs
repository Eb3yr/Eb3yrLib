using System;
using System.Collections.Generic;
using System.Text;

namespace Eb3yrLib.Extensions
{
	public static class ValueTaskExtensions
	{
		public static TResult WaitBlocking<TResult>(this ValueTask<TResult> task)
		{
			while (!task.IsCompleted)
				;

			return task.Result;
		}
	}
}
