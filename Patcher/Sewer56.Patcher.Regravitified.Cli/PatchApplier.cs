#if SRDX
using Sewer56.Patcher.Riders.Dx;
#elif REGRAV
using Sewer56.Patcher.Riders.Regrav;
#endif

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Sewer56.DeltaPatchGenerator.Lib.Utility;
using Sewer56.Patcher.Riders.Common;

namespace Sewer56.Patcher.Riders.Cli;

public static class PatchApplier
{
#if SRDX
    public static IGamePatch Patch = new DxPatch();
#elif REGRAV
    public static IGamePatch Patch = new RegravitifiedPatch();
#endif

    /// <summary>
    /// Patches the user's game given a file path of the source file and delegates for handling UI elements.
    /// </summary>
    /// <param name="sourceFilePath">Path containing the game file.</param>
    /// <param name="showDialog">Delegate for showing a dialog to the screen.</param>
    /// <param name="reportProgressCallback">Callback for reporting progress to the screen.</param>
    public static async Task PatchGame(string sourceFilePath, ShowDialogFunction showDialog, Events.ProgressCallback reportProgressCallback)
    {
        try
        {
            // Select Output
            var timer = Stopwatch.StartNew();
            var outputPath = Path.Combine(Path.GetDirectoryName(sourceFilePath), Patch.FileName);
            await Patch.ApplyPatch(sourceFilePath, outputPath, reportProgressCallback);

            showDialog("Patch Success", $"New ROM Saved to: {outputPath}\n" +
                                        $"Patching completed in: {timer.Elapsed.Minutes}min {timer.Elapsed.Seconds}sec");
        }
        catch (AggregateException ex)
        {
            var text = new StringBuilder();
            for (var x = 0; x < ex.InnerExceptions.Count; x++)
            {
                var exception = ex.InnerExceptions[x];
                text.AppendLine($"{x}. {exception.Message}\n{exception.StackTrace}");
            }

            showDialog("Failed to Convert ROM (Unexpected Error)", text.ToString());
        }
        catch (Exception error)
        {
            showDialog("Failed to Convert ROM", error.Message);
        }
    }

    /// <summary>
    /// Function used to display a dialog to the user's screen.
    /// </summary>
    /// <param name="title">Title of the dialog to display.</param>
    /// <param name="message">The message to display.</param>
    public delegate void ShowDialogFunction(string title, string message);
}