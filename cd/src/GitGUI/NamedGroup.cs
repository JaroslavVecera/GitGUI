using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI
{
    public class NamedGroup
    {
        public IEnumerable<Tuple<int, string>> Items { get; set; }
        public string MatchingText { get; set; }
        public string Header { get; set; }
    }
}
