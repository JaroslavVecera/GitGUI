using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GitGUI.Logic
{
    public class ActionButtonModel : ModelBase
    {
        public double Width { get; set; } = 150;
        public bool FilledPath { get; set; } = true;
        string _text;
        public Geometry PathData { set; get; }
        public string Text
        {
            get { return _text; }
            set { _text = value; OnPropertyChanged(); }
        }
        bool _active = true;
        public bool Active { get { return _active; } set { _active = value; OnPropertyChanged(); } }

        public event Action Clicked;

        public void OnClicked(object sender, RoutedEventArgs e)
        {
            Clicked?.Invoke();
        }
    }
}
