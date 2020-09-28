using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GitGUI
{
    class ZoomAndPanCanvasModel
    {
        Matrix _matrix = Matrix.Identity;
        Matrix TransformMatrix
        {
            get { return _matrix; }
            set { _matrix = value; }
        }

        public event Action<Matrix> TransformMatrixChanged;

        public void Rescale(double factor, Point origin)
        {
            Matrix m = _matrix;
            m.M22 = m.M11 *= factor;
            m.OffsetX = (m.OffsetX - origin.X) * factor + origin.X;
            m.OffsetY = (m.OffsetY - origin.Y) * factor + origin.Y;
            TransformMatrix = m;
        }

        public void Move(Vector move)
        {
            Matrix m = _matrix;
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
