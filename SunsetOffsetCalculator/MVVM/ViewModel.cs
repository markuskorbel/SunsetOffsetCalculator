namespace SunsetOffsetCalculator.MVVM
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    /// Standard viewmodel class base, simply allows property change notifications to be sent.
    /// </summary>
    /// -------------------------------------------------------------------------------------------------
    public class ViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The validation errors of the model.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        private readonly ConcurrentDictionary<string, List<string>> errors =
            new ConcurrentDictionary<string, List<string>>();

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The validation lock object.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        private readonly object validationLock = new object();

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire entity.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Occurs when Property Changed.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets a value indicating whether the entity has validation errors.
        /// </summary>
        /// <value>
        /// true if the entity currently has validation errors; otherwise, false.
        /// </value>
        /// <seealso cref="P:System.ComponentModel.INotifyDataErrorInfo.HasErrors"/>
        /// -------------------------------------------------------------------------------------------------
        public bool HasErrors
        {
            get
            {
                return errors.Any(kv => kv.Value != null && kv.Value.Count > 0);
            }
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to retrieve validation errors for; or null or
        /// <see cref="F:System.String.Empty"/>, to retrieve entity-level errors.
        /// </param>
        /// <returns>
        /// The validation errors for the property or entity.
        /// </returns>
        /// <seealso cref="M:System.ComponentModel.INotifyDataErrorInfo.GetErrors(string)"/>
        /// -------------------------------------------------------------------------------------------------
        public IEnumerable GetErrors(string propertyName)
        {
            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                // Return errors for specific property
                List<string> errorsForName;
                errors.TryGetValue(propertyName, out errorsForName);
                return errorsForName ?? new List<string>();
            }

            // Return all errors
            var allErrors = new List<string>();
            foreach (var subErrorList in errors.Values)
            {
                allErrors.AddRange(subErrorList);
            }

            return allErrors;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the validation errors (as strings) for a specified property or for the entire entity.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to retrieve validation errors for; or null or
        /// <see cref="F:System.String.Empty"/>, to retrieve entity-level errors.
        /// </param>
        /// <returns>
        /// The validation errors for the property or entity.
        /// </returns>
        /// -------------------------------------------------------------------------------------------------
        public IEnumerable<string> GetErrorStrings(string propertyName)
        {
            return (IEnumerable<string>)GetErrors(propertyName);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">
        /// Name of the property.
        /// </param>
        /// -------------------------------------------------------------------------------------------------
        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            // Store the event handler - in case it changes between
            // the line to check it and the line to fire it.
            PropertyChangedEventHandler propertyChanged = PropertyChanged;

            // If the event has been subscribed to, fire it.
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            ValidateAsync();
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the errors changed action.
        /// </summary>
        /// <param name="propertyName">
        /// Name of the property for which the errors have changed.
        /// </param>
        /// -------------------------------------------------------------------------------------------------
        public void OnErrorsChanged(string propertyName)
        {
            var handler = ErrorsChanged;
            handler?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Validates this object.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public void Validate()
        {
            lock (validationLock)
            {
                var validationContext = new ValidationContext(this, null, null);
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(this, validationContext, validationResults, true);

                foreach (var kv in errors.ToList())
                {
                    if (validationResults.All(r => r.MemberNames.All(m => m != kv.Key)))
                    {
                        // ReSharper disable once NotAccessedVariable
                        List<string> outLi;
                        errors.TryRemove(kv.Key, out outLi);
                        OnErrorsChanged(kv.Key);
                    }
                }

                var q = from r in validationResults from m in r.MemberNames group r by m into g select g;

                foreach (var prop in q)
                {
                    var messages = prop.Select(r => r.ErrorMessage).ToList();

                    if (errors.ContainsKey(prop.Key))
                    {
                        // ReSharper disable once NotAccessedVariable
                        List<string> outLi;
                        errors.TryRemove(prop.Key, out outLi);
                    }

                    errors.TryAdd(prop.Key, messages);
                    OnErrorsChanged(prop.Key);
                }
            }
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Validates the object asynchronously.
        /// </summary>
        /// <returns>
        /// The async task handle.
        /// </returns>
        /// -------------------------------------------------------------------------------------------------
        // ReSharper disable once UnusedMethodReturnValue.Global
        public Task ValidateAsync()
        {
            return Task.Run(() => Validate());
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the value of a notifying property.
        /// </summary>
        /// <param name="notifyingProperty">
        /// The notifying property.
        /// </param>
        /// <returns>
        /// The value of the notifying property.
        /// </returns>
        /// -------------------------------------------------------------------------------------------------
        protected object GetValue(NotifyingProperty notifyingProperty)
        {
            return notifyingProperty.Value;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the value of the notifying property.
        /// </summary>
        /// <param name="notifyingProperty">
        /// The notifying property.
        /// </param>
        /// <param name="value">
        /// The value to set.
        /// </param>
        /// <param name="forceUpdate">
        /// If set to <c>true</c> we'll force an update of the binding by calling NotifyPropertyChanged.
        /// </param>
        /// -------------------------------------------------------------------------------------------------
        protected void SetValue(
            NotifyingProperty notifyingProperty,
            object value,
            bool forceUpdate = false)
        {
            // We'll only set the value and notify that it has changed if the
            // value is different - or if we are forcing an update.
            if (notifyingProperty.Value != value || forceUpdate)
            {
                // Set the value
                notifyingProperty.Value = value;

                // Notify that the property has changed
                // ReSharper disable once ExplicitCallerInfoArgument
                NotifyPropertyChanged(notifyingProperty.Name);
            }
        }
    }
}
