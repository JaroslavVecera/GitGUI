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
        ActionPanelModel _local;
        ActionPanelModel _remote;
        public ActionPanelModel LocalRepoPanel { get { return _local; } set { _local = value; AddLocalRepoButtons(); } }
        public ActionPanelModel RemoteRepoPanel { get; set; }

        public event Action Commit;
        public event Action Checkout;

        void AddLocalRepoButtons()
        { 
            AddButton(LocalRepoPanel, "Commit", OnCommit);
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
            Commit?.Invoke();
        }

        void OnCheckout()
        {
            Checkout?.Invoke();
        }
    }
}
