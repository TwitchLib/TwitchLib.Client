using System;

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
        /// <summary>
        ///     first reply after successfully authenticated
        ///     <br></br>
        ///     <br></br>
        ///     <code>Welcome, GLHF!</code>
        /// </summary>
        RPL_001,
        /// <summary>
        ///     second reply after successfully authenticated
        ///     <br></br>
        ///     <br></br>
        ///     <code>Your host is ...</code>
        /// </summary>
        RPL_002,
        /// <summary>
        ///     third reply after successfully authenticated
        ///     <br></br>
        ///     <br></br>
        ///     <code>This server is rather new</code>
        /// </summary>
        RPL_003,
        /// <summary>
        ///     fourth reply after successfully authenticated
        ///     <br></br>
        ///     <br></br>
        ///     <code>-</code>
        /// </summary>
        RPL_004,
        /// <summary>
        ///     fifth reply after joining a channel/room
        ///     <br></br>
        ///     <br></br>
        ///     <code>-</code>
        /// </summary>
        RPL_375,
        /// <summary>
        ///     sixth reply after successfully authenticated
        ///     <br></br>
        ///     <br></br>
        ///     <code>You are in a maze of twisty passages.</code>
        /// </summary>
        RPL_372,
        /// <summary>
        ///     seventh reply after successfully authenticated
        ///     <br></br>
        ///     <br></br>
        ///     <code>&gt;</code>
        /// </summary>
        RPL_376,
        /// <summary>
        ///     second reply after joining a channel
        ///     <br></br>
        ///     <br></br>
        ///     Names List of detected Users
        /// </summary>
        RPL_353,
        /// <summary>
        ///     third reply after joining a channel
        ///     <br></br>
        ///     <br></br>
        ///     End of Names List of detected Users
        /// </summary>
        RPL_366,
        [Obsolete(SystemMessageConstants.ObsoleteWhisperMessage)]
        Whisper,
        RoomState,
        Reconnect,
        ServerChange,
        UserNotice,
        Mode
    }
}