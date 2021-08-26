namespace QuickCompareModel
{
    using System;

    /// <summary> Custom derivative of <see cref="EventArgs"/> to contain a status message. </summary>
    [Serializable()]
    public class StatusChangedEventArgs : EventArgs
    {
        /// <summary> Initialises a new instance of the <see cref="StatusChangedEventArgs"/> class. </summary>
        /// <param name="statusMessage">Current status message.</param>
        public StatusChangedEventArgs(string statusMessage) => this.StatusMessage = statusMessage;

        /// <summary> Gets or sets the current status message. </summary>
        public string StatusMessage { get; set; }
    }
}
