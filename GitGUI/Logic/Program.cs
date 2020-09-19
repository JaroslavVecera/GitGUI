using System;
using System.Windows;
using System.Windows.Input;

namespace GitGUI.Logic
{
    class Program
    {
        ViewHistory ViewHistory { get; } = new ViewHistory();
        CommitTree CommitTree { get; } = CommitTree.GetInstance();
        IProgramState State { get; set; } = Normal.GetInstance();
        public CrossStateData Data { get; } = new CrossStateData();
        ActionPanel ActionPanel { get; } = new ActionPanel();
        Node Aggregating { get; set; }
        Node Aggregated { get; set; }
        public Node AggregationFocused { get; private set; }
        UserManager UserManager { get; } = new UserManager();
        static Program Instance { get; set; } = new Program();

        Program()
        {
            InitializeEventHandlers();
            SetReferencesForStates();
            InitializeUserManager();
        }

        void InitializeUserManager()
        {
            UserManager.AddedUser += AddedUser;
            UserManager.RemovedUser += RemovedUser;
        }

        void AddedUser(User u)
        {

        }

        void RemovedUser(User u)
        {

        }

        void SetReferencesForStates()
        {
            MovingCanvas.GetInstance().Program = this;
            MovingNode.GetInstance().Program = this;
            Normal.GetInstance().Program = this;
        }

        public void Test()
        {
            User i = new User() { Name = "Jarek Večeřa", Email = "jarekvecer@seznam.cz", ImagePath = @"C:\Users\Lenovo\source\repos\GitGUI\photo.jpg" };
            UserManager.Add(i);
            UserManager.Current = i;

            ActionButton commitButton = new ActionButton("Commit");
            commitButton.Clicked += Commit;
            ActionPanel.Actions.Add(commitButton);

            ActionButton checkoutButton = new ActionButton("Checkout");
            checkoutButton.Clicked += Checkout;
            ActionPanel.Actions.Add(checkoutButton);

            ActionButton b2 = new ActionButton("Branch");
            b2.Clicked += TestHand2;
            ActionPanel.Actions.Add(b2);

            ActionButton b3 = new ActionButton("ahoj");
            b3.Clicked += TestHand3;
            ActionPanel.Actions.Add(b3);
        }

        void Commit(object sender, RoutedEventArgs e)
        {
            Commit commit = new Commit("another text for commit node.")
            {
                Author = new LibGit2Sharp.Signature(UserManager.Current.Name, UserManager.Current.Email, DateTimeOffset.Now)
            };
            CommitTree.Commit(commit, UserManager.Current.ImagePath);
        }

        void Checkout(object sender, RoutedEventArgs e)
        {
            CommitTree.Checkout(Data.AttachedNode);
        }

        void TestHand2(object sender, RoutedEventArgs e)
        {
            CommitTree.Branch(new Branch("hooooooooooooooo"));
        }

        void TestHand3(object sender, RoutedEventArgs e)
        {
            CommitTree.Checkout(((CommitNode)CommitTree.Graph.Checkouted.RepresentedNode).Branches[0]);
        }

        public void ChangeState(IProgramState state)
        {
            State = state;
        }

        public void OnMouseUp(Node node, MouseButtonEventArgs e)
        {
            State.MouseUp(node, Data, e);
        }

        public void OnMouseDown(Node node, MouseButtonEventArgs e)
        {
            State.MouseDown(node, Data, e);
        }

        public void OnMouseMove(Node node, MouseEventArgs e)
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

        void HashCopyRequest(CommitNode commitNode)
        {

        }

        public void ShowNode(Node node)
        {
            CommitTree.Mark(node);
            if (node != null)
                ((MainWindow)Application.Current.MainWindow).ShowNodePanel(node);
            else
                ((MainWindow)Application.Current.MainWindow).HideNodePanel();
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
            CommitTree.EventHandlerBatch = batch;
        }

        public void Focus(Node n)
        {
        }

        public void AggregationFocus(Node n)
        {
            AggregationFocused = n;
        }

        public void Aggregate(Node aggregating, Node aggregated)
        {
            Aggregating = aggregating;
            Aggregated = aggregated;
            switch (((App)Application.Current).Settings.AggregatingBehaviour)
            {
                case AggregatingBehaviour.Choose:
                    OpenAggregatingContextMenu();
                    break;
                case AggregatingBehaviour.Merge:
                    Merge();
                    break;
                case AggregatingBehaviour.Rebase:
                    Rebase();
                    break;
            }
        }

        public void Merge()
        {
            CommitTree.Merge(Aggregating, Aggregated, new Commit("Merge commit, ha."));
        }

        public void Rebase()
        {
        }

        void OpenAggregatingContextMenu()
        {
            Application.Current.MainWindow.ContextMenu.IsOpen = true;
        }

        void OnMouseEnter(Node sender, MouseEventArgs e)
        {
            State.MouseEnter(sender, Data);
        }

        void OnMouseLeave(Node sender, MouseEventArgs e)
        {
            State.MouseLeave(sender, Data);
        }

        void UpdateMousePosition(MouseEventArgs e)
        {
            Data.MousePoint = e.GetPosition(((MainWindow)Application.Current.MainWindow).graphView);
        }

        public static Program GetInstance()
        {
            return Instance;
        }
    }
}
