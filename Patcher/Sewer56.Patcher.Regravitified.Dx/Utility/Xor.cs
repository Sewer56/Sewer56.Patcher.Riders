#if SRDXSelfContained
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Sewer56.Patcher.Riders.Dx.Utility;

/// <summary>
/// Implementation of XOR over a 32-byte array.
/// </summary>
public static class Xor
{
    /// <summary>
    /// Converts a string to a given key.
    /// </summary>
    public static byte[] StringToKey(string text)
    {
        var textBytes = Encoding.ASCII.GetBytes(text);
        var key = new byte[32];
        textBytes.AsSpan().CopyTo(key);
        return key;
    }

    /// <summary>
    /// Performs a 32-byte XOR of the given data.
    /// </summary>
    /// <param name="key">Key to use. Should be 32 bytes.</param>
    /// <param name="data">Data to XOR in-place.</param>
    /// <param name="dataLength">Length of data to XOR.</param>
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static unsafe void Xor32(byte* key, byte* data, int dataLength)
    {
        if (Sse3.IsSupported)
            Xor32_Sse3(key, data, dataLength);
        else if (Avx2.IsSupported)
            Xor32_Avx2(key, data, dataLength);
        else
            Xor32_Generic(key, data, dataLength);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static unsafe void Xor32_Sse3(byte* key, byte* data, int dataLength)
    {
        const int UnrollFactor = 2;
        const int SseRegisterSize = 16;
        const int DoubleRegisterSize = SseRegisterSize * 2;

        var xorPattern_1 = Sse3.LoadDquVector128(key);
        var xorPattern_2 = Sse3.LoadDquVector128(key + SseRegisterSize);

        var iterationCount = (dataLength / DoubleRegisterSize) / UnrollFactor;
        var originalDataPtr = data;
        
        // XOR Everything.
        for (int x = 0; x < iterationCount; x++)
        {
            var existingBytes_1_1 = Sse3.LoadDquVector128(data);
            var existingBytes_1_2 = Sse3.LoadDquVector128(data + SseRegisterSize);

            var existingBytes_2_1 = Sse3.LoadDquVector128(data + DoubleRegisterSize);
            var existingBytes_2_2 = Sse3.LoadDquVector128(data + DoubleRegisterSize + SseRegisterSize);

            existingBytes_1_1 = Sse2.Xor(existingBytes_1_1, xorPattern_1);
            existingBytes_1_2 = Sse2.Xor(existingBytes_1_2, xorPattern_2);
            existingBytes_2_1 = Sse2.Xor(existingBytes_2_1, xorPattern_1);
            existingBytes_2_2 = Sse2.Xor(existingBytes_2_2, xorPattern_2);

            Sse2.Store(data, existingBytes_1_1);
            Sse2.Store(data + SseRegisterSize, existingBytes_1_2);
            Sse2.Store(data + DoubleRegisterSize, existingBytes_2_1);
            Sse2.Store(data + DoubleRegisterSize + SseRegisterSize, existingBytes_2_2);

            data += (DoubleRegisterSize * UnrollFactor);
        }

        // Handle remaining bytes.
        var bytesProcessed = data - originalDataPtr;
        Xor32_Generic(key, data, (int)(dataLength - bytesProcessed));
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static unsafe void Xor32_Avx2(byte* key, byte* data, int dataLength)
    {
        const int UnrollFactor = 4;
        const int AvxRegisterSize = 32;

        var xorPattern = Avx.LoadDquVector256(key);
        var iterationCount  = (dataLength / AvxRegisterSize) / UnrollFactor;
        var originalDataPtr = data;

        // XOR Everything.
        for (int x = 0; x < iterationCount; x++)
        {
            var existingBytes_1 = Avx.LoadDquVector256(data);
            var existingBytes_2 = Avx.LoadDquVector256(data + (AvxRegisterSize));
            var existingBytes_3 = Avx.LoadDquVector256(data + (AvxRegisterSize * 2));
            var existingBytes_4 = Avx.LoadDquVector256(data + (AvxRegisterSize * 3));
            existingBytes_1 = Avx2.Xor(existingBytes_1, xorPattern);
            existingBytes_2 = Avx2.Xor(existingBytes_2, xorPattern);
            existingBytes_3 = Avx2.Xor(existingBytes_3, xorPattern);
            existingBytes_4 = Avx2.Xor(existingBytes_4, xorPattern);
            Avx.Store(data, existingBytes_1);
            Avx.Store(data + (AvxRegisterSize), existingBytes_2);
            Avx.Store(data + (AvxRegisterSize * 2), existingBytes_3);
            Avx.Store(data + (AvxRegisterSize * 3), existingBytes_4);

            data += (AvxRegisterSize * UnrollFactor);
        }

        // Handle remaining bytes.
        var bytesProcessed = data - originalDataPtr;
        Xor32_Generic(key, data, (int)(dataLength - bytesProcessed));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static unsafe void Xor32_Generic(byte* key, byte* data, int dataLength)
    {
        const int UnrollFactor = 4;
        const int RegisterSize = sizeof(long);
        
        var xorPattern_1 = *(long*)key;
        var xorPattern_2 = *(long*)(key + RegisterSize);
        var xorPattern_3 = *(long*)(key + RegisterSize * 2);
        var xorPattern_4 = *(long*)(key + RegisterSize * 3);

        var iterationCount = (dataLength / RegisterSize / UnrollFactor);
        var originalDataPtr = data;

        // XOR Everything.
        for (int x = 0; x < iterationCount; x++)
        {
            var existingBytes_1 = *(long*)data;
            var existingBytes_2 = *(long*)(data + RegisterSize);
            var existingBytes_3 = *(long*)(data + RegisterSize * 2);
            var existingBytes_4 = *(long*)(data + RegisterSize * 3);
            existingBytes_1 ^= xorPattern_1;
            existingBytes_2 ^= xorPattern_2;
            existingBytes_3 ^= xorPattern_3;
            existingBytes_4 ^= xorPattern_4;
            *(long*)data = existingBytes_1;
            *(long*)(data + RegisterSize) = existingBytes_2;
            *(long*)(data + RegisterSize * 2) = existingBytes_3;
            *(long*)(data + RegisterSize * 3) = existingBytes_4;

            data += (RegisterSize * UnrollFactor);
        }

        // XOR Remainder with byte XOR
        var bytesProcessed = data - originalDataPtr;
        var remainingBytes = (int)(dataLength - bytesProcessed);
        var dataEnd = data + remainingBytes;

        while (data < dataEnd)
        {
            *data = (byte)(*data ^ *key);
            data++;
            key++;
        }
    }
}
#endif