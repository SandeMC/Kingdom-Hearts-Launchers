using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Kingdom_Hearts_2_Launcher
{
    public partial class App : Application
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Kingdom_Hearts_2_Launcher.App app = new Kingdom_Hearts_2_Launcher.App();
            app.InitializeComponent();
            MainWindow mainWindow = new MainWindow(args);
            app.Run(mainWindow);
        }
    }
}
