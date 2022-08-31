using System.IO;
using Sewer56.DeltaPatchGenerator.Lib.Utility;

namespace Sewer56.Patcher.Riders.Dx
{
    public class Assets
    {
        public static readonly string AssetsFolder = Path.Combine(Paths.ProgramFolder, "Assets/DX");
        public static readonly string TempFolder   = Path.Combine(Paths.ProgramFolder, "Working");
        public static readonly string HashesFolder = Path.Combine(AssetsFolder, "Hashes");

        public static readonly string OriginalHashesFolder = Path.Combine(HashesFolder, "Original ROM");
        public static readonly string ModHashesFolder      = Path.Combine(HashesFolder, "DX ROM");

        public static readonly string PatchesFolder       = Path.Combine(AssetsFolder, "Patches");
        public static readonly string ToDxPatchesFolder = Path.Combine(PatchesFolder, "Vanilla to DX");

#if SRDXSelfContained
        public static readonly string AssetsBundlePath = Path.Combine(Paths.ProgramFolder, "Assets/Bundle.patch");
#endif
    }
}
