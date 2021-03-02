namespace SunsetOffsetCalculator.MVVM
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    /// The ViewModelCommand class - an ICommand that can fire a function.
    /// </summary>
    /// <seealso cref="T:System.Windows.Input.ICommand"/>
    /// -------------------------------------------------------------------------------------------------
    public class Command : ICommand, INotifyPropertyChanged
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Boolean indicating whether the command can execute.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        private bool canExecute;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="canExecute">
        /// if set to <c>true</c> [can execute].
        /// </param>
        /// -------------------------------------------------------------------------------------------------
        public Command(Action action, bool canExecute = true)
        {
            // Set the action.
            Action = action;
            this.canExecute = canExecute;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="parameterizedAction">
        /// The parameterized action.
        /// </param>
        /// <param name="canExecute">
        /// If set to <c>true</c> [can execute].
        /// </param>
        /// -------------------------------------------------------------------------------------------------
        public Command(Action<object> parameterizedAction, bool canExecute = true)
        {
            // Set the action.
            ParameterizedAction = parameterizedAction;
            this.canExecute = canExecute;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Occurs when can execute is changed.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public event EventHandler CanExecuteChanged;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Occurs when the command executed.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public event EventHandler<CommandEventArgs> Executed;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Occurs when the command is about to execute.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public event EventHandler<CancelCommandEventArgs> Executing;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether this instance can execute.
        /// </summary>
        /// <value>
        /// True if this instance can execute; otherwise, false.
        /// </value>
        /// -------------------------------------------------------------------------------------------------
        public bool CanExecute
        {
            get => canExecute;

            set
            {
                if (canExecute != value)
                {
                    canExecute = value;
                    OnPropertyChanged();
                    EventHandler canExecuteChanged = CanExecuteChanged;
                    canExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the action that will be called when the command is invoked.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        protected Action Action { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the parameterized action that will be called when the command is invoked.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        protected Action<object> ParameterizedAction { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="param">
        /// The param.
        /// </param>
        /// -------------------------------------------------------------------------------------------------
        public virtual void DoExecute(object param)
        {
            // Invoke the executing command, allowing the command to be cancelled.
            var args = new CancelCommandEventArgs { Parameter = param, Cancel = false };
            InvokeExecuting(args);

            // If the event has been cancelled, bail now.
            if (args.Cancel)
            {
                return;
            }

            // Call the action or the parameterized action, whichever has been set.
            InvokeAction(param);

            // Call the executed function.
            InvokeExecuted(new CommandEventArgs { Parameter = param });
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command.  If the command does not require data to be passed, this object can
        /// be set to null.
        /// </param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <seealso cref="M:System.Windows.Input.ICommand.CanExecute(object)"/>
        /// -------------------------------------------------------------------------------------------------
        bool ICommand.CanExecute(object parameter)
        {
            return canExecute;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command.  If the command does not require data to be passed, this object can
        /// be set to null.
        /// </param>
        /// <seealso cref="M:System.Windows.Input.ICommand.Execute(object)"/>
        /// -------------------------------------------------------------------------------------------------
        void ICommand.Execute(object parameter)
        {
            DoExecute(parameter);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the action on a different thread, and waits for the result.
        /// </summary>
        /// <param name="param">
        /// The param.
        /// </param>
        /// -------------------------------------------------------------------------------------------------
        protected void InvokeAction(object param)
        {
            Action theAction = Action;
            Action<object> theParameterizedAction = ParameterizedAction;
            if (theAction != null)
            {
                theAction();
            }
            else
            {
                theParameterizedAction?.Invoke(param);
            }
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the command on a different thread, and waits for the result.
        /// </summary>
        /// <param name="args">
        /// Command event information.
        /// </param>
        /// -------------------------------------------------------------------------------------------------
        protected void InvokeExecuted(CommandEventArgs args)
        {
            // Call the executed event.
            Executed?.Invoke(this, args);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the cancallable command on a different thread, and waits for the result.
        /// </summary>
        /// <param name="args">
        /// Cancel command event information.
        /// </param>
        /// -------------------------------------------------------------------------------------------------
        protected void InvokeExecuting(CancelCommandEventArgs args)
        {
            // Call the executed event.
            Executing?.Invoke(this, args);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the property changed action.
        /// </summary>
        /// <param name="propertyName">
        /// Name of the property.
        /// </param>
        /// -------------------------------------------------------------------------------------------------
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
