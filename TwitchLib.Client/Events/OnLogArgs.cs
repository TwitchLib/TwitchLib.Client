using System;

namespace TwitchLib.Client.Events
{
    public class OnLogArgs : EventArgs
    {
        public string BotUsername;
        public string Data;
        public DateTime DateTime;
    }
}
