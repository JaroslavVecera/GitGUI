using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GitGUI.Logic
{
    class CommitEditorTabViewModel : TabViewModel
    {
        CommitEditorTabModel Model { get; set; }

        public ICommand Commit { get; set; }
        public bool IsChecked { get { return Model.IsChecked; } set { Model.IsChecked = value; } }
        public string Message { get { return Model.Message; } set { Model.Message = value; } }

        public string Header { get { return "Create commit"; } }

        public CommitEditorTabViewModel(CommitEditorTabModel model) : base(model)
        {
            Model = model;
            SubscribeModel(model);
            Commit = new RelayCommand(() => Model.Commit());
        }

        void SubscribeModel(CommitEditorTabModel model)
        {

        }
    }
}
