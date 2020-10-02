using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Threading.Tasks;

namespace GitGUI
{
    class ZoomAndPanCanvasViewModel : ViewModelBase
    {
        MatrixTransform CanvasTransform { get; } = new MatrixTransform(Matrix.Identity);
        ZoomAndPanCanvasModel ZoomAndPanCanvasModel { get; set; }

        public ZoomAndPanCanvasViewModel(ZoomAndPanCanvasModel model, ZoomAndPanCanvasView view)
        {
            SetModel(model);
            SetView(view);
        }

        void SetModel(ZoomAndPanCanvasModel model)
        {
            ZoomAndPanCanvasModel = model;
            ZoomAndPanCanvasModel.TransformMatrixChanged += TransformMatrixChanged;
        }

        void SetView(ZoomAndPanCanvasView view)
        {
            view.DataContext = this;
            view.RenderTransform = CanvasTransform;
        }

        private void TransformMatrixChanged(Matrix m)
        {
            CanvasTransform.Matrix = m;
        }
    }
}
