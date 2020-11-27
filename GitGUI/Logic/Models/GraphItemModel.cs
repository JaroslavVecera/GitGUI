using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GitGUI.Logic
{
    public class GraphItemModel : ModelBase
    {
        public bool Marked { get; set; }
        Point _location = new Point(0, 0);
        public bool Shared { get; set; }
        public bool Stashed { get; set; }
        public bool Checkouted { get; set; }
        public Point Location
        {
            get { return _location; }
            set { _location = value; OnPropertyChanged(); }
        }
        
        public event Action<GraphItemModel, MouseButtonEventArgs> MouseDown;
        public event Action<GraphItemModel, MouseEventArgs> MouseEnter;
        public event Action<GraphItemModel, MouseEventArgs> MouseLeave;
        public event Action Pulled;
        public event Action Pushed;

        public void OnMouseDown(MouseButtonEventArgs e)
        {
            MouseDown?.Invoke(this, e);
        }

        public void OnMouseEnter(MouseEventArgs e)
        {
            MouseEnter?.Invoke(this, e);
        }

        public void OnMouseLeave(MouseEventArgs e)
        {
            MouseLeave?.Invoke(this, e);
        }

        public void ForegroundPull()
        {
            Pulled?.Invoke();
        }

        public void BackgroundPush()
        {
            Pushed?.Invoke();
        }
    }
}
