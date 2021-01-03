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
        public Action<GraphItemModel, MouseButtonEventArgs> MouseDownEventHandler { get; set; }
        public Action<GraphItemModel, MouseButtonEventArgs> MouseUpEventHandler { get; set; }
        public Action<GraphItemModel, MouseEventArgs> MouseEnterEventHandler { get; set; }
        public Action<GraphItemModel, MouseEventArgs> MouseLeaveEventHandler { get; set; }
        public Action<CommitNodeModel> AddBranchEventHandler { get; set; }
        public Action<CommitNodeModel> CopyHash { get; set; }
    }
}
