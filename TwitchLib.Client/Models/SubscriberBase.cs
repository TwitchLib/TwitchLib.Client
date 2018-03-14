#if NETSTANDARD
    using TwitchLib.Client.Extensions.NetCore;
#endif
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
    using TwitchLib.Client.Models.Internal;

#if NET452

#endif

namespace TwitchLib.Client.Models
{
    /// <summary>Class representing a resubscriber.</summary>
    public class SubscriberBase
    {
        /// <summary>Property representing list of badges assigned.</summary>
        public List<KeyValuePair<string, string>> Badges { get; }
        /// <summary>Property representing the colorhex of the resubscriber.</summary>
        public string ColorHex { get; }
        /// <summary>Property representing HEX color as a System.Drawing.Color object.</summary>
        public System.Drawing.Color Color { get; }
        /// <summary>Property representing resubscriber's customized display name.</summary>
        public string DisplayName { get; }
        /// <summary>Property representing emote set of resubscriber.</summary>
        public string EmoteSet { get; }
        /// <summary>Property representing resub message id</summary>
        public string Id { get; }
        /// <summary>Property representing login of resubscription event.</summary>
        public string Login { get; }
        /// <summary>Property representing internval system message value.</summary>
        public string SystemMessage { get; }
        /// <summary>Property representing internal system message value, parsed.</summary>
        public string SystemMessageParsed { get; }
        /// <summary>Property representing system message.</summary>
        public string ResubMessage { get; }
        /// <summary>Property representing the plan a user is on.</summary>
        public Enums.SubscriptionPlan SubscriptionPlan { get; } = Enums.SubscriptionPlan.NotSet;
        /// <summary>Property representing the subscription plan name.</summary>
        public string SubscriptionPlanName { get; }
        /// <summary>Property representing the room id.</summary>
        public string RoomId { get; }
        /// <summary>Property representing the user's id.</summary>
        public string UserId { get; }
        /// <summary>Property representing whether or not the resubscriber is a moderator.</summary>
        public bool IsModerator { get; }
        /// <summary>Property representing whether or not the resubscriber is a turbo member.</summary>
        public bool IsTurbo { get; }
        /// <summary>Property representing whether or not the resubscriber is a subscriber (YES).</summary>
        public bool IsSubscriber { get; }
        /// <summary>Property representing whether or not person is a partner.</summary>
        public bool IsPartner { get; }
        /// <summary>Property representing the tmi-sent-ts value.</summary>
        public string TmiSentTs { get; }
        /// <summary>Property representing the user type of the resubscriber.</summary>
        public Enums.UserType UserType { get; }
        /// <summary>Property representing the raw IRC message (for debugging/customized parsing)</summary>
        public string RawIrc { get; }
        /// <summary>Property representing the channel the resubscription happened in.</summary>
        public string Channel { get; }
        // @badges=subscriber/1,turbo/1;color=#2B119C;display-name=JustFunkIt;emotes=;id=9dasn-asdibas-asdba-as8as;login=justfunkit;mod=0;msg-id=resub;msg-param-months=2;room-id=44338537;subscriber=1;system-msg=JustFunkIt\ssubscribed\sfor\s2\smonths\sin\sa\srow!;turbo=1;user-id=26526370;user-type= :tmi.twitch.tv USERNOTICE #burkeblack :AVAST YEE SCURVY DOG

        protected readonly int months;

        /// <summary>Subscriber object constructor.</summary>
        protected SubscriberBase(IrcMessage ircMessage)
        {
            RawIrc = ircMessage.ToString();
            Channel = ircMessage.Channel;
            ResubMessage = ircMessage.Message;

            foreach (var tag in ircMessage.Tags.Keys)
            {
                var tagValue = ircMessage.Tags[tag];
                switch (tag)
                {
                    case Tags.Badges:
                        Badges = new List<KeyValuePair<string, string>>();
                        foreach (var badgeValue in tagValue.Split(','))
                            if (badgeValue.Contains('/'))
                                Badges.Add(new KeyValuePair<string, string>(badgeValue.Split('/')[0], badgeValue.Split('/')[1]));
                        // iterate through badges for special circumstances
                        foreach (var badge in Badges)
                        {
                            if (badge.Key == "partner")
                                IsPartner = true;
                        }
                        break;
                    case Tags.Color:
                        ColorHex = tagValue;
                        if (!string.IsNullOrEmpty(ColorHex))
                            Color = ColorTranslator.FromHtml(ColorHex);
                        break;
                    case Tags.DisplayName:
                        DisplayName = tagValue;
                        break;
                    case Tags.Emotes:
                        EmoteSet = tagValue;
                        break;
                    case Tags.Id:
                        Id = tagValue;
                        break;
                    case Tags.Login:
                        Login = tagValue;
                        break;
                    case Tags.Mod:
                        IsModerator = ConvertToBool(tagValue);
                        break;
                    case Tags.MsgParamMonths:
                        months = int.Parse(tagValue);
                        break;
                    case Tags.MsgParamSubPlan:
                        switch (tagValue.ToLower())
                        {
                            case "prime":
                                SubscriptionPlan = Enums.SubscriptionPlan.Prime;
                                break;
                            case "1000":
                                SubscriptionPlan = Enums.SubscriptionPlan.Tier1;
                                break;
                            case "2000":
                                SubscriptionPlan = Enums.SubscriptionPlan.Tier2;
                                break;
                            case "3000":
                                SubscriptionPlan = Enums.SubscriptionPlan.Tier3;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(tagValue.ToLower));
                        }
                        break;
                    case Tags.MsgParamSubPlanName:
                        SubscriptionPlanName = tagValue.Replace("\\s", " ");
                        break;
                    case Tags.RoomId:
                        RoomId = tagValue;
                        break;
                    case Tags.Subscriber:
                        IsSubscriber = ConvertToBool(tagValue);
                        break;
                    case Tags.SystemMsg:
                        SystemMessage = tagValue;
                        SystemMessageParsed = tagValue.Replace("\\s", " ");
                        break;
                    case Tags.TmiSentTs:
                        TmiSentTs = tagValue;
                        break;
                    case Tags.Turbo:
                        IsTurbo = ConvertToBool(tagValue);
                        break;
                    case Tags.UserId:
                        UserId = tagValue;
                        break;
                    case Tags.UserType:
                        switch (tagValue)
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
                        break;
                }
            }
        }

        private static bool ConvertToBool(string data)
        {
            return data == "1";
        }

        /// <summary>Overriden ToString method, prints out all properties related to resub.</summary>
        public override string ToString()
        {
            return $"Badges: {Badges.Count}, color hex: {ColorHex}, display name: {DisplayName}, emote set: {EmoteSet}, login: {Login}, system message: {SystemMessage}, " +
                $"resub message: {ResubMessage}, months: {months}, room id: {RoomId}, user id: {UserId}, mod: {IsModerator}, turbo: {IsTurbo}, sub: {IsSubscriber}, user type: {UserType}, " +
                   $"channel: {Channel}, raw irc: {RawIrc}";
        }
    }
}
