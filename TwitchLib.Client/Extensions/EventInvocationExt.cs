using System;
using System.Collections.Generic;
using System.Drawing;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Extensions
{
    public static class EventInvocationExt
    {
        public static void InvokeOnBeingHosted(this TwitchClient client, string channel, string botUsername, string hostedByChannel, int viewers, bool isAutoHosted)
        {
            var model = new OnBeingHostedArgs()
            {
                BeingHostedNotification = new BeingHostedNotification(channel, botUsername, hostedByChannel, viewers, isAutoHosted)
            };
            client.RaiseEvent("OnBeingHosted", model);
        }

        public static void InvokeChannelStateChanged(this TwitchClient client, string channel, bool r9k, bool rituals,
            bool subOnly, int slowMode, bool emoteOnly, string broadcasterLanguage, TimeSpan followersOnly, bool mercury, string roomId)
        {
            var state = new ChannelState(r9k, rituals, subOnly, slowMode, emoteOnly, broadcasterLanguage, channel, followersOnly, mercury, roomId);
            var model = new OnChannelStateChangedArgs()
            {
                Channel = channel,
                ChannelState = state
            };
            client.RaiseEvent("OnChannelStateChanged", model);
        }

        public static void InvokeChatCleared(this TwitchClient client, string channel)
        {
            var model = new OnChatClearedArgs()
            {
                Channel = channel
            };
            client.RaiseEvent("OnChatCleared", model);
        }

        public static void InvokeChatCommandsReceived(this TwitchClient client, string botUsername, string userId, string userName, string displayName,
            string colorHex, Color color, EmoteSet emoteSet, string message, UserType userType, string channel, string id, bool isSubscriber, int subscribedMonthCount,
            string roomId, bool isTurbo, bool isModerator, bool isMe, bool isBroadcaster, Noisy noisy, string rawIrcMessage, string emoteReplacedMessage,
            List<KeyValuePair<string, string>> badges, CheerBadge cheerBadge, int bits, double bitsInDollars, string commandText, string argumentsAsString,
            List<string> argumentsAsList, char commandIdentifier)
        {
            var msg = new ChatMessage(botUsername, userId, userName, displayName, colorHex, color, emoteSet, message, userType, channel, id,
                isSubscriber, subscribedMonthCount, roomId, isTurbo, isModerator, isMe, isBroadcaster, noisy, rawIrcMessage, emoteReplacedMessage,
                badges, cheerBadge, bits, bitsInDollars);
            var model = new OnChatCommandReceivedArgs()
            {
                Command = new ChatCommand(msg, commandText, argumentsAsString, argumentsAsList, commandIdentifier)
            };
            client.RaiseEvent("OnChatCommandReceived", model);
        }

        public static void InvokeConnected(this TwitchClient client, string autoJoinChannel, string botUsername)
        {
            var model = new OnConnectedArgs()
            {
                AutoJoinChannel = autoJoinChannel,
                BotUsername = botUsername
            };
            client.RaiseEvent("OnConnected", model);
        }

        public static void InvokeConnectionError(this TwitchClient client, string botUsername, ErrorEvent errorEvent)
        {
            var model = new OnConnectionErrorArgs()
            {
                BotUsername = botUsername,
                Error = errorEvent
            };
            client.RaiseEvent("OnConnectionError", model);
        }

        public static void InvokeDisconnected(this TwitchClient client, string botUsername)
        {
            var model = new OnDisconnectedArgs()
            {
                BotUsername = botUsername
            };
            client.RaiseEvent("OnDisconnected", model);
        }

        public static void InvokeExistingUsersDetected(this TwitchClient client, string channel, List<string> users)
        {
            var model = new OnExistingUsersDetectedArgs()
            {
                Channel = channel,
                Users = users
            };
            client.RaiseEvent("OnExistingUsersDetected", model);
        }

        public static void InvokeGiftedSubscription(this TwitchClient client, string badges, string color, string displayName, string emotes, string id, string login, bool isModerator,
            string msgId, string msgParamMonths, string msgParamRecipientDisplayName, string msgParamRecipientId, string msgParamRecipientUserName,
            string msgParamSubPlanName, SubscriptionPlan msgParamSubPlan, string roomId, bool isSubscriber, string systemMsg, string systemMsgParsed,
            string tmiSentTs, bool isTurbo, UserType userType)
        {
            var model = new OnGiftedSubscriptionArgs()
            {
                GiftedSubscription = new GiftedSubscription(badges, color, displayName, emotes, id, login, isModerator, msgId, msgParamMonths, msgParamRecipientDisplayName,
                msgParamRecipientId, msgParamRecipientUserName, msgParamSubPlanName, msgParamSubPlan, roomId, isSubscriber, systemMsg, systemMsgParsed, tmiSentTs, isTurbo,
                userType)
            };
            client.RaiseEvent("OnGiftedSubscription", model);
        }

        public static void InvokeAnonGiftedSubscription(this TwitchClient client, string badges, string color, string displayName, string emotes, string id, string login, bool isModerator,
            string msgId, string msgParamMonths, string msgParamRecipientDisplayName, string msgParamRecipientId, string msgParamRecipientUserName,
            string msgParamSubPlanName, SubscriptionPlan msgParamSubPlan, string roomId, bool isSubscriber, string systemMsg, string systemMsgParsed,
            string tmiSentTs, bool isTurbo, UserType userType)
        {
            var model = new OnAnonGiftedSubscriptionArgs()
            {
                AnonGiftedSubscription = new AnonGiftedSubscription(badges, color, displayName, emotes, id, login, isModerator, msgId, msgParamMonths, msgParamRecipientDisplayName,
                msgParamRecipientId, msgParamRecipientUserName, msgParamSubPlanName, msgParamSubPlan, roomId, isSubscriber, systemMsg, systemMsgParsed, tmiSentTs, isTurbo,
                userType)
            };
            client.RaiseEvent("OnAnonGiftedSubscription", model);
        }

        public static void InvokeOnHostingStarted(this TwitchClient client, string hostingChannel, string targetChannel, int viewers)
        {
            var model = new OnHostingStartedArgs()
            {
                HostingStarted = new HostingStarted(hostingChannel, targetChannel, viewers)
            };
            client.RaiseEvent("OnHostingStarted", model);
        }

        public static void InvokeOnHostingStopped(this TwitchClient client, string hostingChannel, int viewers)
        {
            var model = new OnHostingStoppedArgs()
            {
                HostingStopped = new HostingStopped(hostingChannel, viewers)
            };
            client.RaiseEvent("OnHostingStopped", model);
        }

        public static void InvokeHostLeft(this TwitchClient client)
        {
            client.RaiseEvent("OnHostLeft");
        }

        public static void InvokeIncorrectLogin(this TwitchClient client, Exceptions.ErrorLoggingInException ex)
        {
            var model = new OnIncorrectLoginArgs()
            {
                Exception = ex
            };
            client.RaiseEvent("OnIncorrectLogin", model);
        }

        public static void InvokeJoinedChannel(this TwitchClient client, string botUsername, string channel)
        {
            var model = new OnJoinedChannelArgs()
            {
                BotUsername = botUsername,
                Channel = channel
            };
            client.RaiseEvent("OnJoinedChannel", model);
        }

        public static void InvokeLeftChannel(this TwitchClient client, string botUsername, string channel)
        {
            var model = new OnLeftChannelArgs()
            {
                BotUsername = botUsername,
                Channel = channel
            };
            client.RaiseEvent("OnLeftChannel", model);
        }

        public static void InvokeLog(this TwitchClient client, string botUsername, string data, DateTime dateTime)
        {
            var model = new OnLogArgs()
            {
                BotUsername = botUsername,
                Data = data,
                DateTime = dateTime
            };
            client.RaiseEvent("OnLog", model);
        }

        public static void InvokeMessageReceived(this TwitchClient client, string botUsername, string userId, string userName, string displayName, string colorHex,
            Color color, EmoteSet emoteSet, string message, UserType userType, string channel, string id, bool isSubscriber, int subscribedMonthCount, string roomId, bool isTurbo,
            bool isModerator, bool isMe, bool isBroadcaster, Noisy noisy, string rawIrcMessage, string emoteReplacedMessage, List<KeyValuePair<string, string>> badges,
            CheerBadge cheerBadge, int bits, double bitsInDollars)
        {
            var model = new OnMessageReceivedArgs()
            {
                ChatMessage = new ChatMessage(botUsername, userId, userName, displayName, colorHex, color, emoteSet, message, userType, channel, id, isSubscriber,
                subscribedMonthCount, roomId, isTurbo, isModerator, isMe, isBroadcaster, noisy, rawIrcMessage, emoteReplacedMessage, badges, cheerBadge, bits,
                bitsInDollars)
            };
            client.RaiseEvent("OnMessageReceived", model);
        }

        public static void InvokeMessageSent(this TwitchClient client, List<KeyValuePair<string, string>> badges, string channel, string colorHex,
            string displayName, string emoteSet, bool isModerator, bool isSubscriber, UserType userType, string message)
        {
            var model = new OnMessageSentArgs()
            {
                SentMessage = new SentMessage(badges, channel, colorHex, displayName, emoteSet, isModerator, isSubscriber, userType, message)
            };
            client.RaiseEvent("OnMessageSent", model);
        }

        public static void InvokeModeratorJoined(this TwitchClient client, string channel, string username)
        {
            var model = new OnModeratorJoinedArgs()
            {
                Channel = channel,
                Username = username
            };
            client.RaiseEvent("OnModeratorJoined", model);
        }

        public static void InvokeModeratorLeft(this TwitchClient client, string channel, string username)
        {
            var model = new OnModeratorLeftArgs()
            {
                Channel = channel,
                Username = username
            };
            client.RaiseEvent("OnModeratorLeft", model);
        }

        public static void InvokeModeratorsReceived(this TwitchClient client, string channel, List<string> moderators)
        {
            var model = new OnModeratorsReceivedArgs()
            {
                Channel = channel,
                Moderators = moderators
            };
            client.RaiseEvent("OnModeratorsReceived", model);
        }

        public static void InvokeNewSubscriber(this TwitchClient client, List<KeyValuePair<string, string>> badges, string colorHex, Color color, string displayName,
            string emoteSet, string id, string login, string systemMessage, string systemMessageParsed, string resubMessage, SubscriptionPlan subscriptionPlan,
            string subscriptionPlanName, string roomId, string userId, bool isModerator, bool isTurbo, bool isSubscriber, bool isPartner, string tmiSentTs,
            UserType userType, string rawIrc, string channel)
        {
            var model = new OnNewSubscriberArgs()
            {
                Subscriber = new Subscriber(badges, colorHex, color, displayName, emoteSet, id, login, systemMessage, systemMessageParsed, resubMessage,
                subscriptionPlan, subscriptionPlanName, roomId, userId, isModerator, isTurbo, isSubscriber, isPartner, tmiSentTs, userType, rawIrc, channel)
            };
            client.RaiseEvent("OnNewSubscriber", model);
        }

        public static void InvokeNowHosting(this TwitchClient client, string channel, string hostedChannel)
        {
            var model = new OnNowHostingArgs()
            {
                Channel = channel,
                HostedChannel = hostedChannel
            };
            client.RaiseEvent("OnNowHosting", model);
        }

        public static void InvokeRaidNotification(this TwitchClient client, string channel, string badges, string color, string displayName, string emotes, string id, string login, bool moderator, string msgId, string msgParamDisplayName,
            string msgParamLogin, string msgParamViewerCount, string roomId, bool subscriber, string systemMsg, string systemMsgParsed, string tmiSentTs, bool turbo, UserType userType)
        {
            var model = new OnRaidNotificationArgs()
            {
                Channel = channel,
                RaidNotificaiton = new RaidNotification(badges, color, displayName, emotes, id, login, moderator, msgId, msgParamDisplayName, msgParamLogin, msgParamViewerCount,
                roomId, subscriber, systemMsg, systemMsgParsed, tmiSentTs, turbo, userType)
            };
            client.RaiseEvent("OnRaidNotification", model);
        }

        public static void InvokeReSubscriber(this TwitchClient client, List<KeyValuePair<string, string>> badges, string colorHex, Color color, string displayName,
            string emoteSet, string id, string login, string systemMessage, string systemMessageParsed, string resubMessage, SubscriptionPlan subscriptionPlan,
            string subscriptionPlanName, string roomId, string userId, bool isModerator, bool isTurbo, bool isSubscriber, bool isPartner, string tmiSentTs,
            UserType userType, string rawIrc, string channel)
        {
            var model = new OnReSubscriberArgs()
            {
                ReSubscriber = new ReSubscriber(badges, colorHex, color, displayName, emoteSet, id, login, systemMessage, systemMessageParsed, resubMessage,
                subscriptionPlan, subscriptionPlanName, roomId, userId, isModerator, isTurbo, isSubscriber, isPartner, tmiSentTs, userType, rawIrc, channel)
            };
            client.RaiseEvent("OnReSubscriber", model);
        }

        public static void InvokeSendReceiveData(this TwitchClient client, string data, SendReceiveDirection direction)
        {
            var model = new OnSendReceiveDataArgs()
            {
                Data = data,
                Direction = direction
            };
            client.RaiseEvent("OnSendReceiveData", model);
        }

        public static void InvokeUserBanned(this TwitchClient client, string channel, string username, string banReason)
        {
            var model = new OnUserBannedArgs()
            {
                UserBan = new UserBan(channel, username, banReason)
            };
            client.RaiseEvent("OnUserBanned", model);
        }

        public static void InvokeUserJoined(this TwitchClient client, string channel, string username)
        {
            var model = new OnUserJoinedArgs()
            {
                Channel = channel,
                Username = username
            };
            client.RaiseEvent("OnUserJoined", model);
        }

        public static void InvokeUserLeft(this TwitchClient client, string channel, string username)
        {
            var model = new OnUserLeftArgs()
            {
                Channel = channel,
                Username = username
            };
            client.RaiseEvent("OnUserLeft", model);
        }

        public static void InvokeUserStateChanged(this TwitchClient client, List<KeyValuePair<string, string>> badges, string colorHex, string displayName,
            string emoteSet, string channel, bool isSubscriber, bool isModerator, UserType userType)
        {
            var model = new OnUserStateChangedArgs()
            {
                UserState = new UserState(badges, colorHex, displayName, emoteSet, channel, isSubscriber, isModerator, userType)
            };
            client.RaiseEvent("OnUserStateChanged", model);
        }

        public static void InvokeUserTimedout(this TwitchClient client, string channel, string username, int timeoutDuration, string timeoutReason)
        {
            var model = new OnUserTimedoutArgs()
            {
                UserTimeout = new UserTimeout(channel, username, timeoutDuration, timeoutReason)
            };
            client.RaiseEvent("OnUserTimedout", model);
        }

        public static void InvokeWhisperCommandReceived(this TwitchClient client, List<KeyValuePair<string, string>> badges, string colorHex, Color color, string username, string displayName, EmoteSet emoteSet, string threadId, string messageId,
            string userId, bool isTurbo, string botUsername, string message, UserType userType, string commandText, string argumentsAsString, List<string> argumentsAsList, char commandIdentifier)
        {
            var whisperMsg = new WhisperMessage(badges, colorHex, color, username, displayName, emoteSet, threadId, messageId, userId, isTurbo, botUsername, message, userType);
            var model = new OnWhisperCommandReceivedArgs()
            {
                Command = new WhisperCommand(whisperMsg, commandText, argumentsAsString, argumentsAsList, commandIdentifier)
            };
            client.RaiseEvent("OnWhisperCommandReceived", model);
        }

        public static void InvokeWhisperReceived(this TwitchClient client, List<KeyValuePair<string, string>> badges, string colorHex, Color color, string username, string displayName, EmoteSet emoteSet, string threadId, string messageId,
            string userId, bool isTurbo, string botUsername, string message, UserType userType)
        {
            var model = new OnWhisperReceivedArgs()
            {
                WhisperMessage = new WhisperMessage(badges, colorHex, color, username, displayName, emoteSet, threadId, messageId, userId, isTurbo, botUsername, message, userType)
            };
            client.RaiseEvent("OnWhisperReceived", model);
        }

        public static void InvokeWhisperSent(this TwitchClient client, string username, string receiver, string message)
        {
            var model = new OnWhisperSentArgs()
            {
                Message = message,
                Receiver = receiver,
                Username = username
            };
            client.RaiseEvent("OnWhisperSent", model);
        }
    }
}
