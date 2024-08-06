using System.IO;
using Sewer56.DeltaPatchGenerator.Lib.Utility;

namespace Sewer56.Patcher.Riders.Te
{
    public class Assets
    {
        public static readonly string AssetsFolder = Path.Combine(Paths.ProgramFolder, "Assets/TE");
        public static readonly string TempFolder   = Path.Combine(Paths.ProgramFolder, "Working");
        public static readonly string HashesFolder = Path.Combine(AssetsFolder, "Hashes");

        public static readonly string OriginalHashesFolder = Path.Combine(HashesFolder, "Original ROM");
        public static readonly string ModHashesFolder      = Path.Combine(HashesFolder, "TE ROM");

        public static readonly string PatchesFolder       = Path.Combine(AssetsFolder, "Patches");
        public static readonly string ToTePatchesFolder = Path.Combine(PatchesFolder, "Vanilla to TE");
    }
}
