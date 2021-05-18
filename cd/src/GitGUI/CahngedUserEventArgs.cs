using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GitGUI
{
    public class ChangedUserEventArgs : RoutedEventArgs
    {
        Logic.User _user;

        public ChangedUserEventArgs(RoutedEvent routedEvent, Logic.User user) : base(routedEvent)
        {
            _user = user;
        }

        public Logic.User User
        {
            get { return _user; }
        }
    }
}
