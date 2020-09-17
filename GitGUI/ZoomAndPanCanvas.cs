using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GitGUI
{
    public class ZoomAndPanCanvas : Canvas
    {
        public MatrixTransform CanvasTransform { get; } = new MatrixTransform(Matrix.Identity);

        public ZoomAndPanCanvas()
        {
            RenderTransform = CanvasTransform;
        }

        static ZoomAndPanCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomAndPanCanvas), new FrameworkPropertyMetadata(typeof(ZoomAndPanCanvas)));
        }

        protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
        {
            ObservableUIElementCollection c = new ObservableUIElementCollection(this, logicalParent);
            c.AddedUIElement += OnChildAdd;
            return c;
        }

        void OnChildAdd(UIElement sender)
        {
        }

        public void Move(Vector move)
        {
            Matrix m = CanvasTransform.Matrix;
            m.OffsetX += move.X;
            m.OffsetY += move.Y;
            CanvasTransform.Matrix = m;
        }

        public void Rescale(double factor, Point origin)
        {
            Matrix m = CanvasTransform.Matrix;
            m.M22 = m.M11 *= factor;
            m.OffsetX = (m.OffsetX - origin.X) * factor + origin.X;
            m.OffsetY = (m.OffsetY - origin.Y) * factor + origin.Y;
            CanvasTransform.Matrix = m;
        }
    }
}
