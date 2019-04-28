namespace TwitchLib.Client.Models.Builders
{
    public sealed class ErrorEventBuilder : IBuilder<ErrorEvent>
    {
        private string _message;

        private ErrorEventBuilder()
        {
        }

        public static ErrorEventBuilder Create()
        {
            return new ErrorEventBuilder();
        }

        public ErrorEvent Build()
        {
            return new ErrorEvent
            {
                Message = _message
            };
        }
    }
}
