using System.Threading.Tasks;
using System.Windows;
using Reloaded.WPF.Theme.Default;
using Sewer56.Patcher.Riders.ViewModel;

namespace Sewer56.Patcher.Riders
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReloadedWindow
    {
        public new MainWindowViewModel ViewModel { get; set; } = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_OnClick(object sender, RoutedEventArgs e) => await Task.Run(() => ViewModel.Patch());
    }
}
