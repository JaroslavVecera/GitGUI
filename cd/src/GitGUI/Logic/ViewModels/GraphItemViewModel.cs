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
    abstract class GraphItemViewModel : ViewModelBase
    {
        public UserControl Control { get; set; }
        bool _hitTestVisible = true;
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
        public bool PlusButton { get { return Model.PlusButton; } }
        public RelayCommand PlusCommand { get; private set; }
        public bool HitTestVisible { get { return _hitTestVisible; } set { _hitTestVisible = value; OnPropertyChanged(); } }

        public GraphItemViewModel(GraphItemModel model, UserControl view)
        {
            Model = model;
            SubscribeModel();
            Control = view;
            view.DataContext = this;
            InitializeCommands();
        }

        protected void InitializeLocation()
        {
            Canvas.SetLeft(Control, 0);
            Canvas.SetTop(Control, 0);
            LocationChanged?.Invoke(Location);
        }

        protected void BackgroundPush()
        {
            Panel.SetZIndex(Control, 2);
            HitTestVisible = true;
        }

        protected void ForegroundPull()
        {
            Panel.SetZIndex(Control, 3);
            HitTestVisible = false;
        }

        public virtual void SubscribeModel()
        {
            Model.PropertyChanged += OnPropertyChanged;
        }

        protected abstract void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e);

        protected void CommonPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
            if (e.PropertyName == "Focused")
                FocusedChanged?.Invoke();
            if (e.PropertyName == "Checkouted")
                CheckoutedChanged?.Invoke();
        }

        public virtual void UnsubscribeModel()
        {
            Model.PropertyChanged -= OnPropertyChanged;
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
            PlusCommand = new RelayCommand(() => (Model).OnAddBranch());
            OnPropertyChanged("PlusCommand");
        }
    }
}
