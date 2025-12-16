using System;
using System.Collections.Generic;
using System.Text;

namespace Eb3yrLib.Extensions
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
    }
}
