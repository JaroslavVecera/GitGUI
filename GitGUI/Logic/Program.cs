using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Ookii.Dialogs.Wpf;

namespace GitGUI.Logic
{
    class Program
    {
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

        Program()
        {
            ActionPanelModel localAM = new ActionPanelModel();

            InitializeMainWindow();
            CreateManagers(localAM);
            InitializeEventHandlers();
            InitializeState();
        }

        void InitializeState()
        {
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
            RepositoryManager.OpenExisting(dialog.SelectedPath);
        }

        void CreateManagers(ActionPanelModel localAM)
        {
            ActionsManager = new ActionsManager();
            ActionsManager.LocalRepoPanel = localAM;
            TabManager = new TabManager(Data.MainWindowModel, localAM);
            TabManager.CommitRequested += Commit;
            TabManager.CanvasMouseDown += OnMouseDown;
            TabManager.CanvasMouseUp += OnMouseUp;
            SubscribeActionsManager();
            CommitManager = CommitManager.GetInstance();
            RepositoryManager = new RepositoryManager();
            RepositoryManager.Closed += RepositoryClosed;
            UserManager = new UserManager();
        }

        public void ChangeUser(User u)
        {
            UserManager.Current = u;
        }

        void RepositoryClosed(RepositoryModel m)
        {
            TabManager.CloseAll();
        }

        private void Commit(string message, IEnumerable<string> paths)
        {
            CommitManager.Commit(null, message, paths);
            TabManager.CloseCommitEditorTab();
        }

        void SubscribeActionsManager()
        {
            ActionsManager.Commit += EditCommit;
        }

        void EditCommit()
        {
            TabManager.NewCommitEditor();
        }

        void Test()
        {
            RepositoryManager.OpenExisting(@"D:\škola\GitGUITests");
        }

        public void InitializeMainWindow()
        {
            MainWindow view = (MainWindow)Application.Current.MainWindow;
            MainWindowModel model = new MainWindowModel();
            MainWindowViewModel viewModel = new MainWindowViewModel(model, view);

            MainWindowModel = model;
            view.Show();
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
        }

        public void Aggregate(BranchLabelModel aggregating, BranchLabelModel aggregated)
        {

        }

        public void AggregationFocus(BranchLabelModel l)
        {

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
        }

        public void Rebase()
        {
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
