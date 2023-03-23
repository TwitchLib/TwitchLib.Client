using System;
using System.Collections.Generic;

using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions.Internal;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Managers;
using TwitchLib.Client.Models;

namespace TwitchLib.Client
{
    public partial class TwitchClient : ITwitchClient_Channel
    {
        // TraceMethodCall should use the Type of the interface,
        // that this class extends;
        // it makes it easier to find the respective occurance from the log file

        #region properties private
        private ChannelManager ChannelManager { get; }
        private List<string> HasSeenJoinedChannels { get; } = new List<string>();
        #endregion properties private


        #region properties public
        public IReadOnlyList<JoinedChannel> JoinedChannels => ChannelManager.JoinedChannels;
        #endregion properties public


        #region events public
        public event EventHandler<OnJoinedChannelArgs> OnJoinedChannel;
        public event EventHandler<OnChannelStateChangedArgs> OnChannelStateChanged;
        public event EventHandler<OnLeftChannelArgs> OnLeftChannel;
        public event EventHandler<OnFailureToReceiveJoinConfirmationArgs> OnFailureToReceiveJoinConfirmation;
        #endregion events public


        #region methods public
        public void JoinChannel(string channel)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Channel));
            if (!IsInitialized) HandleNotInitialized();
            if (!IsConnected) HandleNotConnected();
            ChannelManager.JoinChannel(channel);
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
        #endregion methods public
    }
}
