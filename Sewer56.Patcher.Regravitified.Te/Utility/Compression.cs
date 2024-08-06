#if SRDXSelfContained
using System;
using System.IO;
using System.Threading;
using Sewer56.Patcher.Riders.Common.Utility;
using ZstdSharp;

namespace Sewer56.Patcher.Riders.Dx.Utility;

public class Compression
{
    /// <summary>
    /// Compresses the given bytes.
    /// </summary>
    public static Memory<byte> Compress(byte[] input, IProgress<double> progress)
    {
        const int reportSize = 4194304;
        var compOptions  = new Compressor(Compressor.MaxCompressionLevel);
        using var outputStream = new MemoryStream(input.Length);
        
        // Write compressed size
        outputStream.Write(BitConverter.GetBytes(input.Length));

        // Block is necessary to ensure compressor data is completely flushed
        using (var compressor = new CompressionStream(outputStream, compOptions))
        {
            using var inputStream = new MemoryStream(input);
            inputStream.CopyToEx(compressor, reportSize, progress, null);
        }

        return outputStream.GetBuffer().AsMemory(0, (int)outputStream.Length);
    }

    /// <summary>
    /// Decompresses the given bytes.
    /// </summary>
    public static void Decompress(byte[] input, Stream output, IProgress<double> progress, CancellationToken token = default)
    {
        const int bufferSize   = 4194304;

        var compOptions        = new Decompressor();
        using var inputStream  = new MemoryStream(input);

        // Read decompressed size.
        var buf = GC.AllocateUninitializedArray<byte>(4);
        inputStream.Read(buf);
        var decompressedSize = BitConverter.ToInt32(buf);

        // Decompress
        using var decompressor = new DecompressionStream(inputStream, compOptions);
        decompressor.CopyToEx(output, bufferSize, progress, decompressedSize, token);
    }
}
#endif