namespace SunsetOffsetCalculator.MVVM
{
    using System;

    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    /// CommandEventArgs - simply holds the command parameter.
    /// </summary>
    /// <seealso cref="T:System.EventArgs"/>
    /// -------------------------------------------------------------------------------------------------
    public class CommandEventArgs : EventArgs
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        /// -------------------------------------------------------------------------------------------------
        public object Parameter { get; set; }
    }
}
