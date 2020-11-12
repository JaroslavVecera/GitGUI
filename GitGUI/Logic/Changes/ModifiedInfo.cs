using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class ModifiedInfo : ChangesInfo
    {
        public string Content { get; private set; }

        public ModifiedInfo(string content)
        {
            Content = content;
        }
    }
}
