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
        public BitmapImage Bitmap
        { get
            {
                string path = ((CommitNodeModel)Model).Path;
                if (path == null)
                    return null;
                BitmapImage bit = new BitmapImage();
                bit.BeginInit();
                bit.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bit.CacheOption = BitmapCacheOption.OnLoad;
                bit.UriSource = new Uri(path);
                bit.EndInit();
                return bit;
            }
        }
        public bool EnabledPhoto { get { return ((CommitNodeModel)Model).EnabledPhoto; } }
        public string Message { get { return ((CommitNodeModel)Model).Message; } }
        public RelayCommand PlusCommand { get; private set; }

        public CommitNodeViewModel(CommitNodeModel model, CommitNodeView view) : base(model, view)
        {
            SubscribeViewEvents(view);
            InitializeLocation();
            Model.PropertyChanged += OnPropertyChanged;
        }

        override protected void InitializeCommands()
        {
            base.InitializeCommands();
            PlusCommand = new RelayCommand(() => ((CommitNodeModel)Model).OnAddBranch());
            OnPropertyChanged("PlusCommand");
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
