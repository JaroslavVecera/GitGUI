using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GitGUI.Logic
{
    class MainWindowViewModel : ViewModelBase
    {
        MainWindow _mainWindow;
        public StashMenuViewModel StashMenu { get; set; }
        public MainWindowModel Model { get; set; }
        public List<TabViewModel> Tabs { get { return new List<TabViewModel>(Model.Tabs); } }
        public int SelectedIndex { get { return Model.SelectedIndex; } }
        public List<User> Users { get { return Program.GetInstance().UserManager.KnownUsers; } }
        public string RepoPath { get { return Model.RepoPath; } }
        public bool CanClose { get { return RepoPath != null && RepoPath != ""; } }
        public bool EnabledStashing { get { return CanClose; } }
        public IEnumerable<string> RecentRepos { get { return Model.RecentRepos; } }
        public bool AnyRecentRepos { get { return RecentRepos.Any(); } }
        public RelayCommand<MenuItem> OpenRecentRepo { get { return Model.OpenRecentRepo; } }

        public MainWindowViewModel(MainWindowModel model, MainWindow view)
        {
            _mainWindow = view;
            SubscribeModel(model, view);
            view.DataContext = this;
        }

        void SubscribeModel(MainWindowModel model, MainWindow view)
        {
            Model = model;
            model.ChangedTabs += () => OnPropertyChanged("Tabs");
            model.ChangedIndex += () => OnPropertyChanged("SelectedIndex");
            model.OnContextMenuOpened += () => view.OpenContextMenu();
            model.OnNoAggregationContextMenuOpened += () => view.OpenNoAggregationContextMenu();
            model.PropertyChanged += OnPropertyChanged;
            model.CaptureReleased += () => _mainWindow.ReleaseMouseCapture();
            model.Captured += () => _mainWindow.CaptureMouse();
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
            if (e.PropertyName == "RepoPath")
            {
                OnPropertyChanged("CanClose");
                OnPropertyChanged("EnabledStashing");
            }
            if (e.PropertyName == "RecentRepos")
                OnPropertyChanged("AnyRecentRepos");
        }
    }
}
