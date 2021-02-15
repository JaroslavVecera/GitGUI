using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class MainWindowModel : ModelBase
    {
        public event Action ChangedTabs;
        public event Action ChangedIndex;

        public List<TabViewModel> Tabs { get; private set; } = new List<TabViewModel>();
        public int SelectedIndex { get; private set; }

        public MainWindowModel()
        {
            
        }

        public void SelectTab(TabViewModel vm)
        {
            SelectedIndex = Tabs.IndexOf(vm);
            ChangedIndex?.Invoke();
        }

        public void AddTab(TabViewModel vm)
        {
            Tabs.Add(vm);
            ChangedTabs?.Invoke();
        }

        public void RemoveTab(TabViewModel vm)
        {
            Tabs.Remove(vm);
            ChangedTabs?.Invoke();
            if (SelectedIndex > 0)
                SelectedIndex -= 1;
            ChangedIndex?.Invoke();
        }
    }
}
