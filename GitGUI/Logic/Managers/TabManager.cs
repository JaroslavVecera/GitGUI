using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GitGUI.Logic
{
    class TabManager
    {
        public event Action<string, IEnumerable<string>, IEnumerable<string>> CommitRequested;
        public event Action AbortRequested;
        public event MouseButtonEventHandler CanvasMouseDown;
        public event MouseButtonEventHandler CanvasMouseUp;
        ActionPanelModel ActionPanel { get; set; }
        public MainTabModel MainTabModel { private set; get; }
        Dictionary<CommitNodeModel, CommitViewerTabViewModel> CommitViewers { get; } = new Dictionary<CommitNodeModel, CommitViewerTabViewModel>();
        IEnumerable<TabViewModel> Tabs
        {
            get
            {
                var editorTabs = new List<TabViewModel>();
                if (CommitEditorTab != null)
                    editorTabs.Add(CommitEditorTab);
                if (ConflictEditorTab != null)
                    editorTabs.Add(ConflictEditorTab);
                if (MainTab != null)
                    editorTabs.Add(MainTab);
                return editorTabs.Union(CommitViewers.Values);
            }
        }

        public MainWindowModel MainWindowModel { get; set; }
        CommitEditorTabViewModel CommitEditorTab
        {
            get
            { return (CommitEditorTabViewModel)MainWindowModel.Tabs.Find(
                (x) => x.GetType() == typeof(CommitEditorTabViewModel));
            }
        }

        MainTabViewModel MainTab
        {
            get
            {
                return (MainTabViewModel)MainWindowModel.Tabs.Find(
                (x) => x.GetType() == typeof(MainTabViewModel));
            }
        }

        ConflictEditorTabViewModel ConflictEditorTab
        {
            get
            {
                return (ConflictEditorTabViewModel)MainWindowModel.Tabs.Find(
                  (x) => x.GetType() == typeof(ConflictEditorTabViewModel));
            }
        }

        public Point GraphViewCenter { get { return MainTabModel.GraphViewCenter; } }

        public TabManager(MainWindowModel w, ActionPanelModel localAM)
        {
            ActionPanel = localAM;
            MainWindowModel = w;
        }

        public void AddMainTab()
        {
            if (MainTab != null)
            {
                SelectTab(MainTab);
                return;
            }
            MainTabModel m = new MainTabModel();
            m.CloseRequested += CloseTab;
            m.MouseDown += (args) => CanvasMouseDown?.Invoke(null, args);
            m.MouseUp += (args) => CanvasMouseUp?.Invoke(null, args);
            m.PanelModel = ActionPanel;
            MainTabViewModel vm = new MainTabViewModel(m);
            MainTabModel = m;

            MainWindowModel.AddTab(vm);
            SelectTab(vm);
            Graph.GetInstance().DeployGraph();
        }

        public void NewCommitEditor()
        {
            if (CommitEditorTab != null)
            {
                SelectTab(CommitEditorTab);
                return;
            }
            CommitEditorTabModel m = new CommitEditorTabModel();
            m.CloseRequested += CloseTab;
            m.CommitRequest += Commit;
            CommitEditorTabViewModel vm = new CommitEditorTabViewModel(m);

            MainWindowModel.AddTab(vm);
            SelectTab(vm);
        }

        public void NewConflictEditor()
        {
            if (ConflictEditorTab != null)
            {
                SelectTab(ConflictEditorTab);
                return;
            }
            ConflictEditorTabModel m = new ConflictEditorTabModel();
            m.CloseRequested += CloseTab;
            m.CommitRequest += Commit;
            m.AbortRequest += Abort;
            ConflictEditorTabViewModel vm = new ConflictEditorTabViewModel(m);

            MainWindowModel.AddTab(vm);
            SelectTab(vm);
        }

        private void Abort()
        {
            AbortRequested.Invoke();
        }

        public void NewCommitViewer(CommitNodeModel m)
        {
            CommitViewerTabViewModel cv;
            if (CommitViewers.Keys.Contains(m))
            {
                cv = CommitViewers[m];
            }
            else
            {
                cv = CreateViewer(m);
                CommitViewers.Add(m, cv);
                MainWindowModel.AddTab(cv);
            }
            SelectTab(cv);
        }

        internal void CloseAll()
        {
            Tabs.ToList().ForEach(x => CloseTab(x));
        }

        void Commit(string message, IEnumerable<string> stagedFiles, IEnumerable<string> unstagedFiles)
        {
            CommitRequested?.Invoke(message, stagedFiles, unstagedFiles);
        }

        public void CloseTab(TabViewModel vm)
        {
            if (vm is MainTabViewModel)
                DoCloseTab((MainTabViewModel)vm);
            else if (vm is CommitEditorTabViewModel)
                DoCloseTab((CommitEditorTabViewModel)vm);
            else if (vm is CommitViewerTabViewModel)
                DoCloseTab((CommitViewerTabViewModel)vm);
            else if (vm is ConflictEditorTabViewModel)
                DoCloseTab((ConflictEditorTabViewModel)vm);
            else
                throw new NotImplementedException();
        }

        public void DoCloseTab(MainTabViewModel vm)
        {
            MainWindowModel.RemoveTab(vm);
            vm.Model.CloseRequested -= CloseTab;
            vm.Model.MouseDown -= (args) => CanvasMouseDown?.Invoke(null, args);
            vm.Model.MouseUp -= (args) => CanvasMouseUp?.Invoke(null, args);
        }

        public void DoCloseTab(CommitEditorTabViewModel vm)
        {
            MainWindowModel.RemoveTab(vm);
            vm.Model.CloseRequested -= CloseTab;
            ((CommitEditorTabModel)vm.Model).CommitRequest -= Commit;
            ((CommitEditorTabModel)vm.Model).FreeEvents();
        }

        public void DoCloseTab(ConflictEditorTabViewModel vm)
        {
            MainWindowModel.RemoveTab(vm);
            vm.Model.CloseRequested -= CloseTab;
            ((ConflictEditorTabModel)vm.Model).CommitRequest -= Commit;
            ((ConflictEditorTabModel)vm.Model).AbortRequest -= Abort;
            ((ConflictEditorTabModel)vm.Model).FreeEvents();
        }

        public void DoCloseTab(CommitViewerTabViewModel vm)
        {
            MainWindowModel.RemoveTab(vm);
            vm.Model.CloseRequested -= CloseTab;
            CommitViewers.Remove(((CommitViewerTabModel)(vm).Model).Commit);
            ((CommitViewerTabModel)vm.Model).FreeEvents();
        }

        public void CloseCommitEditorTab()
        {
            MainWindowModel.RemoveTab(CommitEditorTab);
        }

        void SelectTab(TabViewModel vm)
        {
            MainWindowModel.SelectTab(vm);
        }

        public void ShowItem(GraphItemModel m)
        {
            MainTabModel.Shown = m;
        }

        public void TurnConflictState()
        {
            NewConflictEditor();
            if (CommitEditorTab != null)
                CloseTab(CommitEditorTab);
        }

        public void TurnNoConflictState()
        {
            if (ConflictEditorTab != null)
                CloseTab(ConflictEditorTab);
        }

        CommitViewerTabViewModel CreateViewer(CommitNodeModel m)
        {
            CommitViewerTabModel c = new CommitViewerTabModel(m);
            c.CloseRequested += CloseTab;
            CommitViewerTabViewModel vm = new CommitViewerTabViewModel(c);
            return vm;
        }
    }
}
