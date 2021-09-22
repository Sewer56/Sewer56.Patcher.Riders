using System;
using System.Threading.Tasks;
using Reloaded.WPF.Theme.Default;
using System.Windows;
using Sewer56.Patcher.Regravitified.ViewModel;

namespace Sewer56.Patcher.Regravitified
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
