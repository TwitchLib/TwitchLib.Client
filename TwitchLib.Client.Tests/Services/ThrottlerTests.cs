using System.Threading.Tasks;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Interfaces;
using TwitchLib.Client.Services;

using Xunit;

namespace TwitchLib.Client.Tests.Services
{
    public class ThrottlerTests
    {
        [Fact]
        public void ThrottleTest()
        {
            uint sendsAllowedInPeriod = (uint) MessageRateLimit.Limit_20_in_30_Seconds;
            ISendOptions sendOptions = new SendOptions(sendsAllowedInPeriod);
            Throttler throttler = new Throttler(sendOptions);
            // send maximum items within period
            for (int i = 0; i < sendsAllowedInPeriod; i++)
            {
                // no throttling expected
                Assert.False(throttler.Throttle());
            }
            // maximum items within period should be exceeded
            // expect to throttle
            Assert.True(throttler.Throttle());
            // wait for throttlingperiod to exceed
            Task.Delay(sendOptions.ThrottlingPeriod).GetAwaiter().GetResult();
            // expect NO throttling
            Assert.False(throttler.Throttle());
        }
    }
}
