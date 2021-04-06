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
        bool _isItem = false;
        public ActionPanelModel LocalRepoPanel { get { return _local; } set { _local = value; AddLocalRepoButtons(); } }
        public ActionPanelModel RemoteRepoPanel { get { return _remote; } set { _remote = value; AddRemoteRepoButtons(); } }
        event Action ConflictTurned;
        event Action NoConflictTurned;
        bool _isConflict = false;
        ActionButtonModel CheckoutButton { set; get; }
        ActionButtonModel StashButton { set; get; }

        public event Action Commit;
        public event Action Checkout;
        public event Action Stash;

        void AddLocalRepoButtons()
        { 
            AddButton(LocalRepoPanel, "Commit", OnCommit);
            CheckoutButton = AddButton(LocalRepoPanel, "Checkout", OnCheckout);
            StashButton = AddButton(LocalRepoPanel, "Stash", OnStash);
        }

        void AddRemoteRepoButtons()
        {
            AddButton(RemoteRepoPanel, "Test", OnPokus);
            AddButton(RemoteRepoPanel, "Test2", OnPokus2);
        }

        public bool IsCheckoutButtonActive()
        {
            return !_isConflict && _isItem;
        }

        public void OnMarkedItem(bool isItem)
        {
            _isItem = isItem;
            CheckoutButton.Active = IsCheckoutButtonActive();
        }

        public void OnWorkTreeChanged(bool hasChanges)
        {
            StashButton.Active = hasChanges;
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

        void OnStash()
        {
            Stash?.Invoke();
        }

        void OnPokus()
        {
            LibGitNetworkService.GetInstance().Push();
        }

        void OnPokus2()
        {
            LibGitNetworkService.GetInstance().Fetch();
        }
    }
}
