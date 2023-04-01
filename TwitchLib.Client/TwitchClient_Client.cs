using System;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions.Internal;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Internal;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;

namespace TwitchLib.Client
{
    public partial class TwitchClient : ITwitchClient_Client
    {
        // TraceMethodCall should use the Type of the interface,
        // that this class extends;
        // it makes it easier to find the respective occurance from the log file

        #region properties private
        private IClient Client { get; }
        private ClientProtocol Protocol { get; }
        #endregion properties private


        #region methods private
        private void InitializeClient()
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Client));
            Client.OnConnected += Client_OnConnected;
            Client.OnMessage += Client_OnMessage;
            Client.OnDisconnected += Client_OnDisconnected;
            Client.OnFatality += Client_OnFatality;
            // INFO: TwitchLib.Communication.IClient doesnt raise OnConnected when it reconnects!
            Client.OnReconnected += Client_OnConnected;
            Client.OnSendFailed += Client_OnSendFailed;
        }

        private void Client_OnSendFailed(object sender, OnSendFailedEventArgs e)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Client));
            OnSendFailed?.Invoke(this, e);
        }

        private void Client_OnFatality(object sender, OnFatalErrorEventArgs e)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Client));
            OnConnectionError?.Invoke(this, new OnConnectionErrorArgs { BotUsername = TwitchUsername, Error = new ErrorEvent { Message = e.Reason } });
        }
        private void Client_OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Client));
            OnDisconnected?.Invoke(sender, new OnDisconnectedArgs() { BotUsername = TwitchUsername });
        }
        private void Client_OnMessage(object sender, OnMessageEventArgs e)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Client));
            string[] stringSeparators = new[] { "\r\n" };
            string[] lines = e.Message.Split(stringSeparators, StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (line.Length <= 1)
                    continue;

                Log($"Received: {line}");
                OnSendReceiveData?.Invoke(this, new OnSendReceiveDataArgs { Direction = Enums.SendReceiveDirection.Received, Data = line });
                try
                {
                    HandleIrcMessage(line);
                }
                catch (Exception ex)
                {
                    LOGGER.LogExceptionAsError(GetType(), ex);
                    LogError(ex.ToString());
                }
            }
        }
        private void Client_OnConnected(object sender, OnConnectedEventArgs e)
        {
            LOGGER?.TraceMethodCall(typeof(ITwitchClient_Client));
            // IDE0058 - client raises OnSendFailed if this method returns false
            Client.Send(Rfc2812.Pass(ConnectionCredentials.TwitchOAuth));
            Client.Send(Rfc2812.Nick(TwitchUsername));
            Client.Send(Rfc2812.User(TwitchUsername, 0, TwitchUsername));

            if (ConnectionCredentials.Capabilities.Membership)
                Client.Send("CAP REQ twitch.tv/membership");
            if (ConnectionCredentials.Capabilities.Commands)
                Client.Send("CAP REQ twitch.tv/commands");
            if (ConnectionCredentials.Capabilities.Tags)
                Client.Send("CAP REQ twitch.tv/tags");
        }
        #endregion methods private
    }
}
