using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Sewer56.DeltaPatchGenerator.Lib;
using Sewer56.DeltaPatchGenerator.Lib.Model;
using Sewer56.Patcher.Riders.Cli.Cmd;
#if SRDX || REGRAV
using Sewer56.Patcher.Riders.Common.Utility;
#endif
#if REGRAV || SRTE
using Sewer56.DeltaPatchGenerator.Lib.Utility;
using Sewer56.Patcher.Riders.Common.Utility;
#endif
#if SRDXSelfContained
using Sewer56.Patcher.Riders.Dx.Utility;
#endif

#if DEV
using System.Diagnostics;
#endif
using static Sewer56.Patcher.Riders.Cli.Cmd.Options;

namespace Sewer56.Patcher.Riders.Cli
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
                ApplyPatchOptions, ApplyPatchesOptions, PatchGameOptions

#if !SRDXSelfContained
                , ConvertNKitOptions
#endif
#if REGRAV
                , ExtractISO 
                , BuildISO
#endif

#if DEV
                , CreateSelfContainedOptions
#endif
            > (args);

            var tasks = new List<Task>
            {
                parserResult.WithParsedAsync<GenerateHashOptions>(GenerateHash),
                parserResult.WithParsedAsync<VerifyHashOptions>(VerifyHash),
                parserResult.WithParsedAsync<GeneratePatchOptions>(GeneratePatch),

#if !SRDXSelfContained
                parserResult.WithParsedAsync<ConvertNKitOptions>(ConvertNKit),
#endif
#if REGRAV
                parserResult.WithParsedAsync<ExtractISO>(ExtractISO),
                parserResult.WithParsedAsync<BuildISO>(BuildISO),
#endif
                parserResult.WithParsedAsync<ApplyPatchOptions>(ApplyPatch),
                parserResult.WithParsedAsync<ApplyPatchesOptions>(ApplyPatches),
                parserResult.WithParsedAsync<PatchGameOptions>(PatchGame),
#if DEV
                parserResult.WithParsedAsync<CreateSelfContainedOptions>(XorFile)
#endif
            };

            parserResult.WithNotParsed(errs => HandleParseError(parserResult, errs));
            Task.WaitAll(tasks.ToArray());
        }

#if DEV
        private async Task XorFile(CreateSelfContainedOptions createSelfContainedOptions)
        {
            using var progressBar = new ProgressBar();
            var watch = Stopwatch.StartNew();
            progressBar.Report(0.03, "Reading File");
            var originalFile = await File.ReadAllBytesAsync(createSelfContainedOptions.File);

            progressBar.Report(0.03, "Compressing File");
            var file = Compression.Compress(originalFile, new Progress<double>(d => { progressBar.Report(0.03 + (d * 0.93), "Compressing File"); }));

            progressBar.Report(0.96, "Key'ing File");
            var key  = Xor.StringToKey(createSelfContainedOptions.Key);

            unsafe
            {
                fixed (byte* filePtr = &file.Span[0])
                fixed (byte* keyPtr = &key[0])
                {
                    Xor.Xor32(keyPtr, filePtr, file.Length);
                }
            }

            progressBar.Report(0.97, "Saving File");
            var outputFileStream = new FileStream(createSelfContainedOptions.OutFile, FileMode.Create);
            await outputFileStream.WriteAsync(file);

            progressBar.Report(1.0, "Done");
            Console.WriteLine($"{watch.ElapsedMilliseconds}ms");
        }
#endif

        private async Task PatchGame(PatchGameOptions options)
        {
            if (!File.Exists(options.RomPath))
                throw new Exception("Path to the ROM to be patched does not exist.");

            using var progressBar = new ProgressBar();
            void ShowDialog(string title, string message)
            {
                Console.WriteLine(title);
                Console.WriteLine("=====");
                Console.WriteLine(message);
            }

            await PatchApplier.PatchGame(options.RomPath, ShowDialog, (text, progress) => progressBar.Report(progress, text));
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

#if REGRAV
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
#endif

#if !SRDXSelfContained
        private async Task ConvertNKit(ConvertNKitOptions obj)
        {
            using var progressBar = new ProgressBar();
            await NKit.Convert(new NKit.ConvertOptions()
            {
                Source = obj.Source,
                Target = obj.Target,
                Progress = (text, progress) => progressBar.Report(progress, text)
            });
        }
#endif

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