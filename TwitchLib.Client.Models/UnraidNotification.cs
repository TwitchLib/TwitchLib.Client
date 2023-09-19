using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

public class UnraidNotification : UserNoticeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnraidNotification"/> class.
    /// </summary>
    public UnraidNotification(IrcMessage ircMessage) : base(ircMessage)
    {
    }

    /// <inheritdoc/>
    protected override bool TrySet(KeyValuePair<string, string> tag)
    {
        return false;
    }
}