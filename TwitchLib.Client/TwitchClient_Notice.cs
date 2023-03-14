using System;
using System.Collections.Generic;
using System.Linq;

using TwitchLib.Client.Events;
using TwitchLib.Client.Exceptions;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models.Internal;
using TwitchLib.Communication.Extensions;

namespace TwitchLib.Client
{
    public partial class TwitchClient : ITwitchClient_Notice
    {
        // TraceMethodCall should use the Type of the interface,
        // that this class extends;
        // it makes it easier to find the respective occurance from the log file

        public event EventHandler<OnModeratorsReceivedArgs> OnModeratorsReceived;
        public event EventHandler<OnChatColorChangedArgs> OnChatColorChanged;
        public event EventHandler<OnBannedArgs> OnBanned;
        public event EventHandler<OnFollowersOnlyArgs> OnFollowersOnly;
        public event EventHandler<OnSubsOnlyArgs> OnSubsOnly;
        public event EventHandler<OnEmoteOnlyArgs> OnEmoteOnly;
        public event EventHandler<OnSuspendedArgs> OnSuspended;
        public event EventHandler<OnSlowModeArgs> OnSlowMode;
        public event EventHandler<OnR9kModeArgs> OnR9kMode;
        public event EventHandler<OnRequiresVerifiedEmailArgs> OnRequiresVerifiedEmail;
        public event EventHandler<OnRequiresVerifiedPhoneNumberArgs> OnRequiresVerifiedPhoneNumber;
        public event EventHandler<OnRateLimitArgs> OnRateLimit;
        public event EventHandler<OnDuplicateArgs> OnDuplicate;
        public event EventHandler<EventArgs> OnSelfRaidError;
        public event EventHandler<EventArgs> OnNoPermissionError;
        public event EventHandler<EventArgs> OnRaidedChannelIsMatureAudience;
        public event EventHandler<OnBannedEmailAliasArgs> OnBannedEmailAlias;
        public event EventHandler<OnVIPsReceivedArgs> OnVIPsReceived;

        private void HandleNotice(IrcMessage ircMessage)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Notice));
            if (ircMessage.Message.Contains("Improperly formatted auth"))
            {
                OnIncorrectLogin?.Invoke(this, new OnIncorrectLoginArgs { Exception = new ErrorLoggingInException(ircMessage.ToString(), TwitchUsername) });
                return;
            }

            bool success = ircMessage.Tags.TryGetValue(Tags.MsgId, out string msgId);
            if (!success)
            {
                OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel, Location = "NoticeHandling", RawIRC = ircMessage.ToString() });
                UnaccountedFor(ircMessage.ToString());
            }

            switch (msgId)
            {
                case MsgIds.ColorChanged:
                    OnChatColorChanged?.Invoke(this, new OnChatColorChangedArgs { Channel = ircMessage.Channel });
                    break;
                case MsgIds.ModeratorsReceived:
                    OnModeratorsReceived?.Invoke(this, new OnModeratorsReceivedArgs { Channel = ircMessage.Channel, Moderators = ircMessage.Message.Replace(" ", "").Split(':')[1].Split(',').ToList() });
                    break;
                case MsgIds.NoMods:
                    OnModeratorsReceived?.Invoke(this, new OnModeratorsReceivedArgs { Channel = ircMessage.Channel, Moderators = new List<string>() });
                    break;
                case MsgIds.NoPermission:
                    OnNoPermissionError?.Invoke(this, null);
                    break;
                case MsgIds.RaidErrorSelf:
                    OnSelfRaidError?.Invoke(this, null);
                    break;
                case MsgIds.RaidNoticeMature:
                    OnRaidedChannelIsMatureAudience?.Invoke(this, null);
                    break;
                case MsgIds.MsgBannedEmailAlias:
                    OnBannedEmailAlias?.Invoke(this, new OnBannedEmailAliasArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgChannelSuspended:
                    ChannelManager.JoinCanceld(ircMessage.Channel);
                    OnFailureToReceiveJoinConfirmation?.Invoke(this, new OnFailureToReceiveJoinConfirmationArgs
                    {
                        Exception = new FailureToReceiveJoinConfirmationException(ircMessage.Channel, ircMessage.Message)
                    });
                    break;
                case MsgIds.MsgRequiresVerifiedPhoneNumber:
                    OnRequiresVerifiedPhoneNumber?.Invoke(this, new OnRequiresVerifiedPhoneNumberArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgVerifiedEmail:
                    OnRequiresVerifiedEmail?.Invoke(this, new OnRequiresVerifiedEmailArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.NoVIPs:
                    OnVIPsReceived?.Invoke(this, new OnVIPsReceivedArgs { Channel = ircMessage.Channel, VIPs = new List<string>() });
                    break;
                case MsgIds.VIPsSuccess:
                    OnVIPsReceived?.Invoke(this, new OnVIPsReceivedArgs { Channel = ircMessage.Channel, VIPs = ircMessage.Message.Replace(" ", "").Replace(".", "").Split(':')[1].Split(',').ToList() });
                    break;
                case MsgIds.MsgRateLimit:
                    OnRateLimit?.Invoke(this, new OnRateLimitArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgDuplicate:
                    OnDuplicate?.Invoke(this, new OnDuplicateArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgFollowersOnly:
                    OnFollowersOnly?.Invoke(this, new OnFollowersOnlyArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgSubsOnly:
                    OnSubsOnly?.Invoke(this, new OnSubsOnlyArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgEmoteOnly:
                    OnEmoteOnly?.Invoke(this, new OnEmoteOnlyArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgSuspended:
                    OnSuspended?.Invoke(this, new OnSuspendedArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgBanned:
                    OnBanned?.Invoke(this, new OnBannedArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgSlowMode:
                    OnSlowMode?.Invoke(this, new OnSlowModeArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;
                case MsgIds.MsgR9k:
                    OnR9kMode?.Invoke(this, new OnR9kModeArgs { Channel = ircMessage.Channel, Message = ircMessage.Message });
                    break;

                default:
                    OnUnaccountedFor?.Invoke(this, new OnUnaccountedForArgs { BotUsername = TwitchUsername, Channel = ircMessage.Channel, Location = "NoticeHandling", RawIRC = ircMessage.ToString() });
                    UnaccountedFor(ircMessage.ToString());
                    break;
            }
        }
    }
}
