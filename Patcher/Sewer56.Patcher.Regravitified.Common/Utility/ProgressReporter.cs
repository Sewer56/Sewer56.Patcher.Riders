using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Sewer56.DeltaPatchGenerator.Lib.Utility;

namespace Sewer56.Patcher.Riders.Common.Utility
{
    public class ProgressReporter
    {
        /// <summary>
        /// The current progress counter.
        /// </summary>
        public int Counter;

        /// <summary>
        /// Maximum value of the progress counter.
        /// </summary>
        public int MaxCounter;

        /// <summary>
        /// The method to call.
        /// </summary>
        public Events.ProgressCallback Callback;

        public ProgressReporter(int maxSteps, int counter = 0, Events.ProgressCallback callback = null)
        {
            Counter = counter;
            MaxCounter = maxSteps;
            Callback = callback;
        }

        /// <summary>
        /// Reports progress to the method and increments the counter by 1.
        /// </summary>
        /// <param name="text">Text to attach to the progress report.</param>
        public void Report(string text)
        {
            var progress = (double)Counter / MaxCounter;
            Callback?.Invoke(text, progress);
            Counter++;
        }
    }

    public static class ProgressReporterExtensions
    {
        public static async Task ExtractISOAndReport(this ProgressReporter reporter, string isoPath, string isoOutputPath)
        {
            reporter.Report("Extracting ISO");
            var stdOut = new StringBuilder();
            var stdErr = new StringBuilder();

            try
            {
                await Wit.Extract(new Wit.ExtractOptions()
                {
                    Source = isoPath,
                    DataPartitionOnly = true,
                    Target = isoOutputPath
                }, stdOut, stdErr);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "\n" + $"StdOut: {stdOut}" + "\n" + $"StdErr: {stdErr}", e);
            }
        }

        public static async Task ConvertNKitAndReport(this ProgressReporter reporter, string isoPath, string isoOutputPath)
        {
            reporter.Report("Converting NKit to ISO");
            await NKit.Convert(new NKit.ConvertOptions()
            {
                Source = isoPath,
                Target = isoOutputPath
            });
        }

        public static async Task ExtractNKitAndReport(this ProgressReporter reporter, string isoPath, string isoOutputFolder, bool deleteIso = true)
        {
            var convertedPath = Path.Combine(isoOutputFolder, "nkit.iso");
            await reporter.ConvertNKitAndReport(isoPath, convertedPath);
            
            reporter.Report("Extracting ISO");
            await ExtractISOAndReport(reporter, convertedPath, isoOutputFolder);
            if (deleteIso)
                File.Delete(convertedPath);
        }
    }
}