#if !SRDXSelfContained
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Nanook.NKit;
using Sewer56.DeltaPatchGenerator.Lib.Utility;

namespace Sewer56.Patcher.Riders.Common.Utility
{
    public class NKit
    {
        /// <summary>
        /// Returns true if an image file is NKit, else false.
        /// </summary>
        public static bool IsNkit(string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Open);
            using var reader = new BinaryReader(stream);

            stream.Seek(0x200, SeekOrigin.Begin);
            return reader.ReadInt32() == 0x54494B4E; // 'NKIT'
        }

        /// <summary>
        /// Runs a compress command that converts an NKit to an ISO.
        /// </summary>
        public static Task Convert(ConvertOptions options)
        {
            // Validate Parameters.
            ThrowHelpers.ThrowIfNullOrEmpty(options.Source, nameof(options.Source));
            ThrowHelpers.ThrowIfNullOrEmpty(options.Target, nameof(options.Target));

            // Open Source File.
            var sourceFiles = SourceFiles.Scan(new []{ options.Source }, false); // For some strange reason NKit borks on using SourceFiles.OpenFile directly, even if value is same.
            var nkitConvert = new Converter(sourceFiles[0], true);
            if (options.Progress != null)
                nkitConvert.LogProgress += (sender, args) => options.Progress("Converting from NKit", args.TotalProgress);

            return Task.Run(() => nkitConvert.ConvertToIso(false, false, false, false)).ContinueWith(
                task =>
                {
                    File.Move(task.Result.OutputFileName, options.Target, true);
                });
        }

        public class ConvertOptions
        {
            public string Source;
            public string Target;
            public Events.ProgressCallback Progress;
        }
    }
}
#endif