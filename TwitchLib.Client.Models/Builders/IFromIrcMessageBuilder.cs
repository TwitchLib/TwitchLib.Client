#nullable disable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace TwitchLib.Client.Models.Builders
{
    public interface IFromIrcMessageBuilder<T>
    {
        T BuildFromIrcMessage(FromIrcMessageBuilderDataObject fromIrcMessageBuilderDataObject);
    }
}
