using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GitGUI.Logic;

namespace GitGUI.Logic
{
    /// <summary>
    /// Interakční logika pro App.xaml
    /// </summary>
    public partial class App : Application
    {
        public AppSettings Settings { get; } = new AppSettings();
        Program Program { get; set; }

        protected void OnStartup(object sender, StartupEventArgs e)
        {
            Program = Program.GetInstance();
            MainWindow.Show();
        }
    }
}
