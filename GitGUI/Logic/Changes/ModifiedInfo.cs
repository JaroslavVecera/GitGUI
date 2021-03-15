using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class ModifiedInfo : ChangesInfo
    {
        public string Patch { get; private set; }
        public string Content {get {return Patch; } }
        public IEnumerable<Hunk> Hunks { get; private set; }
        public bool Binary { get; private set; }

        public ModifiedInfo(ContentChanges changes)
        {
            Patch = changes.Patch;
            Binary = changes.IsBinaryComparison;
        }

        public ModifiedInfo(PatchEntryChanges changes)
        {
            Patch = changes.Patch;
            Binary = changes.IsBinaryComparison;
            Hunks = Diff.Parse(Patch);
        }
    }
}
