using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GraphX;
using QuickGraph;

namespace GitGUI.Logic
{
    class Graph : BidirectionalGraph<CommitNode, GraphEdge>
    {
        public CommitNode Root { get; set; }
        public Node Marked { get; set; }
        public Node Checkouted { get; set; }
        static Graph Instance { get; set; } = new Graph();
        double ScaleFactor { get; set; } = 1;
        Point Center { get { return GraphViewCenter(); } }
        Dictionary<CommitNode, GraphX.Measure.Size> VertexSizes { get; } = new Dictionary<CommitNode, GraphX.Measure.Size>();

        public void Move(Vector move)
        {
            ((MainWindow)Application.Current.MainWindow).ZoomCanvas.Move(move);
        }

        public void Scale(int wheelDelta, Point mouse)
        {
            double scaleFactor = wheelDelta > 0 ? 1.25 : 0.8;
            AppSettings set = ((App)Application.Current).Settings;
            Point origin = set.UseMouseAsZoomOrigin ? mouse : Center;
            ((MainWindow)Application.Current.MainWindow).ZoomCanvas.Rescale(scaleFactor, origin);
        }

        public void Add(List<Node> representants, CommitNode commit)
        {
            representants.ForEach(node => Add(node, commit));
        }

        public void Add(Node representant, Node node)
        {
            if (node == null)
                throw new ArgumentNullException("node");
            if (representant == null)
                throw new ArgumentNullException("representant");
            CommitNode rep = representant.RepresentedNode;
            if (rep == null)
                throw new ArgumentException("Given node is not pointing to any commit.", "node");
            AddToCommitNode(rep, (dynamic)node); // Keyword "dynamic" causes performance loss.
        }

        public void Add(Node node)
        {
            if (Checkouted != null)
                Add(Checkouted, node);
            else if (node is BranchNode)
                throw new InvalidOperationException("Can not add BranchNode as graph Root.");
            else
            { 
                Root = (CommitNode)node;
                AddVertexWithSize(Root);
                HighlightAsCheckouted(node);
            }
        }

        public void Remove(BranchNode branch)
        {
            if (branch == null)
                throw new ArgumentNullException("branch");
            CommitNode l = branch.LinkedCommitNode;
            l.Branches.Remove(branch);
        }

        public void Remove(CommitNode commit)
        {
            if (commit == null)
                throw new ArgumentNullException("commit");
            throw new NotImplementedException();
        }

        public void RelinkBranchNode(CommitNode desiredOwner, BranchNode branch)
        {
            if (desiredOwner == null || branch == null)
                throw new ArgumentNullException();
            CommitNode owner = branch.LinkedCommitNode;
            owner.Branches.Remove(branch);
            VertexSizes[owner] = owner.RotatedVertexSize;
            desiredOwner.Branches.Add(branch);
            branch.LinkedCommitNode = desiredOwner;
            VertexSizes[desiredOwner] = desiredOwner.RotatedVertexSize;
        }

        public void HighlightAsMarked(Node node)
        {
            if (Marked != null)
                Marked.Marked = false;
            Marked = node;
            if (node != null)
                node.Marked = true;
        }

        public void HighlightAsCheckouted(Node node)
        {
            if (Checkouted != null)
                Checkouted.Checkouted = false;
            Checkouted = node;
            node.Checkouted = true;
        }

        void AddToCommitNode(CommitNode node, BranchNode branch)
        {
            if (branch == null)
                throw new ArgumentNullException("branch");
            if (branch.LinkedCommitNode != null)
                throw new InvalidOperationException("Branch is already assigned to a commit.");
            node.Branches.Add(branch);
            branch.LinkedCommitNode = node;
            VertexSizes[node] = node.RotatedVertexSize;
        }

        void AddToCommitNode(CommitNode node, CommitNode commit)
        {
            if (commit == null)
                throw new ArgumentNullException("commit");
            AddVertexWithSize(commit);
            AddEdge(new GraphEdge(node, commit));
        }

        void AddVertexWithSize(CommitNode commit)
        {
            AddVertex(commit);
            VertexSizes[commit] = commit.RotatedVertexSize;
        }
            

        Point GraphViewCenter()
        {
            ScrollViewer g = ((MainWindow)Application.Current.MainWindow).GraphView;
            return new Point((double)g.ActualWidth / 2, (double)g.ActualHeight / 2);
        }

        public void DeployGraph()
        {
            DeployCommitNodes();
            DeployBranchNodes();
        }

        void DeployBranchNodes()
        {
            Vertices.ToList().ForEach(v => v.Branches.ForEach(b => b.Location = new Point(v.Location.X, v.Location.Y + (1 + v.Branches.IndexOf(b)) * b.GElement.Height + BranchNode.ArrowHeight)));
        }

        void DeployCommitNodes()
        { 
            GXLogicCoreExample core = new GXLogicCoreExample();
            core.Graph = this;
            var sug = GraphX.Common.Enums.LayoutAlgorithmTypeEnum.Sugiyama;
            var fsa = GraphX.Common.Enums.OverlapRemovalAlgorithmTypeEnum.FSA;
            var layout = core.AlgorithmFactory.CreateLayoutAlgorithm(sug, this, null, VertexSizes);
            var overlap = core.AlgorithmFactory.CreateOverlapRemovalAlgorithm(fsa);
            core.GenerateAlgorithmStorage(VertexSizes, new Dictionary<CommitNode, GraphX.Measure.Point>());
            core.CreateNewAlgorithmStorage(layout, overlap, null);
            var d = core.Compute(new System.Threading.CancellationToken());
            Vertices.ToList().ForEach(v => v.Location = new Point(d[v].Y, d[v].X));
        }

        public static Graph GetInstance()
        {
            return Instance;
        }
    }

    class GXLogicCoreExample : GraphX.Logic.Models.GXLogicCore<CommitNode, GraphEdge, Graph>
    {
    }
}
