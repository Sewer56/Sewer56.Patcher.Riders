using Reloaded.WPF.Theme.Default;
using System.Diagnostics;

namespace Sewer56.Patcher.Riders
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : ReloadedWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
            {
                UseShellExecute = true,
                CreateNoWindow = true
            });

            e.Handled = true;
        }
    }
}
