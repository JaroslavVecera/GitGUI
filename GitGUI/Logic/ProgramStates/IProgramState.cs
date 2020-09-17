using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GitGUI.Logic
{
    interface IProgramState
    {
        Program Program { get; set; }
        void MouseUp(Node sender, CrossStateData data, MouseButtonEventArgs e);
        void MouseDown(Node sender, CrossStateData data, MouseButtonEventArgs e);
        void MouseMove(CrossStateData data, MouseEventArgs e);
        void MouseWheelMove(CrossStateData data, int delta);
        void MouseLeaveWindow(CrossStateData data);
        void MouseLeave(Node sender, CrossStateData data);
        void MouseEnter(Node sender, CrossStateData data);
    }
}
