using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    public class AppSettings
    {
        public bool UseMouseAsZoomOrigin { get; set; } = false;
        public bool ShowAuthorMiniatures { get; set; } = true;
        public AggregatingBehaviour AggregatingBehaviour { get; set; } = AggregatingBehaviour.Choose;
    }
}
