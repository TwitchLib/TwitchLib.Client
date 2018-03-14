using System;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    /// <inheritdoc />
    /// <summary>Args representing a user was banned event.</summary>
    public class OnUserBannedArgs : EventArgs
    {
        public UserBan UserBan;
    }
}
