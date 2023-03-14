using System;
using System.Collections.Generic;

using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Managers;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Extensions;

namespace TwitchLib.Client
{
    public partial class TwitchClient : ITwitchClient_Channel
    {
        // TraceMethodCall should use the Type of the interface,
        // that this class extends;
        // it makes it easier to find the respective occurance from the log file

        private ChannelManager ChannelManager { get; }
        private List<string> HasSeenJoinedChannels { get; } = new List<string>();

        public IReadOnlyList<JoinedChannel> JoinedChannels => ChannelManager.JoinedChannels;
        public event EventHandler<OnJoinedChannelArgs> OnJoinedChannel;
        public event EventHandler<OnChannelStateChangedArgs> OnChannelStateChanged;
        public event EventHandler<OnLeftChannelArgs> OnLeftChannel;
        public event EventHandler<OnFailureToReceiveJoinConfirmationArgs> OnFailureToReceiveJoinConfirmation;
        public void JoinChannel(string channel, bool overrideCheck = false)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Channel));
            if (!IsInitialized) HandleNotInitialized();
            if (!IsConnected) HandleNotConnected();
            ChannelManager.JoinChannel(channel, overrideCheck);
        }
        public JoinedChannel GetJoinedChannel(string channel)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Channel));
            if (!IsInitialized) HandleNotInitialized();
            return ChannelManager.GetJoinedChannel(channel);
        }

        public void LeaveChannel(string channel)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Channel));
            if (!IsInitialized) HandleNotInitialized();
            ChannelManager.LeaveChannel(channel);
        }
        public void LeaveChannel(JoinedChannel channel)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Channel));
            if (!IsInitialized) HandleNotInitialized();
            LeaveChannel(channel.Channel);
        }
    }
}
