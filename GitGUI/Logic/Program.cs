using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Ookii.Dialogs.Wpf;

namespace GitGUI.Logic
{
    class Program
    {
        string _dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GitGUI");
        public StashingManager StashingManager { get; set; }
        public RemoteManager RemoteManager { get; set; }
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
        WaitingDialog WaitingDialog { get; set; } = new WaitingDialog();
        Thread DialogThread { get; set; }

        Program()
        {
            ActionPanelModel localAM = new ActionPanelModel();
            MainWindowViewModel mwvm = InitializeMainWindow();
            ActionPanelModel remoteLeftAM = MainWindowModel.RemoteLeftPanelModel;
            ActionPanelModel remoteRightAM = MainWindowModel.RemoteRightPanelModel;
            CreateManagers(localAM, remoteLeftAM, remoteRightAM);
            InitializeEventHandlers();
            InitializeState();
            LibGitService.GetInstance().RepositoryChanged += CheckConflicts;
            LibGitService.GetInstance().RepositoryChanged += () => Show(null);
            LibGitService.GetInstance().RepositoryChanged += () => ActionsManager.OnWorkTreeChanged(LibGitService.GetInstance().HasChanges);
            StashMenuViewModel wm = new StashMenuViewModel(StashingManager.StashMenu);
            mwvm.StashMenu = wm;
            RepositoryManager.Opened += m => MainWindowModel.RepoPath = m.RepositoryPath;
            MainWindowModel.RecentRepos = RepositoryManager.RecentRepos;
            RepositoryManager.Closed += m => { MainWindowModel.RepoPath = ""; MainWindowModel.RecentRepos = RepositoryManager.RecentRepos; RemoteManager.Reset(); };
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

        void OpenRepository(Func<bool> func)
        {
            RepositoryClosed();
            TabManager.AddMainTab();
            if (!func())
                TabManager.CloseAll();
            else
                Graph.GetInstance().ResetTranslate();
        }

        public void OpenSelectedRepository(string path)
        {
            OpenRepository(() => RepositoryManager.OpenExisting(path));
        }

        public void OpenRecentRepository(string path)
        {
            OpenRepository(() => RepositoryManager.OpenRecent(path));
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

        public void CloneRepository()
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Select directory for new repository";
            dialog.UseDescriptionForTitle = true;
            var ans = dialog.ShowDialog();
            if (ans == null || ans == false)
                return;
            CloneRepository(dialog.SelectedPath);
        }

        public void CloneRepository(string path)
        {
            RepositoryClosed();
            TabManager.AddMainTab();
            var v = LibGitService.GetInstance().IsValidRepository(path);
            if (v != RepositoryValidation.Invalid)
            {
                TabManager.CloseAll();
                MessageBox.Show("There is already a repository", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var dialog = new InputDialog() { Text = "Enter clone url", Validator = text => true, Owner = Application.Current.MainWindow };
            var ans = dialog.ShowDialog();
            if (ans == null || ans == false)
                return;
            v = RepositoryManager.Clone(path, dialog.ResponseText);
            if (v != RepositoryValidation.Valid)
                TabManager.CloseAll();
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
            OpenSelectedRepository(dialog.SelectedPath);
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

        void CreateManagers(ActionPanelModel localAM, ActionPanelModel remoteLeftAM, ActionPanelModel remoteRightAM)
        {
            ActionsManager = new ActionsManager();
            ActionsManager.LocalRepoPanel = localAM;
            ActionsManager.RemoteRepoLeftGroupPanel = remoteLeftAM;
            ActionsManager.RemoteRepoRightGroupPanel = remoteRightAM;
            TabManager = new TabManager(Data.MainWindowModel, localAM);
            TabManager.CommitRequested += Commit;
            TabManager.AbortRequested += AbortMerge;
            TabManager.CanvasMouseDown += OnMouseDown;
            TabManager.CanvasMouseUp += OnMouseUp;
            SubscribeActionsManager();
            CommitManager = CommitManager.GetInstance();
            RepositoryManager = new RepositoryManager(_dataFolder);
            UserManager = new UserManager(_dataFolder);
            StashingManager = new StashingManager(_dataFolder);
            RemoteManager = new RemoteManager(_dataFolder);
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
            dialog.Validator = text => Logic.LibGitService.GetInstance().IsValidRefName(text);
            dialog.Text = "Enter name of new branch";
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
            CommitManager.Rebase(Aggregating, Aggregated);
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

        public void ShowWaitingDialog()
        {
            WaitingDialog = new WaitingDialog();
            WaitingDialog.Owner = Application.Current.MainWindow;
            WaitingDialog.ShowDialog();
        }

        public void EndWaitingDialog(WaitingDialogResult r)
        {
            WaitingDialog.Close();
            if (r == WaitingDialogResult.OutOfMemory)
                MessageBox.Show("The repository is too large or there is not enough space in device.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (r == WaitingDialogResult.TooMuchCommits)
                MessageBox.Show("The repository has too much commits.\nLimit is 10 000 commits.", "", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static Program GetInstance()
        {
            if (Instance == null)
                Instance = new Program();
            return Instance;
        }
    }
}
