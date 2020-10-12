using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GitGUI;

namespace GitGUI.Logic
{
    class CommitNodeViewModel : GraphItemViewModel
    {
        public CommitNodeViewModel(CommitNodeModel model, CommitNodeView view) : base(view)
        {

        }
    }
}
