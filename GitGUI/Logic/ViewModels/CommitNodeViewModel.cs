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
        public MouseButtonEventArgs MouseButtonArgs { get; set; }
        public event Action<Point> LocationChanged;

        public CommitNodeViewModel(CommitNodeModel model, CommitNodeView view) : base(view)
        {
            InitializeCommands();
            Model = model;
            view.DataContext = this;
            LocationChanged += view.OnLocationChanged;
            SubscribeModel();
        }

        void InitializeCommands()
        {
            MouseDown = new RelayCommand(() =>
            {
                MouseButtonArgs.Handled = true;
                Model.OnMouseDown(MouseButtonArgs);
            });
        }

        public void SubscribeModel()
        {
            Model.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Location")
                    LocationChanged?.Invoke(Location);
            };
        }
    }
}
