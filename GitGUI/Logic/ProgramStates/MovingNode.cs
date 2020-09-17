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
using System.Diagnostics;

namespace GitGUI.Logic
{
    class MovingNode : IProgramState
    {
        public Program Program { get; set; }
        static MovingNode Instance { get; set; } = new MovingNode();
        MatrixTransform NodeTransform { get; } = new MatrixTransform();
        Matrix LastCanvasMatrix { get; set; } = Matrix.Identity;
        Stopwatch Stopwatch { get; } = new Stopwatch();
        bool _moved;

        MovingNode() { }

        public static MovingNode GetInstance()
        {
            return Instance;
        }

        public void MoveCanvasToMouse(object sender, EventArgs e)
        {
            Vector translate = Translate();
            double milliseconds = Stopwatch.ElapsedMilliseconds;
            Stopwatch.Restart();
            if (translate == new Vector(0, 0))
                return;
            translate *= milliseconds / 10;
            ZoomAndPanCanvas c = ((MainWindow)Application.Current.MainWindow).ZoomCanvas;
            Matrix inv = c.CanvasTransform.Matrix;
            inv.Invert();
            Graph.GetInstance().Move((Vector)translate);
            Matrix m = NodeTransform.Matrix;
            m.Translate(-translate.X * inv.M11, -translate.Y * inv.M22);
            NodeTransform.Matrix = m;
        }

        Vector Translate()
        {
            ZoomAndPanCanvas c = ((MainWindow)Application.Current.MainWindow).ZoomCanvas;
            Vector mouse = (Vector)Program.Data.MousePoint;
            if (mouse.X > 0 && mouse.X < c.ActualWidth && mouse.Y > 0 && mouse.Y < c.ActualHeight)
                return new Vector(0, 0);
            mouse -= new Vector(c.ActualWidth / 2, c.ActualHeight / 2);
            return -mouse / mouse.Length;
        }

        public void SetNode(Node n)
        {
            n.GElement.RenderTransform = NodeTransform;
            n.ForegroundPull();
            _moved = false;
            n.GElement.IsHitTestVisible = false;
            CompositionTarget.Rendering += MoveCanvasToMouse;
            Stopwatch.StartNew();
        }

        public void MouseUp(Node sender, CrossStateData data, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;
            if (Program.AggregationFocused != null)
                Program.Aggregate(data.AttachedNode, Program.AggregationFocused);
            ChangeState(Normal.GetInstance(), data);
            ReturnNodeBack(data);
            if (_moved == false)
                Program.ShowNode(data.AttachedNode);
        }

        public void MouseDown(Node sender, CrossStateData data, MouseButtonEventArgs e) { }

        public void MouseMove(CrossStateData data, MouseEventArgs e)
        {
            ZoomAndPanCanvas c = ((MainWindow)Application.Current.MainWindow).ZoomCanvas;
            MoveNode(data, e, c);
            
        }

        void MoveNode(CrossStateData data, MouseEventArgs e, ZoomAndPanCanvas c)
        { 
            LastCanvasMatrix = c.CanvasTransform.Matrix;
            Matrix mat = LastCanvasMatrix;
            mat.Invert();
            Vector transformedDispl = mat.Transform(data.MouseDisplacement);
            Matrix m = NodeTransform.Matrix;
            m.Translate(transformedDispl.X, transformedDispl.Y);
            NodeTransform.Matrix = m;
            _moved = true;
        }

        public void MouseWheelMove(CrossStateData data, int delta)
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

        public void MouseLeaveWindow(CrossStateData data)
        {
            ReturnNodeBack(data);
            ChangeState(Normal.GetInstance(), data);
        }

        public void MouseLeave(Node sender, CrossStateData data)
        {
            if (sender != data.AttachedNode)
                Program.AggregationFocus(null);
        }

        public void MouseEnter(Node sender, CrossStateData data)
        {
            Program.AggregationFocus(sender);

        }

        void ChangeState(IProgramState state, CrossStateData data)
        {
            data.AttachedNode.GElement.IsHitTestVisible = true;
            data.AttachedNode.BackgroundPush();
            Program.AggregationFocus(null);
            Program.ChangeState(state);
            CompositionTarget.Rendering -= MoveCanvasToMouse;
            Stopwatch.Stop();
        }

        void ReturnNodeBack(CrossStateData data)
        {
            UIElement el = data.AttachedNode.GElement;
            NodeTransform.Matrix = Matrix.Identity;
            el.RenderTransform = null;
            LastCanvasMatrix.SetIdentity();
        }
    }
}
