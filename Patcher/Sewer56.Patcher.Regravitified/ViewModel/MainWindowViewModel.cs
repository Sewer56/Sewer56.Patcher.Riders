// ReSharper disable RedundantUsingDirective
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ookii.Dialogs.Wpf;
using Reloaded.WPF.MVVM;
#if SRDX
using Sewer56.Patcher.Riders.Dx;
#elif REGRAV
using Sewer56.Patcher.Riders.Regrav;
#endif

namespace Sewer56.Patcher.Riders.ViewModel
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
#if SRDX
                var gamePatch = new DxPatch();
#elif REGRAV
                var gamePatch = new RegravitifiedPatch();
#endif

                if (gamePatch.GetInstructionDialog(out var title, out var text))
                    ShowDialog(title, text);

                // Select ROM
                if (!TrySelectIsoFile(out var fileName))
                    return;

                // Select Output
                var timer = Stopwatch.StartNew();
                var outputPath = Path.Combine(Path.GetDirectoryName(fileName), gamePatch.FileName);
                await gamePatch.ApplyPatch(fileName, outputPath, (text, progress) =>
                {
                    Progress = progress * 100;
                    CurrentPatchingStep = text;
                });

                ShowDialog("Patch Success", $"New ROM Saved to: {outputPath}\n" +
                                            $"Patching completed in: {timer.Elapsed.Minutes}min {timer.Elapsed.Seconds}sec");
            }
            catch (AggregateException ex)
            {
                var text = new StringBuilder();
                for (var x = 0; x < ex.InnerExceptions.Count; x++)
                {
                    var exception = ex.InnerExceptions[x];
                    text.AppendLine($"{x}. {exception.Message}\n{exception.StackTrace}");
                }

                ShowDialog("Failed to Convert ROM (Unexpected Error)", text.ToString());
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
