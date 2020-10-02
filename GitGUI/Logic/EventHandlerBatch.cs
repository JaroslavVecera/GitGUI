using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GitGUI.Logic
{
    class EventHandlerBatch
    {
        public MouseButtonEventHandler MouseDownEventHandler { get; set; }
        public MouseEventHandler MouseEnterEventHandler { get; set; }
        public MouseEventHandler MouseLeaveEventHandler { get; set; }
    }
}
