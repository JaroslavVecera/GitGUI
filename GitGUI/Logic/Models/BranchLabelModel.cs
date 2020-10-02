using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace GitGUI
{
    public class BranchLabelModel : GraphItemModel
    {
        public bool Checkouted { get; set; }
        public Branch Branch { get; set; }
    }
}
