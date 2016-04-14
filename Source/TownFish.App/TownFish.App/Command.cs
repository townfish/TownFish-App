using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TownFish.App
{
	public class Command : ICommand
	{
		#region Construction

		public Command(Action executeAction, Func<bool> canExcuteAction = null )
		{
			if (canExcuteAction == null)
				mCanExecuteAction = () => { return true; };
			else
				mCanExecuteAction = canExcuteAction;

			mExecuteAction = executeAction;
		}

		#endregion

		#region Events

		public event EventHandler CanExecuteChanged;

		#endregion

		#region Methods

		public bool CanExecute(object parameter)
		{
			return mCanExecuteAction();
		}

		public void Execute(object parameter)
		{
			mExecuteAction();
		}

		#endregion

		#region Fields

		Func<bool> mCanExecuteAction;
		Action mExecuteAction;

		#endregion
	}
}
