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
        double _maxWidth = 150;
        double _textLength;
        public CommitNodeModel(Commit c)
        {
            Commit = c;
        }

        protected void MeasureTextWidth(TextBlock b)
        {
            b.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            if (b.DesiredSize.Width <= _maxWidth)
                _textLength = b.DesiredSize.Width;
            else
                _textLength = Math.Min(_maxWidth, b.DesiredSize.Width / 2);
        }

        public bool EnabledPhoto
        {
            get { return Path != null && ((App)Application.Current).Settings.ShowAuthorMiniatures; }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; OnPropertyChanged("Path"); OnPropertyChanged("EnabledPhoto"); }
        }

        public void OnChangeProfilPhoto(string path)
        {
            Path = path;
        }
    }
}
