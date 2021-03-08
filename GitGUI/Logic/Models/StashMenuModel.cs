using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    public class StashMenuModel : ModelBase
    {
        public event Action<string> StashDeleted;
        public event Action<string> StashPopped;
        public event Action<string> StashApplyed;

        IEnumerable<Tuple<string, string>> _stashes;
        public IEnumerable<Tuple<string, string>> Stashes { get { return _stashes; } set { _stashes = value; OnPropertyChanged(); } }

        public void Apply(string sha)
        {
            StashApplyed?.Invoke(sha);
        }

        public void Pop(string sha)
        {
            StashPopped?.Invoke(sha);
        }

        public void Delete(string sha)
        {
            StashDeleted?.Invoke(sha);
        }
    }
}
