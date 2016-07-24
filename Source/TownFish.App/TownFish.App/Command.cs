using System;
using System.Windows.Input;


namespace TownFish.App
{
	/// <summary>
	/// Provides a base implementation of the Command pattern.
	/// </summary>
	/// <seealso cref="System.Windows.Input.ICommand" />
	public class Command: ICommand
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="Command"/> class.
		/// </summary>
		/// <param name="executeAction">The execute action.</param>
		/// <param name="canExcuteAction">The can excute action.</param>
		public Command (Action<object> executeAction, Func<object, bool> canExcuteAction = null)
		{
			mCanExecuteAction = canExcuteAction;
			mExecuteAction = executeAction;
		}

		#endregion Construction

		#region Methods

		/// <summary>
		/// Determines whether the command can execute in its current state.
		/// </summary>
		/// <param name="parameter">Data used by the command.
		/// If the command does not require data to be passed, this object can be set to null.</param>
		/// <returns>
		/// true if this command can be executed; otherwise, false.
		/// </returns>
		public bool CanExecute (object parameter = null)
		{
			return mCanExecuteAction?.Invoke (parameter) ?? true;
		}

		/// <summary>
		/// Called when the command is invoked. Invokes the executeAction defined on construction.
		/// </summary>
		/// <param name="parameter">Data used by the command.
		/// If the command does not require data to be passed, this object can be set to null.</param>
		public void Execute (object parameter = null)
		{
			mExecuteAction?.Invoke (parameter);
		}

		/// <summary>
		/// Called by a derived class when a state change means that <see cref="CanExecute"/> will
		/// return a different value when next called, i.e. 'CanExecute has changed'.
		/// Raises the <see cref="CanExecuteChanged"/> event to notify consumers.
		/// </summary>
		protected void OnCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke (this, EventArgs.Empty);
		}

		#endregion Methods

		#region Events

		/// <summary>
		/// Occurs when changes occur that affect whether or not the command should execute.
		/// </summary>
		public event EventHandler CanExecuteChanged;

		#endregion Events

		#region Fields

		Func<object, bool> mCanExecuteAction;
		Action<object> mExecuteAction;

		#endregion Fields
	}
}
