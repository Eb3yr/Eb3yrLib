using Eb3yrLib;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Playground
{
	public static class MemoryExtensions
	{
		extension<T>(Memory<T> memory) where T : unmanaged
		{
			public Memory<byte> AsBytes()
			{
				return new ToByteMemoryManager<T>(memory).Memory;
			}
		}

		private sealed class ToByteMemoryManager<T>(Memory<T> memory) : MemoryManager<byte> where T : unmanaged
		{
			public ToByteMemoryManager(T[] array) : this(array.AsMemory()) { }
			readonly Memory<T> _memory = memory;
			public override Span<byte> GetSpan() => MemoryMarshal.Cast<T, byte>(_memory.Span);
			public override MemoryHandle Pin(int elementIndex = 0) => throw new NotImplementedException();
			public override void Unpin() => throw new NotImplementedException();
			protected override void Dispose(bool disposing) { }
		}
	}

	public static partial class ListExtensions
	{
		extension<T>(List<T> list)
		{
			public T[] BackingArray => Accessors<T>.GetSetListArray(list);

			public Memory<T> AsMemory() => AsMemory(list, list.Count);

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
