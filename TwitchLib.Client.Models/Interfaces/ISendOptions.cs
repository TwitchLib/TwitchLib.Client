using System;

namespace TwitchLib.Client.Models.Interfaces
{

    public interface ISendOptions
    {

        /// <summary>
        ///     Number of Messages Allowed Per Instance of the <see cref="ThrottlingPeriod"/>.
        ///     <br></br>
        ///     <br></br>
        ///     see also <see cref="ThrottlingPeriod"/>:
        ///     <br></br>
        ///     <inheritdoc cref="ThrottlingPeriod"/>
        ///     <br></br>
        ///     <br></br>
        ///     see also <see cref="Enums.MessageRateLimit"/>:
        ///     <br></br>
        ///     <inheritdoc cref="Enums.MessageRateLimit"/>
        /// </summary>
        uint SendsAllowedInPeriod { get; }

        /// <summary>
        /// Minimum time between sending items from the queue [in ms] (default 50ms).
        /// </summary>
        ushort SendDelay { get; }
        /// <summary>
        ///     Period Between each reset of the throttling instance window.
        ///     <br></br>
        ///     is always set to 30 seconds and you cannot change it
        ///     <br></br>
        ///     <br></br>
        ///     <list type="bullet">
        ///         <item>
        ///             <see href="https://dev.twitch.tv/docs/irc/#rate-limits"/>
        ///         </item>
        ///     </list>
        /// </summary>
        TimeSpan ThrottlingPeriod { get; }
        /// <summary>
        /// The amount of time an object can wait to be sent before it is considered dead, and should be skipped (default 30 minutes).
        /// A dead item will be ignored and removed from the send queue when it is hit.
        /// </summary>
        TimeSpan CacheItemTimeout { get; }

        /// <summary>
        /// Maximum number of Queued outgoing messages (default 10_000).
        /// </summary>
        uint QueueCapacity { get; }
    }
}