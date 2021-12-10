using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Sewer56.Patcher.Riders.Cmd;
using Sewer56.Patcher.Riders.Common.Utility;

namespace Sewer56.Patcher.Riders
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length > 0)
                new Commandline(args);
            else
                LaunchGui(Platform.Windows);
        }

        public static void LaunchGui(Platform platform)
        {
            FreeConsole();
            Wit.Init(platform);
            var app = new App();
            app.InitializeComponent();
            app.Run(new MainWindow());
        }

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool FreeConsole();
    }
}
