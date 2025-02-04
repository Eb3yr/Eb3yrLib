﻿using System.Collections;

// Present in this namespace instead of Collections because of GetCircularEnumerator in IEnumerableExt.cs
namespace Eb3yrLib.Extensions
{
	/// <summary>This is stupid. Why did I this?</summary>
	public class CircularEnumerator<T> : IEnumerator<T>
	{
		public CircularEnumerator(IEnumerator<T> enumerator) => _enumerator = enumerator;

		private readonly IEnumerator<T> _enumerator;

		public T Current => _enumerator.Current;

		object IEnumerator.Current => _enumerator.Current!;

		public void Dispose()
		{
			_enumerator.Dispose();
			GC.SuppressFinalize(this);
		}

		public bool MoveNext()
		{
			if (_enumerator.MoveNext())
				return true;

			_enumerator.Reset();
			return _enumerator.MoveNext();  // If the enumerator is empty then it'll return false here. 
		}
		public void Reset() => _enumerator.Reset();

		/// <summary>Enumerate once from the first value. Resets the enumerator position.</summary>
		public IEnumerable<T> EnumerateOnce()
		{
			_enumerator.Reset();
			while (_enumerator.MoveNext())
				yield return _enumerator.Current;
		}

		public IEnumerable<T> EnumerateMany(int count)
		{
			for (; count > 0; count--)
			{
				_enumerator.Reset();
				while (_enumerator.MoveNext())
					yield return _enumerator.Current;
			}
		}
	}
}
