using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GitGUI
{
    public class GraphItemModel
    {
        public bool Marked { get; set; }

        public event MouseButtonEventHandler MouseDown;
        public event MouseEventHandler MouseEnter;
        public event MouseEventHandler MouseLeave;

        public void OnMouseDown(MouseButtonEventArgs e)
        {
            MouseDown?.Invoke(this, e);
        }

        public void OnMouseEnter(MouseEventArgs e)
        {
            MouseEnter?.Invoke(this, e);
        }

        public void OnMouseLeave(MouseEventArgs e)
        {
            MouseLeave?.Invoke(this, e);
        }
    }
}
