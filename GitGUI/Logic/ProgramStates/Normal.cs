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
        CommitNodeModel AimedCommit { get; set; } = null;

        private Normal() { }

        public void MouseDown(object sender, CrossStateData data, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (sender == null)
                    ChangeState(MovingCanvas.GetInstance());
                else if (sender is CommitNodeModel)
                    AimedCommit = (CommitNodeModel)sender;
            }
        }

        public void MouseMove(CrossStateData data, MouseEventArgs e) { }

        public void WindowMouseDown(object sender, CrossStateData data, MouseButtonEventArgs e)
        {

        }

        public void WindowMouseUp(object sender, CrossStateData data, MouseButtonEventArgs e)
        {
            AimedCommit = null;
            MouseEnter(sender, data);
        }

        public void MouseUp(object sender, CrossStateData data, MouseButtonEventArgs e)
        {
            if (sender != null && sender == AimedCommit)
                Program.GetInstance().Show(AimedCommit);
            AimedCommit = null;
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
            if (AimedCommit == null || sender == AimedCommit)
                Graph.GetInstance().HighlightAsFocused((GraphItemModel)sender);
        }

        void ChangeState(IProgramState state)
        {
            AimedCommit = null;
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
