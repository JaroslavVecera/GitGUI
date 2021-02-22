using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Shapes;

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
        }

        void UpdateContent()
        {
            View.Children.Clear();
            Dictionary<LibGit2Sharp.Commit, CommitNodeViewModel> dict = new Dictionary<LibGit2Sharp.Commit, CommitNodeViewModel>();
            Model.Commits.ForEach(m =>
            {
                CommitNodeView v = new CommitNodeView(m.Message, m.EnabledPhoto);
                CommitNodeViewModel vm = new CommitNodeViewModel(m, v);
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
                    Line l = new Line()
                    {
                        X1 = m.Location.X,
                        Y1 = m.Location.Y + 20,
                        X2 = dict[c].Location.X + Math.Min(500, dict[c].Width),
                        Y2 = dict[c].Location.Y + 20,
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Colors.Gray)
                    };
                    View.Children.Add(l);
                }
            });
        }

        private void TransformMatrixChanged(Matrix m)
        {
            CanvasTransform.Matrix = m;
        }
    }
}
