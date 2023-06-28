﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Internal;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Interfaces;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;

namespace TwitchLib.Client.Throttling
{
    internal class ThrottlingService
    {
        private readonly IClient _client;
        private readonly ISendOptions _sendOptions;
        private readonly ILogger _logger;
        private readonly ConcurrentQueue<(DateTime, OutboundChatMessage)> _queue = new ConcurrentQueue<(DateTime, OutboundChatMessage)>();
        private readonly Throttler _throttler;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private Task _sendTask;
        
        internal event AsyncEventHandler<OnMessageThrottledArgs> OnThrottled;
        
        internal ThrottlingService(
            IClient client,
            ISendOptions messageSendOptions,
            ILogger logger = null)
        {
            _logger = logger;
            _client = client;
            _sendOptions = messageSendOptions ?? new SendOptions();
            _throttler = new Throttler(_sendOptions);

            _client.OnConnected -= StartThrottler;
            _client.OnConnected += StartThrottler;
            
            _client.OnReconnected -= StartThrottler;
            _client.OnReconnected += StartThrottler;
            
            _client.OnDisconnected -= StopThrottler;
            _client.OnDisconnected += StopThrottler;
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
            if (!_client.IsConnected ||
                _queue.Count >= _sendOptions.QueueCapacity ||
                message == null)
            {
                return false;
            }
            
            _queue.Enqueue((DateTime.UtcNow, message));
            return true;
        }
        
        private Task StartThrottler(object sender, OnConnectedEventArgs args)
        {
            // Cancel old token first
            _tokenSource.Cancel();
            _tokenSource = new CancellationTokenSource();
            _sendTask= Task.Run(async() => await SendTaskActionAsync(), _tokenSource.Token);
            return Task.CompletedTask;
        }

        private Task StopThrottler(object sender, OnDisconnectedEventArgs args)
        {
            _tokenSource.Cancel();
            return Task.CompletedTask;
        }

        private async Task SendTaskActionAsync()
        {
            while (_client.IsConnected && !_tokenSource.Token.IsCancellationRequested)
            {
                await TrySendAsync();
                await Task.Delay(_sendOptions.SendDelay);
            }
        }
        
        private async Task TrySendAsync()
        {
            try
            {
                var taken = _queue.TryDequeue(out var message);
                // 'message == default' should never happen, but if it does, the check will
                // work correctly due to default state of DateTime and OutboundChatMessage being null.
                if (!taken || message == default)
                {
                    return;
                }
                
                // Check CacheItemTimeout
                if (message.Item1.Add(_sendOptions.CacheItemTimeout) < DateTime.UtcNow)
                {
                    return;
                }
                
                // Check for throttling
                //    If the consumer of this API passes zero for SendsAllowedInPeriod
                //    to the ctor of SendOptions
                //    this Sequence-order makes it transparent
                //    cause Throttle raises the corresponding Event with the needed information.
                if (_throttler.ShouldThrottle())
                {
                    await ThrottleMessage(message.Item2);
                    return;
                }
                
                var messageToSend = message.Item2.ToString();
                await _client.SendAsync(messageToSend);
            }
            catch (TaskCanceledException)
            {
                // NOOP
            }
            catch (OperationCanceledException)
            {
                // NOOP
            }
            catch (Exception ex)
            {
                _logger?.LogException(ex.Message, ex);
                await _client.RaiseEvent(nameof(_client.OnError), new OnErrorEventArgs(ex));
            }
        }

        private async Task ThrottleMessage(OutboundChatMessage itemNotSent)
        {
            const string msg = "Message Throttle Occured. Too Many Messages within the period specified in WebsocketClientOptions.";

            var args = new OnMessageThrottledArgs(
                msg,
                itemNotSent,
                _sendOptions.ThrottlingPeriod,
                _sendOptions.SendsAllowedInPeriod);

           if (OnThrottled != null) await OnThrottled?.Invoke(null, args);
        }
    }
}