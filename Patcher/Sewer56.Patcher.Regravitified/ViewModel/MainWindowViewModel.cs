using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Ookii.Dialogs.Wpf;
using Reloaded.WPF.MVVM;
using Sewer56.Patcher.Regravitified.Dialogs;
using Sewer56.Patcher.Regravitified.Regrav;

namespace Sewer56.Patcher.Regravitified.ViewModel
{
    public class MainWindowViewModel : ObservableObject
    {
        /// <summary>
        /// True if patching is currently going on, else false.
        /// </summary>
        public bool IsPatching { get; set; }

        /// <summary>
        /// Current patching step.
        /// </summary>
        public string CurrentPatchingStep { get; set; }

        /// <summary>
        /// Current Patching Progress.
        /// </summary>
        public double Progress { get; set; }

        public async Task Patch()
        {
            IsPatching = true;
            try
            {
                ShowDialog("The one and only step.", "Please select a Wii ROM of Sonic Riders: Zero Gravity to patch.\n" +
                                                     "Once started, the patching process will take up to 5 minutes.");

                // Select ROM
                if (!TrySelectIsoFile(out var fileName))
                    return;

                // Select Output
                var timer = Stopwatch.StartNew();
                var outputPath = Path.Combine(Path.GetDirectoryName(fileName), "Sonic Riders Regravitified.wbfs");
                await PatchToRegrav.Patch(fileName, outputPath, (text, progress) =>
                {
                    Progress = progress * 100;
                    CurrentPatchingStep = text;
                });

                ShowDialog("Patch Success", $"New ROM Saved to: {outputPath}\n" +
                                            $"Patching completed in: {timer.Elapsed.Minutes}min {timer.Elapsed.Seconds}sec");
            }
            catch (Exception error)
            {
                ShowDialog("Failed to Convert ROM", error.Message);
            }
            finally
            {
                IsPatching = false;
            }
        }

        private static void ShowDialog(string title, string text)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var successWindow = new Dialogs.MessageBox(title, text);
                successWindow.ShowDialog();
            });
        }

        private bool TrySelectIsoFile(out string fileName)
        {
            var openRomDialog = new VistaOpenFileDialog
            {
                Multiselect = false,
                CheckFileExists = true,
                Filter = "Supported ROM|*.iso;*.wbfs;*.nkit.iso|ISO Image (*.iso)|*.iso|NKit Image (*.nkit.iso)|*.nkit.iso|WBFS Image (*.wbfs)|*.wbfs",
                Title = "Select a ROM Image to Patch"
            };

            var result = openRomDialog.ShowDialog().GetValueOrDefault(false);
            fileName = openRomDialog.FileName;
            return result;
        }
    }
}
