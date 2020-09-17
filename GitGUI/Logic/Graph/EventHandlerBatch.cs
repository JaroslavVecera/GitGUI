using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class EventHandlerBatch
    {
        public Node.MouseButtonEventHandler MouseDownEventHandler { get; set; }
        public Node.MouseEventHandler MouseEnterEventHandler { get; set; }
        public Node.MouseEventHandler MouseLeaveEventHandler { get; set; }
    }
}
