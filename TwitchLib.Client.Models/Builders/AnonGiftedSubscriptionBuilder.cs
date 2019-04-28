using System.Collections.Generic;

using TwitchLib.Client.Enums;

namespace TwitchLib.Client.Models.Builders
{
    public sealed class AnonGiftedSubscriptionBuilder : IBuilder<AnonGiftedSubscription>, IFromIrcMessageBuilder<AnonGiftedSubscription>
    {
        public readonly List<KeyValuePair<string, string>> _badges = new List<KeyValuePair<string, string>>();
        public readonly List<KeyValuePair<string, string>> _badgeInfo = new List<KeyValuePair<string, string>>();
        public string _color;
        public string _displayName;
        public string _emotes;
        public string _id;
        public bool _isModerator;
        public bool _isSubscriber;
        public bool _isTurbo;
        public string _login;
        public string _msgId;
        public string _msgParamMonths;
        public string _msgParamRecipientDisplayName;
        public string _msgParamRecipientId;
        public string _msgParamRecipientUserName;
        public string _msgParamSubPlanName;
        public SubscriptionPlan _msgParamSubPlan;
        public string _roomId;
        public string _systemMsg;
        public string _systemMsgParsed;
        public string _tmiSentTs;
        public string _userId;
        public UserType _userType;

        private AnonGiftedSubscriptionBuilder()
        {
        }

        public AnonGiftedSubscriptionBuilder WithBadges(params KeyValuePair<string, string>[] badges)
        {
            _badges.AddRange(badges);
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithBadgeInfos(params KeyValuePair<string, string>[] badgeInfos)
        {
            _badgeInfo.AddRange(badgeInfos);
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithColor(string color)
        {
            _color = color;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithDisplayName(string displayName)
        {
            _displayName = displayName;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithEmotes(string emotes)
        {
            _emotes = emotes;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithId(string id)
        {
            _id = id;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithIsModerator(bool isModerator)
        {
            _isModerator = isModerator;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithIsSubscriber(bool isSubscriber)
        {
            _isSubscriber = isSubscriber;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithIsTurbo(bool isTurbo)
        {
            _isTurbo = isTurbo;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithLogin(string login)
        {
            _login = login;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithMessageId(string msgId)
        {
            _msgId = msgId;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithMsgParamMonths(string msgParamMonths)
        {
            _msgParamMonths = msgParamMonths;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithMsgParamRecipientDisplayName(string msgParamRecipientDisplayName)
        {
            _msgParamRecipientDisplayName = msgParamRecipientDisplayName;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithMsgParamRecipientId(string msgParamRecipientId)
        {
            _msgParamRecipientId = msgParamRecipientId;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithMsgParamRecipientUserName(string msgParamRecipientUserName)
        {
            _msgParamRecipientUserName = msgParamRecipientUserName;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithMsgParamSubPlanName(string msgParamSubPlanName)
        {
            _msgParamSubPlanName = msgParamSubPlanName;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithMsgParamSubPlan(SubscriptionPlan msgParamSubPlan)
        {
            _msgParamSubPlan = msgParamSubPlan;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithRoomId(string roomId)
        {
            _roomId = roomId;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithSystemMsg(string systemMsg)
        {
            _systemMsg = systemMsg;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithSystemMsgParsed(string systemMsgParsed)
        {
            _systemMsgParsed = systemMsgParsed;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithTmiSentTs(string tmiSentTs)
        {
            _tmiSentTs = tmiSentTs;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithUserId(string userId)
        {
            _userId = userId;
            return this;
        }

        public AnonGiftedSubscriptionBuilder WithUserType(UserType userType)
        {
            _userType = userType;
            return this;
        }

        public static AnonGiftedSubscriptionBuilder Create()
        {
            return new AnonGiftedSubscriptionBuilder();
        }

        public AnonGiftedSubscription Build()
        {
            return new AnonGiftedSubscription(
                _badges,
                _badgeInfo,
                _color,
                _displayName,
                _emotes,
                _id,
                _login,
                _isModerator,
                _msgId,
                _msgParamMonths,
                _msgParamRecipientDisplayName,
                _msgParamRecipientId,
                _msgParamRecipientUserName,
                _msgParamSubPlanName,
                _msgParamSubPlan,
                _roomId,
                _isSubscriber,
                _systemMsg,
                _systemMsgParsed,
                _tmiSentTs,
                _isTurbo,
                _userType);
        }

        public AnonGiftedSubscription BuildFromIrcMessage(FromIrcMessageBuilderDataObject fromIrcMessageBuilderDataObject)
        {
            return new AnonGiftedSubscription(fromIrcMessageBuilderDataObject.Message);
        }
    }
}