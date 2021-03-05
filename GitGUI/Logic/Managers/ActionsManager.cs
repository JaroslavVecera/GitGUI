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
        bool _isConflict = false;
        bool _isMarkedBranch = false;
        ActionButtonModel CheckoutButton { set; get; }

        public event Action Commit;
        public event Action Checkout;

        void AddLocalRepoButtons()
        { 
            AddButton(LocalRepoPanel, "Commit", OnCommit, b => b.Text = "Commit merge", b => b.Text = "Commit");
            CheckoutButton = AddButton(LocalRepoPanel, "Checkout", OnCheckout);
        }

        public bool IsCheckoutButtonActive()
        {
            return _isMarkedBranch && !_isConflict;
        }

        public void OnMarkedItem(bool isBranch)
        {
            _isMarkedBranch = isBranch;
            CheckoutButton.Active = IsCheckoutButtonActive();
        }

        ActionButtonModel AddButton(ActionPanelModel panel, string text, Action action)
        {
            ActionButtonModel m = new ActionButtonModel();
            m.Text = text;
            m.Clicked += action;
            panel.Add(m);
            return m;
        }

        ActionButtonModel AddButton(ActionPanelModel panel, string text, Action action, Action<ActionButtonModel> conflictStateAction, Action<ActionButtonModel> noConflictStateAction)
        {
            var m = AddButton(panel, text, action);
            ConflictTurned += () => conflictStateAction(m);
            NoConflictTurned += () => noConflictStateAction(m);
            return m;
        }

        public void TurnConflictState()
        {
            _isConflict = true;
            ConflictTurned?.Invoke();
            CheckoutButton.Active = IsCheckoutButtonActive();
        }

        public void TurnNoConflictState()
        {
            _isConflict = false;
            NoConflictTurned?.Invoke();
            CheckoutButton.Active = IsCheckoutButtonActive();
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
