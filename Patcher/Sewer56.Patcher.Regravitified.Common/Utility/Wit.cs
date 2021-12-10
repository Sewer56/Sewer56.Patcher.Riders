using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CliWrap;
using CliWrap.Builders;
using Sewer56.DeltaPatchGenerator.Lib.Utility;

namespace Sewer56.Patcher.Riders.Common.Utility
{
    public static class Wit
    {
        public static string WitFolder = Path.Combine(Paths.ProgramFolder, "Tools/Binaries/wit");
        public static string WitPath   = Path.Combine(WitFolder, "wit.exe");
        private static Platform _platform;

        public const string DataFolder = "DATA";

        /// <summary>
        /// Initializes Wit for a specific target platform.
        /// </summary>
        public static void Init(Platform platform)
        {
            _platform = platform;
            switch (platform)
            {
                case Platform.Windows:
                    WitFolder = Path.Combine(Paths.ProgramFolder, "Tools/Binaries/wit");
                    WitPath = Path.Combine(WitFolder, "wit.exe");
                    break;
                case Platform.Linux:
                    WitFolder = Path.Combine(Paths.ProgramFolder, "Tools/Binaries/wit-linux");
                    WitPath   = Path.Combine(WitFolder, "wit");
                    WitPath   = TranslatePath(WitPath);
                    break;
                case Platform.OSX:
                    throw new ArgumentOutOfRangeException(nameof(platform), platform, "OSX Is Not Supported because I can't Test it. Contact me if you are running OSX.");
            }
        }

        /// <summary>
        /// Builds an ISO to a given directory.
        /// </summary>
        public static Task<CommandResult> Build(BuildOptions options, Stream standardOutputAndError = null)
        {
            // Validate Parameters
            ThrowHelpers.ThrowIfNullOrEmpty(options.Source, nameof(options.Source));
            ThrowHelpers.ThrowIfNullOrEmpty(options.Target, nameof(options.Target));

            // Translate for Wine
            var executablePath  = WitPath;
            var argumentBuilder = new ArgumentsBuilder();

            if (_platform != Platform.Windows)
            {
                options.Source = TranslatePath(options.Source);
                options.Target = TranslatePath(options.Target);
            }

            // Create arguments.
            argumentBuilder.Add("COPY");
            argumentBuilder.Add($"{options.Source}");
            argumentBuilder.Add($"{options.Target}");
            argumentBuilder.Add("-f");
            argumentBuilder.Add("-o");

            var result = Cli.Wrap(executablePath)
                .WithArguments(argumentBuilder.Build())
                .WithWorkingDirectory(WitFolder);

            if (standardOutputAndError != null)
                result = result.WithStandardOutputPipe(PipeTarget.ToStream(standardOutputAndError))
                               .WithStandardErrorPipe(PipeTarget.ToStream(standardOutputAndError));
            
            return result.ExecuteAsync().Task;
        }

        /// <summary>
        /// Extracts an ISO to a given directory.
        /// </summary>
        public static Task<CommandResult> Extract(ExtractOptions options, StringBuilder standardOutput = null, StringBuilder standardError = null)
        {
            // Validate Parameters
            ThrowHelpers.ThrowIfNullOrEmpty(options.Source, nameof(options.Source));
            ThrowHelpers.ThrowIfNullOrEmpty(options.Target, nameof(options.Target));

            // Translate for Wine
            var executablePath  = WitPath;
            var argumentBuilder = new ArgumentsBuilder();

            if (_platform != Platform.Windows)
            {
                options.Source = TranslatePath(options.Source);
                options.Target = TranslatePath(options.Target);
            }

            // Create arguments.
            argumentBuilder.Add("EXTRACT");
            argumentBuilder.Add($"{options.Source}");
            argumentBuilder.Add($"{options.Target}");
            argumentBuilder.Add("-f");
            argumentBuilder.Add("-o");

            var result = Cli.Wrap(executablePath)
                .WithArguments(argumentBuilder.Build())
                .WithWorkingDirectory(WitFolder);

            if (standardOutput != null)
                result = result.WithStandardOutputPipe(PipeTarget.ToStringBuilder(standardOutput));

            if (standardError != null)
                result = result.WithStandardErrorPipe(PipeTarget.ToStringBuilder(standardError));

            return result.ExecuteAsync().Task.ContinueWith(task =>
            {
                var dataPath = Path.GetFullPath(Path.Combine(options.Target, DataFolder));
                DeleteNonDataPartitions(options, dataPath);
                return task.Result;
            });
        }

        private static void DeleteNonDataPartitions(ExtractOptions options, string dataPath)
        {
            if (!options.DataPartitionOnly || !Directory.Exists(dataPath)) 
                return;

            var directories = Directory.GetDirectories(options.Target);
            foreach (var directory in directories)
            {
                if (!directory.Equals(dataPath, StringComparison.OrdinalIgnoreCase))
                    Directory.Delete(directory, true);
            }

            IOEx.MoveDirectory(dataPath, options.Target);
            Directory.Delete(dataPath, true);
        }

        public class ExtractOptions
        {
            public string Source;
            public string Target;
            public bool DataPartitionOnly;
        }

        public class BuildOptions
        {
            public string Source;
            public string Target;
        }

        private static string TranslatePath(string existingPath)
        {
            var stringBuilder = new StringBuilder();
            var result = Cli.Wrap("winepath")
                .WithArguments($"-u \"{existingPath}\"")
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stringBuilder))
                .ExecuteAsync().Task.Result;

            return stringBuilder.ToString().Replace("\n", "");
        }
    }

    public enum Platform
    {
        Windows,
        Linux,
        OSX
    }
}
