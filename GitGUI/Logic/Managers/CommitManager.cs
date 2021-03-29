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
        LibGitService LibGitService { get; } = LibGitService.GetInstance();
        ViewHistory ViewHistory { get; } = new ViewHistory();
        static CommitManager Instance { get; set; } = new CommitManager();

        private CommitManager() { }

        public void SetupMovingBranchLabel(BranchLabelModel m)
        {
            Graph.SetupMovingBranchLabel(m);
        }

        public void MoveBranch(Vector displacement)
        {
            Graph.MoveBranch(displacement);
        }

        public void BranchLabelToMouse(BranchLabelModel branch, Point mouse)
        {
            Graph.BranchLabelToMouse(branch, mouse);
        }

        public void RestoreBranchLabel(BranchLabelModel m)
        {
            m.Focused = false;
            Graph.RestoreBranchLabel(m);
        }

        public void Move(Vector move)
        {
            Graph.Move(move);
        }

        public void Commit(BranchLabelModel l, string message, IEnumerable<string> stagedFiles, IEnumerable<string> unstagedFiles)
        {
            LibGitService.GetInstance().Add(stagedFiles, unstagedFiles);
            LibGitService.Commit(l, message);
        }

        public void Merge(BranchLabelModel merging, BranchLabelModel merged)
        {
            GraphItemModel checkouted = Graph.Checkouted;
            Checkout(merging);
            bool succes = LibGitService.Merge(merged);
            if (succes)
                Checkout(checkouted);
        }

        public void Checkout(GraphItemModel m)
        {
            Program.GetInstance().StashingManager.ImplicitPush(m);
            LibGitService.Checkout(m);
            Graph.HighlightAsCheckouted(m);
        }

        public void Mark(GraphItemModel i)
        {
            Graph.HighlightAsMarked(i);
        }

        public void AggregationFocus(BranchLabelModel m)
        {
            Graph.HighlightAsAggregationFocused(m);
        }

        public void Branch(GraphItemModel i, string name)
        {
            LibGitService.Branch(i, name);
        }

        public static CommitManager GetInstance()
        {
            return Instance;
        }
    }
}
