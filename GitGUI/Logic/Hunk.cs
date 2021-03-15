using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class Hunk
    {
        public HeaderLine HeaderLine { get; private set; }
        public string Header { get { return HeaderLine.Content; } }
        int InitialOldLineNumber { get { return HeaderLine.InitialOldLineNumber; } }
        int InitialNewLineNumber { get { return HeaderLine.InitialNewLineNumber; } }
        public List<NonHeaderLine> NonHeaderLines { get; private set; } = new List<NonHeaderLine>();

        public Hunk(IEnumerable<string> lines)
        {
            HeaderLine = new HeaderLine(lines.First());
            int oi = InitialOldLineNumber;
            int ni = InitialNewLineNumber;
            foreach(string l in lines.Skip(1))
            {
                if (l.First() == ' ')
                    NonHeaderLines.Add(new UnchangedLine(l, oi++, ni++));
                else if (l.First() == '-')
                    NonHeaderLines.Add(new OldLine(l, oi++));
                else if (l.First() == '+')
                    NonHeaderLines.Add(new NewLine(l, ni++));
            }
        }
    }
}
