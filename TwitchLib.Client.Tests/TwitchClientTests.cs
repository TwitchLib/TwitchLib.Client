using System;
using System.Collections.Generic;
using System.Reflection;

using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

using Xunit;

namespace TwitchLib.Client.Tests
{
    public class TwitchClientTests : ATwitchClientTests<ITwitchClient>
    {
        [Fact]
        public void ConstructorTest()
        {
            try
            {
#pragma warning disable CS8625 // null-literal: should get tested
                ITwitchClient client = new TwitchClient(null);
#pragma warning restore CS8625 // null-literal: should get tested
                Assert.Fail($"{typeof(ArgumentNullException)} expected!");
            }
            catch (Exception e)
            {
                Assert.NotNull(e);
                Assert.IsType<ArgumentNullException>(e);
            }
        }
        [Fact]
        public void ChatCommandIdentifierTest()
        {
            ConnectionCredentials credentials = new ConnectionCredentials(TWITCH_Username, TWITCH_OAuth);
            ITwitchClient client = new TwitchClient(credentials);
            CheckChatCommandIdentifiers(client, 1, new[] { '!' });
            client.RemoveChatCommandIdentifier('!');
            CheckChatCommandIdentifiers(client, 0, Array.Empty<char>());
            client.AddChatCommandIdentifier('!');
            CheckChatCommandIdentifiers(client, 1, new[] { '!' });
            client.AddChatCommandIdentifier('!');
            CheckChatCommandIdentifiers(client, 1, new[] { '!' });
            client.AddChatCommandIdentifier('/');
            CheckChatCommandIdentifiers(client, 2, new[] { '!', '/' });
        }
        private static void CheckChatCommandIdentifiers(ITwitchClient client, int expectedSize, char[] expectedContent)
        {
            TypeInfo typeInfo = client.GetType().GetTypeInfo();
            PropertyInfo? propertyInfo = typeInfo.GetDeclaredProperty("ChatCommandIdentifiers");
            Assert.NotNull(propertyInfo);
            object? propertyValue = propertyInfo.GetValue(client);
            Assert.NotNull(propertyValue);
            ISet<char> chatCommandIdentifiers = Assert.IsAssignableFrom<ISet<char>>(propertyValue);
            Assert.Equal(expectedSize, chatCommandIdentifiers.Count);
            Assert.Equal<IEnumerable<char>>(expectedContent, chatCommandIdentifiers);
        }
    }
}
