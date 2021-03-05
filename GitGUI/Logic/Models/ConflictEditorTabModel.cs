using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class ConflictEditorTabModel : EditorTabModel
    {
        public override string Header { get { return "Resolve conflict"; } }
        public event Action AbortRequest;

        public void Abort()
        {
            AbortRequest?.Invoke();
        }
    }
}
