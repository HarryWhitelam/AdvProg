using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Frontend
{
    /// <summary>
    /// Class <c>ActionCommand</c> is an implementation of ICommand
    /// </summary>
    public class ActionCommand : ICommand
    {
        private readonly Action _action;

        /// <summary>
        /// Method <c>ActionCommand</c> defines an action from XAML to be handled by C# methods
        /// </summary>
        /// <param name="action"><c>action</c> is the action to take place</param>
        public ActionCommand(Action action)
        {
            _action = action;
        }

        /// <summary>
        /// Method <c>Execute</c> causes the action to occur
        /// </summary>
        /// <param name="parameter"><c>parameter</c> allows execute to take a parameter if needed</param>
        public void Execute(object parameter)
        {
            _action();
        }

        /// <summary>
        /// Method <c>CanExecute</c> defines a commands ability to be executed
        /// </summary>
        /// <param name="parameter"><c>parameter</c> allows execute to take a parameter if needed</param>
        /// <returns>Returns true if executable, or false if not</returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Establishes if the value of CanExecute has changed
        /// </summary>
        public event EventHandler CanExecuteChanged;
    }
}
