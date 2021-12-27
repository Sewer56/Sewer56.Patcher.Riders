using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Reloaded.WPF.Theme.Default;

namespace Sewer56.Patcher.Riders.Dialogs
{
    /// <summary>
    /// Interaction logic for ProtagBox.xaml
    /// </summary>
    public partial class ProtagBox : ReloadedWindow
    {
        private MediaPlayer _musicPlayer;

        public ProtagBox(string title, string message, string imageAssetName, string songName) : base()
        {
            InitializeComponent();
            this.Title = title;
            this.Message.Text = message;
            var viewModel = ((WindowViewModel)this.DataContext);

            viewModel.MinimizeButtonVisibility = Visibility.Collapsed;
            viewModel.MaximizeButtonVisibility = Visibility.Collapsed;

            Banner.Source = (ImageSource) TryFindResource(imageAssetName);
            _musicPlayer = new MediaPlayer();
            _musicPlayer.Open(new Uri(Path.Combine(AppContext.BaseDirectory, songName), UriKind.RelativeOrAbsolute));
            _musicPlayer.Play();

            _musicPlayer.MediaEnded += (sender, args) =>
            {
                _musicPlayer.Position = TimeSpan.Zero;
                _musicPlayer.Play();
            };

            this.Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            _musicPlayer.Stop();
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
