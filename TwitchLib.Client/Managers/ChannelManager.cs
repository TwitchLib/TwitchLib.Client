using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using TwitchLib.Client.Delegates;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions.Internal;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Internal;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Interfaces;

namespace TwitchLib.Client.Managers
{
    /// <summary>
    ///     <see langword="class"/> to manage Channels
    ///     <br></br>
    ///     <list type="number">
    ///         <item>
    ///             <see cref="JoinChannel(String)"/>/<see cref="JoinChannels(IEnumerable{String})"/>
    ///             <br></br>
    ///             add the channel to <see cref="WantToJoin"/>
    ///             <br></br>
    ///             and to <see cref="Joining"/>
    ///         </item>
    ///         <item>
    ///             <see cref="JoinChannelTaskAction"/> takes and removes each channel
    ///             <br></br>
    ///             step by step
    ///             <br></br>
    ///             delayed by <see cref="JoinRequestDelay"/>
    ///             <br></br>
    ///             from <see cref="Joining"/>,
    ///             <br></br>
    ///             sends a join-irc-message
    ///             <br></br>
    ///             and puts it into <see cref="JoinRequested"/>
    ///         </item>
    ///         <item>
    ///             by receiving <see cref="Enums.IrcCommand.Join"/>
    ///             <br></br>
    ///             and its 'us' who joins the specific channel
    ///             <br></br>
    ///             the specific channel gets removed from <see cref="JoinRequested"/>
    ///             <br></br>
    ///             and is put into <see cref="Joined"/> (<see cref="JoinedChannels"/>...) as <see cref="JoinedChannel"/>
    ///         </item>
    ///         <item>
    ///             by receiving <see cref="Enums.IrcCommand.Notice"/> with the 'msg-id'-tag <see cref="Models.Internal.MsgIds.MsgChannelSuspended"/>
    ///             <br></br>
    ///             the specific channel gets removed from <see cref="JoinRequested"/>
    ///         </item>
    ///     </list>
    ///     all of that is event-driven
    ///     <br></br>
    ///     <see cref="Subscribe(ITwitchClient)"/>
    /// </summary>
    [SuppressMessage("Style", "IDE0058")]
    internal class ChannelManager : IChannelManageAble
    {
        /// <summary>
        ///     <inheritdoc cref="JoiningExceptions"/>
        /// </summary>
        private static readonly object SYNC = new object();

        #region properties private

        #region logging
        private ILogger LOGGER { get; }
        private Log Log { get; }
        private Log LogError { get; }
        #endregion logging


        private CancellationTokenSource CTS { get; set; }
        private CancellationToken Token => CTS.Token;
        public ConnectionCredentials Credentials { get; set; }
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
        #endregion properties private


        #region properties public
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
        /// <summary>
        ///     <see href="https://dev.twitch.tv/docs/irc/#rate-limits"/>
        ///     <br></br>
        ///     20 joins per 10 seconds would result in 1 join per 500 milliseconds
        ///     <br></br>
        ///     we add 100 milliseconds to be sure, rate-limit isnt hit
        /// </summary>
        public TimeSpan JoinRequestDelay => TimeSpan.FromMilliseconds(600);
        /// <summary>
        ///     delay the whole <see cref="JoinChannelTaskAction"/> for one second
        /// </summary>
        public TimeSpan JoinTaskDelay => TimeSpan.FromSeconds(1);
        #endregion properties public


        #region ctor
        public ChannelManager(IClient client, Log log, Log logError, ILogger logger = null)
        {
            Client = client;
            Log = log;
            LogError = logError;
            LOGGER = logger;
        }
        #endregion ctor


        #region methods public

        #region methods visible to API consumers
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
        #endregion methods visible to API consumers


        #region subscribe to ITwitchClient-Events
        /// <summary>
        ///     subsscribes
        ///     <list type="bullet">
        ///         <item>
        ///             <see cref="ITwitchClient.OnConnected"/> to get this instance started
        ///         </item>
        ///         <item>
        ///             <see cref="ITwitchClient.OnReconnected"/> to get this instance started
        ///         </item>
        ///         <item>
        ///             <see cref="ITwitchClient.OnDisconnected"/> to get this instance stopped
        ///         </item>
        ///         <item>
        ///             <see cref="ITwitchClient.OnJoinedChannel"/> to get this instance add the joined channel to <see cref="JoinedChannels"/> and do some other work
        ///         </item>
        ///         <item>
        ///             <see cref="ITwitchClient.OnFailureToReceiveJoinConfirmation"/> to get this instance do some work
        ///         </item>
        ///     </list>
        /// </summary>
        /// <param name="twitchClient">
        ///     <see cref="ITwitchClient"/>
        /// </param>
        public void Subscribe(ITwitchClient twitchClient)
        {
            twitchClient.OnConnected += TwitchClientOnConnected;
            twitchClient.OnReconnected += TwitchClientOnConnected;
            twitchClient.OnDisconnected += TwitchClientOnDisconnected;
            twitchClient.OnJoinedChannel += TwitchClientOnJoinedChannel;
            twitchClient.OnFailureToReceiveJoinConfirmation += TwitchClientOnFailureToReceiveJoinConfirmation;
        }
        #endregion subscribe to ITwitchClient-Events


        #region methods that should only be used for testing-purposes and by ITwitchClient-EventHandlers
        /// <summary>
        ///     should only be used for testing-purposes
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     if <see cref="ChannelManager"/> is already started
        /// </exception>
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
        /// <summary>
        ///     should only be used for testing-purposes 
        /// </summary>
        public void Stop()
        {
            LOGGER?.TraceMethodCall(GetType());
            CTS?.Cancel();
            // we just want to wait for the task to cancel/complete/run to completion/whatever...
            try { JoiningTask?.GetAwaiter().GetResult(); } catch { }
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
                Joined.TryAdd(channel, new JoinedChannel(channel, Credentials.TwitchUsername));
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
        #endregion methods that should only be used for testing-purposes and by ITwitchClient-EventHandlers

        #endregion methods public


        #region methods private
        private void JoinChannelTaskAction()
        {
            LOGGER?.TraceMethodCall(GetType());
            try
            {
                // lets wait a second before joining...
                Task.Delay(JoinTaskDelay).GetAwaiter().GetResult();
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
            catch (Exception e)
            {
                LOGGER?.LogExceptionAsInformation(GetType(), e);
                if (!typeof(TaskCanceledException).IsAssignableFrom(e.GetType()))
                {
                    LogError?.Invoke(e.ToString());
                    LOGGER?.LogExceptionAsError(GetType(), e);
                }
            }
        }
        private void ReJoinChannels()
        {
            LOGGER?.TraceMethodCall(GetType());
            JoinChannels(WantToJoin);
        }

        #region ITwitchClient-EventHandlers
        private void TwitchClientOnConnected(object sender, OnConnectedArgs e) { Start(); }
        private void TwitchClientOnDisconnected(object sender, OnDisconnectedArgs e) { Stop(); }
        private void TwitchClientOnJoinedChannel(object sender, OnJoinedChannelArgs e) { JoinCompleted(e.Channel); }
        private void TwitchClientOnFailureToReceiveJoinConfirmation(object sender, OnFailureToReceiveJoinConfirmationArgs e) { JoinCanceld(e.Channel); }
        #endregion ITwitchClient-EventHandlers


        private static string CorrectChannelName(string channel)
        {
            string channelName = channel.ToLower().Trim();
            if (channelName.StartsWith("#"))
                channelName = channelName.Substring(1);
            return channelName;
        }
        #endregion methods private

    }
}
