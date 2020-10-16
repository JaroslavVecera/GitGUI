using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GitGUI
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        private Action toExecute;
        private Func<bool> canExecute;
        public RelayCommand(Action methodToExecute, Func<bool> canExecuteEvaluator)
        {
            toExecute = methodToExecute;
            canExecute = canExecuteEvaluator;
        }
        public RelayCommand(Action methodToExecute)
            : this(methodToExecute, null)
        {
        }
        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
            {
                return true;
            }
            else
            {
                bool result = canExecute.Invoke();
                return result;
            }
        }
        public void Execute(object parameter)
        {
            toExecute.Invoke();
        }
    }
}
