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
        bool _firstTimeActivated = true;

        private void OnActivated(object sender, EventArgs e)
        {
            if (_firstTimeActivated)
                Program = Program.GetInstance();
            _firstTimeActivated = false;
        }
    }
}
