using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace GitGUI.Logic
{
    class ActionsManager
    {
        ActionPanelModel _local;
        ActionPanelModel _remoteLeftGroup;
        ActionPanelModel _remoteRightGroup;
        bool _isItem = false;
        public ActionPanelModel LocalRepoPanel { get { return _local; } set { _local = value; AddLocalRepoButtons(); } }
        public ActionPanelModel RemoteRepoLeftGroupPanel { get { return _remoteLeftGroup; } set { _remoteLeftGroup = value; AddRemoteLeftRepoButtons(); } }
        public ActionPanelModel RemoteRepoRightGroupPanel { get { return _remoteRightGroup; } set { _remoteRightGroup = value; AddRemoteRightRepoButtons(); } }
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

        void AddRemoteLeftRepoButtons()
        {
            AddButton(RemoteRepoLeftGroupPanel, "Push", OnPush, 80, "M6,22 L6,9 L2,9 L8,2 L14,9 L10,9 L10,22", true);
        }

        void AddRemoteRightRepoButtons()
        { 
            AddButton(RemoteRepoRightGroupPanel, "Fetch", OnFetch, 80, "M6,2 L6,15 L2,15 L8,22 L14,15 L10,15 L10,2", false);
            AddButton(RemoteRepoRightGroupPanel, "Pull", OnPull, 80, "M6,2 L6,15 L2,15 L8,22 L14,15 L10,15 L10,2", true);
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
            RemoteRepoLeftGroupPanel.Enabled = RemoteRepoRightGroupPanel.Enabled = Program.GetInstance().RemoteManager.CurrentRemotes.Any();
        }

        ActionButtonModel AddButton(ActionPanelModel panel, string text, Action action, double width, string pathData, bool filled)
        {
            ActionButtonModel m = AddButton(panel, text, action);
            m.Width = width;
            m.FilledPath = filled;
            m.PathData = Geometry.Parse(pathData);
            return m;
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

        void OnPush()
        {
            LibGitNetworkService.GetInstance().Push();
        }

        void OnPull()
        {
            LibGitNetworkService.GetInstance().Pull();
        }

        void OnFetch()
        {
            LibGitNetworkService.GetInstance().Fetch();
        }
    }
}
