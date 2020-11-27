using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using LibGit2Sharp;

namespace GitGUI.Logic
{
    public class CommitNodeModel : GraphItemModel
    {
        public Commit Commit { get; private set; }
        string _path;
        public string Message { get { return Commit.Message; } }

        public bool EnabledPhoto
        {
            get { return Path != null && ((App)Application.Current).Settings.ShowAuthorMiniatures; }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; OnPropertyChanged("Path"); }
        }

        public CommitNodeModel(Commit c)
        {
            Commit = c;
            Path = @"C:\Users\Lenovo\source\repos\GitGUI\photo.jpg";
        }
    }
}
