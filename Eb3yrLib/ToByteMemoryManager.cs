using System.Buffers;
using System.Runtime.InteropServices;

namespace Eb3yrLib
{
	public sealed class ToByteMemoryManager<T>(Memory<T> memory) : MemoryManager<byte> where T : unmanaged
	{
		public ToByteMemoryManager(T[] array) : this(array.AsMemory()) { }
		readonly Memory<T> _memory = memory;
		public override Span<byte> GetSpan() => MemoryMarshal.Cast<T, byte>(_memory.Span);
		public override MemoryHandle Pin(int elementIndex = 0) => throw new NotImplementedException();
		public override void Unpin() => throw new NotImplementedException();
		protected override void Dispose(bool disposing) { }
	}
}
