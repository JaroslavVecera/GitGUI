using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using LibGit2Sharp;

namespace GitGUI.Logic
{
    class Graph
    {
        public GraphItemModel Marked { get; set; }
        public GraphItemModel Focused { get; set; }
        public BranchLabelModel Checkouted { get; set; }
        static Graph Instance { get; set; } = new Graph();
        double Zoom { get; set; } = 1;
        Point Center { get { return GraphViewCenter(); } }
        public ZoomAndPanCanvasModel ZoomAndPanCanvasModel { get; } = new ZoomAndPanCanvasModel();
        public Repository Repository { private get; set; }
        public EventHandlerBatch EventHandlerBatch { private get; set; }
        MatrixTransform NodeTransform { get; } = new MatrixTransform();
        Stopwatch Stopwatch { get; set; }

        private Graph()
        {
            DeployGraph();
            LibGitService.GetInstance().RepositoryChanged += () => DeployGraph();
        }

        public void Move(Vector move)
        {
            ZoomAndPanCanvasModel.Move(move);
        }

        public void Scale(int wheelDelta, Point mouse)
        {
            double desiredZoom = Zoom * (wheelDelta > 0 ? 1.25 : 0.8);
            double boundedZoom = Math.Max(0.2, Math.Min(3, desiredZoom));
            double boundedScale = boundedZoom / Zoom;
            Zoom = boundedZoom;
            AppSettings set = ((App)Application.Current).Settings;
            Point origin = set.UseMouseAsZoomOrigin ? mouse : Center;
            ZoomAndPanCanvasModel.Rescale(boundedScale, origin);
        }

        public void HighlightAsMarked(GraphItemModel model)
        {
            if (Marked != null)
                Marked.Marked = false;
            Marked = model;
            if (model != null)
                model.Marked = true;
        }

        public void HighlightAsFocused(GraphItemModel model)
        {
            if (Focused != null)
                Focused.Focused = false;
            Focused = model;
            if (model != null)
                model.Focused = true;
        }

        public void HighlightAsCheckouted(BranchLabelModel branch)
        {
            if (Checkouted != null)
                Checkouted.Checkouted = false;
            Checkouted = branch;
            branch.Checkouted = true;
        }

        Point GraphViewCenter()
        {
            return Program.GetInstance().TabManager.GraphViewCenter;
        }

        public void DeployGraph()
        {
            DeployCommitNodes();
        }

        void DeployCommitNodes()
        {
            int i = 0;
            IQueryableCommitLog l = LibGitService.GetInstance().Commits;
            ZoomAndPanCanvasModel.Commits?.ToList().ForEach(c => UnsubscribeCommitEvents(c));
            ZoomAndPanCanvasModel.Commits = l?.Select(c => new CommitNodeModel(c)).ToList();
            ZoomAndPanCanvasModel.Commits?.ForEach(c =>
            {
                SubscribeCommitEvents(c);
                c.Location = new System.Windows.Point((i++) * 250, 0);
            });
        }

        void UnsubscribeEvents(GraphItemModel m)
        {
            m.MouseDown -= EventHandlerBatch.MouseDownEventHandler;
            m.MouseUp -= EventHandlerBatch.MouseUpEventHandler;
            m.MouseEnter -= EventHandlerBatch.MouseEnterEventHandler;
            m.MouseLeave -= EventHandlerBatch.MouseLeaveEventHandler;
        }

        void SubscribeEvents(GraphItemModel m)
        {
            m.MouseDown += EventHandlerBatch.MouseDownEventHandler;
            m.MouseUp += EventHandlerBatch.MouseUpEventHandler;
            m.MouseEnter += EventHandlerBatch.MouseEnterEventHandler;
            m.MouseLeave += EventHandlerBatch.MouseLeaveEventHandler;
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
            Matrix mat = ZoomAndPanCanvasModel.TransformMatrix;
            Matrix inv = mat;
            inv.Invert();
            Point loc = branch.Location;
            Point tloc = mat.Transform(NodeTransform.Transform(loc));
            Point diff = new Point(mouse.X - tloc.X, mouse.Y - tloc.Y);
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
            Graph.GetInstance().Move((Vector)translate);
            Matrix m = NodeTransform.Matrix;
            m.Translate(-translate.X * inv.M11, -translate.Y * inv.M22);
            NodeTransform.Matrix = m;
        }

        Vector Translate()
        {
            Vector mouse = (Vector)Program.GetInstance().Data.MousePoint;
            if (mouse.X > 0 && mouse.X < ZoomAndPanCanvasModel.Width && mouse.Y > 0 && mouse.Y < ZoomAndPanCanvasModel.Height)
                return new Vector(0, 0);
            mouse -= new Vector(ZoomAndPanCanvasModel.Width / 2, ZoomAndPanCanvasModel.Height / 2);
            return -mouse / mouse.Length;
        }

        public static Graph GetInstance()
        {
            return Instance;
        }
    }
}
