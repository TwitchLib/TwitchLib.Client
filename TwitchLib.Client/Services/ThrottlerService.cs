﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions.Internal;
using TwitchLib.Client.Helpers;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Interfaces;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Extensions;
using TwitchLib.Communication.Interfaces;

namespace TwitchLib.Client.Services
{

    internal class ThrottlerService
    {
        #region variables private
        private long sentMessageCount = 0;
        #endregion variables private


        #region properties private
        private ILogger LOGGER { get; }
        private ConcurrentQueue<Tuple<DateTime, OutboundChatMessage>> Queue { get; } = new ConcurrentQueue<Tuple<DateTime, OutboundChatMessage>>();
        private IClient Client { get; }
        private CancellationTokenSource TokenSource { get; set; }
        private CancellationToken Token => TokenSource.Token;
        private Timer ResetThrottlingWindowTimer { get; set; }
        private ISendOptions SendOptions { get; }
        private TwitchClient TwitchClient { get; }

        /// <summary>
        ///     get is never used, cause the <see cref="Task"/> is canceled by the <see cref="Token"/>
        /// </summary>
        private Task SendTask { get; set; }
        #endregion properties private


        #region ctors
        internal ThrottlerService(IClient client,
                                  TwitchClient twitchClient,
                                  ISendOptions messageSendOptions,
                                  ILogger logger = null)
        {
            LOGGER = logger;
            TwitchClient = twitchClient;
            Client = client;
            SendOptions = messageSendOptions;
        }
        #endregion ctors


        #region methods internal
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
        ///     </list>
        /// </summary>
        /// <param name="twitchClient">
        ///     <see cref="ITwitchClient"/>
        /// </param>
        internal void Subscribe(ITwitchClient twitchClient)
        {
            twitchClient.OnConnected += Start;
            twitchClient.OnReconnected += Start;
            twitchClient.OnDisconnected += Stop;
        }
        /// <summary>
        ///     enqueues the given <paramref name="message"/>
        /// </summary>
        /// <param name="message">
        ///     <see cref="OutboundChatMessage"/>
        /// </param>
        /// <returns>
        ///     <see langword="true"/>, if its enqueued
        ///     <br></br>
        ///     <see langword="false"/> otherwise
        /// </returns>
        internal bool Enqueue(OutboundChatMessage message)
        {
            LOGGER?.TraceMethodCall(GetType());
            try
            {
                if (!Client.IsConnected || Queue.Count >= SendOptions.QueueCapacity)
                    return false;
                Queue.Enqueue(new Tuple<DateTime, OutboundChatMessage>(DateTime.UtcNow, message));
                return true;
            }
            catch (Exception ex)
            {
                LOGGER?.LogExceptionAsError(GetType(), ex);
                RaiseEventHelper.RaiseEvent(TwitchClient, nameof(TwitchClient.OnError), new OnErrorEventArgs() { Exception = ex });
                throw;
            }
        }
        /// <summary>
        ///     should only be used for testing-purposes and within <see cref="Subscribe(ITwitchClient)"/>
        /// </summary>
        /// <param name="sender">
        ///     unused, maybe <see langword="null"/>
        /// </param>
        /// <param name="args">
        ///     unused, maybe <see langword="null"/>
        /// </param>
        internal void Start(object sender, OnConnectedArgs args)
        {
            LOGGER?.TraceMethodCall(GetType());
            StartResetWindowTimer();
            if (TokenSource != null) LOGGER.LogError("{type} should only be started once", GetType().Name);
            StartSendTask();
        }
        /// <summary>
        ///     should only be used for testing-purposes and within <see cref="Subscribe(ITwitchClient)"/>
        /// </summary>
        /// <param name="sender">
        ///     unused, maybe <see langword="null"/>
        /// </param>
        /// <param name="args">
        ///     unused, maybe <see langword="null"/>
        /// </param>
        internal void Stop(object sender, OnDisconnectedArgs args)
        {
            LOGGER?.TraceMethodCall(GetType());
            TokenSource?.Cancel();
            TokenSource = null;
            ResetThrottlingWindowTimer?.Dispose();
        }
        #endregion methods internal


        #region methods private
        private void StartResetWindowTimer()
        {
            LOGGER?.TraceMethodCall(GetType());
            ResetThrottlingWindowTimer = new Timer(ResetCallback,
                                                   null,
                                                   TimeSpan.FromSeconds(0),
                                                   SendOptions.ThrottlingPeriod);
        }
        [SuppressMessage("Style", "IDE0058")]
        private void ResetCallback(object state)
        {
            LOGGER?.TraceMethodCall(GetType());
            Interlocked.Exchange(ref sentMessageCount, 0);
        }
        private void StartSendTask()
        {
            LOGGER?.TraceMethodCall(GetType());
            TokenSource = new CancellationTokenSource();
            SendTask = Task.Run(SendTaskAction, Token);
        }
        private void SendTaskAction()
        {
            LOGGER?.TraceMethodCall(GetType());
            while (Client.IsConnected && !Token.IsCancellationRequested)
            {
                TrySend();
                Task.Delay(SendOptions.SendDelay).GetAwaiter().GetResult();
            }
        }
        private void TrySend()
        {
            long localSentCount = ReadSentCount();
            try
            {
                // Sequence: always try to dequeue first
                bool taken = Queue.TryDequeue(out Tuple<DateTime, OutboundChatMessage> messageTupel);
                if (!taken || messageTupel == null)
                    return;
                // Sequence: now check CacheItemTimeout
                if (messageTupel.Item1.Add(SendOptions.CacheItemTimeout) < DateTime.UtcNow)
                    return;
                // Sequence: now check for throttling
                //           if the consumer of this API passes zero for SendsAllowedInPeriod
                //           to the ctor of SendOptions
                //           this Sequence-order makes it transparent
                //           cause Throttle raises the corresponding Event with the needed information
                if (localSentCount >= SendOptions.SendsAllowedInPeriod)
                {
                    Throttle(messageTupel?.Item2,
                             localSentCount);
                    return;
                }

                Client.Send(messageTupel.Item2.ToString());

                IncrementSentCount();
            }
            catch (Exception ex) when (ex.GetType() == typeof(TaskCanceledException) || ex.GetType() == typeof(OperationCanceledException))
            {
                // occurs if the Tasks are canceled by the CancelationTokenSource.Token
                LOGGER?.LogExceptionAsInformation(GetType(), ex);
            }
            catch (Exception ex)
            {
                LOGGER?.LogExceptionAsError(GetType(), ex);
                // msg may be null
                RaiseEventHelper.RaiseEvent(TwitchClient, nameof(TwitchClient.OnError), new OnErrorEventArgs() { Exception = ex });
            }
        }
        private long ReadSentCount()
        {
            return Interlocked.Read(ref sentMessageCount);
        }
        [SuppressMessage("Style", "IDE0058")]
        private void IncrementSentCount()
        {
            Interlocked.Increment(ref sentMessageCount);
        }
        private void Throttle(OutboundChatMessage itemNotSent,
                              long sentCount)
        {
            LOGGER?.TraceMethodCall(GetType());
            LOGGER?.TraceAction(GetType(), "Message throttled");
            string msg = $"Message Throttle Occured. Too Many Messages within the period specified in WebsocketClientOptions.";
            OnMessageThrottledArgs args = new OnMessageThrottledArgs
            {
                ItemNotSent = itemNotSent,
                Reason = msg,
                AllowedInPeriod = SendOptions.SendsAllowedInPeriod,
                Period = SendOptions.ThrottlingPeriod,
                SentCount = sentCount
            };
            RaiseEventHelper.RaiseEvent(TwitchClient, nameof(TwitchClient.OnMessageThrottled), args);
        }
        #endregion methods private
    }
}