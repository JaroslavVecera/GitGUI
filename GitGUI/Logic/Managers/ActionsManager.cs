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
        event Action ConflictTurned;
        event Action NoConflictTurned;

        public event Action Commit;
        public event Action Checkout;

        void AddLocalRepoButtons()
        { 
            AddButton(LocalRepoPanel, "Commit", OnCommit, b => b.Text = "Commit merge", b => b.Text = "Commit");
            AddButton(LocalRepoPanel, "Checkout", OnCheckout, b => b.Active = false, b => b.Active = true);
        }

        ActionButtonModel AddButton(ActionPanelModel panel, string text, Action action)
        {
            ActionButtonModel m = new ActionButtonModel();
            m.Text = text;
            m.Clicked += action;
            panel.Add(m);
            return m;
        }

        void AddButton(ActionPanelModel panel, string text, Action action, Action<ActionButtonModel> conflictStateAction, Action<ActionButtonModel> noConflictStateAction)
        {
            var m = AddButton(panel, text, action);
            ConflictTurned += () => conflictStateAction(m);
            NoConflictTurned += () => noConflictStateAction(m);
        }

        public void TurnConflictState()
        {
            ConflictTurned?.Invoke();
        }

        public void TurnNoConflictState()
        {
            NoConflictTurned?.Invoke();
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
