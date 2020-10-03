using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class MovingCanvas : IProgramState
    {
        public Program Program { get; set; }
        static MovingCanvas Instance { get; set; } = new MovingCanvas();
        bool _moved = false;

        MovingCanvas() { }

        public void MouseDown(object sender, CrossStateData data, MouseButtonEventArgs e) { }

        public void MouseUp(object sender, CrossStateData data, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (!_moved) Program.Show(null);
                ChangeState(Normal.GetInstance());
            }
        }

        void ChangeState(IProgramState state)
        { 
            Program.ChangeState(state);
            _moved = false;
        }

        public void MouseMove(CrossStateData data, MouseEventArgs e)
        {
            Graph g = Graph.GetInstance();
            g.Move(data.MouseDisplacement);
            _moved = true;
        }

        public void MouseWheelMove(CrossStateData data, int delta) { }

        public void MouseLeaveWindow(CrossStateData data)
        {
            ChangeState(Normal.GetInstance());
        }

        public void MouseLeave(object sender, CrossStateData data)
        {
        }

        public void MouseEnter(object sender, CrossStateData data)
        {
        }

        public static MovingCanvas GetInstance()
        {
            return Instance;
        }
    }
}
