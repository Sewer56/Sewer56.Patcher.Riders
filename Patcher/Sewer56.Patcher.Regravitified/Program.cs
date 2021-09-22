using System;
using System.Runtime.InteropServices;

namespace Sewer56.Patcher.Regravitified
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                new Commandline(args);
            }
            else
            {
                FreeConsole();
                var app = new App();
                app.InitializeComponent();
                app.Run(new MainWindow());
            }
        }

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool FreeConsole();
    }
}
