using System;
using System.Windows.Input;

namespace FontImageHx
{
    public class WindowCommand : ICommand
    {
        public delegate void ExecuteDelegate(object? param);
        public event EventHandler? CanExecuteChanged;
        private readonly ExecuteDelegate _delegate;

        public WindowCommand(ExecuteDelegate execute)
        {
            _delegate = execute;
        }

        public bool CanExecute(object? parameter)
            => true;


        public void Execute(object? parameter)
            => _delegate(parameter);
    }
}
