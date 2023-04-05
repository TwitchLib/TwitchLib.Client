using System.Collections.Generic;

namespace TwitchLib.Client.Models.Interfaces
{
    public interface IAPIReferencesProvider
    {
        /// <summary>
        ///     Reference(s) to Twitch API
        ///     <br></br>
        ///     diamonds are forever ;)
        /// </summary>
        IEnumerable<string> APIReferences { get; }
    }
}
