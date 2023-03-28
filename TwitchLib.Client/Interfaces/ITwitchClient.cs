using TwitchLib.Client.Models;

namespace TwitchLib.Client.Interfaces
{
    /// <summary>
    ///     Interface ITwitchClient
    /// </summary>
    public interface ITwitchClient :
        ITwitchClient_BackendLogging,
        ITwitchClient_Channel,
        ITwitchClient_Client,
        ITwitchClient_Connection,
        ITwitchClient_MessageReceiving,
        ITwitchClient_MessageSending,
        ITwitchClient_Notice,
        ITwitchClient_NoticeUser
    {
        #region properties public
        /// <summary>
        ///     Gets or sets a value indicating whether [disable automatic pong].
        /// </summary>
        bool DisableAutoPong { get; set; }
        /// <summary>
        ///     Gets the twitch username.
        ///     <br></br>
        ///     <see cref="ConnectionCredentials.TwitchUsername"/>
        /// </summary>
        string TwitchUsername { get; }
        /// <summary>
        ///     Gets or sets a value indicating whether [will replace emotes].
        /// </summary>
        bool WillReplaceEmotes { get; set; }
        #endregion properties public


        #region methods public
        /// <summary>
        ///     Adds the chat command identifier.
        /// </summary>
        /// <param name="identifier">
        ///     The identifier.
        /// </param>
        void AddChatCommandIdentifier(char identifier);
        /// <summary>
        ///     Removes the chat command identifier.
        /// </summary>
        /// <param name="identifier">
        ///     The identifier.
        /// </param>
        void RemoveChatCommandIdentifier(char identifier);
        #endregion methods public
    }
}
