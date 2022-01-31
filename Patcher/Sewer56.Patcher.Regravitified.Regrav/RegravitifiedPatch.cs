using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Sewer56.DeltaPatchGenerator.Lib;
using Sewer56.DeltaPatchGenerator.Lib.Model;
using Sewer56.DeltaPatchGenerator.Lib.Utility;
using Sewer56.Patcher.Riders.Common;
using Sewer56.Patcher.Riders.Common.Utility;

namespace Sewer56.Patcher.Riders.Regrav
{
    public class RegravitifiedPatch : IGamePatch
    {
        public string FileName { get; set; } = "Sonic Riders Regravitified.wbfs";

        public bool GetInstructionDialog(out string title, out string text)
        {
            title = "The one and only step.";
            text  = "Please select a Wii ROM of Sonic Riders: Zero Gravity to patch.\n" +
                    "Once started, the patching process will take up to 5 minutes.\n" +
                    "10GB of free space on Primary Drive (usually C) is required.";

            return true;
        }

        /// <param name="isoPath">Folder to the ISO to be patched.</param>
        /// <param name="outputPath">The path to save the ISO to.</param>
        /// <param name="reportProgress">Function that receives information on the current progress.</param>
        public async Task ApplyPatch(string isoPath, string outputPath, Events.ProgressCallback reportProgress = null)
        {
            bool isNkit       = NKit.IsNkit(isoPath);
            var reporter      = new ProgressReporter(CalculateNumberOfSteps(isNkit) - 1, 0, reportProgress);
            var tempFolder    = Assets.TempFolder;
            IOEx.TryEmptyDirectory(Assets.TempFolder);

            // Convert to ISO if necessary
            if (isNkit)
                await reporter.ExtractNKitAndReport(isoPath, tempFolder);
            else
                await reporter.ExtractISOAndReport(isoPath, tempFolder);

            // Apply format patches (ISO -> WBFS)
            reporter.Report("Patching ISO -> WBFS");
            Patch.Apply(PatchData.FromDirectories(Assets.ToWbfsPatchesFolder).ToArray().AsSpan(), tempFolder, tempFolder);
            
            reporter.Report("Patching Region to NTSC-U");
            Patch.Apply(PatchData.FromDirectories(Assets.ToUsPatchesFolder).ToArray().AsSpan(), tempFolder, tempFolder);
            
            reporter.Report("Verifying Clean NTSC-U Copy");
            if (!HashSet.Verify(FileHashSet.FromDirectory(Assets.UsHashesFolder), tempFolder, out var missingFiles, out var mismatchFiles))
            {
                ThrowHelpers.ThrowVerificationFailed("Failed to Verify Clean NTSC-U Copy Post Patching.\n" +
                                        "Most likely this means one of the following: \n" +
                                        "- Your ROM is a bad dump.\n" +
                                        "- Your ROM is from an unsupported region.\n" +
                                        "- Your ROM is uses an unsupported format.\n", missingFiles, mismatchFiles);
            }

            // Apply Regrav patch.
            reporter.Report("Patching NTSC-U -> Regravitified");
            Patch.Apply(PatchData.FromDirectories(Assets.ToRegravPatchesFolder).ToArray().AsSpan(), tempFolder, tempFolder);

            // Remove Redundant Files
            var regravHashes = FileHashSet.FromDirectory(Assets.RegravHashesFolder);
            HashSet.Cleanup(regravHashes, tempFolder);

            // Verify Regrav patch.
            reporter.Report("Verifying Regravitified Copy");
            if (!HashSet.Verify(regravHashes, tempFolder, out missingFiles, out mismatchFiles))
            {
                ThrowHelpers.ThrowVerificationFailed("Failed to Verify Clean Regravitified ROM.\n" + 
                                        "This most likely indicates a bad patch file, or an error in the code.\n",
                                        missingFiles, mismatchFiles);
            }

            // Repack ROM
            reporter.Report("Rebuilding WBFS");
            await using var logStream = new MemoryStream();
            try
            {
                await Wit.Build(new Wit.BuildOptions()
                {
                    Source = tempFolder,
                    Target = outputPath
                }, logStream);
            }
            catch (AggregateException e)
            {
                throw new Exception(e.Flatten().Message + "\n" + $"Log: {Encoding.Default.GetString(logStream.ToArray())}", e);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "\n" + $"Log: {Encoding.Default.GetString(logStream.ToArray())}", e);
            }

            Directory.Delete(tempFolder, true);
            reporter.Report("Done");
        }

        private static int CalculateNumberOfSteps(bool isNkit)
        {
            int extraSteps = isNkit ? 1 : 0;
            return 8 + extraSteps;
        }
    }
}
