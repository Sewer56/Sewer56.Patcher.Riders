using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static Sewer56.Patcher.Riders.Cmd.Options;
using System.Threading.Tasks;
using Sewer56.DeltaPatchGenerator.Lib;
using Sewer56.DeltaPatchGenerator.Lib.Model;
using Sewer56.DeltaPatchGenerator.Lib.Utility;
using Sewer56.Patcher.Riders.Cmd;
using Sewer56.Patcher.Riders.Common.Utility;

namespace Sewer56.Patcher.Riders
{
    /// <summary>
    /// Stores the commandline 
    /// </summary>
    public class Commandline
    {
        public Commandline(string[] args)
        {
            var parser = new Parser(with =>
            {
                with.AutoHelp = true;
                with.CaseSensitive = false;
                with.CaseInsensitiveEnumValues = true;
                with.EnableDashDash = true;
                with.HelpWriter = null;
                with.AutoVersion = true;
            });

            var parserResult = parser.ParseArguments<GenerateHashOptions, VerifyHashOptions, GeneratePatchOptions,
                ConvertNKitOptions, ExtractISO, ApplyPatchOptions, ApplyPatchesOptions, BuildISO>(args);

            var tasks = new List<Task>
            {
                parserResult.WithParsedAsync<GenerateHashOptions>(GenerateHash),
                parserResult.WithParsedAsync<VerifyHashOptions>(VerifyHash),
                parserResult.WithParsedAsync<GeneratePatchOptions>(GeneratePatch),
                parserResult.WithParsedAsync<ConvertNKitOptions>(ConvertNKit),
                parserResult.WithParsedAsync<ExtractISO>(ExtractISO),
                parserResult.WithParsedAsync<ApplyPatchOptions>(ApplyPatch),
                parserResult.WithParsedAsync<ApplyPatchesOptions>(ApplyPatches),
                parserResult.WithParsedAsync<BuildISO>(BuildISO)
            };

            parserResult.WithNotParsed(errs => HandleParseError(parserResult, errs));
            Task.WaitAll(tasks.ToArray());
        }

        private Task ApplyPatches(ApplyPatchesOptions obj)
        {
            using var progressBar = new ProgressBar();
            var patches = PatchData.FromDirectories(obj.Patches);
            var patchSpan = patches.ToArray().AsSpan();
            Patch.Apply(patchSpan, obj.Source, obj.Source, (text, progress) => progressBar.Report(progress, text));
            return Task.CompletedTask;
        }

        private Task ApplyPatch(ApplyPatchOptions obj)
        {
            using var progressBar = new ProgressBar();
            var data = PatchData.FromDirectory(obj.Patch);
            Patch.Apply(data, obj.Source, obj.Source, (text, progress) => progressBar.Report(progress, text));
            return Task.CompletedTask;
        }

        private async Task ExtractISO(ExtractISO obj)
        {
            IOEx.TryDeleteDirectory(obj.Target);
            await Wit.Extract(new Wit.ExtractOptions()
            {
                Source = obj.Source,
                Target = obj.Target,
                DataPartitionOnly = true
            });
        }

        private async Task BuildISO(BuildISO obj)
        {
            await Wit.Build(new Wit.BuildOptions()
            {
                Source = obj.Source,
                Target = obj.Target
            });
        }

        private async Task ConvertNKit(ConvertNKitOptions obj)
        {
            await NKit.Convert(new NKit.ConvertOptions()
            {
                Source = obj.Source,
                Target = obj.Target
            });
        }

        private Task GeneratePatch(GeneratePatchOptions obj)
        {
            using var progressBar = new ProgressBar();
            Patch.Generate(obj.Source, obj.Target, obj.Output, (text, progress) => progressBar.Report(progress, text));
            return Task.CompletedTask;
        }

        private Task VerifyHash(VerifyHashOptions obj)
        {
            bool success;
            List<string> missingFiles;
            List<string> mismatchFiles;

            using (var progressBar = new ProgressBar())
            {
                var hashes = FileHashSet.FromDirectory(obj.HashDir);
                success = HashSet.Verify(hashes, obj.Target, out missingFiles, out mismatchFiles, (text, progress) => progressBar.Report(progress, text));
            }

            if (success)
            {
                Console.WriteLine("Everything is OK!!");
            }
            else
            {
                if (missingFiles.Count > 0)
                {
                    Console.WriteLine("Missing Files Detected");
                    foreach (var missing in missingFiles)
                        Console.WriteLine(missing);
                }

                if (mismatchFiles.Count > 0)
                {
                    Console.WriteLine("Hash Mismatch Detected");
                    foreach (var missing in mismatchFiles)
                        Console.WriteLine(missing);
                }
            }

            return Task.CompletedTask;
        }

        private Task GenerateHash(GenerateHashOptions obj)
        {
            string path;
            using (var progressBar = new ProgressBar())
            {
                var hashSet = HashSet.Generate(obj.Source, (text, progress) => progressBar.Report(progress, text));
                hashSet.ToDirectory(obj.Target, out path);
            }

            Console.WriteLine($"Saved to: {path}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Errors or --help or --version.
        /// </summary>
        static void HandleParseError(ParserResult<object> options, IEnumerable<Error> errs)
        {
            var helpText = HelpText.AutoBuild(options, help =>
            {
                help.Copyright = "Created by Sewer56, licensed under GNU GPL V3";
                help.AutoHelp = false;
                help.AutoVersion = false;
                help.AddDashesToOption = true;
                help.AddEnumValuesToHelpText = true;
                return HelpText.DefaultParsingErrorsHandler(options, help);
            }, example => example, true);

            Console.WriteLine(helpText);
        }
    }
}