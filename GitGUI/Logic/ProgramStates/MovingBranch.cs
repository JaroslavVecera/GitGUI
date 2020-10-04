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
    class MovingBranch : IProgramState
    {
        public Program Program { get; set; }
        static MovingBranch Instance { get; set; } = new MovingBranch();
        bool _moved;

        MovingBranch() { }

        public static MovingBranch GetInstance()
        {
            return Instance;
        }

        public void SetBranchLabel(BranchLabelModel b)
        {
            CommitManager m = CommitManager.GetInstance();
            m.SetupMovingBranchLabel(b);
            _moved = false;
        }

        public void MouseUp(object sender, CrossStateData data, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;
            if (Program.AggregationFocused != null)
                Program.Aggregate(data.AttachedBranch, Program.AggregationFocused);
            ChangeState(Normal.GetInstance(), data);
            if (_moved == false)
                Program.Show(data.AttachedBranch);
        }

        public void MouseDown(object sender, CrossStateData data, MouseButtonEventArgs e) { }

        public void MouseMove(CrossStateData data, MouseEventArgs e)
        {
            CommitManager.GetInstance().MoveBranch(data.MouseDisplacement);
            _moved = true;
        }

        public void MouseWheelMove(CrossStateData data, int delta)
        {
            CommitManager.GetInstance().BranchLabelToMouse(data.AttachedBranch, data.MousePoint);
        }

        public void MouseLeaveWindow(CrossStateData data)
        {
            ChangeState(Normal.GetInstance(), data);
        }

        public void MouseLeave(object sender, CrossStateData data)
        {
            if (sender != data.AttachedBranch)
                Program.AggregationFocus(null);
        }

        public void MouseEnter(object sender, CrossStateData data)
        {
            Program.AggregationFocus((BranchLabelModel)sender);

        }

        void ChangeState(IProgramState state, CrossStateData data)
        {
            CommitManager.GetInstance().RestoreBranchLabel(data.AttachedBranch);
            Program.AggregationFocus(null);
            Program.ChangeState(state);
        }
    }
}
