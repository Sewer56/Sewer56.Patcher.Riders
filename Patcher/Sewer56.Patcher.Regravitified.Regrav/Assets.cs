using System.IO;
using Sewer56.Patcher.Regravitified.Lib.Utility;

namespace Sewer56.Patcher.Regravitified.Regrav
{
    public class Assets
    {
        public static readonly string AssetsFolder = Path.Combine(Paths.ProgramFolder, "Assets");

        public static readonly string TempFolder = Path.Combine(Paths.ProgramFolder, "Working");

        public static readonly string HashesFolder = Path.Combine(AssetsFolder, "Hashes");

        public static readonly string UsHashesFolder = Path.Combine(HashesFolder, "US 1.0.1");

        public static readonly string RegravHashesFolder = Path.Combine(HashesFolder, "Regrav");

        public static readonly string PatchesFolder = Path.Combine(AssetsFolder, "Patches");

        public static readonly string ToWbfsPatchesFolder = Path.Combine(PatchesFolder, "To WBFS");

        public static readonly string ToUsPatchesFolder = Path.Combine(PatchesFolder, "To US");

        public static readonly string ToRegravPatchesFolder = Path.Combine(PatchesFolder, "To Regrav");
    }
}
