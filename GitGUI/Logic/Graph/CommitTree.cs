using System;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace GitGUI.Logic
{
    class CommitTree
    {
        public EventHandlerBatch EventHandlerBatch { get; set; }
        public Graph Graph { get; } = Graph.GetInstance();
        static CommitTree Instance { get; set; } = new CommitTree();

        CommitTree() { }

        public void Commit(Commit commit, string imagePath)
        {
            CommitNode n = NewCommitNode(commit);
            n.Path = imagePath;
            Graph.Add(n);
            if (Graph.Checkouted is BranchNode)
                Graph.RelinkBranchNode(n, (BranchNode)Graph.Checkouted);
            Graph.DeployGraph();
        }

        public void Merge(Node merging, Node merged, Commit commit)
        {
            CommitNode c = NewCommitNode(commit);
            List<Node> l = new List<Node>() { merging, merged };
            Graph.Add(l, c);
            if (merging is BranchNode)
                Graph.RelinkBranchNode(c, (BranchNode)merging);
            Graph.DeployGraph();
        }

        public void Checkout(Node n)
        {
            Graph.HighlightAsCheckouted(n);
        }

        public void Mark(Node n)
        {
            Graph.HighlightAsMarked(n);
        }

        public void Branch(Node n, Branch b)
        {
            BranchNode bn = NewBranchNode(b);
            Graph.Add(n, bn);
            Graph.DeployGraph();
        }

        public void Branch(Branch b)
        {
            Branch(Graph.Checkouted, b);
        }

        CommitNode NewCommitNode(Commit commit)
        {
            CommitNode n = new CommitNode(commit);
            SetEvents(n);
            return n;
        }

        BranchNode NewBranchNode(Branch branch)
        {
            BranchNode n = new BranchNode(branch);
            SetEvents(n);
            return n;
        }
        void SetEvents(Node n)
        {
            n.MouseDown += EventHandlerBatch.MouseDownEventHandler;
            n.MouseEnter += EventHandlerBatch.MouseEnterEventHandler;
            n.MouseLeave += EventHandlerBatch.MouseLeaveEventHandler;
        }

        public static CommitTree GetInstance()
        {
            return Instance;
        }
    }
}
