using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    public class TabModel : ModelBase
    {
        public event Action<TabViewModel> CloseRequested;

        public void Close(TabViewModel vm)
        {
            CloseRequested?.Invoke(vm);
        }
    }
}
