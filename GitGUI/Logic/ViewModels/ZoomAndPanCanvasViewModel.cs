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
            CanvasTransform = new MatrixTransform(Model.TransformMatrix);
            Model.Released += () => View.ReleaseMouseCapture();
            Model.Captured += () => Mouse.Capture(View, CaptureMode.SubTree);
        }

        void UpdateContent()
        {
            View.Children.Clear();
            Dictionary<LibGit2Sharp.Commit, CommitNodeViewModel> dict = new Dictionary<LibGit2Sharp.Commit, CommitNodeViewModel>();
            Model.Commits.ForEach(m =>
            {
                CommitNodeView v = new CommitNodeView();
                CommitNodeViewModel vm = new CommitNodeViewModel(m, v);
                v.Update();
                dict.Add(m.Commit, vm);
                View.Children.Add(v);
            });
            Model.Branches.ForEach(m =>
            {
                BranchLabelView v = new BranchLabelView();
                BranchLabelViewModel vm = new BranchLabelViewModel(m, v);
                View.Children.Add(v);
            });
            Model.Commits.ForEach(m =>
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
                    View.Children.Add(e);
                }
            });
        }

        private void TransformMatrixChanged(Matrix m)
        {
            CanvasTransform.Matrix = m;
        }
    }
}
