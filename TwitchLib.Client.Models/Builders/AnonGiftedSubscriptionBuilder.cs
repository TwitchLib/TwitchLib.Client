using System.Collections.Generic;
using TwitchLib.Client.Enums;

namespace TwitchLib.Client.Models.Builders
{
    public sealed class AnonGiftedSubscriptionBuilder : IBuilder<AnonGiftedSubscription>, IFromIrcMessageBuilder<AnonGiftedSubscription>
    {
        public List<KeyValuePair<string, string>> _badges = new List<KeyValuePair<string, string>>();
        public List<KeyValuePair<string, string>> _badgeInfo = new List<KeyValuePair<string, string>>();
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
            // TODO: Add with* methods
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
