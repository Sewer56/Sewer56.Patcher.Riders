#if SRDXSelfContained
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sewer56.DeltaPatchGenerator.Lib.Utility;
using Sewer56.Patcher.Riders.Common;
using Sewer56.Patcher.Riders.Dx.Utility;
using Standart.Hash.xxHash;

namespace Sewer56.Patcher.Riders.Dx
{
    public class DxSelfContainedPatch : IGamePatch
    {
        public string FileName { get; set; } = "Sonic Riders DX 2.0.1.iso";

        public byte[] Key { get; set; } = Xor.StringToKey("LB2GsjDxia6Po08yC2GoUX8oD3bhDbh");

        public ulong[] KnownHashes = new ulong[]
        {
            2859023419091715420,  // Sonic Riders DX Version 1.0.0
            11478741229583558586, // Sonic Riders DX Version 1.0.1
            12475251216528109950, // Sonic Riders DX Version 2.0.0
            3627067645686224037,  // Sonic Riders Netplay Mod 1.1.2.iso
            17309300132228947760, // Sonic Riders Netplay v1.0.0
            2870049280778264855,  // Sonic Riders NTSC-J.iso
            2542368951511572939,  // Sonic Riders NTSC-J.nkit.gcz
            10764326518125825620, // Sonic Riders NTSC-J.nkit.iso
            5856869444262374057,  // Sonic Riders NTSC-J.rvz # default settings
            12029937369744286389, // Sonic Riders NTSC-J.rvz.max # max compression
            533549964282774057,   // Sonic Riders NTSC-U.iso
            12154119975948057489, // Sonic Riders NTSC-U.nkit.gcz
            1400685689119324890,  // Sonic Riders NTSC-U.nkit.iso
            18299766616444962564, // Sonic Riders NTSC-U.rvz # default settings
            8788505761699537265,  // Sonic Riders NTSC-U.rvz.max # max compression
            3956785190308962574,  // Sonic Riders PAL.iso
            7177568273404592745,  // Sonic Riders PAL.nkit.gcz
            14285416449969503775, // Sonic Riders PAL.nkit.iso
            14370089055075455432, // Sonic Riders PAL.rvz # default settings
            3012584463499541531,  // Sonic Riders PAL.rvz.max # max compression
            802339092105715092,   // SRTE 1.2.0.iso
            1359739435430876755,  // SRTE 1.2.1.iso
            11197275888355094665, // SRTE 1.2.5.iso
            6146882366004597759,  // SRTE 1.3.0.iso
            3345955917279735753,  // SRTE 1.4.0.iso
            16530656863935718301, // SRTE 1.4.1.iso
            3387789659416232848,  // SRTE 1.4.2.iso
            1798809554536730225,  // SRTE 2.0.0.iso
        };

        public bool GetInstructionDialog(out string title, out string text)
        {
            title = "The one and only step.";
            text = "Please select a compatible GameCube ROM of Sonic Riders to patch.\n" +
                   "Supported ROMs include:\n" +
                   "- Sonic Riders (USA, Japan, Europe)\n" +
                   "- Sonic Riders DX (1.0, 1.0.1, 2.0.0)\n" +
                   "- Sonic Riders Netplay (1.0, 1.1.2)\n" +
                   "- Sonic Riders Tournament Edition (1.2, 1.2.1, 1.2.5, 1.3, 1.4, 1.4.1, 1.4.2, 2.0)";

            return true;
        }

        public async Task ApplyPatch(string isoPath, string outputPath, Events.ProgressCallback reportProgress = null)
        {
            reportProgress?.Invoke("Patching ROM from Bundle", 0.0f);
            var isoStream = new FileStream(isoPath, FileMode.Open);
            var unpackRomTask = Task.Run(() =>
            {
                var memoryStream = new MemoryStream(1459978240); // Max GCN ROM Size
                var bundleData = File.ReadAllBytes(Assets.AssetsBundlePath);

                // Unscramble.
                unsafe
                {
                    fixed (byte* bundleDataPtr = bundleData)
                    fixed (byte* keyPtr = Key)
                    {
                        Xor.Xor32(keyPtr, bundleDataPtr, bundleData.Length);
                    }
                }

                // Decompress
                Compression.Decompress(bundleData, memoryStream, new Progress<double>(d =>
                {
                    reportProgress?.Invoke("Patching ROM from Bundle", d);
                }));

                return memoryStream;
            });

            const int bufferSize = 1024 * 1024 * 64;
            var hash = await xxHash64.ComputeHashAsync(isoStream, bufferSize);

            if (!KnownHashes.Contains(hash))
                throw new Exception("Unsupported ROM: Provided ROM to patch is not a supported ROM.");

            var romStream = await unpackRomTask;

            // Truncate Memory Stream.
            var romSize = romStream.Position;
            romStream.Position = 0;
            romStream.SetLength(romSize);

            await using var outputStream = new FileStream(outputPath, FileMode.Create);
            await romStream.CopyToAsync(outputStream);
        }
    }
}
#endif