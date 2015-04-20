using System;
using System.Windows.Input;

namespace JustObjectsPrototype.UI
{
	public class Command : ICommand
	{
		Action _Execute;
		Func<bool> _CanExecute;

		public Command(Action execute, Func<bool> canExecute = null)
		{
			_Execute = execute;
			_CanExecute = canExecute ?? (() => true);
		}

		public bool CanExecute(object parameter)
		{
			return _CanExecute();
		}

		public void Execute(object parameter)
		{
			_Execute();
		}

		public event EventHandler CanExecuteChanged;

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged(this, new EventArgs());
		}
	}
}
