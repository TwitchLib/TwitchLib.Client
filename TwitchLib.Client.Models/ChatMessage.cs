﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Extensions.NetCore;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    /// <summary>Class represents ChatMessage in a Twitch channel.</summary>
    public class ChatMessage : TwitchLibMessage
    {
        protected readonly MessageEmoteCollection _emoteCollection;
        /// <summary>Information associated with badges. Not all badges will be in this list. Use carefully.</summary>
        public List<KeyValuePair<string, string>> BadgeInfo { get; }
        /// <summary>If viewer sent bits in their message, total amount will be here.</summary>
        public int Bits { get; }
        /// <summary>Number of USD (United States Dollars) spent on bits.</summary>
        public double BitsInDollars { get; }
        /// <summary>Twitch channel message was sent from (useful for multi-channel bots).</summary>
        public string Channel { get; }
        /// <summary>If a cheer badge exists, this property represents the raw value and color (more later). Can be null.</summary>
        public CheerBadge CheerBadge { get; }
        /// <summary>Text after emotes have been handled (if desired). Will be null if replaceEmotes is false.</summary>
        public string EmoteReplacedMessage { get; }
        /// <summary>Unique message identifier assigned by Twitch</summary>
        public string Id { get; }
        /// <summary>Chat message from broadcaster identifier flag</summary>
        public bool IsBroadcaster { get; }
        /// <summary>Chat message /me identifier flag.</summary>
        public bool IsMe { get; }
        /// <summary>Channel specific moderator status.</summary>
        public bool IsModerator { get; }
        /// <summary>Channel specific subscriber status.</summary>
        public bool IsSubscriber { get; }
        /// <summary>Twitch chat message contents.</summary>
        public string Message { get; }
        /// <summary>Experimental property noisy determination by Twitch.</summary>
        public Noisy Noisy { get; } = Noisy.NotSet;
        /// <summary>Raw IRC-style text received from Twitch.</summary>
        public string RawIrcMessage { get; }
        /// <summary>Unique identifier of chat room.</summary>
        public string RoomId { get; }
        /// <summary>Number of months a person has been subbed.</summary>
        public int SubscribedMonthCount { get; }        

        //Example IRC message: @badges=moderator/1,warcraft/alliance;color=;display-name=Swiftyspiffyv4;emotes=;mod=1;room-id=40876073;subscriber=0;turbo=0;user-id=103325214;user-type=mod :swiftyspiffyv4!swiftyspiffyv4@swiftyspiffyv4.tmi.twitch.tv PRIVMSG #swiftyspiffy :asd
        /// <summary>Constructor for ChatMessage object.</summary>
        /// <param name="botUsername">The username of the bot that received the message.</param>
        /// <param name="ircMessage">The IRC message from Twitch to be processed.</param>
        /// <param name="emoteCollection">The <see cref="MessageEmoteCollection"/> to register new emotes on and, if desired, use for emote replacement.</param>
        /// <param name="replaceEmotes">Whether to replace emotes for this chat message. Defaults to false.</param>
        public ChatMessage(string botUsername, IrcMessage ircMessage, ref MessageEmoteCollection emoteCollection, bool replaceEmotes = false)
        {
            BotUsername = botUsername;
            RawIrcMessage = ircMessage.ToString();
            Message = ircMessage.Message;
            _emoteCollection = emoteCollection;

            Username = ircMessage.User;
            Channel = ircMessage.Channel;

            foreach (var tag in ircMessage.Tags.Keys)
            {
                var tagValue = ircMessage.Tags[tag];

                switch (tag)
                {
                    case Tags.Badges:
                        Badges = Common.Helpers.ParseBadges(tagValue);
                        // Iterate through saved badges for special circumstances
                        foreach (var badge in Badges)
                        {
                            switch (badge.Key)
                            {
                                case "bits":
                                    CheerBadge = new CheerBadge(int.Parse(badge.Value));
                                    break;
                                case "subscriber":
                                    SubscribedMonthCount = int.Parse(badge.Value);
                                    break;
                            }
                        }
                        break;
                    case Tags.BadgeInfo:
                        BadgeInfo = Common.Helpers.ParseBadges(tagValue);
                        break;
                    case Tags.Bits:
                        Bits = int.Parse(tagValue);
                        BitsInDollars = ConvertBitsToUsd(Bits);
                        break;
                    case Tags.Color:
                        ColorHex = tagValue;
                        if (!string.IsNullOrWhiteSpace(ColorHex))
                            Color = ColorTranslator.FromHtml(ColorHex);
                        break;
                    case Tags.DisplayName:
                        DisplayName = tagValue;
                        break;
                    case Tags.Emotes:
                        EmoteSet = new EmoteSet(tagValue, Message);
                        break;
                    case Tags.Id:
                        Id = tagValue;
                        break;
                    case Tags.Mod:
                        IsModerator = Common.Helpers.ConvertToBool(tagValue);
                        break;
                    case Tags.Noisy:
                        Noisy = Common.Helpers.ConvertToBool(tagValue) ? Noisy.True : Noisy.False;
                        break;
                    case Tags.RoomId:
                        RoomId = tagValue;
                        break;
                    case Tags.Subscriber:
                        IsSubscriber = Common.Helpers.ConvertToBool(tagValue);
                        break;
                    case Tags.Turbo:
                        IsTurbo = Common.Helpers.ConvertToBool(tagValue);
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

            if (Message.Length > 0 && (byte)Message[0] == 1 && (byte)Message[Message.Length - 1] == 1)
            {
                //Actions (/me {action}) are wrapped by byte=1 and prepended with "ACTION "
                //This setup clears all of that leaving just the action's text.
                //If you want to clear just the nonstandard bytes, use:
                //_message = _message.Substring(1, text.Length-2);
                if (Message.StartsWith("\u0001ACTION ") && Message.EndsWith("\u0001"))
                {
                    Message = Message.Trim('\u0001').Substring(7);
                    IsMe = true;
                }
            }

            //Parse the emoteSet
            if (EmoteSet != null && Message != null && EmoteSet.Emotes.Count > 0)
            {
                var uniqueEmotes = EmoteSet.RawEmoteSetString.Split('/');
                foreach (var emote in uniqueEmotes)
                {
                    var firstColon = emote.IndexOf(':');
                    var firstComma = emote.IndexOf(',');
                    if (firstComma == -1) firstComma = emote.Length;
                    var firstDash = emote.IndexOf('-');
                    if (firstColon > 0 && firstDash > firstColon && firstComma > firstDash)
                    {
                        if (int.TryParse(emote.Substring(firstColon + 1, firstDash - firstColon - 1), out var low) &&
                            int.TryParse(emote.Substring(firstDash + 1, firstComma - firstDash - 1), out var high))
                        {
                            if (low >= 0 && low < high && high < Message.Length)
                            {
                                //Valid emote, let's parse
                                var id = emote.Substring(0, firstColon);
                                //Pull the emote text from the message
                                var text = Message.Substring(low, high - low + 1);
                                _emoteCollection.Add(new MessageEmote(id, text));
                            }
                        }
                    }
                }
                if (replaceEmotes)
                {
                    EmoteReplacedMessage = _emoteCollection.ReplaceEmotes(Message);
                }
            }

            if (EmoteSet == null)
                EmoteSet = new EmoteSet(null, Message);

            // Check if display name was set, and if it wasn't, set it to username
            if (string.IsNullOrEmpty(DisplayName))
                DisplayName = Username;

            // Check if message is from broadcaster
            if (string.Equals(Channel, Username, StringComparison.InvariantCultureIgnoreCase))
            {
                UserType = UserType.Broadcaster;
                IsBroadcaster = true;
            }

            if (Channel.Split(':').Length == 3)
            {
                if (string.Equals(Channel.Split(':')[1], UserId, StringComparison.InvariantCultureIgnoreCase))
                {
                    UserType = UserType.Broadcaster;
                    IsBroadcaster = true;
                }
            }
        }

        public ChatMessage(string botUsername, string userId, string userName, string displayName, string colorHex, Color color, EmoteSet emoteSet,
            string message, UserType userType, string channel, string id, bool isSubscriber, int subscribedMonthCount, string roomId, bool isTurbo, bool isModerator,
            bool isMe, bool isBroadcaster, Noisy noisy, string rawIrcMessage, string emoteReplacedMessage, List<KeyValuePair<string, string>> badges,
            CheerBadge cheerBadge, int bits, double bitsInDollars)
        {
            BotUsername = botUsername;
            UserId = userId;
            DisplayName = displayName;
            ColorHex = colorHex;
            Color = color;
            EmoteSet = emoteSet;
            Message = message;
            UserType = userType;
            Channel = channel;
            Id = id;
            IsSubscriber = isSubscriber;
            SubscribedMonthCount = subscribedMonthCount;
            RoomId = roomId;
            IsTurbo = isTurbo;
            IsModerator = isModerator;
            IsMe = isMe;
            IsBroadcaster = isBroadcaster;
            Noisy = Noisy;
            RawIrcMessage = rawIrcMessage;
            EmoteReplacedMessage = emoteReplacedMessage;
            Badges = badges;
            CheerBadge = cheerBadge;
            Bits = bits;
            BitsInDollars = bitsInDollars;
        }

        private static double ConvertBitsToUsd(int bits)
        {
            /*
            Conversion Rates
            100 bits = $1.40
            500 bits = $7.00
            1500 bits = $19.95 (5%)
            5000 bits = $64.40 (8%)
            10000 bits = $126.00 (10%)
            25000 bits = $308.00 (12%)
            */
            if (bits < 1500)
            {
                return (double)bits / 100 * 1.4;
            }
            if (bits < 5000)
            {
                return (double)bits / 1500 * 19.95;
            }
            if (bits < 10000)
            {
                return (double)bits / 5000 * 64.40;
            }
            if (bits < 25000)
            {
                return (double)bits / 10000 * 126;
            }
            return (double)bits / 25000 * 308;
        }
    }
}
