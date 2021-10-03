using System;
using System.IO;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Builders;
using Sewer56.DeltaPatchGenerator.Lib.Utility;

namespace Sewer56.Patcher.Riders.Common.Utility
{
    public class NKit
    {
        public static readonly string NKitFolder = Path.Combine(Paths.ProgramFolder, "Tools/Binaries/nkit");
        public static readonly string NKitPath   = Path.Combine(NKitFolder, "ConvertToISO.exe");
        public static readonly string NKitResultPath = Path.Combine(NKitFolder, "Processed/output.iso");

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
        public static Task<CommandResult> Convert(ConvertOptions options)
        {
            // Validate Parameters
            ThrowHelpers.ThrowIfNullOrEmpty(options.Source, nameof(options.Source));
            ThrowHelpers.ThrowIfNullOrEmpty(options.Target, nameof(options.Target));

            // Create arguments.
            var argumentBuilder = new ArgumentsBuilder();
            argumentBuilder.Add(options.Source);

            var result = Cli.Wrap(NKitPath)
                .WithValidation(CommandResultValidation.None)
                .WithArguments(argumentBuilder.Build())
                .WithWorkingDirectory(NKitFolder)
                .WithStandardOutputPipe(PipeTarget.ToStream(Console.OpenStandardOutput()))
                .ExecuteAsync().Task.ContinueWith(task =>
            {
                // Move ISO
                var path = Path.Combine(NKitFolder, NKitResultPath);
                if (!File.Exists(path))
                    throw new FileNotFoundException("ISO File Output by NKit Not Found");

                File.Move(path, options.Target);
                return task.Result;
            });

            return result;
        }

        public class ConvertOptions
        {
            public string Source;
            public string Target;
        }
    }
}
