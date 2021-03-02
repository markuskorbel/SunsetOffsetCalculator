namespace SunsetOffsetCalculator.MVVM
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    /// CancelCommandEventArgs - just like above but allows the event to be cancelled.
    /// </summary>
    /// <seealso cref="T:JiraWorklog.MVVM.CommandEventArgs"/>
    /// -------------------------------------------------------------------------------------------------
    public class CancelCommandEventArgs : CommandEventArgs
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CancelCommandEventArgs"/> command
        /// should be cancelled.
        /// </summary>
        /// <value>
        /// <c>true</c> if cancel; otherwise, <c>false</c>.
        /// </value>
        /// -------------------------------------------------------------------------------------------------
        public bool Cancel { get; set; }
    }
}
