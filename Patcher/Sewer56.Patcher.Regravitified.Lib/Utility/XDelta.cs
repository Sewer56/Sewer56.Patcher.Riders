using CliWrap;
using System.IO;
using CliWrap.Builders;

namespace Sewer56.Patcher.Regravitified.Lib.Utility
{
    public class XDelta
    {
        public static readonly string XDeltaFolder = Path.Combine(Paths.ProgramFolder, "Tools/Binaries/xdelta");
        public static readonly string XDeltaPath = Path.Combine(XDeltaFolder, "xdelta.exe");

        /// <summary>
        /// Applies an xdelta patch to a file.
        /// </summary>
        public static CommandTask<CommandResult> Apply(ApplyOptions options)
        {
            // Validate Parameters
            ThrowHelpers.ThrowIfNullOrEmpty(options.Source, nameof(options.Source));
            ThrowHelpers.ThrowIfNullOrEmpty(options.Patch, nameof(options.Patch));
            ThrowHelpers.ThrowIfNullOrEmpty(options.Output, nameof(options.Output));

            // Create Arguments
            var argumentBuilder = new ArgumentsBuilder();
            argumentBuilder.Add("-d"); // Decode

            // Add decode arguments
            argumentBuilder.Add("-s");
            argumentBuilder.Add(options.Source, true);
            argumentBuilder.Add(options.Patch, true);
            argumentBuilder.Add(options.Output, true);

            return Cli.Wrap(XDeltaPath)
                .WithArguments(argumentBuilder.Build())
                .WithWorkingDirectory(XDeltaFolder)
                .ExecuteAsync();
        }

        /// <summary>
        /// Runs a compress command that creates a new xdelta file.
        /// </summary>
        public static CommandTask<CommandResult> Compress(CompressOptions options)
        {
            // Validate Parameters
            ThrowHelpers.ThrowIfNullOrEmpty(options.Source, nameof(options.Source));
            ThrowHelpers.ThrowIfNullOrEmpty(options.Target, nameof(options.Target));
            ThrowHelpers.ThrowIfNullOrEmpty(options.Output, nameof(options.Output));

            // Create arguments.
            var argumentBuilder = new ArgumentsBuilder();
            argumentBuilder.Add("-e"); // Encode

            // Add extra arguments
            if (options.DisableCompression)
                argumentBuilder.Add("-S");

            if (options.DisableFilePath)
                argumentBuilder.Add("-A");

            // Add encode arguments.
            argumentBuilder.Add("-s");
            argumentBuilder.Add(options.Source, true);
            argumentBuilder.Add(options.Target, true);
            argumentBuilder.Add(options.Output, true);
            
            return Cli.Wrap(XDeltaPath)
                .WithArguments(argumentBuilder.Build())
                .WithWorkingDirectory(XDeltaFolder)
                .ExecuteAsync();
        }
    }

    public class CompressOptions
    {
        public string Source;
        public string Target;
        public string Output;

        /// <summary>
        /// Disables compression of VCDIFF data.
        /// </summary>
        public bool DisableCompression;

        /// <summary>
        /// Doesn't include original file path in VCDIFF header.
        /// </summary>
        public bool DisableFilePath;
    }

    public class ApplyOptions
    {
        public string Source;
        public string Patch;
        public string Output;
    }
}
