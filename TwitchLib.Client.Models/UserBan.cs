using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models
{
    public class UserBan
    {
        /// <summary>Channel that had ban event.</summary>
        public string Channel;

        /// <summary>User that was banned.</summary>
        public string Username;

        /// <summary>Channel that had ban event. Id.</summary>
        public string RoomId;

        /// <summary>User that was banned. Id.</summary>
        public string TargetUserId;

        public UserBan(IrcMessage ircMessage)
        {
            Channel = ircMessage.Channel;
            Username = ircMessage.Message;

            if (ircMessage.Tags.TryGetValue(Tags.RoomId, out var roomId))
            {
                RoomId = roomId;
            }

            if (ircMessage.Tags.TryGetValue(Tags.TargetUserId, out var targetUserId))
            {
                TargetUserId = targetUserId;
            }
        }

        public UserBan(
            string channel,
            string username,
            string roomId,
            string targetUserId)
        {
            Channel = channel;
            Username = username;
            RoomId = roomId;
            TargetUserId = targetUserId;
        }
    }
}
