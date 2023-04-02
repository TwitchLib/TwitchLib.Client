using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using TwitchLib.Client.Enums;
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
        #region properties private
        private ILogger LOGGER { get; }
        private ConcurrentQueue<Tuple<DateTime, OutboundChatMessage>> Queue { get; } = new ConcurrentQueue<Tuple<DateTime, OutboundChatMessage>>();
        private IClient Client { get; }
        private CancellationTokenSource TokenSource { get; set; }
        private CancellationToken Token => TokenSource.Token;
        private ISendOptions SendOptions { get; }
        private ITwitchClient TwitchClient { get; }
        /// <summary>
        ///     get is never used, cause the <see cref="Task"/> is canceled by the <see cref="Token"/>
        /// </summary>
        private Task SendTask { get; set; }
        private Throttler Throttler { get; }
        #endregion properties private


        #region ctors
        internal ThrottlerService(IClient client,
                                  ITwitchClient twitchClient,
                                  ISendOptions messageSendOptions,
                                  ILogger logger = null)
        {
            LOGGER = logger;
            TwitchClient = twitchClient;
            Client = client;
            SendOptions = messageSendOptions;
            Throttler = new Throttler(SendOptions);
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
            if (!Client.IsConnected
                || Queue.Count >= SendOptions.QueueCapacity
                || message == null)
            {
                return false;
            }
            Queue.Enqueue(new Tuple<DateTime, OutboundChatMessage>(DateTime.UtcNow, message));
            return true;

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
            if (TokenSource != null)
            {
                string message = String.Format("{0} should only be started once", GetType().Name);
                LOGGER?.LogError(message);
                RaiseEventHelper.RaiseEvent(TwitchClient, nameof(TwitchClient.OnError), new OnErrorEventArgs() { Exception = new InvalidOperationException(message) });
                return;
            }
            TokenSource = new CancellationTokenSource();
            SendTask = Task.Run(SendTaskAction, Token);
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
        }
        #endregion methods internal


        #region methods private
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
            try
            {
                // Sequence: always try to dequeue first
                bool taken = Queue.TryDequeue(out Tuple<DateTime, OutboundChatMessage> messageTupel);
                if (!taken || messageTupel == null)
                {
                    return;
                }
                // Sequence: now check CacheItemTimeout
                if (messageTupel.Item1.Add(SendOptions.CacheItemTimeout) < DateTime.UtcNow)
                {
                    return;
                }
                // Sequence: now check for throttling
                //           if the consumer of this API passes zero for SendsAllowedInPeriod
                //           to the ctor of SendOptions
                //           this Sequence-order makes it transparent
                //           cause Throttle raises the corresponding Event with the needed information
                if (Throttler.Throttle())
                {
                    Throttle(messageTupel?.Item2);
                    return;
                }
                string messageToSend = messageTupel.Item2.ToString();
                bool sent = Client.Send(messageToSend);
                // only if it seems to be sent
                // IClient raises the corresponding error and send-failed event if its not the case
                if (sent)
                {
                    OnSendReceiveDataArgs args = new OnSendReceiveDataArgs()
                    {
                        Direction = SendReceiveDirection.Sent,
                        Data = messageToSend
                    };
                    RaiseEventHelper.RaiseEvent(TwitchClient, nameof(TwitchClient.OnSendReceiveData), args);
                }
            }
            catch (Exception ex) when (ex.GetType() == typeof(TaskCanceledException) || ex.GetType() == typeof(OperationCanceledException))
            {
                // occurs if the Tasks are canceled by the CancelationTokenSource.Token
                LOGGER?.LogExceptionAsInformation(GetType(), ex);
            }
            catch (Exception ex)
            {
                LOGGER?.LogExceptionAsError(GetType(), ex);
                RaiseEventHelper.RaiseEvent(TwitchClient, nameof(TwitchClient.OnError), new OnErrorEventArgs() { Exception = ex });
            }
        }
        private void Throttle(OutboundChatMessage itemNotSent)
        {
            LOGGER?.TraceMethodCall(GetType());
            LOGGER?.TraceAction(GetType(), "Message throttled");
            string msg = $"Message Throttle Occured. Too Many Messages within the period specified in WebsocketClientOptions.";
            OnMessageThrottledArgs args = new OnMessageThrottledArgs
            {
                ItemNotSent = itemNotSent,
                Reason = msg,
                AllowedInPeriod = SendOptions.SendsAllowedInPeriod,
                Period = SendOptions.ThrottlingPeriod
            };
            RaiseEventHelper.RaiseEvent(TwitchClient, nameof(TwitchClient.OnMessageThrottled), args);
        }
        #endregion methods private
    }
}