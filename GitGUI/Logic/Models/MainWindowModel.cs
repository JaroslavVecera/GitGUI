using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GitGUI.Logic
{
    class MainWindowModel : ModelBase
    {
        public event Action ChangedTabs;
        public event Action ChangedIndex;
        IEnumerable<string> _recentRepos;
        string _repoPath = "";
        bool _isWaiting = false;
        bool _noConflicts = true;
        public bool NoConflicts { get { return _noConflicts; } set { _noConflicts = value; OnPropertyChanged(); } }

        public event Action Captured;
        public event Action CaptureReleased;
        public List<TabViewModel> Tabs { get; private set; } = new List<TabViewModel>();
        public int SelectedIndex { get; private set; }
        public event Action OnContextMenuOpened;
        public event Action OnNoAggregationContextMenuOpened;
        public string RepoPath { get { return _repoPath; } set { _repoPath = value; OnPropertyChanged(); } }
        public IEnumerable<string> RecentRepos { get { return _recentRepos; } set { _recentRepos = value; OnPropertyChanged(); } }
        public RelayCommand<MenuItem> OpenRecentRepo { get; private set; }
        public RelayCommand CreateNewUser { get; private set; }
        public RelayCommand ShareDatabase { get; private set; }
        public RelayCommand UpdateDatabase { get; private set; }
        public RelayCommand CreateNewRemote { get; private set; }
        public Remote SelectedRemote { get { return Program.GetInstance().RemoteManager.SelectedRemote; } set { Program.GetInstance().RemoteManager.SelectedRemote = value; } }
        public ActionPanelModel RemoteLeftPanelModel { get; } = new ActionPanelModel();
        public ActionPanelModel RemoteRightPanelModel { get; } = new ActionPanelModel();
        public bool IsWaiting { get { return _isWaiting; } set { _isWaiting = value; OnPropertyChanged(); } }

        public void OnUsersChanged()
        {
            OnPropertyChanged("Users");
        }

        public MainWindowModel()
        {
            InitializeCommands();
        }

        void InitializeCommands()
        { 
            OpenRecentRepo = new RelayCommand<MenuItem>(i => Program.GetInstance().OpenRecentRepository((string)i.Header));
            CreateNewUser = new RelayCommand(() => Program.GetInstance().UserManager.CreateNewUser());
            CreateNewRemote = new RelayCommand(() => Program.GetInstance().RemoteManager.CreateRemote());
            UpdateDatabase = new RelayCommand(() => Program.GetInstance().UserManager.UpdateDatabase());
            ShareDatabase = new RelayCommand(() => Program.GetInstance().UserManager.ShareDatabase());
        }

        public void OpenAggregatingContextMenu()
        {
            OnContextMenuOpened?.Invoke();
        }

        public void RaiseCantAggregateWhenConflict()
        {
            OnNoAggregationContextMenuOpened?.Invoke();
        }

        public void SelectTab(TabViewModel vm)
        {
            SelectedIndex = Tabs.IndexOf(vm);
            ChangedIndex?.Invoke();
        }

        public void AddTab(TabViewModel vm)
        {
            Tabs.Add(vm);
            ChangedTabs?.Invoke();
        }

        public void RemoveTab(TabViewModel vm)
        {
            Tabs.Remove(vm);
            ChangedTabs?.Invoke();
            if (SelectedIndex > 0)
                SelectedIndex -= 1;
            ChangedIndex?.Invoke();
        }
    }
}
