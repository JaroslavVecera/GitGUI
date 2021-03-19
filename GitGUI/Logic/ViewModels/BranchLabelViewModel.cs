using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace GitGUI.Logic
{
    class BranchLabelViewModel : GraphItemViewModel
    {
        public string Name { get { return ((BranchLabelModel)Model).Name; } }
        public bool Arrow { get { return ((BranchLabelModel)Model).Arrow; } }
        public MatrixTransform RenderTransform { get { return ((BranchLabelModel)Model).RenderTransform; } }
        public bool IsRemote { get { return ((BranchLabelModel)Model).IsRemote; } }
        public bool IsTracked { get { return ((BranchLabelModel)Model).IsTracking; } }

        public BranchLabelViewModel(BranchLabelModel model, BranchLabelView view) : base (model, view)
        {
            Panel.SetZIndex(view, 2);
            SubscribeViewEvents(view);
            InitializeLocation();
            model.Pushed += () => { Panel.SetZIndex(view, 2); HitTestVisible = true; };
            model.Pulled += () => { Panel.SetZIndex(view, 3); HitTestVisible = false; };
        }

        void SubscribeViewEvents(BranchLabelView view)
        {
            LocationChanged += view.OnLocationChanged;
            FocusedChanged += view.OnFocusedChanged;
            MarkedChanged += view.OnMarkedChanged;
        }
    }
}
