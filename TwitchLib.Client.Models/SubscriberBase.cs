using System.Drawing;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models.Common;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    /// <summary>Class representing a resubscriber.</summary>
    public class SubscriberBase
    {
        /// <summary>Property representing list of badges assigned.</summary>
        public List<KeyValuePair<string, string>> Badges { get; }

        /// <summary>Metadata associated with each badge</summary>
        public List<KeyValuePair<string, string>> BadgeInfo { get; }

        /// <summary>Property representing HEX color as a System.Drawing.Color object.</summary>
        public Color Color { get; }

        /// <summary>Property representing resubscriber's customized display name.</summary>
        public string DisplayName { get; }

        /// <summary>Property representing emote set of resubscriber.</summary>
        public string EmoteSet { get; }

        /// <summary>Property representing resub message id</summary>
        public string Id { get; }

        /// <summary>Property representing whether or not the resubscriber is a moderator.</summary>
        public bool IsModerator { get; }

        /// <summary>Property representing whether or not person is a partner.</summary>
        public bool IsPartner { get; }

        /// <summary>Property representing whether or not the resubscriber is a subscriber (YES).</summary>
        public bool IsSubscriber { get; }

        /// <summary>Property representing whether or not the resubscriber is a turbo member.</summary>
        public bool IsTurbo { get; }

        /// <summary>Property representing login of resubscription event.</summary>
        public string Login { get; }

        public string MsgId { get; }

        public string MsgParamCumulativeMonths { get; }

        public bool MsgParamShouldShareStreak { get; }

        public string MsgParamStreakMonths { get; }

        /// <summary>Property representing the raw IRC message (for debugging/customized parsing)</summary>
        public string RawIrc { get; }

        /// <summary>Property representing system message.</summary>
        public string ResubMessage { get; }

        /// <summary>Property representing the room id.</summary>
        public string RoomId { get; }

        /// <summary>Property representing the plan a user is on.</summary>
        public SubscriptionPlan SubscriptionPlan { get; } = SubscriptionPlan.NotSet;

        /// <summary>Property representing the subscription plan name.</summary>
        public string SubscriptionPlanName { get; }

        /// <summary>Property representing internval system message value.</summary>
        public string SystemMessage { get; }

        /// <summary>Property representing internal system message value, parsed.</summary>
        public string SystemMessageParsed { get; }

        /// <summary>Property representing the tmi-sent-ts value.</summary>
        public string TmiSentTs { get; }

        /// <summary>Property representing the user's id.</summary>
        public string UserId { get; }

        /// <summary>Property representing the user type of the resubscriber.</summary>
        public UserType UserType { get; }

        public string Channel { get; }

        // @badges=subscriber/1,turbo/1;color=#2B119C;display-name=JustFunkIt;emotes=;id=9dasn-asdibas-asdba-as8as;login=justfunkit;mod=0;msg-id=resub;msg-param-months=2;room-id=44338537;subscriber=1;system-msg=JustFunkIt\ssubscribed\sfor\s2\smonths\sin\sa\srow!;turbo=1;user-id=26526370;user-type= :tmi.twitch.tv USERNOTICE #burkeblack :AVAST YEE SCURVY DOG

        protected readonly int monthsInternal;

        /// <summary>Subscriber object constructor.</summary>
        protected SubscriberBase(IrcMessage ircMessage)
        {
            RawIrc = ircMessage.ToString();
            ResubMessage = ircMessage.Message;

            foreach (var tag in ircMessage.Tags)
            {
                switch (tag.Value)
                {
                    case Tags.Badges:
                        Badges = TagHelper.ToBadges(tag.Value);
                        // iterate through badges for special circumstances
                        foreach (var badge in Badges)
                        {
                            if (badge.Key == "partner")
                                IsPartner = true;
                        }
                        break;
                    case Tags.BadgeInfo:
                        BadgeInfo = TagHelper.ToBadges(tag.Value);
                        break;
                    case Tags.Color:
                        Color = TagHelper.ToColor(tag.Value);
                        break;
                    case Tags.DisplayName:
                        DisplayName = tag.Value;
                        break;
                    case Tags.Emotes:
                        EmoteSet = tag.Value;
                        break;
                    case Tags.Id:
                        Id = tag.Value;
                        break;
                    case Tags.Login:
                        Login = tag.Value;
                        break;
                    case Tags.Mod:
                        IsModerator = TagHelper.ToBool(tag.Value);
                        break;
                    case Tags.MsgId:
                        MsgId = tag.Value;
                        break;
                    case Tags.MsgParamCumulativeMonths:
                        MsgParamCumulativeMonths = tag.Value;
                        break;
                    case Tags.MsgParamStreakMonths:
                        MsgParamStreakMonths = tag.Value;
                        break;
                    case Tags.MsgParamShouldShareStreak:
                        MsgParamShouldShareStreak = TagHelper.ToBool(tag.Value);
                        break;
                    case Tags.MsgParamSubPlan:
                        SubscriptionPlan = TagHelper.ToSubscriptionPlan(tag.Value);
                        break;
                    case Tags.MsgParamSubPlanName:
                        SubscriptionPlanName = tag.Value.Replace("\\s", " ");
                        break;
                    case Tags.RoomId:
                        RoomId = tag.Value;
                        break;
                    case Tags.Subscriber:
                        IsSubscriber = TagHelper.ToBool(tag.Value);
                        break;
                    case Tags.SystemMsg:
                        SystemMessage = tag.Value;
                        SystemMessageParsed = tag.Value.Replace("\\s", " ");
                        break;
                    case Tags.TmiSentTs:
                        TmiSentTs = tag.Value;
                        break;
                    case Tags.Turbo:
                        IsTurbo = TagHelper.ToBool(tag.Value);
                        break;
                    case Tags.UserId:
                        UserId = tag.Value;
                        break;
                    case Tags.UserType:
                        UserType = TagHelper.ToUserType(tag.Value);
                        break;
                }
            }
        }

        internal SubscriberBase(
            List<KeyValuePair<string, string>> badges,
            List<KeyValuePair<string, string>> badgeInfo,
            Color color,
            string displayName,
            string emoteSet,
            string id,
            string login,
            string systemMessage,
            string msgId,
            string msgParamCumulativeMonths,
            string msgParamStreakMonths,
            bool msgParamShouldShareStreak,
            string systemMessageParsed,
            string resubMessage,
            SubscriptionPlan subscriptionPlan,
            string subscriptionPlanName,
            string roomId,
            string userId,
            bool isModerator,
            bool isTurbo,
            bool isSubscriber,
            bool isPartner,
            string tmiSentTs,
            UserType userType,
            string rawIrc,
            string channel,
            int months)
        {
            Badges = badges;
            BadgeInfo = badgeInfo;
            Color = color;
            DisplayName = displayName;
            EmoteSet = emoteSet;
            Id = id;
            Login = login;
            MsgId = msgId;
            MsgParamCumulativeMonths = msgParamCumulativeMonths;
            MsgParamStreakMonths = msgParamStreakMonths;
            MsgParamShouldShareStreak = msgParamShouldShareStreak;
            SystemMessage = systemMessage;
            SystemMessageParsed = systemMessageParsed;
            ResubMessage = resubMessage;
            SubscriptionPlan = subscriptionPlan;
            SubscriptionPlanName = subscriptionPlanName;
            RoomId = roomId;
            UserId = UserId;
            IsModerator = isModerator;
            IsTurbo = isTurbo;
            IsSubscriber = isSubscriber;
            IsPartner = isPartner;
            TmiSentTs = tmiSentTs;
            UserType = userType;
            RawIrc = rawIrc;
            monthsInternal = months;
            UserId = userId;
            Channel = channel;
        }

        /// <summary>Overriden ToString method, prints out all properties related to resub.</summary>
        public override string ToString()
        {
            return $"Badges: {Badges.Count}, color: {Color}, display name: {DisplayName}, emote set: {EmoteSet}, login: {Login}, system message: {SystemMessage}, msgId: {MsgId}, msgParamCumulativeMonths: {MsgParamCumulativeMonths}" +
                $"msgParamStreakMonths: {MsgParamStreakMonths}, msgParamShouldShareStreak: {MsgParamShouldShareStreak}, resub message: {ResubMessage}, months: {monthsInternal}, room id: {RoomId}, user id: {UserId}, mod: {IsModerator}, turbo: {IsTurbo}, sub: {IsSubscriber}, user type: {UserType}, raw irc: {RawIrc}";
        }
    }
}
