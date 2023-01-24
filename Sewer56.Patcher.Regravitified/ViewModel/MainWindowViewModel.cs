// ReSharper disable RedundantUsingDirective
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Ookii.Dialogs.Wpf;
using Reloaded.WPF.MVVM;
using Sewer56.Patcher.Riders.Cli;
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
                if (PatchApplier.Patch.GetInstructionDialog(out var title, out var text))
                    ShowDialog(title, text);

                // Select ROM
                if (!TrySelectIsoFile(out var fileName))
                    return;

                // Select Output
                await PatchApplier.PatchGame(fileName, ShowDialog, (text, progress) =>
                {
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Progress = progress * 100;
                        CurrentPatchingStep = text;
                    });
                });
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
