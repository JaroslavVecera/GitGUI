using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace GitGUI.Logic
{
    class ActionsManager
    {
        public ActionPanelModel RemoteRepoPanel { get; set; }
        public ActionPanelModel LocalRepoPanel { get; set; }

        public ActionsManager()
        {
            CreateLocalRepoPanel();
            AddLocalRepoButtons();
        }

        void CreateLocalRepoPanel()
        {
            ActionPanelView view = ((MainWindow)App.Current.MainWindow).actionPanel;
            LocalRepoPanel = new ActionPanelModel();
            ActionPanelViewModel viewModel = new ActionPanelViewModel(LocalRepoPanel, view);
        }

        void AddLocalRepoButtons()
        { 
            AddButton(LocalRepoPanel, "Commit", OnCommit);
            AddButton(LocalRepoPanel, "Checkout", OnCheckout);
            AddButton(LocalRepoPanel, "Checkout", OnCheckout);
        }

        void AddButton(ActionPanelModel panel, string text, Action action)
        {
            ActionButtonModel m = new ActionButtonModel();
            m.Text = text;
            m.Clicked += action;
            panel.Add(m);
        }

        void OnCommit()
        {
            Console.WriteLine("po");
        }

        void OnCheckout()
        {

        }
    }
}
