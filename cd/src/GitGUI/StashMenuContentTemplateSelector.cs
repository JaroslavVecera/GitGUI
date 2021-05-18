using GitGUI.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GitGUI
{
    class StashMenuContentTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(container is FrameworkElement elem))
            {
                return null;
            }
            if ((item as StashMenuViewModel).Stashes == null || !(item as StashMenuViewModel).Stashes.Any())
            {
                return elem.FindResource("NoStashes") as DataTemplate;
            }
            else
            {
                return elem.FindResource("SomeStashes") as DataTemplate;
            }
        }
    }
}
