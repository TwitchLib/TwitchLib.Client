using System;
using System.Collections.Generic;

using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Interfaces
{
    /// <summary>
    ///     everthing related to <see cref="Models.JoinedChannel"/>s
    /// </summary>
    public interface ITwitchClient_Channel
    {
        /// <summary>
        ///     <see cref="Models.JoinedChannel"/>
        /// </summary>
        IReadOnlyList<JoinedChannel> JoinedChannels { get; }

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

        /// <summary>
        /// </summary>
        /// <param name="channel">
        ///     name of the channel
        /// </param>
        /// <returns>
        ///     <see cref="Models.JoinedChannel"/>
        /// </returns>
        JoinedChannel GetJoinedChannel(string channel);

        /// <summary>
        /// </summary>
        /// <param name="channel">
        ///     The channel.
        /// </param>
        /// <param name="overrideCheck">
        ///     if set to <see langword="true"/> [override check].
        /// </param>
        void JoinChannel(string channel, bool overrideCheck = false);
        /// <summary>
        /// </summary>
        /// <param name="channel">
        ///     <see cref="Models.JoinedChannel"/>
        ///     <br></br>
        ///     <br></br>
        ///     <seealso cref="GetJoinedChannel(String)"/>
        /// </param>
        void LeaveChannel(JoinedChannel channel);
        /// <summary>
        /// </summary>
        /// <param name="channel">
        ///     The channel.
        /// </param>
        void LeaveChannel(string channel);
    }
}
