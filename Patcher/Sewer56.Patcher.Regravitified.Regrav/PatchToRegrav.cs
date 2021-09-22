using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Sewer56.Patcher.Regravitified.Lib;
using Sewer56.Patcher.Regravitified.Lib.Model;
using Sewer56.Patcher.Regravitified.Lib.Utility;

namespace Sewer56.Patcher.Regravitified.Regrav
{
    public static class PatchToRegrav
    {
        /// <param name="isoPath">Folder to the ISO to be patched.</param>
        /// <param name="outputPath">The path to save the ISO to.</param>
        /// <param name="reportProgress">Function that receives information on the current progress.</param>
        public static async Task Patch(string isoPath, string outputPath, Events.ProgressCallback reportProgress = null)
        {
            bool isNkit       = NKit.IsNkit(isoPath);
            var reporter      = new ProgressReporter(CalculateNumberOfSteps(isNkit) - 1, 0, reportProgress);
            var tempFolder    = Assets.TempFolder;
            IOEx.TryEmptyDirectory(Assets.TempFolder);

            // Convert to ISO if necessary
            if (isNkit)
                await ExtractNKit(isoPath, tempFolder, reporter);
            else
                await ExtractISO(isoPath, tempFolder, reporter);

            // Apply format patches (ISO -> WBFS)
            reporter.Report("Patching ISO -> WBFS");
            Lib.Patch.Apply(PatchData.FromDirectories(Assets.ToWbfsPatchesFolder).ToArray().AsSpan(), tempFolder, tempFolder);
            
            reporter.Report("Patching Region to NTSC-U");
            Lib.Patch.Apply(PatchData.FromDirectories(Assets.ToUsPatchesFolder).ToArray().AsSpan(), tempFolder, tempFolder);
            
            reporter.Report("Verifying Clean NTSC-U Copy");
            if (!Lib.HashSet.Verify(FileHashSet.FromDirectory(Assets.UsHashesFolder), tempFolder, out var missingFiles, out var mismatchFiles))
            {
                ThrowHelpers.ThrowVerificationFailed("Failed to Verify Clean NTSC-U Copy Post Patching.\n" +
                                        "Most likely this means one of the following: \n" +
                                        "- Your ROM is a bad dump.\n" +
                                        "- Your ROM is from an unsupported region.\n" +
                                        "- Your ROM is uses an unsupported format.\n", missingFiles, mismatchFiles);
            }

            // Apply Regrav patch.
            reporter.Report("Patching NTSC-U -> Regravitified");
            Lib.Patch.Apply(PatchData.FromDirectories(Assets.ToRegravPatchesFolder).ToArray().AsSpan(), tempFolder, tempFolder);

            // Remove Redundant Files
            var regravHashes = FileHashSet.FromDirectory(Assets.RegravHashesFolder);
            HashSet.Cleanup(regravHashes, tempFolder);

            // Verify Regrav patch.
            reporter.Report("Verifying Regravitified Copy");
            if (!Lib.HashSet.Verify(regravHashes, tempFolder, out missingFiles, out mismatchFiles))
            {
                ThrowHelpers.ThrowVerificationFailed("Failed to Verify Clean Regravitified ROM.\n" + 
                                        "This most likely indicates a bad patch file, or an error in the code.\n",
                                        missingFiles, mismatchFiles);
            }

            // Repack ROM
            reporter.Report("Rebuilding WBFS");
            await Wit.Build(new Wit.BuildOptions()
            {
                Source = tempFolder,
                Target = outputPath
            });
            
            Directory.Delete(tempFolder, true);
            reporter.Report("Done");
        }

        private static async Task ExtractISO(string isoPath, string isoOutputPath, ProgressReporter reporter)
        {
            reporter.Report("Extracting ISO");
            await Wit.Extract(new Wit.ExtractOptions()
            {
                Source = isoPath,
                DataPartitionOnly = true,
                Target = isoOutputPath
            });
        }

        private static async Task ExtractNKit(string isoPath, string isoOutputPath, ProgressReporter reporter)
        {
            reporter.Report("Converting NKit to ISO");
            var convertedPath = Path.Combine(isoOutputPath, "nkit.iso");
            await NKit.Convert(new NKit.ConvertOptions()
            {
                Source = isoPath,
                Target = convertedPath
            });

            reporter.Report("Extracting ISO");
            await ExtractISO(convertedPath, isoOutputPath, reporter);
            File.Delete(convertedPath);
        }

        private static int CalculateNumberOfSteps(bool isNkit)
        {
            int extraSteps = isNkit ? 1 : 0;
            return 7 + extraSteps;
        }
    }
}
