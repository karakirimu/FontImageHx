using System;
using System.Windows.Input;

namespace FontBmpGen
{
    public class MainWindowCommand : ICommand
    {
        public delegate void ExecuteDelegate(object? param);
        public event EventHandler? CanExecuteChanged;
        private ExecuteDelegate _delegate;

        public MainWindowCommand(ExecuteDelegate execute)
        {
            _delegate = execute;
        }

        public bool CanExecute(object? parameter)
            => true;


        public void Execute(object? parameter)
            => _delegate(parameter);
    }
}
