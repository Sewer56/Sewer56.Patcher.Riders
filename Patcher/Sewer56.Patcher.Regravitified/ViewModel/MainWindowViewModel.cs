// ReSharper disable RedundantUsingDirective
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
                    Progress = progress * 100;
                    CurrentPatchingStep = text;
                });

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var successWindow = new Dialogs.ProtagBox("Important Announcement!",
                        "In this disaster, truly all of our resolve has been in vain. Countless lives and valuable developers have been lost.\n\n" +
                        "However... look around you. Look at how our home is being reborn, just as the cherry blossoms at the main gate bloom proudly, even on this dying land. Look at your comrades by your side. Look at the passion still burning bright in their eyes despite our plight. What is it that drives us on?\n\n" +
                        "What makes us rise back up when our whole bodies are battered and bruised? It is the fact that devoting our whole bodies and souls to fighting despair is the duty given to those of us still alive, and the fact that doing so is the one and only way to honor those who have given their lives for humanity's victory.\n\n" +
                        "Listen to the voices of those sleeping in the Earth. Listen to the voices of those who met their end in the sea. Listen to the voices of those lost to the sky. ....The time has come for their dying wishes to be fulfilled.\n\n" +
                        "And now, these young soldiers are about to set off. They will carry on their backs the wishes of us, and of those long gone, as they depart to face the enemy alone and without backup. Whether history chooses to glorify them or not... we will remember them.\n\n" +
                        "We will engrave into our hearts the noble deeds of those who are not permitted to reveal even their names.\n\n" +
                        "Departing young ones... Do not forgive us for being unable to teach you anything but fighting. Do not forgive our inability to avoid sending you onto the battlefield. I pray that your act of bravery will form the cornerstone of a world where we no longer send such young men and women into battle.\n\n" +
                        "The same nights await us all. While I breathe, I hope. Through difficulties to honors. Through hardships... to paradise.",
                        "Cultagonist", "Assets/ChurchSong.aac");
                    successWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    successWindow.ShowDialog();
                });

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var successWindow = new Dialogs.ProtagBox("Congratulations", "You have Successfully Installed TE 1.4.1", "Social_20", "Assets/ChingChengHanji.aac");
                    successWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    successWindow.ShowDialog();
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
