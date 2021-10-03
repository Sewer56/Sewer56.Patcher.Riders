using System;
using System.IO;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Builders;
using Sewer56.DeltaPatchGenerator.Lib.Utility;

namespace Sewer56.Patcher.Riders.Common.Utility
{
    public static class Wit
    {
        public static readonly string WitFolder = Path.Combine(Paths.ProgramFolder, "Tools/Binaries/wit");
        public static readonly string WitPath = Path.Combine(WitFolder, "wit.exe");
        public const string DataFolder = "DATA";

        /// <summary>
        /// Builds an ISO to a given directory.
        /// </summary>
        public static Task<CommandResult> Build(BuildOptions options)
        {
            // Validate Parameters
            ThrowHelpers.ThrowIfNullOrEmpty(options.Source, nameof(options.Source));
            ThrowHelpers.ThrowIfNullOrEmpty(options.Target, nameof(options.Target));

            // Create arguments.
            var argumentBuilder = new ArgumentsBuilder();
            argumentBuilder.Add("COPY");
            argumentBuilder.Add(options.Source);
            argumentBuilder.Add(options.Target);
            argumentBuilder.Add("-f");
            argumentBuilder.Add("-o");

            var result = Cli.Wrap(WitPath)
                .WithArguments(argumentBuilder.Build())
                .WithWorkingDirectory(WitFolder)
                .ExecuteAsync().Task;
            
            return result;
        }

        /// <summary>
        /// Extracts an ISO to a given directory.
        /// </summary>
        public static Task<CommandResult> Extract(ExtractOptions options)
        {
            // Validate Parameters
            ThrowHelpers.ThrowIfNullOrEmpty(options.Source, nameof(options.Source));
            ThrowHelpers.ThrowIfNullOrEmpty(options.Target, nameof(options.Target));

            // Create arguments.
            var argumentBuilder = new ArgumentsBuilder();
            argumentBuilder.Add("EXTRACT"); 
            argumentBuilder.Add(options.Source);
            argumentBuilder.Add("-d");
            argumentBuilder.Add(options.Target);
            argumentBuilder.Add("-f");
            argumentBuilder.Add("-o");

            var result = Cli.Wrap(WitPath)
                .WithArguments(argumentBuilder.Build())
                .WithWorkingDirectory(WitFolder)
                .ExecuteAsync().Task.ContinueWith(task =>
                {
                    var dataPath = Path.GetFullPath(Path.Combine(options.Target, DataFolder));
                    DeleteNonDataPartitions(options, dataPath);
                    return task.Result;
                });

            return result;
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
    }
}
