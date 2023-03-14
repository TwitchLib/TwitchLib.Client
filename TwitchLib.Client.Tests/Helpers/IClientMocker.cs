using System;

using Moq;

using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;

namespace TwitchLib.Client.Tests.Helpers
{
    /// <summary>
    ///     <see href="https://github.com/Moq/moq4/wiki/Quickstart"/>
    ///     <br></br>
    ///     <br></br>
    ///     <see href="https://github.com/Moq/moq4/wiki/Quickstart#events"/>
    /// </summary>
    internal static class IClientMocker
    {
        /// <summary>
        ///     mocks up an <see cref="IClient"/>
        ///     <br></br>
        ///     <br></br>
        ///     <see cref="IClient.Send(String)"/> triggers,
        ///     <br></br>
        ///     <see cref="IClient"/> to raise <see cref="IClient.OnMessage"/>
        ///     <br></br>
        ///     with <paramref name="message"/> as <see cref="OnMessageEventArgs.Message"/>
        ///     <br></br>
        ///     <br></br>
        ///     Example:
        ///     <br></br>
        ///     <code>
        ///         IClient client = IClientMocker.GetMessageRaisingICLient(...);
        ///         client.Send(String.Empty);
        ///         // now the client raises OnMessage
        ///     </code>
        /// </summary>
        /// <param name="message">
        ///     <see cref="OnMessageEventArgs.Message"/>
        /// </param>
        /// <returns>
        ///     mocked <see cref="IClient"/>
        /// </returns>
        public static IClient GetMessageRaisingICLient(string message)
        {
            Mock<IClient> mock = new Mock<IClient>();

            mock.SetupAdd(c => c.OnMessage += It.IsAny<EventHandler<OnMessageEventArgs>>());

            mock.Setup(c => c.Send(It.IsAny<string>())).Returns(true)
                .Raises(c => c.OnMessage += null, new OnMessageEventArgs() { Message = message });

            IClient client = mock.Object;
            return client;
        }
    }
}
