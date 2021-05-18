using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Input;

namespace GitGUI.Logic
{
    public class ZoomAndPanCanvasViewModel : ViewModelBase
    {
        public MatrixTransform CanvasTransform { get; private set; }
        public ZoomAndPanCanvasView View { get; set; }
        ZoomAndPanCanvasModel Model { get; set; }
        List<CommitNodeViewModel> Commits { get; } = new List<CommitNodeViewModel>();
        List<BranchLabelViewModel> Branches { get; } = new List<BranchLabelViewModel>();
        List<Edge> Edges { get; } = new List<Edge>();

        public ZoomAndPanCanvasViewModel(ZoomAndPanCanvasModel model, ZoomAndPanCanvasView view)
        {
            SetModel(model);
            View = view;
            View.DataContext = this;
        }

        void SetModel(ZoomAndPanCanvasModel model)
        {
            Model = model;
            Model.TransformMatrixChanged += TransformMatrixChanged;
            Model.ContentUpdated += UpdateContent;
            Model.ContentCleared += CleareContent;
            CanvasTransform = new MatrixTransform(Model.TransformMatrix);
            Model.Released += () => View.ReleaseMouseCapture();
            Model.Captured += () => Mouse.Capture(View, CaptureMode.SubTree);
        }

        void CleareContent()
        {
            Commits.Clear();
            Branches.Clear();
            Edges.Clear();
            View.Children.Clear();
        }

        void UpdateContent(Tuple<double, double, double, double> screenBoundaries)
        {
            if (Model.Commits.Count == 0)
                return;
            List<CommitNodeViewModel> badCommits = Commits.Where(commit => commit.Location.X > screenBoundaries.Item3 || commit.Location.X + 251 < screenBoundaries.Item1).ToList();
            List<BranchLabelViewModel> badBranches = Branches.Where(branch => branch.Location.X > screenBoundaries.Item3 || branch.Location.X + 251 < screenBoundaries.Item1 && branch.HitTestVisible).ToList();
            List<Edge> badEdges = Edges.Where(edge => (edge.Sink.X < screenBoundaries.Item1 - 251 && edge.Source.X < screenBoundaries.Item1 - 251) || (edge.Sink.X > screenBoundaries.Item3 + 300 && edge.Source.X > screenBoundaries.Item3 + 300)).ToList();
            foreach (var edge in badEdges) { Edges.Remove(edge); View.Children.Remove(edge); }
            foreach (var bb in badBranches) { View.Children.Remove(bb.Control); Branches.Remove(bb); bb.UnsubscribeModel(); }
            List<Point> badCommitLocations = badCommits.Select(commit => commit.Location).ToList();
            foreach (var bc in badCommits) { View.Children.Remove(bc.Control); Commits.Remove(bc); bc.UnsubscribeModel(); }
            List<Point> commitLocations = Commits.Select(commit => commit.Location).ToList();
            List<Point> branchLocations = Branches.Select(branch => branch.Location).ToList();
            List<CommitNodeModel> newCommits = new List<CommitNodeModel>();
            List<BranchLabelModel> newBranches = new List<BranchLabelModel>();
            int f, c;
            for (f = 0; f < Model.Commits.Count; f++)
            {
                if (Model.Commits[f].Location.X + 251 >= screenBoundaries.Item1)
                    break;
            }
            for (c = f; c < Model.Commits.Count; c++)
            {
                if (Model.Commits[c].Location.X <= screenBoundaries.Item3)
                {
                    if (!commitLocations.Contains(Model.Commits[c].Location))
                        newCommits.Add(Model.Commits[c]);
                }
                else break;
            }
            foreach (CommitNodeModel m in newCommits)
            {
                CommitNodeView v = new CommitNodeView();
                CommitNodeViewModel vm = new CommitNodeViewModel(m, v);
                v.Update();
                Commits.Add(vm);
                View.Children.Add(v);
            }
            for (c = 0; c < Model.Branches.Count; c++)
            {
                if (!branchLocations.Contains(Model.Branches[c].Location) && newCommits.Any(commit => commit.Commit == Model.Branches[c].Branch.Tip))
                        newBranches.Add(Model.Branches[c]);
            }
            foreach (BranchLabelModel m in newBranches)
            {
                BranchLabelView v = new BranchLabelView();
                BranchLabelViewModel vm = new BranchLabelViewModel(m, v);
                View.Children.Add(v);
                Branches.Add(vm);
            }
            foreach (var m in newCommits)
            {
                foreach (var pair in Model.Edges[m])
                {
                    Point source = new Point(pair.Item2.Location.X, pair.Item2.Location.Y + 20);
                    Point sink = new Point(pair.Item1.Location.X + pair.Item1.MaxWidth, pair.Item1.Location.Y + 20);
                    if (!Edges.Any(edge => edge.Source == source && edge.Sink == sink))
                    {
                        Edge edge = new Edge() { Source = source, Sink = sink, StrokeThickness = 2, Stroke = new SolidColorBrush(Colors.Gray) };
                        Edges.Add(edge);
                        View.Children.Add(edge);
                    }
                }
            }
        }

        void UpdateContentOld()
        {
            Dictionary<LibGit2Sharp.Commit, CommitNodeViewModel> dict = new Dictionary<LibGit2Sharp.Commit, CommitNodeViewModel>();
            List<Control> controls = new List<Control>();
            foreach (CommitNodeModel m in Model.Commits)
            {
                CommitNodeView v = new CommitNodeView();
                CommitNodeViewModel vm = new CommitNodeViewModel(m, v);
                v.Update();
                dict.Add(m.Commit, vm);
                controls.Add(v);
            }
            foreach (BranchLabelModel m in Model.Branches)
            {
                BranchLabelView v = new BranchLabelView();
                BranchLabelViewModel vm = new BranchLabelViewModel(m, v);
                controls.Add(v);
            }
            foreach (CommitNodeModel m in Model.Commits)
            {
                foreach (LibGit2Sharp.Commit c in m.Commit.Parents)
                {
                    Point source = new Point(m.Location.X, m.Location.Y + 20);
                    Point sink = new Point(dict[c].Location.X + Math.Min(500, dict[c].MaxWidth), dict[c].Location.Y + 20);
                    Edge e = new Edge()
                    {
                        Source = source,
                        Sink = sink,
                        StrokeThickness = 2,
                        Stroke = new SolidColorBrush(Colors.Gray)
                    };
                    controls.Add(e);
                }
            }
            View.Children.Clear();
            controls.ForEach(c => View.Children.Add(c));
        }

        private void TransformMatrixChanged(Matrix m)
        {
            CanvasTransform.Matrix = m;
        }
    }
}
