using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;

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
            client._raiseEvent("OnBeingHosted", model);
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
            client._raiseEvent("OnChannelStateChanged", model);
        }

        public static void InvokeChatCleared(this TwitchClient client, string channel)
        {
            var model = new OnChatClearedArgs()
            {
                Channel = channel
            };
            client._raiseEvent("OnChatCleared", model);
        }

        public static void InvokeChatCommandsReceived(this TwitchClient client)
        {
            throw new NotImplementedException();
        }

        public static void InvokeConnected(this TwitchClient client, string autoJoinChannel, string botUsername)
        {
            var model = new OnConnectedArgs()
            {
                AutoJoinChannel = autoJoinChannel,
                BotUsername = botUsername
            };
            client._raiseEvent("OnConnected", model);
        }

        public static void InvokeConnectionError(this TwitchClient client, string botUsername, ErrorEvent errorEvent)
        {
            var model = new OnConnectionErrorArgs()
            {
                BotUsername = botUsername,
                Error = errorEvent
            };
            client._raiseEvent("OnConnectionError", model);
        }

        public static void InvokeDisconnected(this TwitchClient client, string botUsername)
        {
            var model = new OnDisconnectedArgs()
            {
                BotUsername = botUsername
            };
            client._raiseEvent("OnDisconnected", model);
        }

        public static void InvokeExistingUsersDetected(this TwitchClient client, string channel, List<string> users)
        {
            var model = new OnExistingUsersDetectedArgs()
            {
                Channel = channel,
                Users = users
            };
            client._raiseEvent("OnExistingUsersDetected", model);
        }

        public static void InvokeOnGiftedSubscription(this TwitchClient client)
        {
            throw new NotImplementedException();
        }

        public static void InvokeOnHostingStarted(this TwitchClient client, string hostingChannel, string targetChannel, int viewers)
        {
            var model = new OnHostingStartedArgs()
            {
                HostingStarted = new HostingStarted(hostingChannel, targetChannel, viewers)
            };
            client._raiseEvent("OnHostingStarted", model);
        }

        public static void InvokeOnHostingStopped(this TwitchClient client, string hostingChannel, int viewers)
        {
            var model = new OnHostingStoppedArgs()
            {
                HostingStopped = new HostingStopped(hostingChannel, viewers)
            };
            client._raiseEvent("OnHostingStoped", model);
        }

        public static void InvokeHostLeft(this TwitchClient client)
        {
            client._raiseEvent("OnHostLeft");
        }

        public static void InvokeIncorrectLogin(this TwitchClient client, Exceptions.ErrorLoggingInException ex)
        {
            var model = new OnIncorrectLoginArgs()
            {
                Exception = ex
            };
            client._raiseEvent("OnIncorrectLogin", model);
        }

        public static void InvokeJoinedChannel(this TwitchClient client, string botUsername, string channel)
        {
            var model = new OnJoinedChannelArgs()
            {
                BotUsername = botUsername,
                Channel = channel
            };
            client._raiseEvent("OnJoinedChannel", model);
        }

        public static void InvokeLeftChannel(this TwitchClient client, string botUsername, string channel)
        {
            var model = new OnLeftChannelArgs()
            {
                BotUsername = botUsername,
                Channel = channel
            };
            client._raiseEvent("OnLeftChannel", model);
        }

        public static void InvokeLog(this TwitchClient client, string botUsername, string data, DateTime dateTime)
        {
            var model = new OnLogArgs()
            {
                BotUsername = botUsername,
                Data = data,
                DateTime = dateTime
            };
            client._raiseEvent("OnLog", model);
        }

        public static void InvokeMessageReceived(this TwitchClient client, string botUsername, string userId, string userName, string displayName, string colorHex,
            Color color, EmoteSet emoteSet, string message, UserType userType, string channel, bool isSubscriber, int subscribedMonthCount, string roomId, bool isTurbo,
            bool isModerator, bool isMe, bool isBroadcaster, Noisy noisy, string rawIrcMessage, string emoteReplacedMessage, List<KeyValuePair<string, string>> badges,
            CheerBadge cheerBadge, int bits, double bitsInDollars)
        {
            var model = new OnMessageReceivedArgs()
            {
                ChatMessage = new ChatMessage(botUsername, userId, userName, displayName, colorHex, color, emoteSet, message, userType, channel, isSubscriber,
                subscribedMonthCount, roomId, isTurbo, isModerator, isMe, isBroadcaster, noisy, rawIrcMessage, emoteReplacedMessage, badges, cheerBadge, bits,
                bitsInDollars)
            };
            client._raiseEvent("OnMessageReceived", model);
        }

        public static void InvokeMessageSent(this TwitchClient client, List<KeyValuePair<string, string>> badges, string channel, string colorHex,
            string displayName, string emoteSet, bool isModerator, bool isSubscriber, UserType userType, string message)
        {
            var model = new OnMessageSentArgs()
            {
                SentMessage = new SentMessage(badges, channel, colorHex, displayName, emoteSet, isModerator, isSubscriber, userType, message)
            };
            client._raiseEvent("OnMessageSent", model);
        }

        public static void InvokeModeratorJoined(this TwitchClient client, string channel, string username)
        {
            var model = new OnModeratorJoinedArgs()
            {
                Channel = channel,
                Username = username
            };
            client._raiseEvent("OnModeratorJoined", model);
        }

        public static void InvokeModeratorLeft(this TwitchClient client, string channel, string username)
        {
            var model = new OnModeratorLeftArgs()
            {
                Channel = channel,
                Username = username
            };
            client._raiseEvent("OnModeratorLeft", model);
        }

        public static void InvokeModeratorsReceived(this TwitchClient client, string channel, List<string> moderators)
        {
            var model = new OnModeratorsReceivedArgs()
            {
                Channel = channel,
                Moderators = moderators
            };
            client._raiseEvent("OnModeratorsReceived", model);
        }

        public static void InvokeNewSubscriber(this TwitchClient client, string channel, string colorHex, string displayName, string emotes,
            string id, string login, bool mod, int msgParamMonths, string msgParamSubPlanName, string roomId, bool subscriber, string systemMsg,
            string tmiSentTs, bool turbo, string userId, UserType userType)
        {
            string userTypeStr = "viewer";
            switch (userType)
            {
                case UserType.Moderator:
                    userTypeStr = "mod";
                    break;
                case UserType.GlobalModerator:
                    userTypeStr = "global_mod";
                    break;
                case UserType.Admin:
                    userTypeStr = "admin";
                    break;
                case UserType.Staff:
                    userTypeStr = "staff";
                    break;
            }

            var irc = new IrcMessage(Enums.Internal.IrcCommand.UserNotice, new string[] { channel }, "", new Dictionary<string, string>()
            {
                { Tags.Badges, "" },
                { Tags.Color, colorHex },
                { Tags.DisplayName, displayName },
                { Tags.Emotes, emotes },
                { Tags.Id, id },
                { Tags.Login, login },
                { Tags.Mod, mod ? "1" : "0" },
                { Tags.MsgParamMonths, msgParamMonths.ToString() },
                { Tags.MsgParamSubPlanName, msgParamSubPlanName },
                { Tags.RoomId, roomId },
                { Tags.Subscriber, subscriber ? "1" : "0" },
                { Tags.SystemMsg, systemMsg },
                { Tags.TmiSentTs, tmiSentTs },
                { Tags.Turbo, turbo ? "1" : "0" },
                { Tags.UserId, userId },
                { Tags.UserType, userTypeStr }
            });
            var model = new OnNewSubscriberArgs()
            {
                Subscriber = new Subscriber(irc)
            };
            client._raiseEvent("OnNewSubscriber", model);
        }

        public static void InvokeNowHosting(this TwitchClient client, string channel, string hostedChannel)
        {
            var model = new OnNowHostingArgs()
            {
                Channel = channel,
                HostedChannel = hostedChannel
            };
            client._raiseEvent("OnNowHosting", model);
        }

        public static void InvokeRaidNotification()
        {
            throw new NotImplementedException();
        }

        public static void InvokeReSubscriber()
        {
            throw new NotImplementedException();
        }

        public static void InvokeSendReceiveData(this TwitchClient client, string data, SendReceiveDirection direction)
        {
            var model = new OnSendReceiveDataArgs()
            {
                Data = data,
                Direction = direction
            };
            client._raiseEvent("OnSendReceiveData", model);
        }

        public static void InvokeUserBanned(this TwitchClient client, string channel, string username, string banReason)
        {
            var model = new OnUserBannedArgs()
            {
                UserBan = new UserBan(channel, username, banReason)
            };
            client._raiseEvent("OnUserBanned", model);
        }

        public static void InvokeUserJoined(this TwitchClient client, string channel, string username)
        {
            var model = new OnUserJoinedArgs()
            {
                Channel = channel,
                Username = username
            };
            client._raiseEvent("OnUserJoined", model);
        }

        public static void InvokeUserLeft(this TwitchClient client, string channel, string username)
        {
            var model = new OnUserLeftArgs()
            {
                Channel = channel,
                Username = username
            };
            client._raiseEvent("OnUserLeft", model);
        }

        public static void InvokeUserStateChanged(this TwitchClient client, List<KeyValuePair<string, string>> badges, string colorHex, string displayName,
            string emoteSet, string channel, bool isSubscriber, bool isModerator, UserType userType)
        {
            var model = new OnUserStateChangedArgs()
            {
                UserState = new UserState(badges, colorHex, displayName, emoteSet, channel, isSubscriber, isModerator, userType)
            };
            client._raiseEvent("OnUserStateChanged", model);
        }

        public static void InvokeUserTimedout(this TwitchClient client, string channel, string username, int timeoutDuration, string timeoutReason)
        {
            var model = new OnUserTimedoutArgs()
            {
                UserTimeout = new UserTimeout(channel, username, timeoutDuration, timeoutReason)
            };
            client._raiseEvent("OnUserTimedout", model);
        }

        public static void InvokeWhisperCommandReceived(this TwitchClient client)
        {
            throw new NotImplementedException();
        }

        public static void InvokeWhisperReceived(this TwitchClient client)
        {
            throw new NotImplementedException();
        }

        public static void InvokeWhisperSent(this TwitchClient client, string username, string receiver, string message)
        {
            var model = new OnWhisperSentArgs()
            {
                Message = message,
                Receiver = receiver,
                Username = username
            };
            client._raiseEvent("OnWhisperSent", model);
        }
    }
}
