using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GitGUI.Logic
{
    class TabManager
    {
        public MainWindowModel MainWindowModel { get; set; }
        CommitEditorTabViewModel CommitEditorTab { get { return (CommitEditorTabViewModel)MainWindowModel.Tabs.Find((x) => x.GetType() == typeof(CommitEditorTabViewModel)); } }

        public TabManager(MainWindowModel w, ActionPanelModel localAM)
        {
            MainWindowModel = w;
            AddMainTab(localAM);
        }

        void AddMainTab(ActionPanelModel localAM)
        {
            MainTabModel m = new MainTabModel();
            m.CloseRequested += CloseTab;
            MainTabViewModel vm = new MainTabViewModel(m);
            m.PanelModel = localAM;

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
            CommitEditorTabViewModel vm = new CommitEditorTabViewModel(m);

            MainWindowModel.AddTab(vm);
            SelectTab(vm);
        }

        private void CloseTab(TabViewModel vm)
        {
            MainWindowModel.RemoveTab(vm);
        }

        void SelectTab(TabViewModel vm)
        {
            MainWindowModel.SelectTab(vm);
        }
    }
}
