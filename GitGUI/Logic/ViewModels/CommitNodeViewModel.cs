using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using GitGUI;

namespace GitGUI.Logic
{
    class CommitNodeViewModel : GraphItemViewModel
    {
        CommitNodeModel Model { get; set; }
        public string Path { get { return Model.Path; } }
        public bool EnabledPhoto { get { return Model.EnabledPhoto; } }
        public string Message { get { return Model.Message; } }
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

        public CommitNodeViewModel(CommitNodeModel model, CommitNodeView view) : base(view)
        {
            InitializeCommands();
            Model = model;
            view.DataContext = this;
            SubscribeViewEvents(view);
            SubscribeModel();
        }

        void SubscribeViewEvents(CommitNodeView view)
        {
            LocationChanged += view.OnLocationChanged;
            FocusedChanged += view.OnFocusedChanged;
            MarkedChanged += view.OnMarkedChanged;
            CheckoutedChanged += view.OnCheckoutedChanged;

        }

        void InitializeCommands()
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

        public void SubscribeModel()
        {
            Model.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Marked")
                {

                }
                OnPropertyChanged(e.PropertyName);
                if (e.PropertyName == "Location")
                    LocationChanged?.Invoke(Location);
                else if (e.PropertyName == "Focused")
                    FocusedChanged?.Invoke();
            };
        }
    }
}
