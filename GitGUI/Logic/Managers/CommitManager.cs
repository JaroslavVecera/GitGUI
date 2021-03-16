﻿using System;
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

        public void Scale(int wheelDelta, Point mouse)
        {
            Graph.Scale(wheelDelta, mouse);
        }

        public void Move(Vector move)
        {
            Graph.Move(move);
        }

        public void Commit(BranchLabelModel l, string message, IEnumerable<string> stagedFiles, IEnumerable<string> unstagedFiles)
        {
            LibGitService.GetInstance().Add(stagedFiles, unstagedFiles);
            LibGitService.Commit(l, message);
            Graph.DeployGraph();
        }

        public void Merge(BranchLabelModel merging, BranchLabelModel merged)
        {
            BranchLabelModel currentBranch = Graph.Checkouted;
            Checkout(merging);
            bool succes = LibGitService.Merge(merged);
            if (succes)
                Checkout(currentBranch);
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
