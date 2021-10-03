using System.Threading.Tasks;
using Sewer56.DeltaPatchGenerator.Lib.Utility;

namespace Sewer56.Patcher.Riders.Common
{
    public interface IGamePatch
    {
        /// <summary>
        /// File name of the output file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Applies a patch to the game;
        /// </summary>
        /// <param name="isoPath">Folder to the ISO to be patched.</param>
        /// <param name="outputPath">The path to save the ISO to.</param>
        /// <param name="reportProgress">Function that receives information on the current progress.</param>
        public Task ApplyPatch(string isoPath, string outputPath, Events.ProgressCallback reportProgress = null);

        /// <summary>
        /// Gets the text to display in a help window before selecting the ISO.
        /// </summary>
        /// <param name="title">The title of the box.</param>
        /// <param name="text">The text to display.</param>
        /// <returns>True if to display dialog, else false.</returns>
        public bool GetInstructionDialog(out string title, out string text);
    }
}