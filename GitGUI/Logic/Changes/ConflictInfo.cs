using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class ConflictInfo : ChangesInfo
    {
        public string Content { get { return "Conflict file.\n(resolve conflicts and commit)"; } }
    }
}
