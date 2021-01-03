using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class BranchLabelViewModel : GraphItemViewModel
    {
        public string Name { get { return ((BranchLabelModel)Model).Name; } }

        public BranchLabelViewModel(BranchLabelModel model, BranchLabelView view) : base (model, view)
        {
            SubscribeViewEvents(view);
            InitializeLocation();
        }

        void SubscribeViewEvents(BranchLabelView view)
        {
            LocationChanged += view.OnLocationChanged;

            /*FocusedChanged += view.OnFocusedChanged;
            MarkedChanged += view.OnMarkedChanged;
            CheckoutedChanged += view.OnCheckoutedChanged;*/
        }
    }
}
