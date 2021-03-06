﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using LibGit2Sharp;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GitGUI.Logic
{
    class Graph
    {
        public GraphItemModel Marked { get; set; }
        public GraphItemModel Focused { get; set; }
        public GraphItemModel Checkouted { get; set; }
        public BranchLabelModel AggregationFocused { get; set; }
        static Graph Instance { get; set; } = new Graph();
        double Zoom { get; set; } = 1;
        Point Center { get { return GraphViewCenter(); } }
        public Size Size { get; set; }
        public Point Position { get; set; }
        public ZoomAndPanCanvasModel ZoomAndPanCanvasModel { get; } = new ZoomAndPanCanvasModel();
        public EventHandlerBatch EventHandlerBatch { private get; set; }
        MatrixTransform NodeTransform { get; } = new MatrixTransform();
        Stopwatch Stopwatch { get; set; }
        WaitingDialogResult WaitingResult { get; set; } = WaitingDialogResult.Flawless;

        public void CaptureMaouse()
        {
            ZoomAndPanCanvasModel.CaptureMouse();
        }

        public void ReleaseMouseCapture()
        {
            ZoomAndPanCanvasModel.ReleaseMouseCapture();
        }

        private Graph()
        {
            LibGitService.GetInstance().RepositoryChangedPreview += () => DeployGraph();
        }

        public bool Contains(Point p)
        {
            return Position.X <= p.X && Position.X + Size.Width >= p.X && Position.Y <= p.Y && Position.Y + Size.Height >= p.Y;
        }

        public Vector Move(Vector move)
        {
            Point p = GraphViewCenter();
            return ZoomAndPanCanvasModel.Move(move, new Size(p.X * 2, p.Y * 2));
        }

        Vector AggresiveMove(Vector move)
        {
            Point p = GraphViewCenter();
            return ZoomAndPanCanvasModel.AggresiveMove(move, new Size(p.X * 2, p.Y * 2));
        }

        public void ResetTranslate()
        {
            ZoomAndPanCanvasModel.Rescale(1 / Zoom, new Point(0, 0));
            Zoom = 1;
            Move(new Vector(double.MaxValue, double.MaxValue));
        }

        public void CheckBoundaries()
        {
            Move(new Vector(0, 0));
        }

        public void Scale(int wheelDelta, Point mouse)
        {
            Point p = GraphViewCenter();
            double desiredZoom = Zoom * (wheelDelta > 0 ? 1.25 : 0.8);
            double boundedZoom = Math.Max(0.5, Math.Min(3, desiredZoom));
            double boundedScale = boundedZoom / Zoom;
            Zoom = boundedZoom;
            AppSettings set = ((App)Application.Current).Settings;
            Point origin = set.UseMouseAsZoomOrigin ? mouse : Center;
            ZoomAndPanCanvasModel.Rescale(boundedScale, origin);
            ZoomAndPanCanvasModel.Move(new Vector(0, 0), new Size(p.X * 2, p.Y * 2));
        }

        public void HighlightAsMarked(GraphItemModel model)
        {
            if (Marked != null)
                Marked.Marked = false;
            Marked = model;
            if (model != null)
                model.Marked = true;
        }

        public void HighlightAsAggregationFocused(BranchLabelModel model)
        {
            if (AggregationFocused != null)
                AggregationFocused.AggregationFocused = false;
            AggregationFocused = model;
            if (model != null)
                model.AggregationFocused = true;
        }

        public void HighlightAsFocused(GraphItemModel model)
        {
            if (Focused != null)
                Focused.Focused = false;
            Focused = model;
            if (model != null)
                model.Focused = true;
        }

        public void HighlightAsCheckouted(GraphItemModel m)
        {
            if (Checkouted != null)
                Checkouted.Checkouted = false;
            Checkouted = m;
            m.Checkouted = true;
        }

        Point GraphViewCenter()
        {
            return Program.GetInstance().TabManager.GraphViewCenter;
        }

        public void DeployGraph()
        {
            WaitingResult = WaitingDialogResult.Flawless;
            ZoomAndPanCanvasModel.Clear();
            ((Action)DoDeployGraph).BeginInvoke(OnDeployedGraph, null);
            Program.GetInstance().ShowWaitingDialog();
        }

        public void AimHead()
        {
            GraphItemModel i = ZoomAndPanCanvasModel.Branches.Find(b => b.IsHead);
            if (i == null)
                i = ZoomAndPanCanvasModel.Commits.Find(c => c.Checkouted);
            Aim(i);
        }

        public Tuple<List<CommitNodeModel>, List<BranchLabelModel>, List<CommitNodeModel>> Find(string text)
        {
            var commitsByMessage = ZoomAndPanCanvasModel.Commits?.Where(c => c.Message.StartsWith(text)).Take(3).ToList();
            var branches = ZoomAndPanCanvasModel.Branches?.Where(b => b.Name.StartsWith(text)).Take(3).ToList();
            var commitsBySha = ZoomAndPanCanvasModel.Commits?.Where(c => c.Sha.StartsWith(text)).Take(3).ToList();
            return new Tuple<List<CommitNodeModel>, List<BranchLabelModel>, List<CommitNodeModel>>(commitsByMessage, branches, commitsBySha);
        }

        public void Aim(GraphItemModel i)
        {
            ResetTranslate();
            AggresiveMove(new Vector(-i.Location.X, -i.Location.Y));
            Program.GetInstance().Show(i);
        }

        void OnDeployedGraph(IAsyncResult ar)
        {
            Application.Current.Dispatcher.Invoke(() => UpdateGraph(ar));
        }

        void UpdateGraph(IAsyncResult ar)
        { 
            if (WaitingResult == WaitingDialogResult.Flawless)
            {
                Point p = GraphViewCenter();
                ZoomAndPanCanvasModel.Update(new Size(p.X * 2, p.Y * 2));
            }
            else
                Program.GetInstance().CloseCurrentRepository();
            Program.GetInstance().EndWaitingDialog(WaitingResult);
        }

        void DoDeployGraph()
        {
            WaitingResult = WaitingDialogResult.Flawless;
            try
            {
                DeployCommitNodes();
                DeployBranchNodes();
                UpdateCheckouted();
                ZoomAndPanCanvasModel.CreateEdgePairs();
            }
            catch (OutOfMemoryException e)
            {
                WaitingResult = WaitingDialogResult.OutOfMemory;
            }
            catch (TooMuchCommitsException e)
            {
                WaitingResult = WaitingDialogResult.TooMuchCommits;
            }
        }

        void UpdateCheckouted()
        {
            Branch head = LibGitService.GetInstance().Head;
            BranchLabelModel checkoutedBranch = ZoomAndPanCanvasModel.Branches.Find(b => b.Branch.CanonicalName == head.CanonicalName);
            if (checkoutedBranch != null)
                HighlightAsCheckouted(checkoutedBranch);
            else
            {
                CommitNodeModel checkoutedCommit = ZoomAndPanCanvasModel.Commits.Find(c => c.Sha == head.Reference.TargetIdentifier);
                if (checkoutedCommit != null)
                    HighlightAsCheckouted(checkoutedCommit);
            }
        }

        void DeployBranchNodes()
        {
            List<BranchLabelModel> branchModels = new List<BranchLabelModel>();
            ZoomAndPanCanvasModel.Branches?.ToList().ForEach(b => UnsubscribeEvents(b));
            Dictionary<Commit, CommitNodeModel> pairs = ZoomAndPanCanvasModel.Commits.ToDictionary(x => x.Commit);
            List<IGrouping<Commit, Branch>> branchGroups = LibGitService.GetInstance().Branches.GroupBy(b => b.Tip).ToList();
            foreach (var branchGroup in branchGroups)
            {
                int y = 1;
                List<BranchLabelModel> mg = branchGroup.ToList().Select(b =>
                {
                    CommitNodeModel tipNode = pairs[b.Tip];
                    tipNode.PlusButton = false;
                    Point cl = tipNode.Location;
                    BranchLabelModel m = new BranchLabelModel() { Location = new Point(cl.X, cl.Y - 10 - 32 * (y++)), Branch = b };
                    branchModels.Add(m);
                    return m;
                }).ToList();
                mg.First().Arrow = true;
                mg.Last().PlusButton = true;
            }
            branchModels.ForEach(b => SubscribeEvents(b));
            ZoomAndPanCanvasModel.Branches = branchModels;
        }

        void  DeployCommitNodes()
        {
            var inProgress = Program.GetInstance().StashingManager.ImplicitStashBases;
            var commitRows = LibGitService.GetInstance().CommitRows();
            ZoomAndPanCanvasModel.Commits?.ToList().ForEach(c => UnsubscribeCommitEvents(c));
            List<CommitNodeModel> commitModels = new List<CommitNodeModel>();
            int x = 0;
            List<CommitNodeModel> commits = commitRows.Select(pair =>
            {
                Commit c = pair.Item1;
                BitmapImage picture = null;
                if (c.Author.Name != "" && c.Author.Email != "")
                {
                    Identity i = new Identity(c.Author.Name, c.Author.Email);
                    picture = Program.GetInstance().UserManager.FindUserPictureByIdentity(i);
                }
                CommitNodeModel m = new CommitNodeModel(c, picture) { Location = new Point(x, pair.Item2 * 70) };
                x += (int)m.MaxWidth + 150;
                if (inProgress != null && inProgress.Contains(c))
                    m.InProgress = true;
                return m;
            }).ToList();
            commits.ForEach(c => commitModels.Add(c));
            commitModels.ForEach(c => SubscribeCommitEvents(c));
            ZoomAndPanCanvasModel.Commits = commitModels;
        }

        void Old()
            { 
            Hashtable branchCommits = LibGitService.GetInstance().BranchCommits();
            int y = 0;
            ZoomAndPanCanvasModel.Commits?.ToList().ForEach(c => UnsubscribeCommitEvents(c));
            List<CommitNodeModel> models = new List<CommitNodeModel>();
            foreach (Branch b in branchCommits.Keys)
            {
                int x = 0;
                List<Commit> l = (List<Commit>)branchCommits[b];
                List<CommitNodeModel> commits = l?.Select(c =>
                {
                    Identity i = new Identity(c.Author.Name, c.Author.Email);
                    BitmapImage picture = Program.GetInstance().UserManager.FindUserPictureByIdentity(i);
                    return new CommitNodeModel(c, picture);
                }).ToList();
                commits.Reverse();
                commits.ForEach(c => c.Location = new System.Windows.Point((x++) * 250, y));
                commits.ForEach(c => models.Add(c));
                y += 50;
            }
            models.ForEach(c => SubscribeCommitEvents(c));
            ZoomAndPanCanvasModel.Commits = models;
        }

        void UnsubscribeEvents(GraphItemModel m)
        {
            m.MouseDown -= EventHandlerBatch.MouseDownEventHandler;
            m.MouseUp -= EventHandlerBatch.MouseUpEventHandler;
            m.MouseEnter -= EventHandlerBatch.MouseEnterEventHandler;
            m.MouseLeave -= EventHandlerBatch.MouseLeaveEventHandler;
            m.AddBranch -= EventHandlerBatch.AddBranchEventHandler;
            if (m is CommitNodeModel)
            {
                ((CommitNodeModel)m).ShowChanges -= EventHandlerBatch.ShowChangesEventHandler;
            }
        }

        void SubscribeEvents(GraphItemModel m)
        {
            m.MouseDown += EventHandlerBatch.MouseDownEventHandler;
            m.MouseUp += EventHandlerBatch.MouseUpEventHandler;
            m.MouseEnter += EventHandlerBatch.MouseEnterEventHandler;
            m.MouseLeave += EventHandlerBatch.MouseLeaveEventHandler;
            m.AddBranch += EventHandlerBatch.AddBranchEventHandler;
            if (m is CommitNodeModel)
            {
                ((CommitNodeModel)m).ShowChanges += EventHandlerBatch.ShowChangesEventHandler;
            }
        }

        void UnsubscribeCommitEvents(CommitNodeModel m)
        {
            m.CopyShaRequested -= EventHandlerBatch.CopyHash;
            UnsubscribeEvents(m);
        }

        void SubscribeCommitEvents(CommitNodeModel m)
        {
            m.CopyShaRequested += EventHandlerBatch.CopyHash;
            SubscribeEvents(m);
        }

        public void MoveBranch(Vector displacement)
        {
            Matrix mat = ZoomAndPanCanvasModel.TransformMatrix;
            mat.Invert();
            Vector transformedDispl = mat.Transform(displacement);
            Matrix m = NodeTransform.Matrix;
            m.Translate(transformedDispl.X, transformedDispl.Y);
            NodeTransform.Matrix = m;
        }

        public void SetupMovingBranchLabel(BranchLabelModel m)
        {
            m.RenderTransform = NodeTransform;
            m.ForegroundPull();
            m.IsHitTestVisible = false;
            CompositionTarget.Rendering += MoveCanvasToMouse;
            Stopwatch = Stopwatch.StartNew();
        }

        public void RestoreBranchLabel(BranchLabelModel m)
        {
            m.RenderTransform = null;
            m.IsHitTestVisible = true;
            m.BackgroundPush();
            Stopwatch.Stop();
            NodeTransform.Matrix = Matrix.Identity;
            CompositionTarget.Rendering -= MoveCanvasToMouse;
        }

        public void BranchLabelToMouse(BranchLabelModel branch, Point mouse)
        {
            Vector mouseRelativeToGraph = new Vector(mouse.X, mouse.Y) - new Vector(Position.X, Position.Y);
            Matrix mat = ZoomAndPanCanvasModel.TransformMatrix;
            Matrix inv = mat;
            inv.Invert();
            Point loc = branch.Location;
            Point tloc = mat.Transform(NodeTransform.Transform(loc));
            Point diff = new Point(mouseRelativeToGraph.X - tloc.X, mouseRelativeToGraph.Y - tloc.Y);
            ScaleTransform scale = new ScaleTransform(inv.M11, inv.M22);
            diff = scale.Transform(diff);
            Matrix m = NodeTransform.Matrix;
            m.Translate(diff.X, diff.Y);
            NodeTransform.Matrix = m;
        }

        void MoveCanvasToMouse(object sender, EventArgs e)
        {
            Vector translate = Translate();
            double milliseconds = Stopwatch.ElapsedMilliseconds;
            Stopwatch.Restart();
            if (translate == new Vector(0, 0))
                return;
            translate *= milliseconds / 10;
            Matrix inv = ZoomAndPanCanvasModel.TransformMatrix;
            inv.Invert();
            var trimmedTranslate = Move(translate);
            Matrix m = NodeTransform.Matrix;
            m.Translate(-trimmedTranslate.X * inv.M11, -trimmedTranslate.Y * inv.M22);
            NodeTransform.Matrix = m;
        }

        Vector Translate()
        {
            Vector mouseRelativeToGraph = (Vector)Program.GetInstance().Data.MousePoint - new Vector(Position.X, Position.Y);
            if (mouseRelativeToGraph.X > Size.Width / 10 && mouseRelativeToGraph.X < Size.Width * 9 / 10 && mouseRelativeToGraph.Y > Size.Height / 10 && mouseRelativeToGraph.Y < Size.Height * 9 / 10)
                return new Vector(0, 0);
            mouseRelativeToGraph -= new Vector(Size.Width / 2, Size.Height / 2);
            return -mouseRelativeToGraph / mouseRelativeToGraph.Length;
        }

        public static Graph GetInstance()
        {
            return Instance;
        }
    }
}
