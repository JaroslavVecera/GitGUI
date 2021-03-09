using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GitGUI;

namespace GitGUI.Logic
{
    class CommitNodeViewModel : GraphItemViewModel
    {
        public BitmapImage Bitmap { get { return ((CommitNodeModel)Model).BitmapImage; } }
        public bool EnabledPhoto { get { return ((CommitNodeModel)Model).EnabledPhoto; } }
        public string Message { get { return ((CommitNodeModel)Model).Message; } }
        public double LeftContactDist { get { return ((CommitNodeModel)Model).LeftContactDist; } }
        public double RightContactDist { get { return ((CommitNodeModel)Model).RightContactDist; } }
        public double TextStartDist { get { return ((CommitNodeModel)Model).TextStartDist; } }
        public double MaxWidth { get { return ((CommitNodeModel)Model).MaxWidth; } }
        public double TextWidth { get { return ((CommitNodeModel)Model).TextWidth; } }

        public bool InProgress { get { return ((CommitNodeModel)Model).InProgress; } }

        public CommitNodeViewModel(CommitNodeModel model, CommitNodeView view) : base(model, view)
        {
            SubscribeViewEvents(view);
            InitializeLocation();
            Model.PropertyChanged += OnPropertyChanged;
        }

        void SubscribeViewEvents(CommitNodeView view)
        {
            LocationChanged += view.OnLocationChanged;
            FocusedChanged += view.OnFocusedChanged;
            MarkedChanged += view.OnMarkedChanged;
            CheckoutedChanged += view.OnCheckoutedChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Path")
                OnPropertyChanged("Bitmap");
            if (e.PropertyName == "Message")
                OnPropertyChanged("TextWidth");
            OnPropertyChanged(e.PropertyName);
        }
    }
}
