using System;

using TwitchLib.Client.Models.Interfaces;

namespace TwitchLib.Client.Services {
    internal class Throttler {
        private ISendOptions SendOptions { get; }
        private DateTime First { get; set; } = DateTime.UtcNow;
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
            } else if (this.SentItemCount >= this.SendOptions.SendsAllowedInPeriod) {
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
            return DateTime.UtcNow.Subtract(this.First) > this.SendOptions.ThrottlingPeriod;
        }
    }
}