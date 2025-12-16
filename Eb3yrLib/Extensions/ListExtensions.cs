using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Eb3yrLib.Extensions
{
	public static partial class ListExtensions
	{
		extension<T>(List<T> list)
		{
			/// <summary>Gets the backing array of the list.</summary>
			/// <remarks>Has the length list.Capacity, not list.Count.</remarks>
			public T[] BackingArray => Accessors<T>.GetSetListArray(list);

			public Memory<T> AsMemory() => AsMemory(list, list.Count);

			/// <param name="size">Length of the returned Memory<T>. When unspecified, AsMemory() uses list.Count.</param>
			public Memory<T> AsMemory(int size) => new(Accessors<T>.GetSetListArray(list), 0, size);
		}

		extension<T>(List<T> list) where T : unmanaged
		{
			public Memory<byte> AsBytes() => list.AsMemory().AsBytes();
			public Memory<byte> AsBytes(int size) => list.AsMemory(size).AsBytes();
		}

		private static class Accessors<T>
		{
			[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_items")]
			public extern static ref T[] GetSetListArray(List<T> list);
		}
    }
}
