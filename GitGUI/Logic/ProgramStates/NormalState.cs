using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class NormalState : IProgramState
    {
        static NormalState Instance { get; set; }
        public void MouseDown(Node sender, StateRelatedData data, MouseButtonEventArgs e)
        {
            Program p = Program.GetInstance();
            if (e.ChangedButton == MouseButton.Left)
            {
                if (sender == null)
                    p.ChangeState(MovingCanvasState.GetInstance());
                else
                {
                    data.AttachedNode = sender;
                    MovingNodeState s = MovingNodeState.GetInstance();
                    p.ChangeState(s);
                    s.SetTemporalyMoving(sender);
                }
            }
        }

        public void MouseMove(StateRelatedData data, MouseEventArgs e) { }

        public void MouseUp(Node sender, StateRelatedData data, MouseButtonEventArgs e) { }

        public void MouseWheelMove(StateRelatedData data, int delta) { }

        public void MouseLeave(StateRelatedData data) { }

        public static NormalState GetInstance()
        {
            if (Instance == null)
                Instance = new NormalState();
            return Instance;
        }
    }
}
