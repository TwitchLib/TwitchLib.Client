using TwitchLib.Client.Models.Internal;

namespace TwitchLib.Client.Models.Builders
{
    public interface IFromIrcMessageBuilder<T>
    {
        T BuildFromIrcMessage(IrcMessage ircMessage);
    }
}
