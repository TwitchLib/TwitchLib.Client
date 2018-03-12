﻿using TwitchLib.Client.Common;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class RaidNotification
    {
        public string Badges { get; }
        public string Color { get; }
        public string DisplayName { get; }
        public string Emotes { get; }
        public string Id { get; }
        public string Login { get; }
        public bool Moderator { get; }
        public string MsgId { get; }
        public string MsgParamDisplayName { get; }
        public string MsgParamLogin { get; }
        public string MsgParamViewerCount { get; }
        public string RoomId { get; }
        public bool Subscriber { get; }
        public string SystemMsg { get; }
        public string SystemMsgParsed { get; }
        public string TmiSentTs { get; }
        public bool Turbo { get; }
        public string UserId { get; }
        public UserType UserType { get; }

        // @badges=;color=#FF0000;display-name=Heinki;emotes=;id=4fb7ab2d-aa2c-4886-a286-46e20443f3d6;login=heinki;mod=0;msg-id=raid;msg-param-displayName=Heinki;msg-param-login=heinki;msg-param-viewerCount=4;room-id=27229958;subscriber=0;system-msg=4\sraiders\sfrom\sHeinki\shave\sjoined\n!;tmi-sent-ts=1510249711023;turbo=0;user-id=44110799;user-type= :tmi.twitch.tv USERNOTICE #pandablack
        public RaidNotification(IrcMessage ircMessage)
        {
            foreach (var tag in ircMessage.Tags.Keys)
            {
                var tagValue = ircMessage.Tags[tag];

                switch (tag)
                {
                    case Tags.Badges:
                        Badges = tagValue;
                        break;
                    case Tags.Color:
                        Color = tagValue;
                        break;
                    case Tags.DisplayName:
                        DisplayName = tagValue;
                        break;
                    case Tags.Emotes:
                        Emotes = tagValue;
                        break;
                    case Tags.Login:
                        Login = tagValue;
                        break;
                    case Tags.Mod:
                        Moderator = Helpers.ConvertToBool(tagValue);
                        break;
                    case Tags.MsgId:
                        MsgId = tagValue;
                        break;
                    case Tags.MsgParamDisplayname:
                        MsgParamDisplayName = tagValue;
                        break;
                    case Tags.MsgParamLogin:
                        MsgParamLogin = tagValue;
                        break;
                    case Tags.MsgParamViewerCount:
                        MsgParamViewerCount = tagValue;
                        break;
                    case Tags.RoomId:
                        RoomId = tagValue;
                        break;
                    case Tags.Subscriber:
                        Subscriber = Helpers.ConvertToBool(tagValue);
                        break;
                    case Tags.SystemMsg:
                        SystemMsg = tagValue;
                        SystemMsgParsed = tagValue.Replace("\\s", " ").Replace("\\n", "");
                        break;
                    case Tags.TmiSentTs:
                        TmiSentTs = tagValue;
                        break;
                    case Tags.Turbo:
                        Turbo = Helpers.ConvertToBool(tagValue);
                        break;
                    case Tags.UserId:
                        UserId = tagValue;
                        break;
                    case Tags.UserType:
                        switch (tagValue)
                        {
                            case "mod":
                                UserType = UserType.Moderator;
                                break;
                            case "global_mod":
                                UserType = UserType.GlobalModerator;
                                break;
                            case "admin":
                                UserType = UserType.Admin;
                                break;
                            case "staff":
                                UserType = UserType.Staff;
                                break;
                            default:
                                UserType = UserType.Viewer;
                                break;
                        }
                        break;
                }
            }
        }
    }
}

