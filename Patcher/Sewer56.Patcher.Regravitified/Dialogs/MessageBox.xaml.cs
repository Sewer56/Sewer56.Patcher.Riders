using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Reloaded.WPF.Theme.Default;

namespace Sewer56.Patcher.Regravitified.Dialogs
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox : ReloadedWindow
    {
        public MessageBox(string title, string message) : base()
        {
            InitializeComponent();
            this.Title = title;
            this.Message.Text = message;
            var viewModel = ((WindowViewModel)this.DataContext);

            viewModel.MinimizeButtonVisibility = Visibility.Collapsed;
            viewModel.MaximizeButtonVisibility = Visibility.Collapsed;
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
