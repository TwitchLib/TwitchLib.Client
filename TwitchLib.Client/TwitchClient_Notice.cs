using System;

using TwitchLib.Client.Events;
using TwitchLib.Client.Exceptions;
using TwitchLib.Client.Extensions.Internal;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client
{
    public partial class TwitchClient : ITwitchClient_Notice
    {
        // TraceMethodCall should use the Type of the interface,
        // that this class extends;
        // it makes it easier to find the respective occurance from the log file

        #region events public
        public event EventHandler<OnChatColorChangedArgs>? OnChatColorChanged;
        public event EventHandler<OnBannedArgs>? OnBanned;
        public event EventHandler<OnFollowersOnlyArgs>? OnFollowersOnly;
        public event EventHandler<OnSubsOnlyArgs>? OnSubsOnly;
        public event EventHandler<OnEmoteOnlyArgs>? OnEmoteOnly;
        public event EventHandler<OnSuspendedArgs>? OnSuspended;
        public event EventHandler<OnSlowModeArgs>? OnSlowMode;
        public event EventHandler<OnR9kModeArgs>? OnR9kMode;
        public event EventHandler<OnRequiresVerifiedEmailArgs>? OnRequiresVerifiedEmail;
        public event EventHandler<OnRequiresVerifiedPhoneNumberArgs>? OnRequiresVerifiedPhoneNumber;
        public event EventHandler<OnRateLimitArgs>? OnRateLimit;
        public event EventHandler<OnDuplicateArgs>? OnDuplicate;
        public event EventHandler<EventArgs>? OnSelfRaidError;
        public event EventHandler<EventArgs>? OnNoPermissionError;
        public event EventHandler<EventArgs>? OnRaidedChannelIsMatureAudience;
        public event EventHandler<OnBannedEmailAliasArgs>? OnBannedEmailAlias;
        #endregion events public


        #region methods private
        private bool HandleNotice(IrcMessage ircMessage)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Notice));
            bool success = ircMessage.Tags.TryGetValue(Tags.MsgId, out string msgId);
            if (!success)
            {
                OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs(ircMessage.Channel, TwitchUsername, "NoticeHandling", ircMessage.ToString()));
                UnaccountedFor(ircMessage.ToString());
                return false;
            }

            switch (msgId)
            {
                case MsgIds.ColorChanged:
                    OnChatColorChanged?.Invoke(this, new OnChatColorChangedArgs(ircMessage.Channel));
                    break;
                case MsgIds.NoPermission:
                    OnNoPermissionError?.Invoke(this, EventArgs.Empty);
                    break;
                case MsgIds.RaidErrorSelf:
                    OnSelfRaidError?.Invoke(this, EventArgs.Empty);
                    break;
                case MsgIds.RaidNoticeMature:
                    OnRaidedChannelIsMatureAudience?.Invoke(this, EventArgs.Empty);
                    break;
                case MsgIds.MsgBannedEmailAlias:
                    OnBannedEmailAlias?.Invoke(this, new OnBannedEmailAliasArgs(ircMessage.Channel, ircMessage.Message));
                    break;
                case MsgIds.MsgChannelSuspended:
                    OnFailureToReceiveJoinConfirmation?.Invoke(this, new OnFailureToReceiveJoinConfirmationArgs(ircMessage.Channel, new FailureToReceiveJoinConfirmationException(ircMessage.Channel, ircMessage.Message)));
                    break;
                case MsgIds.MsgRequiresVerifiedPhoneNumber:
                    OnRequiresVerifiedPhoneNumber?.Invoke(this, new OnRequiresVerifiedPhoneNumberArgs(ircMessage.Channel, ircMessage.Message));
                    break;
                case MsgIds.MsgVerifiedEmail:
                    OnRequiresVerifiedEmail?.Invoke(this, new OnRequiresVerifiedEmailArgs(ircMessage.Channel, ircMessage.Message));
                    break;
                case MsgIds.MsgRateLimit:
                    OnRateLimit?.Invoke(this, new OnRateLimitArgs(ircMessage.Channel, ircMessage.Message));
                    break;
                case MsgIds.MsgDuplicate:
                    OnDuplicate?.Invoke(this, new OnDuplicateArgs(ircMessage.Channel, ircMessage.Message));
                    break;
                case MsgIds.MsgFollowersOnly:
                    OnFollowersOnly?.Invoke(this, new OnFollowersOnlyArgs(ircMessage.Channel, ircMessage.Message, true));
                    break;
                case MsgIds.FollowersOn:
                    OnFollowersOnly?.Invoke(this, new OnFollowersOnlyArgs(ircMessage.Channel, ircMessage.Message, true));
                    break;
                case MsgIds.FollowersOnZero:
                    OnFollowersOnly?.Invoke(this, new OnFollowersOnlyArgs(ircMessage.Channel, ircMessage.Message, true));
                    break;
                case MsgIds.FollowersOff:
                    OnFollowersOnly?.Invoke(this, new OnFollowersOnlyArgs(ircMessage.Channel, ircMessage.Message, false));
                    break;
                case MsgIds.MsgSubsOnly:
                    OnSubsOnly?.Invoke(this, new OnSubsOnlyArgs(ircMessage.Channel, ircMessage.Message, true));
                    break;
                case MsgIds.SubsOn:
                    OnSubsOnly?.Invoke(this, new OnSubsOnlyArgs(ircMessage.Channel, ircMessage.Message, true));
                    break;
                case MsgIds.SubsOff:
                    OnSubsOnly?.Invoke(this, new OnSubsOnlyArgs(ircMessage.Channel, ircMessage.Message, false));
                    break;
                case MsgIds.MsgEmoteOnly:
                    OnEmoteOnly?.Invoke(this, new OnEmoteOnlyArgs(ircMessage.Channel, ircMessage.Message, true));
                    break;
                case MsgIds.EmoteOnlyOn:
                    OnEmoteOnly?.Invoke(this, new OnEmoteOnlyArgs(ircMessage.Channel, ircMessage.Message, true));
                    break;
                case MsgIds.EmoteOnlyOff:
                    OnEmoteOnly?.Invoke(this, new OnEmoteOnlyArgs(ircMessage.Channel, ircMessage.Message, false));
                    break;
                case MsgIds.MsgSuspended:
                    OnSuspended?.Invoke(this, new OnSuspendedArgs(ircMessage.Channel, ircMessage.Message));
                    break;
                case MsgIds.MsgBanned:
                    OnBanned?.Invoke(this, new OnBannedArgs(ircMessage.Channel, ircMessage.Message));
                    break;
                case MsgIds.MsgSlowMode:
                    OnSlowMode?.Invoke(this, new OnSlowModeArgs(ircMessage.Channel, ircMessage.Message, true));
                    break;
                case MsgIds.SlowOn:
                    OnSlowMode?.Invoke(this, new OnSlowModeArgs(ircMessage.Channel, ircMessage.Message, true));
                    break;
                case MsgIds.SlowOff:
                    OnSlowMode?.Invoke(this, new OnSlowModeArgs(ircMessage.Channel, ircMessage.Message, false));
                    break;
                case MsgIds.MsgR9k:
                    OnR9kMode?.Invoke(this, new OnR9kModeArgs(ircMessage.Channel, ircMessage.Message, true));
                    break;
                case MsgIds.R9KOn:
                    OnR9kMode?.Invoke(this, new OnR9kModeArgs(ircMessage.Channel, ircMessage.Message, true));
                    break;
                case MsgIds.R9KOff:
                    OnR9kMode?.Invoke(this, new OnR9kModeArgs(ircMessage.Channel, ircMessage.Message, false));
                    break;

                default:
                    OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs(ircMessage.Channel, TwitchUsername, "NoticeHandling", ircMessage.ToString()));
                    UnaccountedFor(ircMessage.ToString());
                    return false;
            }
            return true;
        }
        #endregion methods private
    }
}
