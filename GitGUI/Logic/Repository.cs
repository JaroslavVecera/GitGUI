using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class Repository
    {
        public string Path { get; set; }
        public DateTimeOffset LastUse { get; set; }
    }
}
