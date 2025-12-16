using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Playground
{
	public readonly struct BitArrays
	{
		readonly BitArray[] arrays;
		readonly int chunkSize;
		public long Length { get; init; }
		public BitArrays(long length, int chunkSizeBits = 64_000)
		{
			Length = length;
			chunkSize = chunkSizeBits;
			long count = length / chunkSizeBits;
			int rem = (int)(length - count * chunkSizeBits);

			arrays = new BitArray[rem != 0 ? count + 1 : count];

			for (int i = 0; i < count; i++)
				arrays[i] = new BitArray(chunkSizeBits);

			if (rem != 0)
				arrays[^1] = new BitArray(rem);
		}

		public bool this[long idx]
		{
			get => Get(idx);
			set => Set(idx, value);
		}

		public bool Get(long index)
		{
			var (i, j) = GetIndices(index);
			return arrays[i][j];
		}

		public void Set(long index, bool value)
		{
			var (i, j) = GetIndices(index);
			arrays[i][j] = value;

		}

		private (int i, int j) GetIndices(long index)
		{
			int i = (int)(index / chunkSize);
			int j = (int)(index % chunkSize);
			return (i, j);
		}
	}
}
