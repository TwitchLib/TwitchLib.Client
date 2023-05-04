using System;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Interfaces;

namespace TwitchLib.Client.Throttling
{
    internal class Throttler
    {
        private readonly ISendOptions _sendOptions;
        private DateTime? _firstMessage;
        private uint _sentItemCount;

        internal Throttler(ISendOptions sendOptions)
        {
            _sendOptions = sendOptions ?? new SendOptions();
        }

        /// <returns>
        ///     <see langword="false"/>, if the current item may be sent
        ///     <see langword="true"/>, if the current item <b>has to be throttled/may not be sent</b>
        /// </returns>
        public bool ShouldThrottle()
        {
            if (IsThrottlingPeriodExceeded())
            {
                Reset();
            }

            // no else-if to catch `this.SendOptions.SendsAllowedInPeriod = 0`
            if (_sentItemCount >= _sendOptions.SendsAllowedInPeriod)
            {
                return true;
            }

            _sentItemCount++;
            return false;
        }

        private void Reset()
        {
            _firstMessage = DateTime.UtcNow;
            _sentItemCount = 0;
        }

        private bool IsThrottlingPeriodExceeded()
        {
            return _firstMessage == null || 
                   DateTime.UtcNow.Subtract(_firstMessage.Value) > _sendOptions.ThrottlingPeriod;
        }
    }
}