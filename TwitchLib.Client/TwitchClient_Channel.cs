using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Timers;

using TwitchLib.Client.Events;
using TwitchLib.Client.Exceptions;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Internal;
using TwitchLib.Client.Managers;
using TwitchLib.Client.Models;

namespace TwitchLib.Client
{
    public partial class TwitchClient : ITwitchClient_Channel
    {
        private Queue<JoinedChannel> JoinChannelQueue { get; } = new Queue<JoinedChannel>();
        private bool CurrentlyJoiningChannels { get; set; }
        private Timer JoinTimer { get; set; }
        private List<KeyValuePair<string, DateTime>> AwaitingJoins { get; set; }
        private JoinedChannelManager JoinedChannelManager { get; }
        private List<string> HasSeenJoinedChannels { get; } = new List<string>();

        public IReadOnlyList<JoinedChannel> JoinedChannels => JoinedChannelManager.GetJoinedChannels();
        public event EventHandler<OnJoinedChannelArgs> OnJoinedChannel;
        public event EventHandler<OnChannelStateChangedArgs> OnChannelStateChanged;
        public event EventHandler<OnLeftChannelArgs> OnLeftChannel;
        public event EventHandler<OnFailureToReceiveJoinConfirmationArgs> OnFailureToReceiveJoinConfirmation;
        public void JoinChannel(string channel, bool overrideCheck = false)
        {
            if (!IsInitialized) HandleNotInitialized();
            if (!IsConnected) HandleNotConnected();
            // Check to see if client is already in channel
            if (JoinedChannels.FirstOrDefault(x => x.Channel.ToLower() == channel && !overrideCheck) != null)
                return;
            if (channel[0] == '#') channel = channel.Substring(1);
            JoinChannelQueue.Enqueue(new JoinedChannel(channel));
            if (!CurrentlyJoiningChannels)
                QueueingJoinCheck();
        }
        public JoinedChannel GetJoinedChannel(string channel)
        {
            if (!IsInitialized) HandleNotInitialized();
            if (JoinedChannels.Count == 0)
                throw new BadStateException("Must be connected to at least one channel.");
            if (channel[0] == '#') channel = channel.Substring(1);
            return JoinedChannelManager.GetJoinedChannel(channel);
        }

        public void LeaveChannel(string channel)
        {
            if (!IsInitialized) HandleNotInitialized();
            // Channel MUST be lower case
            channel = channel.ToLower();
            if (channel[0] == '#') channel = channel.Substring(1);
            Log($"Leaving channel: {channel}");
            JoinedChannel joinedChannel = JoinedChannelManager.GetJoinedChannel(channel);
            if (joinedChannel != null)
                // IDE0058 - client raises OnSendFailed if this method returns false
                Client.Send(Rfc2812.Part($"#{channel}"));
        }
        public void LeaveChannel(JoinedChannel channel)
        {
            if (!IsInitialized) HandleNotInitialized();
            LeaveChannel(channel.Channel);
        }
        [SuppressMessage("Style", "IDE0058")]
        private void QueueingJoinCheck()
        {
            if (JoinChannelQueue.Count > 0)
            {
                CurrentlyJoiningChannels = true;
                JoinedChannel channelToJoin = JoinChannelQueue.Dequeue();
                Log($"Joining channel: {channelToJoin.Channel}");

                // IDE0058 - client raises OnSendFailed if this method returns false
                // important we set channel to lower case when sending join message
                Client.Send(Rfc2812.Join($"#{channelToJoin.Channel.ToLower()}"));
                JoinedChannelManager.AddJoinedChannel(new JoinedChannel(channelToJoin.Channel));
                StartJoinedChannelTimer(channelToJoin.Channel);
            }
            else
            {
                Log("Finished channel joining queue.");
            }
        }
        private void StartJoinedChannelTimer(string channel)
        {
            if (JoinTimer == null)
            {
                JoinTimer = new System.Timers.Timer(1000);
                JoinTimer.Elapsed += JoinChannelTimeout;
                AwaitingJoins = new List<KeyValuePair<string, DateTime>>();
            }
            // channel is ToLower()'d because ROOMSTATE (which is the event the client uses to remove
            // this channel from _awaitingJoins list) contains the username as always lowercase. This means
            // if we don't ToLower(), the channel never gets removed, and FailureToReceiveJoinConfirmation
            // fires.
            AwaitingJoins.Add(new KeyValuePair<string, DateTime>(channel.ToLower(), DateTime.Now));
            if (!JoinTimer.Enabled)
                JoinTimer.Start();
        }
        private void JoinChannelTimeout(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (AwaitingJoins.Any())
            {
                List<KeyValuePair<string, DateTime>> expiredChannels = AwaitingJoins.Where(x => (DateTime.Now - x.Value).TotalSeconds > 5).ToList();
                if (expiredChannels.Any())
                {
                    // IDE0058
                    AwaitingJoins.RemoveAll(x => (DateTime.Now - x.Value).TotalSeconds > 5);
                    foreach (KeyValuePair<string, DateTime> expiredChannel in expiredChannels)
                    {
                        JoinedChannelManager.RemoveJoinedChannel(expiredChannel.Key.ToLowerInvariant());
                        OnFailureToReceiveJoinConfirmation?.Invoke(this, new OnFailureToReceiveJoinConfirmationArgs { Exception = new FailureToReceiveJoinConfirmationException(expiredChannel.Key) });
                    }
                }
            }
            else
            {
                JoinTimer.Stop();
                CurrentlyJoiningChannels = false;
                QueueingJoinCheck();
            }
        }
    }
}
