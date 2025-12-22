using Playground;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Console;

//FindSetOfInts.Run();
//await FindSetOfInts.RunAsync();

//WriteLine(UnsafeFoo.GetInstanceSizeOfManagedType<Foo>());
var sw = Stopwatch.StartNew();
BinaryInt32Intersection.Intersect(FindSetOfInts.Path1, FindSetOfInts.Path2, FindSetOfInts.PathOut);
sw.Stop();

WriteLine($"Completed after {sw.ElapsedMilliseconds / 1000f}s.");

WriteLine("Done");
ReadLine();

public static class UnsafeFoo
{

	public static T BastardNativeAllocator<T, TSpan>(Span<TSpan> span) where T : class where TSpan : unmanaged
		=> BastardNativeAllocator<T>(MemoryMarshal.Cast<TSpan, byte>(span));    // This is probably a really shit idea, but it's convenient.

	public static unsafe T BastardNativeAllocator<T>(Span<byte> span) where T : class
	{
		int size = GetInstanceSizeOfManagedType<T>();
		if (span.Length < size)
			throw new ArgumentException($"Span is too short for type {nameof(T)}. The provided Span has length {span.Length}, which is less than {size} required for {nameof(T)}.");

		T val;
		fixed (void* ptr = span)
		{
			val = BastardNativeAllocator<T>(ptr, size);
		}
		return val;
	}

	public static unsafe T BastardNativeAllocator<T>(void* ptr, int typeSize) where T : class
	{
		// We're interested in getting a new T out of the unmanaged block of memory - either on the unmanaged heap or the stack.
		// The problem is getting the ctor. How do I manage passing stuff to that? I don't want to use classes, I want to avoid allocs.
		// If I have to alloc a delegate every time that's ass.

		// This could be really fucking funny with an arena allocator written in C#. No need to fk around with the GC.

		// First 8 bytes are sync block, we'll leave this uninitialised.
		// Then 8 bytes are the methodtable pointer
		// Then the rest is the type itself, and padding (if used)
		
		nint* nptr = (nint*)ptr;
		*(nptr + 0) = nint.Zero;
		*(nptr + 1) = typeof(T).TypeHandle.Value;

		return Unsafe.As<nint, T>(ref *(nptr + 1)); // I don't understand why we add 1 but praise be https://dev.to/maximiliysiss/bending-net-how-to-stack-allocate-reference-types-in-c-73g
		// See warning about it never working correctly from EgorBo in the comments there lol
	}

	// I could probably build some sort of abstraction that uses a frozen dictionary once the size stops changing frequently. Who cares though.
	private static readonly Dictionary<Type, int> _instanceSizes = [];
	public static int GetInstanceSizeOfManagedType<T>() where T : class
	{
		if (_instanceSizes.TryGetValue(typeof(T), out int val))
			return val;

		// Not cached, compute it now.
		// Either we can use reflection, some weird stuff that may or may not exist, or some hacky thing with the GC after instantiating one.
		// We'll do the latter now and rethink life later.

		int size;

		size = -1;	// DELETE THIS

		if (typeof(T).IsAutoLayout)
		{
			// I believe we can just run through each property and add its Unsafe.SizeOf ?
			// If it were sequential then we'd be baked.
		}
		else if (typeof(T).IsLayoutSequential)
		{
			// It pads to the largest, up to 8 bytes. We need to do one pass to find the largest, then another to calculate the size.
		}
		else if (typeof(T).IsExplicitLayout)
		{
			throw new NotSupportedException("Does not support explicit class layouts.");
		}
		else
		{
			throw new ArgumentException("Class is neither auto layout, sequential layout, or explicit layout. How did we get here?");
		}

		// https://devblogs.microsoft.com/premier-developer/managed-object-internals-part-4-fields-layout/
		// !!!

		size += 2 * nint.Size;	// object header
		_instanceSizes.Add(typeof(T), size);

		throw new NotImplementedException();
		return size;
	}
}

public class Foo
{
	public long l1 = 3;
	public int i2 = -7;
}


// This is the solution ChatGPT eventually settled on after trying and critiquing bad ones several times. It tried to external sort!
public static class BinaryInt32Intersection
{
	public static unsafe void Intersect(string aPath, string bPath, string outputPath)
	{
		const long BitCount = 1L << 32;      // 2^32
		const long ByteCount = BitCount / 8; // 512MB

		// Allocate bitset (512MB)
		byte[] bitset = new byte[ByteCount];

		// 1️⃣ Read A and mark presence
		using (var fs = new FileStream(aPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 20))
		using (var br = new BinaryReader(fs))
		{
			while (fs.Position < fs.Length)
			{
				int value = br.ReadInt32();
				uint index = (uint)(value - int.MinValue);
				bitset[index >> 3] |= (byte)(1 << (int)(index & 7));
			}
		}

		// 2️⃣ Read B, emit intersection, clear bits to avoid duplicates
		using (var fs = new FileStream(bPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 20))
		using (var br = new BinaryReader(fs))
		using (var outFs = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 20))
		using (var bw = new BinaryWriter(outFs))
		{
			while (fs.Position < fs.Length)
			{
				int value = br.ReadInt32();
				uint index = (uint)(value - int.MinValue);

				byte mask = (byte)(1 << (int)(index & 7));
				ref byte cell = ref bitset[index >> 3];

				if ((cell & mask) != 0)
				{
					bw.Write(value);
					cell &= (byte)~mask; // prevent duplicate output
				}
			}
		}
	}
}