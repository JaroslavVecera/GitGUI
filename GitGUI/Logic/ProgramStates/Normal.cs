using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class Normal : IProgramState
    {
        public Program Program { get; set; }
        static Normal Instance { get; set; }
        public void MouseDown(Node sender, CrossStateData data, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (sender == null)
                    Program.ChangeState(MovingCanvas.GetInstance());
                else
                {
                    data.AttachedNode = sender;
                    MovingNode s = MovingNode.GetInstance();
                    Program.ChangeState(s);
                    s.SetNode(sender);
                }
            }
        }

        public void MouseMove(CrossStateData data, MouseEventArgs e) { }

        public void MouseUp(Node sender, CrossStateData data, MouseButtonEventArgs e) { }

        public void MouseWheelMove(CrossStateData data, int delta) { }

        public void MouseLeaveWindow(CrossStateData data) { }

        public void MouseLeave(Node sender, CrossStateData data)
        {
            Program.Focus(null);
        }

        public void MouseEnter(Node sender, CrossStateData data)
        {
            Program.Focus(sender);
        }

        public static Normal GetInstance()
        {
            if (Instance == null)
                Instance = new Normal();
            return Instance;
        }
    }
}
