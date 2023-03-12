using System.Collections.Generic;

using TwitchLib.Client.Models;

namespace TwitchLib.Client.Interfaces
{
    public interface IChannelManageAble
    {
        /// <summary>
        ///     <see cref="IReadOnlyList{T}"/>
        /// </summary>
        IReadOnlyList<JoinedChannel> JoinedChannels { get; }
        /// <summary>
        ///     enqueues the given <paramref name="channel"/> to get joined
        /// </summary>
        /// <param name="channel">
        ///     name of the channel to join
        /// </param>
        /// <param name="overrideCheck">
        ///     <see langword="false"/> is the <see langword="feault"/>
        ///     <br></br>
        ///     <br></br>
        ///     in case of the <see langword="default"/> <see langword="false"/>, the channel wont be joined if its already joined
        ///     <br></br>
        ///     <br></br>
        ///     if set to <see langword="true"/>, the channel is going to be joined, though its probably joined already
        /// </param>
        void JoinChannel(string channel, bool overrideCheck = false);
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
        JoinedChannel GetJoinedChannel(string channel);
        /// <summary>
        ///     
        /// </summary>
        /// <param name="channel">
        ///     name of the channel to leave
        /// </param>
        void LeaveChannel(string channel);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel">
        ///     <see cref="JoinedChannel"/> to leave
        /// </param>
        void LeaveChannel(JoinedChannel channel);

    }
}
