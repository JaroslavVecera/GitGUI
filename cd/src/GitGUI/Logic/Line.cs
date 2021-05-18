using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    public enum LineType
    {
        HunkHeader,
        Unchanged,
        Old,
        New
    }

    public abstract class Line
    {
        protected LineType _type;
        public string Content { get; protected set; }
        public LineType Type { get { return _type; } }
        public int IntType { get { return (int)Type; } }

        protected Line(string content)
        {
            Content = content;
        }
    }

    public class HeaderLine : Line
    {
        public int InitialNewLineNumber { get { return int.Parse(new String(Content.Substring(Content.IndexOf('+') + 1).TakeWhile(Char.IsDigit).ToArray())); } }
        public int InitialOldLineNumber { get { return int.Parse(new String(Content.Substring(Content.IndexOf('-') + 1).TakeWhile(Char.IsDigit).ToArray())); } }

        public HeaderLine(string line) : base(line)
        {
            _type = LineType.HunkHeader;
        }
    }

    public abstract class NonHeaderLine : Line
    {
        public int? NewLineNumber { get; protected set; } = null;
        public int? OldLineNumber { get; protected set; } = null;

        public NonHeaderLine(string line) : base(String.Concat(line.Skip(1))) { }
    }

    public class UnchangedLine : NonHeaderLine
    {
        public UnchangedLine(string line, int oldLineNumber, int newLineNumber) : base(line)
        {
            OldLineNumber = oldLineNumber;
            NewLineNumber = newLineNumber;
        }
    }

    public class OldLine : NonHeaderLine
    {
        public OldLine(string line, int lineNumber) : base(line)
        {
            OldLineNumber = lineNumber;
            _type = LineType.Old;
        }
    }

    public class NewLine : NonHeaderLine
    {
        public NewLine(string line, int lineNumber) : base(line)
        {
            NewLineNumber = lineNumber;
            _type = LineType.New;
        }
    }
}
