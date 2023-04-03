using System;

using TwitchLib.Client.Models.Interfaces;

namespace TwitchLib.Client.Services {
    internal class Throttler {
        private ISendOptions SendOptions { get; }
        /// <summary>
        ///     <see cref="DateTime"/> of the first message
        ///     <br></br>
        ///     to determine, when <see cref="ISendOptions.ThrottlingPeriod"/> begins
        ///     <br></br>
        ///     <br></br>
        ///     has to be initialized with <see langword="null"/>
        ///     <br></br>
        ///     reason:
        ///     <list type="number">
        ///         <item>
        ///             <see cref="Throttler"/> gets instantiated and <see cref="First"/> gets initialized
        ///         </item>
        ///         <item>
        ///             <see cref="ISendOptions.ThrottlingPeriod"/> is always set to 30 seconds
        ///         </item>
        ///         <item>
        ///             start sending messages after 20 seconds after 'step 1' and hitting <see cref="ISendOptions.SendsAllowedInPeriod"/> (<see cref="Enums.MessageRateLimit"/>)
        ///         </item>
        ///         <item>
        ///             we dont take care about it and dont stop sending messages
        ///         </item>
        ///         <item>
        ///             <see cref="ISendOptions.ThrottlingPeriod"/> gets exceeded
        ///         </item>
        ///         <item>
        ///             <see cref="Reset"/> is called
        ///         </item>
        ///         <item>
        ///             but we still hit the Rate-Limit,
        ///             now server-side,
        ///             cause <see cref="ISendOptions.ThrottlingPeriod"/>
        ///             <br></br>
        ///             starts at 'step 1 (second 0) + step 3 (20 seconds)'
        ///             <br></br>
        ///             and ends at 'step 1 (second 0) + step 3 (20 seconds) + <see cref="ISendOptions.ThrottlingPeriod"/> (30 secconds)'
        ///             <br></br>
        ///             in other words: at second 50 calculated from step 1
        ///         </item>
        ///     </list>
        ///     <see href="https://dev.twitch.tv/docs/irc/#rate-limits"/>
        ///     <br></br>
        ///     1. paragraph:
        ///     <br></br>
        ///     The Twitch IRC server enforces the following limits.
        ///     It is up to your bot to keep track of its usage and not exceed the limits.
        ///     Rate limit counters begin when the server processes the first message and resets at the end of the window.
        ///     For example,
        ///     if the limit is 20 messages per 30 seconds,
        ///     the window starts when the server processes the first message
        ///     and lasts for 30 seconds.
        ///     At the end of the window,
        ///     the counter resets
        ///     and a new window begins with the next message.
        /// </summary>
        private DateTime? First { get; set; } = null;
        private uint SentItemCount { get; set; } = 0;
        public Throttler(ISendOptions sendOptions) {
            this.SendOptions = sendOptions;
        }
        /// <returns>
        ///     <see langword="false"/>, if the current item may be sent
        ///     <br></br>
        ///     <br></br>
        ///     <see langword="true"/>, if the current item <b>has to be throttled/may not be sent</b>
        /// </returns>
        public bool Throttle() {
            if (this.IsThrottlingPeriodExceeded()) {
                this.Reset();
            }
            // no else-if to catch `this.SendOptions.SendsAllowedInPeriod = 0`
            if (this.SentItemCount >= this.SendOptions.SendsAllowedInPeriod) {
                return true;
            }
            this.SentItemCount++;
            return false;
        }
        /// <summary>
        ///     resets
        ///     <list type="bullet">
        ///         <item>
        ///             <see cref="First"/>
        ///         </item>
        ///         <item>
        ///             <see cref="SentItemCount"/>
        ///         </item>
        ///     </list>
        /// </summary>
        private void Reset() {
            this.First = DateTime.UtcNow;
            this.SentItemCount = 0;
        }
        /// <summary>
        ///     checks if <see cref="ISendOptions.ThrottlingPeriod"/> exceeded
        /// </summary>
        /// <returns>
        ///     <see langword="true"/>, if the <see cref="TimeSpan"/> between <see cref="DateTime.UtcNow"/> and <see cref="First"/> is greater than <see cref="ISendOptions.ThrottlingPeriod"/>
        ///     <br></br>
        ///     <see langword="false"/> otherwise
        /// </returns>
        private bool IsThrottlingPeriodExceeded() {
            return this.First == null
                   || DateTime.UtcNow.Subtract((DateTime) this.First) > this.SendOptions.ThrottlingPeriod;
        }
    }
}