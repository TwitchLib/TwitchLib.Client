#nullable disable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace TwitchLib.Client.Models.Builders
{
    public sealed class RitualNewChatterBuilder : IFromIrcMessageBuilder<RitualNewChatter>
    {
        public RitualNewChatter BuildFromIrcMessage(FromIrcMessageBuilderDataObject fromIrcMessageBuilderDataObject)
        {
            return new RitualNewChatter(fromIrcMessageBuilderDataObject.Message);
        }
    }
}