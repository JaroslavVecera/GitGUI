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
        public event Action<string, IEnumerable<string>> CommitRequested;
        public event MouseButtonEventHandler CanvasMouseDown;
        MainTabModel MainTabModel { set; get; }

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
            m.MouseDown += (args) =>
            CanvasMouseDown?.Invoke(null, args);
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

        void Commit(string message, IEnumerable<string> paths)
        {
            CommitRequested?.Invoke(message, paths);
        }

        public void CloseTab(TabViewModel vm)
        {
            MainWindowModel.RemoveTab(vm);
        }

        public void CloseCommitEditorTab()
        {
            MainWindowModel.RemoveTab(CommitEditorTab);
        }

        void SelectTab(TabViewModel vm)
        {
            MainWindowModel.SelectTab(vm);
        }
    }
}
