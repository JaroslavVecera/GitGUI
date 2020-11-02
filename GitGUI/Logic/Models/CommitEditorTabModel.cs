using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    public class CommitEditorTabModel : TabModel
    {
        public bool IsChecked { get; set; } = true;
        public string Message { get; set; } = "";

        public void Commit()
        {

        }
    }
}
