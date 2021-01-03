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
        public string Path { get { return ((CommitNodeModel)Model).Path; } }
        public bool EnabledPhoto { get { return ((CommitNodeModel)Model).EnabledPhoto; } }
        public string Message { get { return ((CommitNodeModel)Model).Message; } }
        public RelayCommand PlusCommand { get; private set; }

        public CommitNodeViewModel(CommitNodeModel model, CommitNodeView view) : base(model, view)
        {
            SubscribeViewEvents(view);
            InitializeLocation();
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
    }
}
