using System;

using Moq;

using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;
using TwitchLib.Communication.Models;

namespace TwitchLib.Client.Tests.TestHelper
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
            Mock<IClient> mock = GetIClientMock();
            mock.Setup(c => c.Send(It.IsAny<string>()))
                .Returns(true)
                .Raises(c => c.OnMessage += null, new OnMessageEventArgs() { Message = message });

            IClient client = mock.Object;
            return client;
        }
        public static Mock<IClient> GetIClientMock()
        {
            Mock<IClient> mock = new Mock<IClient>();
            //
            mock.Setup(c => c.IsConnected).Returns(true);
            mock.Setup(c => c.Options).Returns(new ClientOptions());
            //
            mock.SetupAdd(c => c.OnConnected += It.IsAny<EventHandler<OnConnectedEventArgs>>());
            mock.Setup<bool>(c => c.Open())
                .Returns(true)
                .Raises(c => c.OnConnected += null, new OnConnectedEventArgs());
            //
            mock.SetupAdd(c => c.OnDisconnected += It.IsAny<EventHandler<OnDisconnectedEventArgs>>());
            mock.Setup(c => c.Close())
                .Raises(c => c.OnDisconnected += null, new OnDisconnectedEventArgs());
            //
            mock.SetupAdd(c => c.OnReconnected += It.IsAny<EventHandler<OnConnectedEventArgs>>());
            mock.Setup<bool>(c => c.Reconnect())
                .Returns(true)
                .Raises(c => c.OnReconnected += null, new OnConnectedEventArgs());
            //
            mock.SetupAdd(c => c.OnFatality += It.IsAny<EventHandler<OnFatalErrorEventArgs>>());
            mock.Setup(c => c.Send(It.IsAny<string>()))
                .Returns(false)
                .Raises(c => c.OnFatality += null, new OnFatalErrorEventArgs("Fatal network error."));

            mock.SetupAdd(c => c.OnMessage += It.IsAny<EventHandler<OnMessageEventArgs>>());
            return mock;
        }
        /// <summary>
        ///     adds message to <paramref name="sendMessageSequence"/>
        ///     <br></br>
        ///     a call to <see cref="IClient.Send(String)"/> raises the given <paramref name="messageToSend"/>
        /// </summary>
        /// <param name="messageToSend">
        ///     message wich <see cref="IClient"/> raises with the next call to <see cref="IClient.Send(String)"/>
        /// </param>
        /// <param name="mock">
        ///     affected <see cref="Mock{T}"/> of <see cref="IClient"/>
        /// </param>
        /// <param name="sendMessageSequence">
        ///     <see cref="MockSequence"/>, the action should be added to
        /// </param>
        public static void AddToSendMessageSequence(string messageToSend,
                                                    Mock<IClient> mock,
                                                    MockSequence sendMessageSequence)
        {
            mock.InSequence(sendMessageSequence)
                            .Setup(c => c.Send(It.IsAny<string>()))
                            .Returns(true)
                            .Raises(c => c.OnMessage += null, new OnMessageEventArgs() { Message = messageToSend });
        }
        /// <summary>
        ///     adds joining a channel to the given <paramref name="sendMessageSequence"/>
        /// </summary>
        /// <param name="messageExpectedToJoin">
        ///     the message, we expect is passed to <see cref="IClient.Send(String)"/> 
        /// </param>
        /// <param name="messageJoinCompleted">
        ///     message with wich <see cref="IClient"/> replies to <paramref name="messageExpectedToJoin"/>
        /// </param>
        /// <param name="mock">
        ///     affected <see cref="Mock{T}"/> of <see cref="IClient"/>
        /// </param>
        /// <param name="sendMessageSequence">
        ///     <see cref="MockSequence"/>, the action should be added to
        /// </param>
        public static void AddJoinToSendMessageSequence(string messageExpectedToJoin,
                                                        string messageJoinCompleted,
                                                        Mock<IClient> mock,
                                                        MockSequence sendMessageSequence)
        {
            mock.InSequence(sendMessageSequence)
                            .Setup(c => c.Send(It.Is<string>(s => String.Equals(messageExpectedToJoin, s))))
                            .Returns(true)
                            .Raises(c => c.OnMessage += null, new OnMessageEventArgs() { Message = messageJoinCompleted });
        }
        /// <summary>
        ///     adds the sequence of messages that the <see cref="ITwitchClient"/> is going to send,
        ///     <br></br>
        ///     after <see cref="IClient"/> raises <see cref="IClient.OnConnected"/> or <see cref="IClient.OnReconnected"/>
        /// </summary>
        /// <param name="messageLogin">
        /// </param>
        /// <param name="mock">
        ///     affected <see cref="Mock{T}"/> of <see cref="IClient"/>
        /// </param>
        /// <param name="sendMessageSequence">
        ///     <see cref="MockSequence"/> that holds the actions for sequential calls to <see cref="IClient.Send(String)"/>
        /// </param>
        public static void AddLogInToSendMessageSequence(string messageLogin,
                                                         Mock<IClient> mock,
                                                         MockSequence sendMessageSequence)
        {
            // ITwichClient.Client_OnConnected sends PASS
            mock.InSequence(sendMessageSequence)
                .Setup(c => c.Send(It.IsAny<string>()))
                .Returns(true);
            // ITwichClient.Client_OnConnected sends NICK
            mock.InSequence(sendMessageSequence)
                .Setup(c => c.Send(It.IsAny<string>()))
                .Returns(true);
            // ITwichClient.Client_OnConnected sends USER
            mock.InSequence(sendMessageSequence)
                .Setup(c => c.Send(It.IsAny<string>()))
                .Returns(true);
            // ITwichClient.Client_OnConnected sends CAP membership
            mock.InSequence(sendMessageSequence)
                .Setup(c => c.Send(It.IsAny<string>()))
                .Returns(true);
            // ITwichClient.Client_OnConnected sends CAP commands
            mock.InSequence(sendMessageSequence)
                .Setup(c => c.Send(It.IsAny<string>()))
                .Returns(true);
            // ITwichClient.Client_OnConnected sends CAP tags
            // only this last call to IClient.Send() has to trigger raise OnMessage
            mock.InSequence(sendMessageSequence)
                .Setup(c => c.Send(It.IsAny<string>()))
                .Returns(true)
                .Raises(c => c.OnMessage += null, new OnMessageEventArgs() { Message = messageLogin });
        }
    }
}
