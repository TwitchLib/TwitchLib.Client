using System;

using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions.Internal;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace TwitchLib.Client
{
    public partial class TwitchClient : ITwitchClient_MessageSending
    {

        // TraceMethodCall should use the Type of the interface,
        // that this class extends;
        // it makes it easier to find the respective occurance from the log file

        #region events public
        public event EventHandler<OnMessageSentArgs> OnMessageSent;
        public event EventHandler<OnMessageThrottledEventArgs> OnMessageThrottled;
        #endregion events public


        #region methods public
        public void SendRaw(string message)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_MessageSending));
            Log($"Writing: {message}");
            // IDE0058 - client raises OnSendFailed if this method returns false
            Client.Send(message);
            OnSendReceiveData?.Invoke(this, new OnSendReceiveDataArgs { Direction = Enums.SendReceiveDirection.Sent, Data = message });
        }
        public void SendQueuedItem(string message)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_MessageSending));
            // IDE0058 - client raises OnSendFailed if this method returns false
            Client.Send(message);
        }
        public void SendMessage(JoinedChannel channel, string message, bool dryRun = false)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_MessageSending));
            SendTwitchMessage(channel, message, null, dryRun);
        }
        public void SendMessage(string channel, string message, bool dryRun = false)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_MessageSending));
            SendMessage(GetJoinedChannel(channel), message, dryRun);
        }
        public void SendReply(JoinedChannel channel, string replyToId, string message, bool dryRun = false)
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
            string message = "PONG";
            Log($"Writing: {message}");
            // IDE0058 - client raises OnSendFailed if this method returns false
            Client.SendPONG();
            OnSendReceiveData?.Invoke(this, new OnSendReceiveDataArgs { Direction = Enums.SendReceiveDirection.Sent, Data = message });
        }
        private void SendTwitchMessage(JoinedChannel channel, string message, string replyToId = null, bool dryRun = false)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_MessageSending));
            if (channel == null || message == null || dryRun) return;
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
            // IDE0058 - client raises OnSendFailed if this method returns false
            Client.Send(twitchMessage.ToString());
        }
        #endregion methods private
    }
}
