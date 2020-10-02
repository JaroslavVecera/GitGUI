using System;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using LibGit2Sharp;

namespace GitGUI.Logic
{
    class CommitManager
    {
        public EventHandlerBatch EventHandlerBatch
        {
            set { Graph.EventHandlerBatch = value; }
        }
        public Graph Graph { get; } = Graph.GetInstance();
        LibGitService LibGitService { get; set; } = new LibGitService();
        static CommitManager Instance { get; set; } = new CommitManager();

        private CommitManager() { }

        public void Add(List<string> files)
        {
            LibGitService.Add(files);
        }

        public void Scale(int wheelDelta, Point mouse)
        {
            Graph.Scale(wheelDelta, mouse);
        }

        public void Move(Vector move)
        {
            Graph.Move(move);
        }

        public void OpenRepository(string path)
        {
            Graph.Repository = LibGitService.OpenRepository(path);
        }

        public void CreateRepository(string path)
        {
            Graph.Repository = LibGitService.OpenNewRepository(path);
            LibGitService.Branch(null, "ahoj");
            LibGitService.Branch(null, "master");
        }

        public void Commit(BranchLabelModel l, string message)
        {
            LibGitService.Commit(l, message);
            Graph.DeployGraph();
        }

        public void Merge(BranchLabelModel merging, BranchLabelModel merged)
        {
            LibGitService.Merge(merging, merged);
            Graph.DeployGraph();
        }

        public void Checkout(BranchLabelModel b)
        {
            LibGitService.Checkout(b);
            Graph.HighlightAsCheckouted(b);
        }

        public void Mark(GraphItemModel i)
        {
            Graph.HighlightAsMarked(i);
        }

        public void Branch(GraphItemModel i, string name)
        {
            LibGitService.Branch(i, name);
            Graph.DeployGraph();
        }

        public static CommitManager GetInstance()
        {
            return Instance;
        }
    }
}
