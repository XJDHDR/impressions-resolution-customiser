// This file is or was originally a part of the Game Mods project by XJDHDR, which can be found here:
// https://github.com/XJDHDR/game-mods
//
// The license for it may be found here:
// https://github.com/XJDHDR/game-mods/blob/master/LICENSE.md
//

using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Zeus_and_Poseidon.ViewModels
{
	/// <summary>
	/// A command whose sole purpose is to relay its functionality to other
	/// objects by invoking delegates. The default return value for the
	/// CanExecute method is 'true'.
	/// </summary>
	public class RelayCommand : ICommand
	{
		private readonly Action<object> execute;
		private readonly Predicate<object> canExecute;


		/// <summary>
		/// Creates a new command that can always execute.
		/// </summary>
		/// <param name="Execute">The execution logic.</param>
		public RelayCommand(Action<object> Execute)
			: this(Execute, null)
		{
		}

		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="Execute">The execution logic.</param>
		/// <param name="CanExecute">The execution status logic.</param>
		public RelayCommand(Action<object> Execute, Predicate<object> CanExecute)
		{
			execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
			canExecute = CanExecute;
		}


		[DebuggerStepThrough]
		public bool CanExecute(object Parameters)
		{
			if (canExecute != null && Parameters == null)
				throw new ArgumentNullException(nameof(Parameters));

			return canExecute?.Invoke(Parameters) ?? true;
		}

		public event EventHandler CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}

		public void Execute(object Parameters) =>
			execute(Parameters);
	}
}
