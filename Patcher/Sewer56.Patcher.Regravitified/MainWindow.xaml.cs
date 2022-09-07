using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Reloaded.WPF.Theme.Default;
using Sewer56.DeltaPatchGenerator.Lib.Utility;
using Sewer56.Patcher.Riders.Effect;
using Sewer56.Patcher.Riders.Utility;
using Sewer56.Patcher.Riders.ViewModel;

namespace Sewer56.Patcher.Riders
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReloadedWindow
    {
        public new MainWindowViewModel ViewModel { get; set; } = new MainWindowViewModel();

        private BassMusicPlayer _bassMusicPlayer;
        private ThemeHueShiftEffect _themeHueShiftEffect;
        private TitleDXv2Effect _titleDxEffect;
        private LogoDXv2Effect _logoDxV2Effect;

        public MainWindow()
        {
            InitializeComponent();
            _themeHueShiftEffect = new ThemeHueShiftEffect(this);
            this.Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            _bassMusicPlayer = new BassMusicPlayer(Path.Combine(Paths.ProgramFolder, "lhs_rld10.xm"));
            _bassMusicPlayer.Play();
            _titleDxEffect = new TitleDXv2Effect(this);
            _logoDxV2Effect = new LogoDXv2Effect(this, this.Banner);
            
            // Hack to prevent resize on long text.
            await Task.Delay(2000);
            this.SizeToContent = SizeToContent.Manual;
        }

        private async void Button_OnClick(object sender, RoutedEventArgs e) => await Task.Run(() => ViewModel.Patch());

        private void Music_Click(object sender, RoutedEventArgs e)
        {
            _bassMusicPlayer.Toggle();
        }
    }
}
