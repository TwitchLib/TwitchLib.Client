#if NETSTANDARD
    using TwitchLib.Client.Extensions.NetCore;
#endif
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
#if NET452

#endif

namespace TwitchLib.Client.Models
{
    /// <summary>Class represents ChatMessage in a Twitch channel.</summary>
    public class ChatMessage
    {
        private readonly MessageEmoteCollection _emoteCollection;

        /// <summary>Twitch username of the bot that received the message.</summary>
        public string BotUsername { get; protected set; }
        /// <summary>Twitch-unique integer assigned on per account basis.</summary>
        public string UserId { get; protected set; }
        /// <summary>Username of sender of chat message.</summary>
        public string Username { get; protected set; }
        /// <summary>Case-sensitive username of sender of chat message.</summary>
        public string DisplayName { get; protected set; }
        /// <summary>Hex representation of username color in chat (THIS CAN BE NULL IF VIEWER HASN'T SET COLOR).</summary>
        public string ColorHex { get; protected set; }
        /// <summary>Property representing HEX color as a System.Drawing.Color object.</summary>
        public System.Drawing.Color Color { get; protected set; }
        /// <summary>Emote Ids that exist in message.</summary>
        public EmoteSet EmoteSet { get; protected set; }
        /// <summary>Twitch chat message contents.</summary>
        public string Message { get; protected set; }
        /// <summary>User type can be viewer, moderator, global mod, admin, or staff</summary>
        public Enums.UserType UserType { get; protected set; }
        /// <summary>Twitch channel message was sent from (useful for multi-channel bots).</summary>
        public string Channel { get; protected set; }
        /// <summary>Channel specific subscriber status.</summary>
        public bool IsSubscriber { get; protected set; }
        /// <summary>Number of months a person has been subbed.</summary>
        public int SubscribedMonthCount { get; protected set; }
        /// <summary>Unique identifier of chat room.</summary>
        public string RoomId { get; protected set; }
        /// <summary>Twitch site-wide turbo status.</summary>
        public bool IsTurbo { get; protected set; }
        /// <summary>Channel specific moderator status.</summary>
        public bool IsModerator { get; protected set; }
        /// <summary>Chat message /me identifier flag.</summary>
        public bool IsMe { get; protected set; }
        /// <summary>Chat message from broadcaster identifier flag</summary>
        public bool IsBroadcaster { get; protected set; }
        /// <summary>Experimental property noisy determination by Twitch.</summary>
        public Enums.Noisy Noisy { get; protected set; } = Enums.Noisy.NotSet;
        /// <summary>Raw IRC-style text received from Twitch.</summary>
        public string RawIrcMessage { get; protected set; }
        /// <summary>Text after emotes have been handled (if desired). Will be null if replaceEmotes is false.</summary>
        public string EmoteReplacedMessage { get; protected set; }
        /// <summary>List of key-value pair badges.</summary>
        public List<KeyValuePair<string, string>> Badges { get; protected set; }
        /// <summary>If a cheer badge exists, this property represents the raw value and color (more later). Can be null.</summary>
        public CheerBadge CheerBadge { get; protected set; }
        /// <summary>If viewer sent bits in their message, total amount will be here.</summary>
        public int Bits { get; protected set; }
        /// <summary>Number of USD (United States Dollars) spent on bits.</summary>
        public double BitsInDollars { get; protected set; }

        private readonly string _emoteSetStorage;

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
            EmoteSet = new EmoteSet(_emoteSetStorage, Message);

            Username = ircMessage.User;
            Channel = ircMessage.Channel.Remove(0, 1);

            var successUserType = ircMessage.Tags.TryGetValue("user-type", out var userTypeStr);
            if (successUserType)
            {
                switch (userTypeStr)
                {
                    case "mod":
                        UserType = Enums.UserType.Moderator;
                        break;
                    case "global_mod":
                        UserType = Enums.UserType.GlobalModerator;
                        break;
                    case "admin":
                        UserType = Enums.UserType.Admin;
                        break;
                    case "staff":
                        UserType = Enums.UserType.Staff;
                        break;
                    default:
                        UserType = Enums.UserType.Viewer;
                        break;
                }
            }

            var successBadges = ircMessage.Tags.TryGetValue("badges", out var badges);
            if (successBadges)
            {
                Badges = new List<KeyValuePair<string, string>>();
                if (badges.Contains('/'))
                {
                    if (!badges.Contains(","))
                        Badges.Add(new KeyValuePair<string, string>(badges.Split('/')[0], badges.Split('/')[1]));
                    else
                        foreach (var badge in badges.Split(','))
                            Badges.Add(new KeyValuePair<string, string>(badge.Split('/')[0], badge.Split('/')[1]));
                }

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
            }

            var successBits = ircMessage.Tags.TryGetValue("bits", out var bits);
            if (successBits)
            {
                Bits = int.Parse(bits);
                BitsInDollars = ConvertBitsToUsd(Bits);
            }

            var successColor = ircMessage.Tags.TryGetValue("color", out var color);
            if (successColor)
            {
                if (string.IsNullOrWhiteSpace(ColorHex))
                    ColorHex = color;
                if (!string.IsNullOrWhiteSpace(ColorHex))
                    Color = ColorTranslator.FromHtml(ColorHex);
            }

            var successDisplayName = ircMessage.Tags.TryGetValue("display-name", out var displayname);
            if (successDisplayName)
            {
                DisplayName = displayname;
            }

            var successEmotes = ircMessage.Tags.TryGetValue("emotes", out var emotes);
            if (successEmotes)
            {
                if (string.IsNullOrWhiteSpace(_emoteSetStorage))
                    _emoteSetStorage = emotes;
            }

            var successSubscriber = ircMessage.Tags.TryGetValue("subscriber", out var subscriber);
            if (successSubscriber)
            {
                IsSubscriber = ConvertToBool(subscriber);
            }

            var successTurbo = ircMessage.Tags.TryGetValue("turbo", out var turbo);
            if (successTurbo)
            {
                IsTurbo = ConvertToBool(turbo);
            }

            var successUserId = ircMessage.Tags.TryGetValue("user-id", out var userId);
            if (successUserId)
            {
                UserId = userId;
            }

            var successModerator = ircMessage.Tags.TryGetValue("mod", out var moderator);
            if (successModerator)
            {
                IsModerator = ConvertToBool(moderator);
            }

            var successRoomId = ircMessage.Tags.TryGetValue("room-id", out var roomId);
            if (successRoomId)
            {
                RoomId = roomId;
            }

            var successNoisy = ircMessage.Tags.TryGetValue("noisy", out var noisy);
            if (successNoisy)
            {
                Noisy = ConvertToBool(noisy) ? Enums.Noisy.True : Enums.Noisy.False;
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

            // Check if display name was set, and if it wasn't, set it to username
            if (string.IsNullOrEmpty(DisplayName))
                DisplayName = Username;

            // Check if message is from broadcaster
            if (string.Equals(Channel, Username, StringComparison.InvariantCultureIgnoreCase))
            {
                UserType = Enums.UserType.Broadcaster;
                IsBroadcaster = true;
            }

            if (Channel.Split(':').Length == 3)
            {
                if (string.Equals(Channel.Split(':')[1], UserId, StringComparison.InvariantCultureIgnoreCase))
                {
                    UserType = Enums.UserType.Broadcaster;
                    IsBroadcaster = true;
                }
            }
        }

        /// <summary>Chat Message constructor with passed in values.</summary>
        public ChatMessage(List<KeyValuePair<string, string>> badges, string channel, string colorHex, string displayName,
            EmoteSet emoteSet, bool moderator, bool subscriber, Enums.UserType userType, string message)
        {
            Badges = badges;
            Channel = channel;
            ColorHex = colorHex;
            Username = DisplayName = displayName;
            EmoteSet = emoteSet;
            IsModerator = moderator;
            IsSubscriber = subscriber;
            UserType = userType;
            Message = message;
        }

        private static bool ConvertToBool(string data)
        {
            return data == "1";
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
