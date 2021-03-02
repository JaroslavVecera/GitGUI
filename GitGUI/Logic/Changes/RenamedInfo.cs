using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    public class RenamedInfo :ChangesInfo
    {
        string OldName { get; set; }
        string NewName { get; set; }
    }
}
