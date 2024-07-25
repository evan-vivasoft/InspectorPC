using System;
using System.Windows.Input;

namespace Inspector.UI.Initialization.Commands
{
    internal class RelayCommand : ICommand
    {
        private Action<object> execute;

        public RelayCommand(Action<object> execute)
            : this(execute, true)
        { }

        public RelayCommand(Action<object> execute, bool isExecutable)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;

            IsExecutable = isExecutable;
        }

        private bool isExecutable;
        public bool IsExecutable
        {
            get { return isExecutable; }
            set
            {
                if (value != isExecutable)
                {
                    isExecutable = value;
                    OnCanExecuteChanged();
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return isExecutable;
        }

        public event EventHandler CanExecuteChanged;

        protected void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
