namespace TwitchLib.Client.Models.Builders
{
    public sealed class TwitchLibMessageBuilder : IBuilder<TwitchLibMessage>
    {
        private readonly TempTwitchLibMessage _message = new TempTwitchLibMessage();

        private TwitchLibMessageBuilder()
        {
            // TODO: Add with methods
        }

        public static TwitchLibMessageBuilder Create()
        {
            return new TwitchLibMessageBuilder();
        }

        public TwitchLibMessage Build()
        {
            return _message;
        }

        private class TempTwitchLibMessage : TwitchLibMessage
        {
        }
    }
}
