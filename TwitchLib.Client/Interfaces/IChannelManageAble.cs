using System.Collections.Generic;

using TwitchLib.Client.Models;

namespace TwitchLib.Client.Interfaces
{
    public interface IChannelManageAble
    {
        #region properties public
        /// <summary>
        ///     <see cref="IReadOnlyList{T}"/>
        /// </summary>
        IReadOnlyList<JoinedChannel> JoinedChannels { get; }
        #endregion properties public


        #region methods public
        /// <summary>
        ///     enqueues the given <paramref name="channel"/> to get joined
        /// </summary>
        /// <param name="channel">
        ///     name of the channel to join
        /// </param>
        void JoinChannel(string? channel);
        /// <summary>
        ///     enqueues the given <paramref name="channels"/> to get joined
        /// </summary>
        /// <param name="channels">
        ///     names of the channels to join
        /// </param>
        void JoinChannels(IEnumerable<string?> channels);
        /// <summary>
        ///     returns the <see cref="JoinedChannel"/> of the channel with the given <paramref name="channel"/>-name
        ///     <br></br>
        ///     <br></br>
        ///     <see langword="null"/> if its (still) not joined
        /// </summary>
        /// <param name="channel">
        ///     name of the channel
        /// </param>
        /// <returns>
        ///     <see cref="JoinedChannel"/> if the channel with the given <paramref name="channel"/>-name is joined
        ///     <br></br>
        ///     <br></br>
        ///     <see langword="null"/> otherwise (if its (still) not joined)
        /// </returns>
        JoinedChannel? GetJoinedChannel(string? channel);
        /// <summary>
        ///     
        /// </summary>
        /// <param name="channel">
        ///     name of the channel to leave
        /// </param>
        void LeaveChannel(string? channel);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel">
        ///     <see cref="JoinedChannel"/> to leave
        /// </param>
        void LeaveChannel(JoinedChannel? channel);
        #endregion methods public
    }
}
