using System;
using System.Reflection;

using Microsoft.Extensions.Logging;

using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Communication.Events;

namespace TwitchLib.Client
{
    public partial class TwitchClient : ITwitchClient_BackendLogging
    {
        // TraceMethodCall should use the Type of the interface,
        // that this class extends;
        // it makes it easier to find the respective occurance from the log file

        #region events public
        public event EventHandler<OnErrorEventArgs> OnError;
        public event EventHandler<OnLogArgs> OnLog;
        public event EventHandler<OnSendReceiveDataArgs> OnSendReceiveData;
        public event EventHandler<OnUnaccountedForArgs> OnUnaccountedFor;
        #endregion events public


        #region methods private
        private void UnaccountedFor(string ircString)
        {
            Log($"Unaccounted for: {ircString} (please create a TwitchLib GitHub issue :P)");
        }
        private void Log(string message, bool includeDate = false, bool includeTime = false)
        {
            string dateTimeStr;
            if (includeDate && includeTime)
                dateTimeStr = $"{DateTime.UtcNow}";
            else if (includeDate)
                dateTimeStr = $"{DateTime.UtcNow.ToShortDateString()}";
            else
                dateTimeStr = $"{DateTime.UtcNow.ToShortTimeString()}";

            if (includeDate || includeTime)
                LOGGER?.LogInformation($"[TwitchLib, {Assembly.GetExecutingAssembly().GetName().Version} - {dateTimeStr}] {message}");
            else
                LOGGER?.LogInformation($"[TwitchLib, {Assembly.GetExecutingAssembly().GetName().Version}] {message}");

            OnLog?.Invoke(this, new OnLogArgs { BotUsername = ConnectionCredentials?.TwitchUsername, Data = message, DateTime = DateTime.UtcNow });
        }
        private void LogError(string message, bool includeDate = false, bool includeTime = false)
        {
            string dateTimeStr;
            if (includeDate && includeTime)
                dateTimeStr = $"{DateTime.UtcNow}";
            else if (includeDate)
                dateTimeStr = $"{DateTime.UtcNow.ToShortDateString()}";
            else
                dateTimeStr = $"{DateTime.UtcNow.ToShortTimeString()}";

            if (includeDate || includeTime)
                LOGGER?.LogError($"[TwitchLib, {Assembly.GetExecutingAssembly().GetName().Version} - {dateTimeStr}] {message}");
            else
                LOGGER?.LogError($"[TwitchLib, {Assembly.GetExecutingAssembly().GetName().Version}] {message}");

            OnLog?.Invoke(this, new OnLogArgs { BotUsername = ConnectionCredentials?.TwitchUsername, Data = message, DateTime = DateTime.UtcNow });
        }
        #endregion methods private
    }
}
