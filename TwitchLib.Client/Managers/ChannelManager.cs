﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using TwitchLib.Client.Delegates;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Internal;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Interfaces;

namespace TwitchLib.Client.Managers
{
    internal class ChannelManager : IChannelManageAble
    {
        /// <summary>
        ///     <inheritdoc cref="JoiningExceptions"/>
        /// </summary>
        private static readonly object SYNC = new object();



        private Log Log { get; }
        private Log LogError { get; }



        private CancellationTokenSource CTS { get; set; }
        private CancellationToken Token => CTS.Token;
        private IClient Client { get; }

        private Task JoiningTask { get; set; }

        /// <summary>
        ///     holds the names of the channels, that we want to join, to be able to ReJoin them after a reconnect
        /// </summary>
        private ISet<string> WantToJoin { get; } = new HashSet<string>();
        /// <summary>
        ///     <see cref="ConcurrentQueue{T}"/> with the channelnames that are taken step by step by the <see cref="JoiningTask"/>
        /// </summary>
        private ConcurrentQueue<string> Joining { get; } = new ConcurrentQueue<string>();
        /// <summary>
        ///     for the following (probably) corner-case:
        ///     <list type="number">
        ///         <item>
        ///             enqueued to <see cref="Joining"/>
        ///         </item>
        ///         <item>
        ///             not <see cref="Joined"/>
        ///         </item>
        ///         <item>
        ///             and <see cref="LeaveChannel(JoinedChannel)"/>/<see cref="LeaveChannel(String)"/> requested
        ///         </item>
        ///     </list>
        /// </summary>
        private ISet<string> JoiningExceptions { get; } = new HashSet<string>();
        /// <summary>
        ///     <see cref="ConcurrentDictionary{TKey, TValue}"/> with the channels that are already joined
        /// </summary>
        private ConcurrentDictionary<string, JoinedChannel> Joined { get; } = new ConcurrentDictionary<string, JoinedChannel>();



        /// <summary>
        ///     <see cref="IReadOnlyList{T}"/> of <see cref="Joined"/>.Values
        /// </summary>
        public IReadOnlyList<JoinedChannel> JoinedChannels => Joined.Values.ToList().AsReadOnly();



        public ChannelManager(IClient client, Log log, Log logError)
        {
            Client = client;
            Log = log;
            LogError = logError;
        }



        public JoinedChannel GetJoinedChannel(string channel)
        {
            if (IsChannelNameNullOrEmptyOrWhitespace(channel)) return null;
            channel = CorrectChannelName(channel);
            bool found = Joined.TryGetValue(channel, out JoinedChannel joinedChannel);
            if (found)
            {
                return joinedChannel;
            }
            return null;
        }

        public void JoinChannel(string channel, bool overrideCheck = false)
        {
            if (IsChannelNameNullOrEmptyOrWhitespace(channel)) return;
            channel = CorrectChannelName(channel);
            // TODO: what is overrideCheck?

            // Check to see if client is already in channel
            if (Joined.Keys.Contains(channel))
                return;
            lock (SYNC)
            {
                bool added = WantToJoin.Add(channel);
                // if the channel we want to join is not added to ChannelsJoinDesired,
                // its already added, so we dont need to enque it join again
                if (added) Joining.Enqueue(channel);
            }
        }

        public void JoinChannels(IEnumerable<string> channels, bool overrideCheck = false)
        {
            if (channels == null) return;
            foreach (string channel in channels)
            {
                if (IsChannelNameNullOrEmptyOrWhitespace(channel))
                {
                    continue;
                }

                JoinChannel(channel, overrideCheck);
            }
        }

        public void LeaveChannel(string channel)
        {
            channel = CorrectChannelName(channel);
            Log($"Leaving channel: {channel}");
            lock (SYNC)
            {
                bool removedFromDesired = WantToJoin.Remove(channel);
                bool removedFromJoined = Joined.TryRemove(channel, out JoinedChannel joinedChannel);
                if (removedFromJoined && joinedChannel != null)
                {
                    // IDE0058 - client raises OnSendFailed if this method returns false
                    Client.Send(Rfc2812.Part($"#{channel}"));
                }
                if (removedFromDesired && !removedFromJoined)
                {
                    // we wanted to join,
                    // but we didnt join yet
                    // and now we want to leave again (so we dont need to join)
                    // thats why we need to SYNC
                    JoiningExceptions.Add(channel);
                }
            }
        }

        public void LeaveChannel(JoinedChannel channel)
        {
            LeaveChannel(channel.Channel);
        }
        public void ReJoinChannels()
        {
            JoinChannels(WantToJoin);
        }
        public void Start()
        {
            if (CTS != null)
            {
                throw new InvalidOperationException($"{nameof(ChannelManager)} should only be started once.");
            }
            CTS = new CancellationTokenSource();
            // we can do this here, cause it has no impact, if there are channels that we want to join, ok, if not, also ok
            ReJoinChannels();
            JoiningTask = Task.Run(JoinChannelTaskAction, Token);
        }
        public void Stop()
        {
            CTS?.Cancel();
            // TODO: wait for JoinChannelTask to stop
            CTS?.Dispose();
            lock (SYNC)
            {
                Joined.Clear();
                JoiningExceptions.Clear();
                while (Joining.TryDequeue(out string channel)) { }
            }
            // has to be done last,
            // that it wont get restarted
            CTS = null;
        }
        private void JoinChannelTaskAction()
        {
            while (Token != null && !Token.IsCancellationRequested)
            {
                Task.Delay(TimeSpan.FromSeconds(1)).GetAwaiter().GetResult();
                string channelToJoin = null;
                lock (SYNC)
                {
                    bool dequeued = Joining.TryDequeue(out channelToJoin);
                    if (!dequeued || channelToJoin == null)
                    {
                        continue;
                    }
                    if (JoiningExceptions.Remove(channelToJoin))
                    {
                        // if the channelToJoin
                        // is within the JoiningExceptions
                        // it got removed
                        // and we dont join that ones
                        continue;
                    }
                }

                Log($"Joining channel: {channelToJoin}");
                // IDE0058 - client raises OnSendFailed if this method returns false
                // important we set channel to lower case when sending join message
                Client.Send(Rfc2812.Join($"#{channelToJoin.ToLower()}"));
                Joined.TryAdd(channelToJoin, new JoinedChannel(channelToJoin));
            }
        }

        private static string CorrectChannelName(string channel)
        {
            string channelName = channel.ToLower();
            if (channelName.StartsWith("#"))
                channelName = channelName.Substring(1);
            return channelName;
        }
        private static bool IsChannelNameNullOrEmptyOrWhitespace(string channel)
        {
            return String.IsNullOrEmpty(channel) || String.IsNullOrWhiteSpace(channel);
        }
    }
}
