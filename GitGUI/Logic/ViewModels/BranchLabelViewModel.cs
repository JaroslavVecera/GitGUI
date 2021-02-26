using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GitGUI.Logic
{
    class BranchLabelViewModel : GraphItemViewModel
    {
        public string Name { get { return ((BranchLabelModel)Model).Name; } }
        public bool Arrow { get { return ((BranchLabelModel)Model).Arrow; } }

        public BranchLabelViewModel(BranchLabelModel model, BranchLabelView view) : base (model, view)
        {
            Panel.SetZIndex(view, 2);
            SubscribeViewEvents(view);
            InitializeLocation();
        }

        void SubscribeViewEvents(BranchLabelView view)
        {
            LocationChanged += view.OnLocationChanged;
            FocusedChanged += view.OnFocusedChanged;
            MarkedChanged += view.OnMarkedChanged;
            CheckoutedChanged += view.OnCheckoutedChanged;
        }
    }
}
