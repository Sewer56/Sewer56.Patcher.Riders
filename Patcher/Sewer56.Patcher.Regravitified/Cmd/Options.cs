using CommandLine;

namespace Sewer56.Patcher.Riders.Cmd
{
    public class Options
    {
        [Verb("GenerateHashes", HelpText = "Generates a text file containing hashes of each file.")]
        internal class GenerateHashOptions
        {
            [Option("src", Required = true,  HelpText = "The source folder to generate hashes from.")]
            public string Source { get; internal set; }

            [Option("tgt", Required = true, HelpText = "The folder to write the hash list to.")]
            public string Target { get; internal set; }
        }

        [Verb("VerifyHashes", HelpText = "Verifies hashes within a folder from a hash list.")]
        internal class VerifyHashOptions
        {
            [Option("hash", Required = true, HelpText = "The folder to read the hash list from.")]
            public string HashDir { get; internal set; }

            [Option("tgt", Required = true, HelpText = "The folder to verify.")]
            public string Target { get; internal set; }
        }

        [Verb("ConvertNKit", HelpText = "Converts an NKit into an ISO.")]
        internal class ConvertNKitOptions
        {
            [Option("src", Required = true, HelpText = "The source nkit.iso to convert.")]
            public string Source { get; internal set; }

            [Option("tgt", Required = true, HelpText = "Path to the resulting ISO file.")]
            public string Target { get; internal set; }
        }

        [Verb("ExtractISO", HelpText = "Converts an ISO into the filesystem files. Extracts DATA partition only.")]
        internal class ExtractISO
        {
            [Option("src", Required = true, HelpText = "The source iso to convert.")]
            public string Source { get; internal set; }

            [Option("tgt", Required = true, HelpText = "Path to the folder where to store the ISO contents. This folder is deleted before extracting.")]
            public string Target { get; internal set; }
        }

        [Verb("BuildISO", HelpText = "Builds a new ISO from a given folder.")]
        internal class BuildISO
        {
            [Option("src", Required = true, HelpText = "The folder to be built into an ISO.")]
            public string Source { get; internal set; }

            [Option("tgt", Required = true, HelpText = "The path to set the new ISO.")]
            public string Target { get; internal set; }
        }

        [Verb("GeneratePatch", HelpText = "Generates a patch which converts the contents from one directory into the contents of another directory. Does not (yet) add new files!!")]
        internal class GeneratePatchOptions
        {
            [Option("src", Required = true, HelpText = "The folder to patch into the target folder.")]
            public string Source { get; internal set; }

            [Option("tgt", Required = true, HelpText = "The folder containing the desired result.")]
            public string Target { get; internal set; }

            [Option("out", Required = true, HelpText = "The folder containing the patch data.")]
            public string Output { get; internal set; }
        }

        [Verb("ApplyPatch", HelpText = "Applies a patch to the given folder.")]
        internal class ApplyPatchOptions
        {
            [Option("src", Required = true, HelpText = "The folder to be patched into the target folder.")]
            public string Source { get; internal set; }

            [Option("patch", Required = true, HelpText = "The folder containing the patch data.")]
            public string Patch { get; internal set; }
        }

        [Verb("ApplyPatches", HelpText = "Applies a set of patches (each inside a folder) to the given folder.")]
        internal class ApplyPatchesOptions
        {
            [Option("src", Required = true, HelpText = "The folder to be patched into the target folder.")]
            public string Source { get; internal set; }

            [Option("patches", Required = true, HelpText = "The folder containing folders which contain patch data.")]
            public string Patches { get; internal set; }
        }
    }
}
