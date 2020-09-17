using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class MovingCanvasState : IProgramState
    {
        static MovingCanvasState Instance { get; set; } = new MovingCanvasState();

        MovingCanvasState() { }

        public void MouseDown(Node sender, StateRelatedData data, MouseButtonEventArgs e) { }

        public void MouseUp(Node sender, StateRelatedData data, MouseButtonEventArgs e)
        {
            Program p = Program.GetInstance();
            if (e.ChangedButton == MouseButton.Left)
                p.ChangeState(NormalState.GetInstance());
        }

        public void MouseMove(StateRelatedData data, MouseEventArgs e)
        {
            Graph g = Graph.GetInstance();
            g.Move(data.MouseDisplacement);
        }

        public void MouseWheelMove(StateRelatedData data, int delta) { }

        public void MouseLeave(StateRelatedData data) { }

        public static MovingCanvasState GetInstance()
        {
            return Instance;
        }
    }
}
