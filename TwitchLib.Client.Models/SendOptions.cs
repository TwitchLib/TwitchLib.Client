using System;
using TwitchLib.Client.Models.Interfaces;

namespace TwitchLib.Client.Models
{
    public class SendOptions : ISendOptions
    {
        /// <inheritdoc/>
        public uint SendsAllowedInPeriod { get; }

        /// <inheritdoc/>
        public ushort SendDelay { get; }

        /// <inheritdoc/>
        public TimeSpan ThrottlingPeriod { get; } = TimeSpan.FromSeconds(30);

        /// <inheritdoc/>
        public uint QueueCapacity { get; }

        /// <inheritdoc/>
        public TimeSpan CacheItemTimeout { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendOptions"/> class.
        /// </summary>
        /// <param name="sendsAllowedInPeriod">
        ///     A <see langword="value"/> of zero means: 
        ///     <br></br>
        ///     all messages that are enqueued to send
        ///     are going to be throttled!
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
        public SendOptions(
            uint sendsAllowedInPeriod = 20,
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