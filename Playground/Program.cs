using Playground;
using Eb3yrLib;
using Eb3yrLib.Extensions;
using Eb3yrLib.Mathematics;
using Eb3yrLib.Collections;
using Eb3yrLib.Aerodynamics;
using Eb3yrLib.Trees;
using System.Collections;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Numerics.Tensors;
using System.Diagnostics;

Span<char> cc = new char[64];
for (int i = 0; i < cc.Length; i++)
{
	cc[i] = (char)(i + 161);
}

Console.WriteLine(cc.ToArray().ToFormattedString());

for (int i = 0; i < cc.Length; i++)
{
	int j = IndexOfSimd(new string(cc), cc[i]);
	Console.WriteLine($"char: {cc[i]}, i: {i}, j: {j}, at j: {(j > -1 ? cc[j] : "notfound")}");
}

static unsafe int IndexOfSimd(string str, char value)
{
	int i = 0;
	fixed (char* ptr = str)
	{
		if (str.Length <= 64)
			goto Switch;

		Vector<ushort> vec;
		Vector<ushort> values = Vector.Create<ushort>(value);
		for (; i < str.Length - Vector<ushort>.Count; i += Vector<ushort>.Count)
		{
			vec = Unsafe.Read<Vector<ushort>>(ptr + i);
			if (Vector.EqualsAny(vec, values))
			{
				int max = i + Vector<ushort>.Count;
				while (i < max)
				{
					//if (cptr[i++] == value) return i;
					//if (cptr[i++] == value) return i;
					//if (cptr[i++] == value) return i;
					//if (cptr[i++] == value) return i;
					if (ptr[i++] == value ||
						ptr[i++] == value ||
						ptr[i++] == value ||
						ptr[i++] == value)
						return i - 1;
				}
			}
		}

		Switch:
		int remaining = str.Length - i;
		while (remaining > 0)
		{
			switch (remaining & 3)
			{
				case 0:
					if (ptr[i++] == value) return i - 1;
					goto case 3;
				case 3:
					if (ptr[i++] == value) return i - 1;
					goto case 2;
				case 2:
					if (ptr[i++] == value) return i - 1;
					goto case 1;
				case 1:
					if (ptr[i++] == value) return i - 1;
					break;
			}
			remaining -= 4;
		}
	}
	return -1;
}

Console.WriteLine("Done");
Console.ReadLine();

namespace Playground
{

}

// SIMD the particle sim code
// Can do maths on groups of four, then non-vectorised between the group of 4-8 for each batch loaded into a vector