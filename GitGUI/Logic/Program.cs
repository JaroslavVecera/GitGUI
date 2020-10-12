using System;
using System.Windows;
using System.Windows.Input;

namespace GitGUI.Logic
{
    class Program
    {
        CommitManager CommitManager { get; } = CommitManager.GetInstance();
        RepositoryManager RepositoryManager { get; } = new RepositoryManager();
        IProgramState State { get; set; }
        public CrossStateData Data { get; } = new CrossStateData();
        MainWindowModel MainWindowModel { get { return Data.MainWindowModel; } set { Data.MainWindowModel = value; } }
        static Program Instance { get; set; }
        public BranchLabelModel AggregationFocused { get; set; }

        Program()
        {
            InitializeEventHandlers();
            InitializeMainWindow();
            Test();
        }

        void Test()
        {
            RepositoryManager.OpenExisting(new RepositoryModel(@"D:\škola\GitGUITests"));
        }

        public void InitializeMainWindow()
        {
            MainWindow view = new MainWindow();
            MainWindowModel model = new MainWindowModel();
            MainWindowViewModel viewModel = new MainWindowViewModel();

            Application.Current.MainWindow = view;

            viewModel.Model = model;
            view.DataContext = view;
            MainWindowModel = model;
        }

        public void ChangeState(IProgramState state)
        {
            State = state;
        }

        public void OnMouseUp(object item, MouseButtonEventArgs e)
        {
            State.MouseUp(item, Data, e);
        }

        public void OnMouseDown(object item, MouseButtonEventArgs e)
        {
            State.MouseDown(item, Data, e);
        }

        public void OnMouseMove(object item, MouseEventArgs e)
        {
            UpdateMousePosition(e);
            if (Data.MouseDisplacement == new Vector(0, 0))
                return;
            State.MouseMove(Data, e);
        }

        public void OnMouseWheel(MouseWheelEventArgs e)
        {
            Graph.GetInstance().Scale(e.Delta, Data.MousePoint);
            State.MouseWheelMove(Data, e.Delta);
        }

        void HashCopyRequest(CommitNodeModel commitNode)
        {

        }

        public void Show(GraphItemModel item)
        {
            CommitManager.Mark(item);
        }

        public void Focus(GraphItemModel item)
        {

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
                MouseEnterEventHandler = OnMouseEnter,
                MouseLeaveEventHandler = OnMouseLeave
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

        void OnMouseLeave(object sender, MouseEventArgs e)
        {
            State.MouseLeave(sender, Data);
        }

        void UpdateMousePosition(MouseEventArgs e)
        {
            Data.MousePoint = e.GetPosition(((MainWindow)Application.Current.MainWindow).graphView);
        }

        public static Program GetInstance()
        {
            if (Instance == null)
                Instance = new Program();
            return Instance;
        }
    }
}
