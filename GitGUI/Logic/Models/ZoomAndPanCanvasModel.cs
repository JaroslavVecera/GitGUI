using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GitGUI.Logic
{
    public class ZoomAndPanCanvasModel : ModelBase
    {
        Matrix _matrix = new Matrix(1, 0, 0, 1, 0, 0);
        public event Action<Tuple<double,double,double,double>> ContentUpdated;
        public event Action ContentCleared;

        public double Width { get; set; }
        public double Height { get; set; }

        public Matrix TransformMatrix { get { return _matrix; } set { _matrix = value; TransformMatrixChanged.Invoke(value); } }
        public Point Center { get { return new Point(Width / 2, Height / 2); } }
        public List<CommitNodeModel> Commits { get; set; }
        public List<BranchLabelModel> Branches { get; set; }
        public Dictionary<LibGit2Sharp.Commit, CommitNodeModel> CommitToModel { get; } = new Dictionary<LibGit2Sharp.Commit, CommitNodeModel>();
        public Dictionary<CommitNodeModel, List<Tuple<CommitNodeModel, CommitNodeModel>>> Edges { get; set; } = new Dictionary<CommitNodeModel, List<Tuple<CommitNodeModel, CommitNodeModel>>>();
        public event Action<Matrix> TransformMatrixChanged;
        Tuple<double, double, double, double> CanvasBoundaries { get; set; }
        double Margin { get; } = 100;
        public event Action Released;
        public event Action Captured;
        Tuple<double, double, double, double> GetViewportBoundaries(Size screenSize)
        {
            Matrix m = TransformMatrix;
            m.Invert();
            Point tl = m.Transform(new Point(0, 0));
            Point br = m.Transform(new Point(screenSize.Width, screenSize.Height));
            return new Tuple<double, double, double, double>(tl.X, tl.Y, br.X, br.Y);
        }

        public void CaptureMouse()
        {
            Captured?.Invoke();
        }

        public void ReleaseMouseCapture()
        {
            Released?.Invoke();
        }

        public void Update(Size screenSize)
        {
            ComputeCanvasBoundaries();
            UpdateControls(screenSize);
        }

        public void Clear()
        {
            Commits?.Clear();
            Branches?.Clear();
            Edges?.Clear();
            CommitToModel?.Clear();
            ContentCleared?.Invoke();
        }

        void UpdateControls(Size screenSize)
        { 
            ContentUpdated?.Invoke(GetViewportBoundaries(screenSize));
        }

        void ComputeCanvasBoundaries()
        {
            if (!Commits.Any())
            {
                CanvasBoundaries = new Tuple<double, double, double, double>(0, 0, 0, 0);
                return;
            }
            double i1, i2, i3, i4;
            i1 = i3 = Commits.First().Location.X;
            i2 = i4 = Commits.First().Location.Y;
            foreach (GraphItemModel m in Commits.Cast<GraphItemModel>().Union(Branches))
            {
                i1 = Math.Min(i1, m.Location.X);
                i2 = Math.Min(i2, m.Location.Y);
                i3 = Math.Max(i3, m.Location.X);
                i4 = Math.Max(i4, m.Location.Y);
            }
            CanvasBoundaries = new Tuple<double, double, double, double>(i1, i2, i3, i4);
        }

        public void Rescale(double factor, Point origin)
        {
            Matrix m = TransformMatrix;
            m.M22 = m.M11 *= factor;
            m.OffsetX = (m.OffsetX - origin.X) * factor + origin.X;
            m.OffsetY = (m.OffsetY - origin.Y) * factor + origin.Y;
            TransformMatrix = m;
        }

        public Vector Move(Vector move, Size screenSize)
        {
            if (CanvasBoundaries == null)
                return new Vector(0, 0);
            Matrix m = TransformMatrix;
            Point tl = TransformMatrix.Transform(new Point(CanvasBoundaries.Item1 - Margin, CanvasBoundaries.Item2 - Margin));
            Point br = TransformMatrix.Transform(new Point(CanvasBoundaries.Item3 + 150 + Margin, CanvasBoundaries.Item4 + 40 + Margin));
            Size canvasSize = new Size(br.X - tl.X, br.Y - tl.Y);
            Size minSize = new Size(Math.Min(canvasSize.Width, screenSize.Width), Math.Min(screenSize.Height, canvasSize.Height));

            Vector res = new Vector();
            if (0 < tl.X + move.X)
                DoMove(new Vector(res.X = -tl.X, 0));
            else if (br.X + move.X < minSize.Width)
                DoMove(new Vector(res.X = minSize.Width - br.X, 0));
            else
                DoMove(new Vector(res.X = move.X, 0));
            if (0 < tl.Y + move.Y)
                DoMove(new Vector(0, res.Y = -tl.Y));
            else if (br.Y + move.Y < minSize.Height)
                DoMove(new Vector(0, res.Y = minSize.Height - br.Y));
            else
                DoMove(new Vector(0, res.Y = move.Y));
            UpdateControls(screenSize);
            return res;
        }

        void DoMove(Vector move)
        { 
            Matrix m = TransformMatrix;
            m.OffsetX += move.X;
            m.OffsetY += move.Y;
            TransformMatrix = m;
        }

        public void ResetTransform()
        {
            TransformMatrix = Matrix.Identity;
        }

        void OnTransformMatrixChanged()
        {
            TransformMatrixChanged?.Invoke(TransformMatrix);
        }

        public void CreateEdgePairs()
        {
            CommitToModel.Clear();
            Edges.Clear();
            foreach (var c in Commits)
            {
                CommitToModel.Add(c.Commit, c);
                Edges.Add(c, new List<Tuple<CommitNodeModel, CommitNodeModel>>());
            }
            foreach (var c in Commits)
            {
                foreach (var parent in c.Commit.Parents)
                {
                    Edges[CommitToModel[parent]].Add(new Tuple<CommitNodeModel, CommitNodeModel>(CommitToModel[parent], c));
                    Edges[c].Add(new Tuple<CommitNodeModel, CommitNodeModel>(CommitToModel[parent], c));
                }
            }
        }
    }
}
