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
        public event Action ContentUpdated;

        public double Width { get; set; }
        public double Height { get; set; }

        public Matrix TransformMatrix { get { return _matrix; } set { _matrix = value; TransformMatrixChanged.Invoke(value); } }
        public Point Center { get { return new Point(Width / 2, Height / 2); } }
        public List<CommitNodeModel> Commits { get; set; }
        public List<BranchLabelModel> Branches { get; set; }
        public List<Tuple<Point, Point>> Edges { get; set; }
        public event Action<Matrix> TransformMatrixChanged;
        Tuple<double, double, double, double> CanvasBoundaries { get; set; }
        double Margin { get; } = 100;

        public void Update()
        {
            ContentUpdated?.Invoke();
            ComputeCanvasBoundaries();
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
    }
}
