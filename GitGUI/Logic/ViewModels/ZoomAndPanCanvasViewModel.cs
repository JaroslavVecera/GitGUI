using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Controls;

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
            int i = 0;
            Model.Commits.ToList().ForEach(m =>
            {
                CommitNodeView v = new CommitNodeView();
                CommitNodeViewModel vm = new CommitNodeViewModel(m, v);
                View.Children.Add(v);
                m.Location = new System.Windows.Point((i++) * 150, i * 60);
            });
        }

        private void TransformMatrixChanged(Matrix m)
        {
            CanvasTransform.Matrix = m;
        }
    }
}
