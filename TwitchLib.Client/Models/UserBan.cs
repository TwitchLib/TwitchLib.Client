using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class UserBan
    {
        /// <summary>Channel that had ban event.</summary>
        public string Channel;
        /// <summary>User that was banned.</summary>
        public string Username;
        /// <summary>Reason for ban, if it was provided.</summary>
        public string BanReason;

        public UserBan(IrcMessage ircMessage)
        {
            Channel = ircMessage.Channel;
            Username = ircMessage.Message;

            var successBanReason = ircMessage.Tags.TryGetValue(Tags.BanReason, out var banReason);
            if (successBanReason)
            {
                BanReason = banReason;
            }
        }
    }
}