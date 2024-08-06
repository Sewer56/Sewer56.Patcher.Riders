#if SRTE
using System;
using System.IO;
using System.Threading.Tasks;
using Sewer56.DeltaPatchGenerator.Lib;
using Sewer56.DeltaPatchGenerator.Lib.Model;
using Sewer56.DeltaPatchGenerator.Lib.Utility;
using Sewer56.Patcher.Riders.Common;
using Sewer56.Patcher.Riders.Common.Utility;

namespace Sewer56.Patcher.Riders.Te
{
    public class TePatch : IGamePatch
    {
        public const string ExpectedRomName = "Sonic Riders (USA) (En,Ja,Fr,De,Es,It).iso";

        public string FileName { get; set; } = "Sonic Riders TE 2.4.iso";

        public bool GetInstructionDialog(out string title, out string text)
        {
            title = "The one and only step.";
            text = "Please select a Sonic Riders ISO to patch.\n" +
                   "Once started, the patching process will take a few seconds.";

            return true;
        }

        public async Task ApplyPatch(string isoPath, string outputPath, Events.ProgressCallback reportProgress = null)
        {
            bool isNkit = NKit.IsNkit(isoPath);
            var reporter = new ProgressReporter(CalculateNumberOfSteps(isNkit) - 1, 0, reportProgress);
            var tempFolder = Assets.TempFolder;
            IOEx.TryEmptyDirectory(Assets.TempFolder);
            string intermediateIsoPath = null;

            // Convert to ISO if necessary
            if (isNkit)
            {
                intermediateIsoPath = Path.Combine(tempFolder, "nkit.iso");
                await reporter.ConvertNKitAndReport(isoPath, intermediateIsoPath);
                isoPath = intermediateIsoPath;
            }

            using var renameIso = new TemporarilyRenameFile(isoPath, ExpectedRomName);
            var sourceFolder = Path.GetDirectoryName(isoPath);

            // Verify Patch
            reporter.Report("Verifying Clean NTSC-U Copy");
            if (!HashSet.Verify(FileHashSet.FromDirectory(Assets.OriginalHashesFolder), sourceFolder, out var missingFiles, out var mismatchFiles))
            {
                ThrowHelpers.ThrowVerificationFailed("Failed to Verify Clean NTSC-U Copy Pre Patching.\n" +
                                                     "Most likely this means one of the following: \n" +
                                                     "- Your ROM is a bad dump.\n" +
                                                     "- Your ROM is from an unsupported region.\n" +
                                                     "- Your ROM is uses an unsupported format.\n", missingFiles, mismatchFiles);
            }

            // Apply Patch
            reporter.Report("Patching NTSC-U -> Riders TE");
            var appliedPatchPath = Path.Combine(tempFolder, ExpectedRomName);
            Patch.Apply(PatchData.FromDirectories(Assets.ToTePatchesFolder).ToArray().AsSpan(), sourceFolder, tempFolder);

            // Verify Patch
            reporter.Report("Verifying Patched Game");
            if (!HashSet.Verify(FileHashSet.FromDirectory(Assets.ModHashesFolder), tempFolder, out missingFiles, out mismatchFiles))
            {
                ThrowHelpers.ThrowVerificationFailed("Failed to Verify Patched Game.\n" +
                                                     "Most likely this means one of the following: \n" +
                                                     "- Your ROM is a bad dump.\n" +
                                                     "- Your ROM is from an unsupported region.\n" +
                                                     "- Your ROM is uses an unsupported format.\n", missingFiles, mismatchFiles);
            }

            // Move new ROM.
            File.Move(appliedPatchPath, outputPath, true);
        }

        private static int CalculateNumberOfSteps(bool isNkit)
        {
            int extraSteps = isNkit ? 1 : 0;
            return 3 + extraSteps;
        }
    }
}
#endif