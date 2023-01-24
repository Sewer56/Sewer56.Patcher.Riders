using System;
using System.Buffers;
using System.IO;
using System.Threading;

namespace Sewer56.Patcher.Riders.Common.Utility;

/// <summary>
/// Miscellaneous extensions to help work with streams.
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Reads from a given stream into an existing buffer and writes out to the other stream.
    /// </summary>
    /// <param name="source">Where to copy the data from.</param>
    /// <param name="destination">Where to copy the data to.</param>
    /// <param name="buffer">The buffer to use for copying.</param>
    public static int CopyBufferedTo(this Stream source, Stream destination, byte[] buffer)
    {
        var bytesCopied = source.Read(buffer);
        destination.Write(buffer, 0, bytesCopied);
        return bytesCopied;
    }

    /// <summary>
    /// Copies data from a stream to another stream asynchronously, with support for 
    /// </summary>
    /// <param name="source">Where to copy the data from.</param>
    /// <param name="destination">Where to copy the data to.</param>
    /// <param name="bufferSize">Size of chunks used to copy from source to destination.</param>
    /// <param name="progress">Can be used to report current copying progress.</param>
    /// <param name="contentLength">Length of content to be downloaded. Provide this if source stream doesn't support length property but it is known.</param>
    public static void CopyToEx(this Stream source, Stream destination, int bufferSize = 262144, IProgress<double>? progress = null, long? contentLength = null, CancellationToken? cancellationToken = default)
    {
        using var buffer = new ArrayRental<byte>(bufferSize);
        var totalBytesCopied = 0L;
        int bytesCopied;

        bool supportsLength = true;
        long length = 0;

        if (!contentLength.HasValue)
        {
            try
            {
                length = source.Length;
                if (length == 0)
                    length = 1; // just in case.
            }
            catch (Exception) { supportsLength = false; }
        }
        else
        {
            length = contentLength.Value;
        }

        do
        {
            bytesCopied = source.CopyBufferedTo(destination, buffer.Array);
            totalBytesCopied += bytesCopied;
            if (supportsLength)
                progress?.Report((double)totalBytesCopied / length);

            if (cancellationToken is { IsCancellationRequested: true })
                break;
        }
        while (bytesCopied > 0);

        progress?.Report(1.0);
    }

    /// <summary>
    /// Copies data from a stream to another stream asynchronously, with support for progress reporting.
    /// </summary>
    /// <param name="source">Where to copy the data from.</param>
    /// <param name="destination">Where to copy the data to.</param>
    /// <param name="bufferSize">Size of chunks used to copy from source to destination.</param>
    /// <param name="progress">Can be used to report current copying progress.</param>
    public static void CopyToEx(this Stream source, Stream destination, int bufferSize = 262144, IProgress<double>? progress = null, CancellationToken? cancellationToken = default)
    {
        CopyToEx(source, destination, bufferSize, progress, null, cancellationToken);
    }
}

/// <summary>
/// Allows you to temporarily rent an amount of memory from a shared pool.
/// Use with the `using` statement.
/// </summary>
/// <typeparam name="T"></typeparam>
public struct ArrayRental<T> : IDisposable
{
    /// <summary>
    /// The rented array of data.
    /// </summary>
    public T[] Array { get; }

    /// <summary>
    /// Rents an Array from a shared pool.
    /// </summary>
    /// <param name="minimumLength">Minimum length to rent.</param>
    public ArrayRental(int minimumLength) => Array = ArrayPool<T>.Shared.Rent(minimumLength);

    /// <summary>
    /// Returns the data back to the pool.
    /// </summary>
    public void Dispose() => ArrayPool<T>.Shared.Return(Array);
}