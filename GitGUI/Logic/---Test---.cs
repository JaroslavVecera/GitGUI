using System;
using System.Collections.Generic;
using LibGit2Sharp;

namespace GitGUI.Logic
{
    public class Branch
    {
        public string Name { get; set; } = "";

        public Branch(string name)
        {
            Name = name;
        }
    }

    public class Commit
    {
        public string Message { get; set; } = "";
        public Signature Author { get; set; }

        public Commit(string message)
        {
            Message = message;
        }
    }
}
