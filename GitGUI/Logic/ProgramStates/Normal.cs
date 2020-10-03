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
        public void MouseDown(object sender, CrossStateData data, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (sender == null)
                    Program.ChangeState(MovingCanvas.GetInstance());
                else
                {
                    data.AttachedBranch = (BranchLabelModel)sender;
                    MovingBranch s = MovingBranch.GetInstance();
                    Program.ChangeState(s);
                    s.SetBranchLabel(data.AttachedBranch);
                }
            }
        }

        public void MouseMove(CrossStateData data, MouseEventArgs e) { }

        public void MouseUp(object sender, CrossStateData data, MouseButtonEventArgs e) { }

        public void MouseWheelMove(CrossStateData data, int delta) { }

        public void MouseLeaveWindow(CrossStateData data) { }

        public void MouseLeave(object sender, CrossStateData data)
        {
            Program.Focus(null);
        }

        public void MouseEnter(object sender, CrossStateData data)
        {
            Program.Focus((GraphItemModel)sender);
        }

        public static Normal GetInstance()
        {
            if (Instance == null)
                Instance = new Normal();
            return Instance;
        }
    }
}
