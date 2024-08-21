using System;

namespace Sewer56.Patcher.Riders
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var process = System.Diagnostics.Process.Start(@"ServerWork\main.exe");
            process.WaitForExit();
            var app = new App();
            app.InitializeComponent();
            app.Run(new MainWindow());
        }
    }
}
