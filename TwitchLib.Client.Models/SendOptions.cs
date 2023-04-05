using System;

using TwitchLib.Client.Models.Interfaces;

namespace TwitchLib.Client.Models
{

    public class SendOptions : ISendOptions
    {
        public uint SendsAllowedInPeriod { get; }
        public ushort SendDelay { get; }
        public TimeSpan ThrottlingPeriod { get; } = TimeSpan.FromSeconds(30);
        public uint QueueCapacity { get; }
        public TimeSpan CacheItemTimeout { get; }
        /// <summary>
        /// </summary>
        /// <param name="sendsAllowedInPeriod">
        ///     a <see langword="value"/> of zero means: 
        ///     <br></br>
        ///     all messages that are enqueued to send
        ///     are going to be throttled!
        ///     <br></br>
        ///     <br></br>
        ///     <inheritdoc cref="SendsAllowedInPeriod"/>
        /// </param>
        /// <param name="queueCapacity">
        ///     <inheritdoc cref="QueueCapacity"/>
        /// </param>
        /// <param name="cacheItemTimeoutInMinutes">
        ///     <inheritdoc cref="CacheItemTimeout"/>
        /// </param>
        /// <param name="sendDelay">
        ///     <inheritdoc cref="SendDelay"/>
        /// </param>
        public SendOptions(uint sendsAllowedInPeriod,
                           uint queueCapacity = 10_000,
                           uint cacheItemTimeoutInMinutes = 30,
                           ushort sendDelay = 50)
        {
            SendsAllowedInPeriod = sendsAllowedInPeriod;
            QueueCapacity = queueCapacity;
            CacheItemTimeout = TimeSpan.FromMinutes(cacheItemTimeoutInMinutes);
            SendDelay = sendDelay;
        }
    }
}