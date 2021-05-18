using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using LibGit2Sharp;

namespace GitGUI.Logic
{
    public class BranchLabelModel : GraphItemModel
    {
        MatrixTransform _transform;
        bool _hitTestVisible = true, _arrow = false, _aggregationFocused;
        public Branch Branch { get; set; }
        public string Name { get { return Branch.FriendlyName; } }
        public bool IsRemote { get { return Branch.IsRemote; } }
        public bool IsTracking { get { return Branch.IsTracking; } }
        public string Remote { get { return Branch.RemoteName; } }
        public string TrackedBranch {  get { return Branch.TrackedBranch?.FriendlyName; } }
        public int? AheadBy { get { return Branch.TrackingDetails?.AheadBy; } }
        public int? BehindBy { get { return Branch.TrackingDetails?.BehindBy; } }
        public bool IsHead { get { return Branch.IsCurrentRepositoryHead; } }
        public bool Arrow { get { return _arrow; } set { _arrow = value; OnPropertyChanged(); } }
        public bool AggregationFocused { get { return _aggregationFocused; } set { _aggregationFocused = value; OnPropertyChanged(); } }
        public int IntType {  get { if (Branch.IsTracking) return 1; else if (Branch.IsRemote) return 2; else return 0; } }

        public BranchLabelModel()
        {
            PlusButton = false;
        }

        public MatrixTransform RenderTransform
        {
            get { return _transform; }
            set { _transform = value;  OnPropertyChanged(); }
        }
        public bool IsHitTestVisible
        {
            get { return _hitTestVisible; }
            set { _hitTestVisible = value; OnPropertyChanged(); }
        }
    }
}
