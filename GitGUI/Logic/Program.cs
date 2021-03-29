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
            LibGitService.GetInstance().RepositoryChanged += () => Show(null);
            LibGitService.GetInstance().RepositoryChanged += () => ActionsManager.OnWorkTreeChanged(LibGitService.GetInstance().HasChanges);
            StashMenuViewModel wm = new StashMenuViewModel(StashingManager.StashMenu);
            mwvm.StashMenu = wm;
            RepositoryManager.Opened += m => MainWindowModel.RepoPath = m.RepositoryPath;
            MainWindowModel.RecentRepos = RepositoryManager.RecentRepos;
            RepositoryManager.Closed += m => { MainWindowModel.RepoPath = ""; MainWindowModel.RecentRepos = RepositoryManager.RecentRepos; };
            RepositoryManager.RecentRepositoryChanged += () => MainWindowModel.RecentRepos = RepositoryManager.RecentRepos;
            UserManager.UsersChanged += () => MainWindowModel.OnUsersChanged();
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

        public void OpenRepository(string path)
        {
            RepositoryClosed();
            TabManager.AddMainTab();
            if (!RepositoryManager.OpenExisting(path))
                TabManager.CloseAll();
            else
                Graph.GetInstance().ResetTranslate();
        }

        public void OpenRecentRepository(string path)
        {
            RepositoryClosed();
            TabManager.AddMainTab();
            if (!RepositoryManager.OpenRecent(path))
                TabManager.CloseAll();
            else
                Graph.GetInstance().ResetTranslate();
        }

        public void CreateRepository(string path)
        {
            RepositoryClosed();
            TabManager.AddMainTab();
            var v = RepositoryManager.Create(path);
            if (v != RepositoryValidation.Invalid)
            {
                TabManager.CloseAll();
                MessageBox.Show("There is already a repository", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Graph.GetInstance().ResetTranslate();
            }
        }

        public void OpenRepository()
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Select directory of existing repository";
            dialog.UseDescriptionForTitle = true;
            var ans = dialog.ShowDialog();
            if (ans == null || ans == false)
                return;
            OpenRepository(dialog.SelectedPath);
        }

        public void CreateRepository()
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Select directory for new repository";
            dialog.UseDescriptionForTitle = true;
            var ans = dialog.ShowDialog();
            if (ans == null || ans == false)
                return;
            CreateRepository(dialog.SelectedPath);
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
            UserManager.ChangeUser(u);
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
            ActionsManager.Stash += () => LibGitService.GetInstance().Stash();
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
            CommitManager.Checkout(marked);
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
            Graph.GetInstance().ReleaseMouseCapture();
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
            Graph.GetInstance().ReleaseMouseCapture();
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
            ActionsManager.OnMarkedItem(item != null);
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
            CommitManager.AggregationFocus(l);
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
