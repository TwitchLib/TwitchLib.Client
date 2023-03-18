using System;

using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions.Internal;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Extensions;

namespace TwitchLib.Client
{
    public partial class TwitchClient : ITwitchClient_MessageSending
    {

        // TraceMethodCall should use the Type of the interface,
        // that this class extends;
        // it makes it easier to find the respective occurance from the log file

        public event EventHandler<OnMessageSentArgs> OnMessageSent;
        public event EventHandler<OnMessageThrottledEventArgs> OnMessageThrottled;

        public void SendRaw(string message)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_MessageSending));
            if (!IsInitialized) HandleNotInitialized();

            Log($"Writing: {message}");
            // IDE0058 - client raises OnSendFailed if this method returns false
            Client.Send(message);
            OnSendReceiveData?.Invoke(this, new OnSendReceiveDataArgs { Direction = Enums.SendReceiveDirection.Sent, Data = message });
        }
        public void SendQueuedItem(string message)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_MessageSending));
            if (!IsInitialized) HandleNotInitialized();
            // IDE0058 - client raises OnSendFailed if this method returns false
            Client.Send(message);
        }
        private void SendPONG()
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_MessageSending));
            if (!IsInitialized) HandleNotInitialized();
            string message = "PONG";
            Log($"Writing: {message}");
            // IDE0058 - client raises OnSendFailed if this method returns false
            Client.SendPONG();
            OnSendReceiveData?.Invoke(this, new OnSendReceiveDataArgs { Direction = Enums.SendReceiveDirection.Sent, Data = message });
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

        private void SendTwitchMessage(JoinedChannel channel, string message, string replyToId = null, bool dryRun = false)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_MessageSending));
            if (!IsInitialized) HandleNotInitialized();
            if (channel == null || message == null || dryRun) return;
            if (message.Length > 500)
            {
                LogError("Message length has exceeded the maximum character count. (500)");
                return;
            }

            OutboundChatMessage twitchMessage = new OutboundChatMessage
            {
                Channel = channel.Channel,
                Username = ConnectionCredentials.TwitchUsername,
                Message = message
            };
            if (replyToId != null)
            {
                twitchMessage.ReplyToId = replyToId;
            }
            // TODO: here is an error, but first i have to check out the UserState.:
            // take a look at the other location of this TODOs text
            //LastMessageSent = message;

            // IDE0058 - client raises OnSendFailed if this method returns false
            Client.Send(twitchMessage.ToString());
        }
    }
}
