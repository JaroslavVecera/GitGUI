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

        public List<TabViewModel> Tabs { get; private set; } = new List<TabViewModel>();
        public int SelectedIndex { get; private set; }
        public event Action OnContextMenuOpened;
        public event Action OnNoAggregationContextMenuOpened;
        public string RepoPath { get { return _repoPath; } set { _repoPath = value; OnPropertyChanged(); } }
        public IEnumerable<string> RecentRepos { get { return _recentRepos; } set { _recentRepos = value; OnPropertyChanged(); } }
        public RelayCommand<MenuItem> OpenRecentRepo { get; }

        public MainWindowModel()
        {
            OpenRecentRepo = new RelayCommand<MenuItem>(i => Program.GetInstance().OpenRepository((string)i.Header));
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
