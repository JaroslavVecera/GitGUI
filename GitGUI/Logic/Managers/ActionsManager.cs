using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GitGUI.Logic
{
    class ActionsManager
    {
        public ActionPanelModel RemoteRepoPanel { get; set; }
        public ActionPanelModel LocalRepoPanel { get; set; }

        void AddButton(ActionPanelModel panel, string text, Action action)
        {
            ActionButtonModel m = new ActionButtonModel();
            Button view = new Button();
            ActionButtonViewModel vm = new ActionButtonViewModel(m, view);
            m.Clicked += action;
            m.Text = text;
            panel.Add(view);
        }

        public ActionsManager()
        {
            AddButtons();
        }

        void AddButtons()
        { 
            AddButton(LocalRepoPanel, "Commit", OnCommit);
        }

        void OnCommit()
        {

        }
    }
}
