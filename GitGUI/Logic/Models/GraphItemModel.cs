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
        bool _marked, _focused, _checkouted, _plusButton = true;
        public bool Marked { get { return _marked; } set { _marked = value; OnPropertyChanged(); } }
        public bool Focused { get { return _focused; } set { _focused = value; OnPropertyChanged(); } }
        public bool Checkouted { get { return _checkouted; } set { _checkouted = value; OnPropertyChanged(); } }
        Point _location = new Point(0, 0);
        public bool Shared { get; set; }
        public bool Stashed { get; set; }
        public bool PlusButton { get { return _plusButton; } set { _plusButton = value; OnPropertyChanged(); } }
        public Point Location
        {
            get { return _location; }
            set { _location = value; OnPropertyChanged(); }
        }

        public void OnAddBranch()
        {
            AddBranch?.Invoke(this);
        }

        public event Action<GraphItemModel> AddBranch;
        public event Action<GraphItemModel, MouseButtonEventArgs> MouseDown;
        public event Action<GraphItemModel, MouseButtonEventArgs> MouseUp;
        public event Action<GraphItemModel, MouseEventArgs> MouseEnter;
        public event Action<GraphItemModel, MouseEventArgs> MouseLeave;
        public event Action Pulled;
        public event Action Pushed;

        public void OnMouseDown(MouseButtonEventArgs e)
        {
            MouseDown?.Invoke(this, e);
        }

        public void OnMouseUp(MouseButtonEventArgs e)
        {
            MouseUp?.Invoke(this, e);
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
