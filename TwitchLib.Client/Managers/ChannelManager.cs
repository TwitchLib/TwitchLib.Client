using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using TwitchLib.Client.Delegates;
using TwitchLib.Client.Extensions.Internal;
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


        private readonly ILogger LOGGER;


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
        ///     <see cref="ConcurrentQueue{T}"/> with the channelnames that are going to be joined step by step by the <see cref="JoiningTask"/>
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
        ///     channels we want to join and we already sent a message to twitch
        /// </summary>
        private ISet<string> JoinRequested { get; } = new HashSet<string>();
        /// <summary>
        ///     <see cref="ConcurrentDictionary{TKey, TValue}"/> with the channels that are already joined
        /// </summary>
        private ConcurrentDictionary<string, JoinedChannel> Joined { get; } = new ConcurrentDictionary<string, JoinedChannel>();
        /// <summary>
        ///     <see cref="IReadOnlyCollection{T}"/> with channel-names to join when connected
        ///     <br></br>
        ///     <br></br>
        ///     the presence of this <see cref="IReadOnlyCollection{T}"/> <b>does not</b> indicate they are already joined!
        /// </summary>
        public IReadOnlyCollection<string> AutoJoinChannels => WantToJoin.ToList().AsReadOnly();
        /// <summary>
        ///     <see cref="IReadOnlyList{T}"/> of <see cref="Joined"/>.Values
        /// </summary>
        public IReadOnlyList<JoinedChannel> JoinedChannels => Joined.Values.ToList().AsReadOnly();
        public IReadOnlyCollection<string> JoiningChannels { get { lock (SYNC) { return Joining.ToList().AsReadOnly(); } } }
        public IReadOnlyCollection<string> JoiningChannelsExceptions { get { lock (SYNC) { return JoiningExceptions.ToList().AsReadOnly(); } } }
        public IReadOnlyCollection<string> JoinChannelRequested { get { lock (SYNC) { return JoinRequested.ToList().AsReadOnly(); } } }
        public TimeSpan JoinRequestDelay => TimeSpan.FromSeconds(1);

        public ChannelManager(IClient client, Log log, Log logError, ILogger logger = null)
        {
            Client = client;
            Log = log;
            LogError = logError;
            LOGGER = logger;
        }



        public JoinedChannel GetJoinedChannel(string channel)
        {
            LOGGER?.TraceMethodCall(GetType());
            if (channel.IsNullOrEmptyOrWhitespace()) return null;
            channel = CorrectChannelName(channel);
            // no sync is needed, its a cuncurrent dictionary
            bool found = Joined.TryGetValue(channel, out JoinedChannel joinedChannel);
            if (found)
            {
                return joinedChannel;
            }
            return null;
        }
        public void JoinChannel(string channel)
        {
            LOGGER?.TraceMethodCall(GetType());
            if (channel.IsNullOrEmptyOrWhitespace()) return;
            channel = CorrectChannelName(channel);

            lock (SYNC)
            {
                // Check to see if client is already in channel
                if (Joined.Keys.Contains(channel)) return;
                if (JoiningExceptions.Contains(channel)) return;
                if (JoinRequested.Contains(channel)) return;
                // no contains check needed, its a HashSet<>
                WantToJoin.Add(channel);
                if (!Joining.Contains(channel))
                {
                    Joining.Enqueue(channel);
                }
                // now the task does its work
                // it takes the channel from Joining
                // requests to join
                // puts the channel into JoinRequested
                // afterwards JoinComplete has to be called to complete the join and to put the channel into Joined
            }
        }
        public void JoinChannels(IEnumerable<string> channels)
        {
            LOGGER?.TraceMethodCall(GetType());
            if (channels == null) return;
            foreach (string channel in channels)
            {
                JoinChannel(channel);
            }
        }
        public void LeaveChannel(string channel)
        {
            LOGGER?.TraceMethodCall(GetType());
            if (channel.IsNullOrEmptyOrWhitespace()) return;
            channel = CorrectChannelName(channel);
            Log?.Invoke($"Leaving channel: {channel}");
            lock (SYNC)
            {
                // never ever touch the joining-queue here

                bool removedFromJoined = Joined.TryRemove(channel, out JoinedChannel joinedChannel);
                if (removedFromJoined && joinedChannel != null)
                {
                    // if its already joined: send PART
                    // IDE0058 - client raises OnSendFailed if this method returns false
                    Client.Send(Rfc2812.Part($"#{channel}"));
                    // we dont want to join it anymore
                    WantToJoin.Remove(channel);
                    // we are fine
                    return;
                }
                if (JoinRequested.Contains(channel))
                {
                    // join already requested
                    // but we want to leave again
                    // we wanted to join,
                    // but we didnt join yet
                    // and now we want to leave again (so we dont need to join)
                    JoiningExceptions.Add(channel);
                    // dont remove from requested!
                    // gets handled on JoinCompleted in combination with JoiningExceptions!
                }
            }
        }
        public void LeaveChannel(JoinedChannel channel)
        {
            LOGGER?.TraceMethodCall(GetType());
            LeaveChannel(channel.Channel);
        }
        public void ReJoinChannels()
        {
            LOGGER?.TraceMethodCall(GetType());
            JoinChannels(WantToJoin);
        }
        public void Start()
        {
            LOGGER?.TraceMethodCall(GetType());
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
            LOGGER?.TraceMethodCall(GetType());
            CTS?.Cancel();
            JoiningTask?.GetAwaiter().GetResult();
            CTS?.Dispose();
            lock (SYNC)
            {
                JoiningExceptions.Clear();
                JoinRequested.Clear();
                while (Joining.TryDequeue(out string channel)) { }
                Joined.Clear();
            }
            // has to be done last,
            // that it wont get restarted
            CTS = null;
        }
        public void JoinCompleted(string channel)
        {
            LOGGER?.TraceMethodCall(GetType());
            if (Token == null || Token.IsCancellationRequested) return;
            if (channel.IsNullOrEmptyOrWhitespace()) return;
            channel = CorrectChannelName(channel);
            lock (SYNC)
            {
                JoinRequested.Remove(channel);
                Joined.TryAdd(channel, new JoinedChannel(channel));
                // ChannelManager requested JOIN
                // meanwhile we want to leave
                //
                // leave may not break the process and has to wait until joining is completed
                //
                // channel joining is now completed
                // we have to issue LeaveChannel to send PART
                if (JoiningExceptions.Contains(channel)) LeaveChannel(channel);
            }
        }
        public void JoinCanceld(string channel)
        {
            LOGGER?.TraceMethodCall(GetType());
            if (Token == null || Token.IsCancellationRequested) return;
            if (channel.IsNullOrEmptyOrWhitespace()) return;
            channel = CorrectChannelName(channel);
            lock (SYNC) { JoinRequested.Remove(channel); }
        }
        private void JoinChannelTaskAction()
        {
            LOGGER?.TraceMethodCall(GetType());
            while (Token != null && !Token.IsCancellationRequested)
            {
                Task.Delay(JoinRequestDelay).GetAwaiter().GetResult();
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
                    Log?.Invoke($"Joining channel: {channelToJoin}");
                    // IDE0058 - client raises OnSendFailed if this method returns false
                    // important we set channel to lower case when sending join message
                    Client.Send(Rfc2812.Join($"#{channelToJoin.ToLower()}"));
                    JoinRequested.Add(channelToJoin);
                }
            }
        }
        private static string CorrectChannelName(string channel)
        {
            string channelName = channel.ToLower().Trim();
            if (channelName.StartsWith("#"))
                channelName = channelName.Substring(1);
            return channelName;
        }
    }
}
