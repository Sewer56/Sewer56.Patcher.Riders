#if SRDXSelfContained
using System;
using System.Linq;
using Sewer56.Patcher.Riders.Dx.Utility;
using Standart.Hash.xxHash;
using Xunit;

namespace Sewer56.Patcher.Riders.Tests;

public class XorTests
{
    [Fact]
    public unsafe void XorMethods_ProduceEqualResults()
    {
        var key = Xor.StringToKey("It is about time, deez nutz!!");
        var data = CreateRandomArray(12345);
        var data_avx = data.ToArray();
        var data_sse = data.ToArray();

        var oldHash = xxHash64.ComputeHash(data);

        fixed (byte* dataPtr = &data[0])
        fixed (byte* dataSsePtr = &data_avx[0])
        fixed (byte* dataAvxPtr = &data_sse[0])
        fixed (byte* keyPtr = &key[0])
        {
            Xor.Xor32_Generic(keyPtr, dataPtr, data.Length);
            Xor.Xor32_Sse3(keyPtr, dataSsePtr, data.Length);
            Xor.Xor32_Avx2(keyPtr, dataAvxPtr, data.Length);

            // Check all are equal.
            Assert.Equal(data, data_avx);
            Assert.Equal(data, data_sse);

            // Undo and assert for equal again.
            Xor.Xor32_Generic(keyPtr, dataPtr, data.Length);
            Xor.Xor32_Sse3(keyPtr, dataSsePtr, data.Length);
            Xor.Xor32_Avx2(keyPtr, dataAvxPtr, data.Length);

            Assert.Equal(data, data_avx);
            Assert.Equal(data, data_sse);

            // Compare hashes
            Assert.Equal(oldHash, xxHash64.ComputeHash(data));
        }
    }

    private static byte[] CreateRandomArray(int size)
    {
        var data = new byte[size];
        var random = new Random();

        for (int x = 0; x < data.Length; x++)
            data[x] = (byte)random.Next(0, byte.MaxValue);

        return data;
    }
}
#endif