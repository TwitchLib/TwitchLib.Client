using System;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions.Internal;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Internal;
using TwitchLib.Client.Models;
using TwitchLib.Client.Services;
using TwitchLib.Communication.Events;

namespace TwitchLib.Client
{
    public partial class TwitchClient : ITwitchClient_MessageSending
    {

        // TraceMethodCall should use the Type of the interface,
        // that this class extends;
        // it makes it easier to find the respective occurance from the log file
        #region properties private
        private ThrottlerService ThrottlerService { get; }
        #endregion properties private


        #region events public
        public event EventHandler<OnMessageSentArgs>? OnMessageSent;
        public event EventHandler<OnSendFailedEventArgs>? OnSendFailed;
        public event EventHandler<OnMessageThrottledArgs>? OnMessageThrottled;
        #endregion events public


        #region methods public
        public void SendRaw(string message)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_MessageSending));
            Log($"Sending raw: '{message}'");
            bool sent = Client.Send(message);
            // only if its really sent
            // IClient raises OnSendFailed otherwise
            if (sent) OnSendReceiveData?.Invoke(this, new OnSendReceiveDataArgs(SendReceiveDirection.Sent, message));
        }
        public void SendMessage(JoinedChannel? channel, string message, bool dryRun = false)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_MessageSending));
            SendTwitchMessage(channel, message, null, dryRun);
        }
        public void SendMessage(string channel, string message, bool dryRun = false)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_MessageSending));
            SendMessage(GetJoinedChannel(channel), message, dryRun);
        }
        public void SendReply(JoinedChannel? channel, string replyToId, string message, bool dryRun = false)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_MessageSending));
            SendTwitchMessage(channel, message, replyToId, dryRun);
        }
        public void SendReply(string channel, string replyToId, string message, bool dryRun = false)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_MessageSending));
            SendReply(GetJoinedChannel(channel), replyToId, message, dryRun);
        }
        #endregion methods public


        #region methods private
        private void SendPONG()
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_MessageSending));
            string message = Rfc2812.Pong(":tmi.twitch.tv");
            SendRaw(message);
        }
        private void SendTwitchMessage(JoinedChannel? channel, string message, string? replyToId = null, bool dryRun = false)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_MessageSending));
            if (channel == null || message == null) return;
            if (message.Length > 500)
            {
                LogError("Message length has exceeded the maximum character count. (500)");
                return;
            }

            OutboundChatMessage twitchMessage = new OutboundChatMessage
            {
                Channel = channel.Channel,
                Username = TwitchUsername,
                Message = message
            };
            if (replyToId != null)
            {
                twitchMessage.ReplyToId = replyToId;
            }
            if (dryRun) return;
            ThrottlerService.Enqueue(twitchMessage);
            // ThrottlerService invokes this instances SendRaw(string message)
            // which then raises OnSendReceiveData,
            // if the message seems to be sent
        }
        #endregion methods private
    }
}
