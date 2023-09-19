using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

/// <summary>Class representing Announcement in a Twitch channel.</summary>
public class Announcement : UserNoticeBase
{
    /// <summary>Property representing the color value of the announcement.</summary>
    public string MsgParamColor { get; protected set; } = default!;

    /// <summary>Property representing the message of the announcement.</summary>
    public string Message { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Announcement"/> class.
    /// </summary>
    /// <param name="ircMessage">The IRC message from Twitch to be processed.</param>
    public Announcement(IrcMessage ircMessage) : base(ircMessage)
    {
        Message = ircMessage.Message;
    }

    /// <inheritdoc/>
    protected override bool TrySet(KeyValuePair<string, string> tag)
    {
        switch (tag.Key)
        {
            case Tags.MsgParamColor:
                MsgParamColor = tag.Value;
                break;
            default:
                return false;
        }
        return true;
    }
}
