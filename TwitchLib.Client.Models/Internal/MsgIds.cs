﻿using System;

namespace TwitchLib.Client.Models.Internal
{
    public static class MsgIds
    {
        public const string AlreadyBanned = "already_banned";
        public const string AlreadyEmotesOff = "already_emotes_off";
        public const string AlreadyEmotesOn = "already_emotes_on";
        public const string AlreadyR9KOff = "already_r9k_off";
        public const string AlreadyR9KOn = "already_r9k_on";
        public const string AlreadySubsOff = "already_subs_off";
        public const string AlreadySubsOn = "already_subs_on";
        public const string Announcement = "announcement";
        public const string BadUnbanNoBan = "bad_unban_no_ban";
        public const string BanSuccess = "ban_success";
        public const string ColorChanged = "color_changed";
        public const string EmoteOnlyOff = "emote_only_off";
        public const string EmoteOnlyOn = "emote_only_on";
        public const string FollowersOn = "followers_on";
        public const string FollowersOnZero = "followers_on_zero";
        public const string FollowersOff = "followers_off";
        public const string HighlightedMessage = "highlighted-message";
        [Obsolete("Not listet (anymore?): https://dev.twitch.tv/docs/irc/msg-id/")]
        public const string MsgBannedEmailAlias = "msg_banned_email_alias";
        public const string MsgChannelSuspended = "msg_channel_suspended";
        public const string MsgRequiresVerifiedPhoneNumber = "msg_requires_verified_phone_number";
        public const string MsgVerifiedEmail = "msg_verified_email";
        public const string MsgRateLimit = "msg_ratelimit";
        public const string MsgDuplicate = "msg_duplicate";
        public const string MsgR9k = "msg_r9k";
        public const string MsgFollowersOnly = "msg_followersonly";
        public const string MsgSubsOnly = "msg_subsonly";
        public const string MsgEmoteOnly = "msg_emoteonly";
        public const string MsgSuspended = "msg_suspended";
        public const string MsgBanned = "msg_banned";
        public const string MsgSlowMode = "msg_slowmode";
        public const string NoPermission = "no_permission";
        public const string PrimePaidUprade = "primepaidupgrade";
        public const string Raid = "raid";
        public const string UnRaid = "unraid";
        public const string RaidErrorSelf = "raid_error_self";
        public const string RaidNoticeMature = "raid_notice_mature";
        public const string ReSubscription = "resub";
        public const string R9KOff = "r9k_off";
        public const string R9KOn = "r9k_on";
        public const string SlowOn = "slow_on";
        public const string SlowOff = "slow_off";
        public const string SubGift = "subgift";
        public const string CommunitySubscription = "submysterygift";
        public const string ContinuedGiftedSubscription = "giftpaidupgrade";
        public const string Subscription = "sub";
        public const string SubsOff = "subs_off";
        public const string SubsOn = "subs_on";
        public const string TimeoutSuccess = "timeout_success";
        public const string UnbanSuccess = "unban_success";
        public const string UnrecognizedCmd = "unrecognized_cmd";
        public const string UserIntro = "user-intro";
        public const string SkipSubsModeMessage = "skip-subs-mode-message";
    }
}