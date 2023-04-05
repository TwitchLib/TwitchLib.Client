﻿using TwitchLib.Client.Events.Abstracts;

namespace TwitchLib.Client.Events
{
    /// <summary>
    ///     Args representing a NOTICE telling the client that a verified phone number is required to chat.
    ///     Implements the <see cref="System.EventArgs" />
    /// </summary>
    public class OnRequiresVerifiedPhoneNumberArgs : AChannelProvidingEventArgs
    {
        /// <summary>
        ///     Property representing message send with the NOTICE
        /// </summary>
        public string? Message { get; set; }
    }
}