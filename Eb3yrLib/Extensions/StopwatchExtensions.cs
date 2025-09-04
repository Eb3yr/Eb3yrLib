using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Eb3yrLib.Extensions
{
	public static class StopwatchExtensions
	{
		extension (Stopwatch stopwatch)
		{
			public double ElapsedMicroseconds
			{
				get => (double)stopwatch.ElapsedTicks / Stopwatch.Frequency * 1_000_000;
			}
		}
	}
}
