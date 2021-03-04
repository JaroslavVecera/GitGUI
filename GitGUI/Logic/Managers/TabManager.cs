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
        public event MouseButtonEventHandler CanvasMouseDown;
        public event MouseButtonEventHandler CanvasMouseUp;
        public MainTabModel MainTabModel { private set; get; }
        Dictionary<CommitNodeModel, CommitViewerTabViewModel> CommitViewers { get; } = new Dictionary<CommitNodeModel, CommitViewerTabViewModel>();
        IEnumerable<TabViewModel> Tabs
        {
            get
            {
                var editorTabs = new List<TabViewModel>();
                if (CommitEditorTab != null)
                    editorTabs.Add(CommitEditorTab);
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

        public Point GraphViewCenter { get { return MainTabModel.GraphViewCenter; } }

        public TabManager(MainWindowModel w, ActionPanelModel localAM)
        {
            MainWindowModel = w;
            AddMainTab(localAM);
        }

        void AddMainTab(ActionPanelModel localAM)
        {
            MainTabModel m = new MainTabModel();
            m.CloseRequested += CloseTab;
            m.MouseDown += (args) => CanvasMouseDown?.Invoke(null, args);
            m.MouseUp += (args) => CanvasMouseUp?.Invoke(null, args);
            MainTabViewModel vm = new MainTabViewModel(m);
            m.PanelModel = localAM;
            MainTabModel = m;

            MainWindowModel.AddTab(vm);
            SelectTab(vm);
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

        }

        public void TurnNoConflictState()
        {

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
