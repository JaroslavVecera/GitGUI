using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    public class ChangesTreeFileItem : ChangesTreeItem
    {
        public ChangeType Type { get; set; }

        public override IEnumerable<string> GetCheckedPaths(string prefix)
        {
            List<string> res = new List<string>();
            if (IsChecked)
                res.Add(prefix + '/' + Name);
            return res;
        }
    }
}
