using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models;

public class Ritual : UserNoticeBase
{
    /// <summary>
    /// The name of the ritual being celebrated.
    /// </summary>
    public string MsgParamRitualName { get; protected set; } = default!;

    public string Message { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Ritual"/> class.
    /// </summary>
    public Ritual(IrcMessage ircMessage) : base(ircMessage)
    {
        Message = ircMessage.Message;
    }

    /// <inheritdoc/>
    protected override bool TrySet(KeyValuePair<string, string> tag)
    {
        switch (tag.Key)
        {
            case Tags.MsgParamRitualName:
                MsgParamRitualName = tag.Value;
                break;
            default:
                return false;
        }
        return true;
    }
}