using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GitGUI;

namespace GitGUI.Logic
{
    class CommitNodeViewModel : GraphItemViewModel
    {
        public BitmapImage Bitmap { get { return ((CommitNodeModel)Model).BitmapImage; } }
        public bool EnabledPhoto { get { return ((CommitNodeModel)Model).EnabledPhoto; } }
        public string Message { get { return ((CommitNodeModel)Model).Message; } }
        public double Width
        {
            get; set;
        }
        public bool InProgress { get { return ((CommitNodeModel)Model).InProgress; } }

        public CommitNodeViewModel(CommitNodeModel model, CommitNodeView view) : base(model, view)
        {
            SubscribeViewEvents(view);
            InitializeLocation();
            Model.PropertyChanged += OnPropertyChanged;
            Width = view.EdgeOffset;
        }

        void SetViewProperties(CommitNodeView view)
        {
            view.EnabledPhoto = EnabledPhoto;
            view.message.Text = Message;
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
            OnPropertyChanged(e.PropertyName);
        }
    }
}
