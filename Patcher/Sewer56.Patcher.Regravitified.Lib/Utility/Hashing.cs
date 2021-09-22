using System.IO;
using Standart.Hash.xxHash;

namespace Sewer56.Patcher.Regravitified.Lib.Utility
{
    public static class Hashing
    {
        public const int HashBufferSize = 50_000_000; // 50 MiB

        public static ulong CalculateHash(Stream stream, ulong seed = 0) => xxHash64.ComputeHash(stream, HashBufferSize, seed);

        public static ulong CalculateHash(string filePath, ulong seed = 0)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open);
            return xxHash64.ComputeHash(fileStream, HashBufferSize, seed);
        }
    }
}
