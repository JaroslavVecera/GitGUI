using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GitGUI.Logic
{
    public class ActionPanelModel : ModelBase
    {
        public event Action<ActionButtonViewModel> Added;
        bool _enabled = true;
        public bool Enabled { get { return _enabled; } set { _enabled = value; ButtonViewModels.ForEach(m => m.Model.Active = value); } }

        public List<ActionButtonViewModel> ButtonViewModels { get; } = new List<ActionButtonViewModel>();

        public void Add(ActionButtonModel buttonModel)
        {
            ActionButtonViewModel vm = new ActionButtonViewModel(buttonModel);
            ButtonViewModels.Add(vm);
            Enable(buttonModel);
            Added?.Invoke(vm);
        }

        public void Remove(ActionButtonModel buttonModel)
        {

        }

        public void Enable(ActionButtonModel buttonModel)
        {

        }

        public void Disable(ActionButtonModel buttonModel)
        {

        }
    }
}
