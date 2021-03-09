using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Ookii.Dialogs.Wpf;

namespace GitGUI.Logic
{
    class Program
    {
        public StashingManager StashingManager { get; set; }
        CommitManager CommitManager { get; set; }
        RepositoryManager RepositoryManager { get; set; }
        ActionsManager ActionsManager { get; set; }
        public TabManager TabManager { get; set; }
        public UserManager UserManager { get; set;}
        IProgramState State { get; set; }
        public CrossStateData Data { get; } = new CrossStateData();
        MainWindowModel MainWindowModel { get { return Data.MainWindowModel; } set { Data.MainWindowModel = value; } }
        static Program Instance { get; set; }
        public BranchLabelModel AggregationFocused { get; set; }
        BranchLabelModel Aggregating { get; set; }
        BranchLabelModel Aggregated { get; set; }
        bool _conflict = false;

        Program()
        {
            ActionPanelModel localAM = new ActionPanelModel();

            MainWindowViewModel mwvm = InitializeMainWindow();
            CreateManagers(localAM);
            InitializeEventHandlers();
            InitializeState();
            LibGitService.GetInstance().RepositoryChanged += CheckConflicts;
            StashMenuViewModel wm = new StashMenuViewModel(StashingManager.StashMenu);
            mwvm.StashMenu = wm;
            RepositoryManager.Opened += m => MainWindowModel.RepoPath = m.RepositoryPath;
            RepositoryManager.Closed += m => MainWindowModel.RepoPath = "";
        }

        public void CloseCurrentRepository()
        {
            TabManager.CloseAll();
            RepositoryManager.CloseCurrent();
        }

        void InitializeState()
        {
            MovingBranch.GetInstance().Program = this;
            var n = Normal.GetInstance();
            n.Program = this;
            State = n;

        }

        public void OpenRepository()
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Select directory of existing repository";
            dialog.UseDescriptionForTitle = true;
            var ans = dialog.ShowDialog();
            if (ans == null || ans == false)
                return;
            RepositoryClosed();
            TabManager.AddMainTab();
            RepositoryManager.OpenExisting(dialog.SelectedPath);
        }

        void CreateManagers(ActionPanelModel localAM)
        {
            ActionsManager = new ActionsManager();
            ActionsManager.LocalRepoPanel = localAM;
            TabManager = new TabManager(Data.MainWindowModel, localAM);
            TabManager.CommitRequested += Commit;
            TabManager.AbortRequested += AbortMerge;
            TabManager.CanvasMouseDown += OnMouseDown;
            TabManager.CanvasMouseUp += OnMouseUp;
            SubscribeActionsManager();
            CommitManager = CommitManager.GetInstance();
            RepositoryManager = new RepositoryManager();
            UserManager = new UserManager();
            StashingManager = new StashingManager();
        }

        private void AbortMerge()
        {
            LibGitService.GetInstance().AbortMerge();
        }

        public void ChangeUser(User u)
        {
            UserManager.Current = u;
        }

        public void CheckConflicts()
        {
            _conflict = LibGitService.GetInstance().IsInConflictState;
            if (_conflict)
            {
                ActionsManager.TurnConflictState();
                TabManager.TurnConflictState();
            }
            else
            {
                ActionsManager.TurnNoConflictState();
                TabManager.TurnNoConflictState();
            }
        }

        void RepositoryClosed()
        {
            TabManager.CloseAll();
        }

        private void Commit(string message, IEnumerable<string> stagedFiles, IEnumerable<string> unstagedFiles)
        {
            CommitManager.Commit(null, message, stagedFiles, unstagedFiles);
            TabManager.CloseCommitEditorTab();
        }

        void SubscribeActionsManager()
        {
            ActionsManager.Commit += EditCommit;
            ActionsManager.Checkout += CheckoutMarked;
        }

        void EditCommit()
        {
            if (_conflict)
                TabManager.NewConflictEditor();
            else
                TabManager.NewCommitEditor();
        }

        void CheckoutMarked()
        {
            GraphItemModel marked = TabManager.MainTabModel.Shown;
            if (!(marked is BranchLabelModel))
                return;
            StashingManager.ImplicitPush((BranchLabelModel)marked);
            CommitManager.Checkout((BranchLabelModel)marked);
        }

        void Test()
        {
            RepositoryManager.OpenExisting(@"D:\škola\GitGUITests");
        }

        MainWindowViewModel InitializeMainWindow()
        {
            MainWindow view = (MainWindow)Application.Current.MainWindow;
            MainWindowModel model = new MainWindowModel();
            MainWindowViewModel viewModel = new MainWindowViewModel(model, view);

            MainWindowModel = model;
            view.Show();
            return viewModel;
        }

        public void ChangeState(IProgramState state)
        {
            State = state;
        }

        public void OnWindowMouseUp(object item, MouseButtonEventArgs e)
        {
            State.WindowMouseUp(item, Data, e);
        }

        public void OnWindowMouseDown(object item, MouseButtonEventArgs e)
        {
            State.WindowMouseDown(item, Data, e);
        }

        public void OnMouseDown(object item, MouseButtonEventArgs e)
        {
            State.MouseDown(item, Data, e);
        }

        public void OnMouseUp(object item, MouseButtonEventArgs e)
        {
            State.MouseUp(item, Data, e);
        }

        public void OnMouseMove(object item, MouseEventArgs e)
        {
            UpdateMousePosition(e);
            if (Data.MouseDisplacement == new Vector(0, 0))
                return;
            State.MouseMove(Data, e);
        }

        public void OnAddBranch(GraphItemModel m)
        {
            var dialog = new InputDialog();
            if (dialog.ShowDialog() == true)
            {
                LibGitService.GetInstance().Branch(m, dialog.ResponseText);
            }
        }

        public void OnMouseWheel(MouseWheelEventArgs e)
        {
            Graph.GetInstance().Scale(e.Delta, Data.MousePoint);
            State.MouseWheelMove(Data, e.Delta);
        }

        void HashCopyRequest(CommitNodeModel commitNode)
        {
            Clipboard.SetText(commitNode.Sha);
        }

        public void Show(GraphItemModel item)
        {
            CommitManager.Mark(item);
            TabManager.ShowItem(item);
            ActionsManager.OnMarkedItem(item is BranchLabelModel);
        }

        public void Aggregate(BranchLabelModel aggregating, BranchLabelModel aggregated)
        {
            Aggregating = aggregating;
            Aggregated = aggregated;
            if (_conflict)
                MainWindowModel.RaiseCantAggregateWhenConflict();
            else
                MainWindowModel.OpenAggregatingContextMenu();
        }

        public void AggregationFocus(BranchLabelModel l)
        {
            AggregationFocused = l;
        }

        public void MouseLeaveWindow()
        {
            State.MouseLeaveWindow(Data);
        }

        void InitializeEventHandlers()
        {
            EventHandlerBatch batch = new EventHandlerBatch
            {
                MouseDownEventHandler = OnMouseDown,
                MouseUpEventHandler = OnMouseUp,
                MouseEnterEventHandler = OnMouseEnter,
                MouseLeaveEventHandler = OnMouseLeave,
                CopyHash = HashCopyRequest,
                AddBranchEventHandler = OnAddBranch,
                ShowChangesEventHandler = OnShowChanges 
            };
            CommitManager.EventHandlerBatch = batch;
        }

        public void Merge()
        {
            CommitManager.Merge(Aggregating, Aggregated);
        }

        public void Rebase()
        {
            //LibGitService.GetInstance().Rebase(Aggregating, Aggregated);
            Graph.GetInstance().DeployGraph();
        }

        void OpenAggregatingContextMenu()
        {
            Application.Current.MainWindow.ContextMenu.IsOpen = true;
        }

        void OnMouseEnter(object sender, MouseEventArgs e)
        {
            State.MouseEnter(sender, Data);
        }

        void OnShowChanges(CommitNodeModel m)
        {
            TabManager.NewCommitViewer(m);
        }

        void OnMouseLeave(object sender, MouseEventArgs e)
        {
            State.MouseLeave(sender, Data);
        }

        void UpdateMousePosition(MouseEventArgs e)
        {
            Data.MousePoint = e.GetPosition((MainWindow)Application.Current.MainWindow);
        }

        public static Program GetInstance()
        {
            if (Instance == null)
                Instance = new Program();
            return Instance;
        }
    }
}
