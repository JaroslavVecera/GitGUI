using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class MainWindowViewModel : ViewModelBase
    {
        public MainWindowModel Model { get; set; }
        public List<TabViewModel> Tabs { get { return new List<TabViewModel>(Model.Tabs); } }
        public int SelectedIndex { get { return Model.SelectedIndex; } }
        public List<User> Users { get { return Program.GetInstance().UserManager.KnownUsers; } }

        public MainWindowViewModel(MainWindowModel model, MainWindow view)
        {
            SubscribeModel(model, view);
            view.DataContext = this;
        }

        void SubscribeModel(MainWindowModel model, MainWindow view)
        {
            Model = model;
            model.ChangedTabs += () => OnPropertyChanged("Tabs");
            model.ChangedIndex += () => OnPropertyChanged("SelectedIndex");
            model.OnContextMenuOpened += () => view.OpenContextMenu();
        }
    }
}
