﻿using System;

using TwitchLib.Client.Events;

namespace TwitchLib.Client.Interfaces
{
    /// <summary>
    ///     everthing related to <see cref="Models.JoinedChannel"/>s
    /// </summary>
    public interface ITwitchClient_Channel : IChannelManageAble
    {

        /// <summary>
        ///     Occurs when [on channel state changed].
        /// </summary>
        event EventHandler<OnChannelStateChangedArgs> OnChannelStateChanged;
        /// <summary>
        ///     Occurs when [on joined channel].
        /// </summary>
        event EventHandler<OnJoinedChannelArgs> OnJoinedChannel;
        /// <summary>
        ///     Occurs when [on fail to join channel].
        /// </summary>
        event EventHandler<OnFailureToReceiveJoinConfirmationArgs> OnFailureToReceiveJoinConfirmation;
        /// <summary>
        ///     Occurs when [on left channel].
        /// </summary>
        event EventHandler<OnLeftChannelArgs> OnLeftChannel;
    }
}