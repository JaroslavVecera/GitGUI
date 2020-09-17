using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace GitGUI.Logic
{
    class MovingNodeState : IProgramState
    {
        static MovingNodeState Instance { get; set; } = new MovingNodeState();
        MatrixTransform NodeTransform { get; } = new MatrixTransform();
        Matrix LastCanvasMatrix { get; set; } = Matrix.Identity;
        bool _moved;

        MovingNodeState() { }

        public static MovingNodeState GetInstance()
        {
            return Instance;
        }

        public void SetTemporalyMoving(Node n)
        {
            n.GElement.RenderTransform = NodeTransform;
            n.ForegroundPull();
            _moved = false;
        }

        public void MouseUp(Node sender, StateRelatedData data, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;
            Program p = Program.GetInstance();
            p.ChangeState(NormalState.GetInstance());
            ReturnNodeBack(data);
            data.AttachedNode.BackgroundPush();
            if (_moved == false)
                CommitTree.GetInstance().Mark(data.AttachedNode);
        }

        public void MouseDown(Node sender, StateRelatedData data, MouseButtonEventArgs e) { }

        public void MouseMove(StateRelatedData data, MouseEventArgs e)
        {
            ZoomAndPanCanvas c = ((MainWindow)Application.Current.MainWindow).ZoomCanvas;
            LastCanvasMatrix = c.CanvasTransform.Matrix;
            Matrix mat = LastCanvasMatrix;
            mat.Invert();
            Vector transformedDispl = mat.Transform(data.MouseDisplacement);
            Matrix m = NodeTransform.Matrix;
            m.Translate(transformedDispl.X, transformedDispl.Y);
            NodeTransform.Matrix = m;
            _moved = true;
        }

        public void MouseWheelMove(StateRelatedData data, int delta)
        {
            ZoomAndPanCanvas c = ((MainWindow)Application.Current.MainWindow).ZoomCanvas;
            Matrix mat = c.CanvasTransform.Matrix;
            Matrix inv = mat;
            inv.Invert();
            Point loc = data.AttachedNode.Location;
            Point tloc = mat.Transform(NodeTransform.Transform(loc));
            Point diff = new Point(data.MousePoint.X - tloc.X, data.MousePoint.Y - tloc.Y);
            ScaleTransform scale = new ScaleTransform(inv.M11, inv.M22);
            diff = scale.Transform(diff);
            Matrix m = NodeTransform.Matrix;
            m.Translate(diff.X, diff.Y);
            NodeTransform.Matrix = m;
        }

        public void MouseLeave(StateRelatedData data)
        {
            Program p = Program.GetInstance();
            ReturnNodeBack(data);
            p.ChangeState(NormalState.GetInstance());
        }

        void ReturnNodeBack(StateRelatedData data)
        {
            UIElement el = data.AttachedNode.GElement;
            NodeTransform.Matrix = Matrix.Identity;
            el.RenderTransform = null;
            LastCanvasMatrix.SetIdentity();
        }
    }
}
