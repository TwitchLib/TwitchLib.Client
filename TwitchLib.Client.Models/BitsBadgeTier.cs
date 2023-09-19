using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

public class BitsBadgeTier : UserNoticeBase
{
    /// <summary>
    /// The tier of the Bits badge the user just earned. For example, 100, 1000, or 10000.
    /// </summary>
    public int MsgParamThreshold { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BitsBadgeTier"/> class.
    /// </summary>
    public BitsBadgeTier(IrcMessage ircMessage) : base(ircMessage)
    {
    }

    /// <inheritdoc/>
    protected override bool TrySet(KeyValuePair<string, string> tag)
    {
        switch (tag.Key)
        {
            case Tags.MsgParamThreshold:
                MsgParamThreshold = int.Parse(tag.Value);
                break;
        }
        return true;
    }
}