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
        GraphItemModel Aimed { get; set; } = null;

        private Normal() { }

        public void MouseDown(object sender, CrossStateData data, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (sender == null)
                    ChangeState(MovingCanvas.GetInstance());
                else
                    Aimed = (GraphItemModel)sender;
            }
        }

        public void MouseMove(CrossStateData data, MouseEventArgs e)
        {
            if (Aimed != null && Aimed is BranchLabelModel)
            {
                data.AttachedBranch = (BranchLabelModel)Aimed;
                MovingBranch mb = MovingBranch.GetInstance();
                mb.SetBranchLabel(data.AttachedBranch);
                ChangeState(mb);
            }
        }

        public void WindowMouseDown(object sender, CrossStateData data, MouseButtonEventArgs e)
        {

        }

        public void WindowMouseUp(object sender, CrossStateData data, MouseButtonEventArgs e)
        {
            Aimed = null;
            MouseEnter(sender, data);
        }

        public void MouseUp(object sender, CrossStateData data, MouseButtonEventArgs e)
        {
            if (sender != null && sender == Aimed)
                Program.GetInstance().Show(Aimed);
            Aimed = null;
            MouseEnter(sender, data);
        }

        public void MouseWheelMove(CrossStateData data, int delta) { }

        public void MouseLeaveWindow(CrossStateData data) { }

        public void MouseLeave(object sender, CrossStateData data)
        {
            Graph.GetInstance().HighlightAsFocused(null);
        }

        public void MouseEnter(object sender, CrossStateData data)
        {
            if (Aimed == null || sender == Aimed)
                Graph.GetInstance().HighlightAsFocused((GraphItemModel)sender);
        }

        void ChangeState(IProgramState state)
        {
            Aimed = null;
            Program.ChangeState(state);
        }

        public static Normal GetInstance()
        {
            if (Instance == null)
                Instance = new Normal();
            return Instance;
        }
    }
}
