﻿using System;
using TwitchLib.Client.Common;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    /// <summary>Class representing a channel state as received from Twitch chat.</summary>
    public class ChannelState
    {
        /// <summary>Property representing whether R9K is being applied to chat or not. WILL BE NULL IF VALUE NOT PRESENT.</summary>
        public bool? R9K { get; }
        /// <summary>Property representing whether Rituals is enabled or not. WILL BE NULL IF VALUE NOT PRESENT.</summary>
        public bool? Rituals { get; }
        /// <summary>Property representing whether Sub Mode is being applied to chat or not. WILL BE NULL IF VALUE NOT PRESENT.</summary>
        public bool? SubOnly { get; }
        /// <summary>Property representing whether Slow mode is being applied to chat or not. WILL BE NULL IF VALUE NOT PRESENT.</summary>
        public bool? SlowMode { get; }
        /// <summary>Property representing whether EmoteOnly mode is being applied to chat or not. WILL BE NULL IF VALUE NOT PRESENT.</summary>
        public bool? EmoteOnly { get; }
        /// <summary>Property representing the current broadcaster language.</summary>
        public string BroadcasterLanguage { get; }
        /// <summary>Property representing the current channel.</summary>
        public string Channel { get; }
        /// <summary>Property representing how long needed to be following to talk </summary>
        public TimeSpan FollowersOnly { get; }
        /// <summary>Property representing mercury value. Not sure what it's for.</summary>
        public bool Mercury { get; }
        /// <summary>Twitch assignedc room id</summary>
        public string RoomId { get; }

        /// <summary>ChannelState object constructor.</summary>
        public ChannelState(IrcMessage ircMessage)
        {
            //@broadcaster-lang=;emote-only=0;r9k=0;slow=0;subs-only=1 :tmi.twitch.tv ROOMSTATE #burkeblack
            foreach(var tag in ircMessage.Tags.Keys)
            {
                var tagValue = ircMessage.Tags[tag];

                switch(tag)
                {
                    case Tags.BroadcasterLang:
                        BroadcasterLanguage = tagValue;
                        break;
                    case Tags.EmoteOnly:
                        EmoteOnly = Helpers.ConvertToBool(tagValue);
                        break;
                    case Tags.R9K:
                        R9K = Helpers.ConvertToBool(tagValue);
                        break;
                    case Tags.Rituals:
                        Rituals = Helpers.ConvertToBool(tagValue);
                        break;
                    case Tags.Slow:
                        SlowMode = Helpers.ConvertToBool(tagValue);
                        break;
                    case Tags.SubsOnly:
                        SubOnly = Helpers.ConvertToBool(tagValue);
                        break;
                    case Tags.FollowersOnly:
                        var minutes = int.Parse(tagValue);
                        FollowersOnly = TimeSpan.FromMinutes(minutes == -1 ? 0 : minutes);
                        break;
                    case Tags.RoomId:
                        RoomId = tagValue;
                        break;
                    case Tags.Mercury:
                        Mercury = Helpers.ConvertToBool(tagValue);
                        break;
                    default:
                        Console.WriteLine("[TwitchLib][ChannelState] Unaccounted for: " + tag);
                        break;
                }
            }
            Channel = ircMessage.Channel;
        }
    }
}