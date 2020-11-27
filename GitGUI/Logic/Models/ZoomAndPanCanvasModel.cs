﻿using System;
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
        List<CommitNodeModel> _commits;
        public List<CommitNodeModel> Commits { get { return _commits; } set { _commits = value; ContentUpdated?.Invoke(); } }
        public event Action<Matrix> TransformMatrixChanged;

        public void Rescale(double factor, Point origin)
        {
            Matrix m = TransformMatrix;
            m.M22 = m.M11 *= factor;
            m.OffsetX = (m.OffsetX - origin.X) * factor + origin.X;
            m.OffsetY = (m.OffsetY - origin.Y) * factor + origin.Y;
            TransformMatrix = m;
        }

        public void Move(Vector move)
        {
            Matrix m = TransformMatrix;
            m.OffsetX += move.X;
            m.OffsetY += move.Y;
            TransformMatrix = m;
        }

        void OnTransformMatrixChanged()
        {
            TransformMatrixChanged?.Invoke(TransformMatrix);
        }
    }
}
