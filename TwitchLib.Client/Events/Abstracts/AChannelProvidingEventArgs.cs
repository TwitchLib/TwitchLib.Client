using System;

namespace TwitchLib.Client.Events.Abstracts
{
    /// <summary>
    ///     <see cref="EventArgs"/> that provide <see cref="Channel"/>-Name
    /// </summary>
    public abstract class AChannelProvidingEventArgs : EventArgs
    {
        /// <summary>
        ///     Channel-Name
        /// </summary>
        public string? Channel { get; set; }
    }
}
