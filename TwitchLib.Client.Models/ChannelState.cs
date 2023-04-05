using System;

using Microsoft.Extensions.Logging;

using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    /// <summary>
    ///     Class representing a channel state as received from Twitch chat.
    ///     <br></br>
    ///     <br></br>
    ///     <see href="https://dev.twitch.tv/docs/irc/tags/#roomstate-tags"/>
    ///     <br></br>
    ///     <br></br>
    ///     <b>2nd paragraph:</b>
    ///     <br></br>
    ///     For JOIN messages, the message contains all chat room setting tags,
    ///     <br></br>
    ///     but for actions that change a single chat room setting,
    ///     <br></br>
    ///     the message includes only that chat room setting tag.
    ///     <br></br>
    ///     For example,
    ///     <br></br>
    ///     if the moderator turned on unique chat,
    ///     <br></br>
    ///     the message includes only the r9k tag.
    ///     <br></br>
    ///     <br></br>
    ///     <b>after joining a channel,</b>
    ///     <br></br>
    ///     ctor <see cref="ChannelState(IrcMessage, ILogger)"/> should be used
    ///     <br></br>
    ///     <b>afterwards,</b>
    ///     <br></br>
    ///     we only receive changes, so <see cref="Apply(IrcMessage, ILogger)"/> should be used
    /// </summary>
    public class ChannelState
    {
        #region properties public

        #region wont change within lifecycle
        /// <summary>
        ///     Property representing the current channel.
        /// </summary>
        public string Channel { get; }
        /// <summary>
        ///     Twitch assigned room id
        /// </summary>
        public string? RoomId { get; }
        #endregion wont change within lifecycle


        /// <summary>
        ///     Property representing the current broadcaster language.
        /// </summary>
        public string? BroadcasterLanguage { get; private set; }
        /// <summary>
        ///     Property representing whether EmoteOnly mode is being applied to chat or not.
        /// </summary>
        public bool EmoteOnly { get; private set; } = false;
        /// <summary>
        ///     Property representing how long needed to be following to talk.
        ///     <br></br>
        ///     If <see langword="null"/>, FollowersOnly is not enabled.
        /// </summary>
        public TimeSpan? FollowersOnly { get; private set; } = null;
        /// <summary>
        ///     Property representing mercury value. Not sure what it's for.
        /// </summary>
        public bool Mercury { get; private set; } = false;
        /// <summary>
        ///     Property representing whether R9K is being applied to chat or not.
        /// </summary>
        public bool R9K { get; private set; } = false;
        /// <summary>
        ///     Property representing whether Rituals is enabled or not.
        /// </summary>
        public bool Rituals { get; private set; } = false;
        /// <summary>
        ///     Property representing whether Slow mode is being applied to chat or not.
        ///     <br></br>
        ///     value of zero, numeric null, means off
        /// </summary>
        public int SlowMode { get; private set; } = 0;
        /// <summary>
        ///     Property representing whether Sub Mode is being applied to chat or not.
        /// </summary>
        public bool SubOnly { get; private set; } = false;
        #endregion properties public


        #region ctor
        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="ircMessage">
        ///     <see cref="IrcMessage"/>
        /// </param>
        /// <param name="logger">
        ///     optional <see cref="ILogger"/>
        /// </param>
        public ChannelState(IrcMessage ircMessage, ILogger? logger = null)
        {

            Channel = ircMessage.Channel;
            if (ircMessage.Tags.ContainsKey(Tags.RoomId)) RoomId = ircMessage.Tags[Tags.RoomId];

            Apply(ircMessage, logger);
        }
        public ChannelState(
            bool r9k,
            bool rituals,
            bool subonly,
            int slowMode,
            bool emoteOnly,
            string broadcasterLanguage,
            string channel,
            TimeSpan followersOnly,
            bool mercury,
            string roomId)
        {
            R9K = r9k;
            Rituals = rituals;
            SubOnly = subonly;
            SlowMode = slowMode;
            EmoteOnly = emoteOnly;
            BroadcasterLanguage = broadcasterLanguage;
            Channel = channel;
            FollowersOnly = followersOnly;
            Mercury = mercury;
            RoomId = roomId;
        }
        #endregion ctor


        #region methods public
        public void Apply(IrcMessage ircMessage, ILogger? logger = null)
        {
            //@broadcaster-lang=;emote-only=0;r9k=0;slow=0;subs-only=1 :tmi.twitch.tv ROOMSTATE #burkeblack
            foreach (string tag in ircMessage.Tags.Keys)
            {
                string tagValue = ircMessage.Tags[tag];

                switch (tag)
                {
                    case Tags.BroadcasterLang:
                        BroadcasterLanguage = tagValue;
                        break;
                    case Tags.EmoteOnly:
                        EmoteOnly = Common.Helpers.ConvertToBool(tagValue);
                        break;
                    case Tags.R9K:
                        R9K = Common.Helpers.ConvertToBool(tagValue);
                        break;
                    case Tags.Rituals:
                        Rituals = Common.Helpers.ConvertToBool(tagValue);
                        break;
                    case Tags.Slow:
                        if (Int32.TryParse(tagValue, out int slowDuration) && slowDuration >= 0)
                            SlowMode = slowDuration;
                        break;
                    case Tags.SubsOnly:
                        SubOnly = Common.Helpers.ConvertToBool(tagValue);
                        break;
                    case Tags.FollowersOnly:
                        if (Int32.TryParse(tagValue, out int minutes))
                        {
                            if (minutes == -1)
                            {
                                FollowersOnly = null;
                            }
                            else if (minutes > -1)
                            {
                                FollowersOnly = TimeSpan.FromMinutes(minutes);
                            }
                        }
                        break;
                    case Tags.Mercury:
                        Mercury = Common.Helpers.ConvertToBool(tagValue);
                        break;
                    default:
                        Exception ex = new ArgumentOutOfRangeException(nameof(tagValue),
                                                                       tagValue,
                                                                       $"switch-case and/or {nameof(Tags)} have/has to be extended.");
                        logger?.LogExceptionAsError(GetType(), ex);
                        break;
                }
            }
        }
        #endregion methods public
    }
}
