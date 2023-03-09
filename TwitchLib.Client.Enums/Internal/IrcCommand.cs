﻿using System;

namespace TwitchLib.Client.Enums.Internal
{
    public enum IrcCommand
    {
        Unknown,
        PrivMsg,
        Notice,
        Ping,
        Pong,
        Join,
        Part,
        ClearChat,
        ClearMsg,
        UserState,
        GlobalUserState,
        Nick,
        Pass,
        Cap,
        RPL_001,
        RPL_002,
        RPL_003,
        RPL_004,
        RPL_353,
        RPL_366,
        RPL_372,
        RPL_375,
        RPL_376,
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        Whisper,
        RoomState,
        Reconnect,
        ServerChange,
        UserNotice,
        Mode
    }
}