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
    class GraphItemViewModel : ViewModelBase
    {
        UserControl _control;
        protected virtual GraphItemModel Model { get; set; }
        public Point Location { get { return Model.Location; } }
        public RelayCommand MouseDown { get; private set; }
        public RelayCommand MouseUp { get; private set; }
        public RelayCommand MouseEnter { get; private set; }
        public RelayCommand MouseLeave { get; private set; }
        public bool Focused { get { return Model.Focused; } }
        public bool Marked { get { return Model.Marked; } }
        public bool Checkouted { get { return Model.Checkouted; } }
        public MouseButtonEventArgs MouseButtonArgs { get; set; }
        public System.Windows.Input.MouseEventArgs MouseArgs { get; set; }
        public event Action<Point> LocationChanged;
        public event Action FocusedChanged;
        public event Action MarkedChanged;
        public event Action CheckoutedChanged;

        public GraphItemViewModel(GraphItemModel model, UserControl view)
        {
            Model = model;
            SubscribeModel();
            _control = view;
            view.DataContext = this;
            InitializeCommands();
        }

        protected void InitializeLocation()
        {
            Canvas.SetLeft(_control, 0);
            Canvas.SetTop(_control, 0);
            LocationChanged?.Invoke(Location);
        }

        void BackgroundPush()
        {
            Panel.SetZIndex(_control, 0);
        }

        void ForegroundPull()
        {
            Panel.SetZIndex(_control, 1);
        }

        public void SubscribeModel()
        {
            Model.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Marked")
                {

                }
                OnPropertyChanged(e.PropertyName);
                if (e.PropertyName == "Focused")
                    FocusedChanged?.Invoke();
            };
        }

        protected virtual void InitializeCommands()
        {
            MouseDown = new RelayCommand(() =>
            {
                MouseButtonArgs.Handled = true;
                Model.OnMouseDown(MouseButtonArgs);
            });
            MouseUp = new RelayCommand(() =>
            {
                MouseButtonArgs.Handled = true;
                Model.OnMouseUp(MouseButtonArgs);
            });
            MouseEnter = new RelayCommand(() =>
            {
                MouseArgs.Handled = true;
                Model.OnMouseEnter(MouseArgs);
            });
            MouseLeave = new RelayCommand(() =>
            {
                MouseArgs.Handled = true;
                Model.OnMouseLeave(MouseArgs);
            });
        }
    }
}
