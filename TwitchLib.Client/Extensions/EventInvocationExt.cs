using System;
using System.Collections.Generic;
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

        public static void InvokeNewSubscriber(this TwitchClient client, string channel, string colorHex, string displayName, string emotes, 
            string id, string login, bool mod, int msgParamMonths, string msgParamSubPlanName, string roomId, bool subscriber, string systemMsg,
            string tmiSentTs, bool turbo, string userId, UserType userType)
        {
            string userTypeStr = "viewer";
            switch(userType)
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
    }
}
