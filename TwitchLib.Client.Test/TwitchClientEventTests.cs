using System;
using Xunit;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;
using System.Threading;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit.Sdk;
using System.Globalization;

namespace TwitchLib.Client.Test
{
    public class TwitchClientEventTests
    {
        private const string TWITCH_BOT_USERNAME = "testuser";
        private const string TWITCH_CHANNEL = "testchannel";
        private readonly MockIClient _mockClient;

        public TwitchClientEventTests()
        {
            _mockClient = new MockIClient();
        }

        [Fact]
        public async Task ClientCanReceiveData()
        {
            var client = new TwitchClient(_mockClient);
            await Assert.RaisesAsync<OnSendReceiveDataArgs>(
                    h => client.OnSendReceiveData += h,
                    h => client.OnSendReceiveData -= h,
                    async () =>
                    {
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                        client.Connect();
                        await _mockClient.ReceiveMessage($":tmi.twitch.tv 001 {TWITCH_BOT_USERNAME} :Welcome, GLHF!");
                    });
        }

        [Fact]
        public async Task ClientCanJoinChannels()
        {
            var client = new TwitchClient(_mockClient);
            client.OnConnected += async (sender, e) =>
            {
                client.JoinChannel(TWITCH_CHANNEL);
                await ReceivedRoomState();
            };

            await Assert.RaisesAsync<OnJoinedChannelArgs>(
                   h => client.OnJoinedChannel += h,
                   h => client.OnJoinedChannel -= h,
                   async () =>
                   {
                       client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                       client.Connect();
                       await ReceivedTwitchConnected();
                   });

        }

        [Fact]
        public async Task MessageEmoteCollectionFilled()
        {
            var finish = DateTime.Now.AddSeconds(10);
            var client = new TwitchClient(_mockClient);
            var emoteCount = 0;
            client.OnConnected += (sender, e) => ReceivedTestMessage();
            client.OnMessageReceived += async (sender, e) =>  emoteCount = e.ChatMessage.EmoteSet.Emotes.Count;

            client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
            client.Connect();
            await ReceivedTwitchConnected();

            while (emoteCount == 0 && DateTime.Now < finish)
            { }

            Assert.NotEqual((double)0, emoteCount, 0);
        }

        [Fact]
        public async void ClientRaisesOnConnected()
        {
            var client = new TwitchClient(_mockClient);

            await Assert.RaisesAsync<OnConnectedArgs>(
                    h => client.OnConnected += h,
                    h => client.OnConnected -= h,
                    async () =>
                    {
                        client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                        client.Connect();
                        await ReceivedTwitchConnected();
                    });
        }

        [Fact]
        public async Task ClientRaisesOnMessageReceived()
        {
            var client = new TwitchClient(_mockClient);

            await Assert.RaisesAsync<OnMessageReceivedArgs>(
                  h => client.OnMessageReceived += h,
                  h => client.OnMessageReceived -= h,
                  async () =>
                  {
                      client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                      client.Connect();
                      await ReceivedTestMessage();
                  });
        }

        [Fact]
        public async Task ClientRaisesOnJoinedChannel()
        {
            var client = new TwitchClient(_mockClient);

            await Assert.RaisesAsync<OnJoinedChannelArgs>(
                  h => client.OnJoinedChannel += h,
                  h => client.OnJoinedChannel -= h,
                  async () =>
                  {
                      client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                      client.Connect();
                      await ReceivedTwitchConnected();
                      client.JoinChannel(TWITCH_CHANNEL);
                      await ReceivedRoomState();
                  });
        }

        [Fact]
        public async Task ClientChannelAddedToJoinedChannels()
        {
            var client = new TwitchClient(_mockClient);
            client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
            client.Connect();
            await ReceivedTwitchConnected();
            client.JoinChannel(TWITCH_CHANNEL);

            Assert.Equal((double)1, client.JoinedChannels.Count,0);
        }

        [Fact]
        public async Task ClientRaisesOnDisconnected()
        {
            var client = new TwitchClient(_mockClient);

            await Assert.RaisesAsync<OnDisconnectedEventArgs>(
                  h => client.OnDisconnected += h,
                  h => client.OnDisconnected -= h,
                  async () =>
                  {
                      client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                      client.Connect();
                      await ReceivedTwitchConnected();
                      client.JoinChannel(TWITCH_CHANNEL);
                      await ReceivedRoomState();
                      client.Disconnect();
                  });
        }

        [Fact]
        public async Task ClientReconnectsOk()
        {
            var client = new TwitchClient(_mockClient);
            var pauseConnected = new ManualResetEvent(false);
            var pauseReconnected = new ManualResetEvent(false);

            await Assert.RaisesAsync<OnConnectedArgs>(
                h => client.OnReconnected += h,
                h => client.OnReconnected -= h,
                async () =>
                {
                    client.OnConnected += async (s, e) =>
                    {
                        pauseConnected.Set();
                        client.Disconnect();
                    };

                    client.OnDisconnected += async (s, e) => { client.Reconnect(); };
                    client.OnReconnected += async (s, e) => { pauseReconnected.Set(); };

                    client.Initialize(new Models.ConnectionCredentials(TWITCH_BOT_USERNAME, "OAuth"));
                    client.Connect();
                    await ReceivedTwitchConnected();

                    Assert.True(pauseConnected.WaitOne(5000));
                    Assert.True(pauseReconnected.WaitOne(60000));
                });
        }

        #region Messages for Tests

        private async Task ReceivedUserNoticeMessage()
        {
            await _mockClient.ReceiveMessage("@badges=subscriber/0;color=#0000FF;display-name=KittyJinxu;emotes=30259:0-6;id=1154b7c0-8923-464e-a66b-3ef55b1d4e50;login=kittyjinxu;mod=0;msg-id=ritual;msg-param-ritual-name=new_chatter;room-id=35740817;subscriber=1;system-msg=@KittyJinxu\\sis\\snew\\shere.\\sSay\\shello!;tmi-sent-ts=1514387871555;turbo=0;user-id=187446639;user-type= USERNOTICE #testchannel kittyjinxu > #testchannel: HeyGuys");
        }

        private async Task ReceivedTestMessage()
        {
            await _mockClient.ReceiveMessage("@badges=subscriber/0,premium/1;color=#005C0B;display-name=KIJUI;emotes=30259:0-6;id=fefffeeb-1e87-4adf-9912-ca371a18cbfd;mod=0;room-id=22510310;subscriber=1;tmi-sent-ts=1530128909202;turbo=0;user-id=25517628;user-type= :kijui!kijui@kijui.tmi.twitch.tv PRIVMSG #testchannel :TEST MESSAGE");
            // await _mockClient.ReceiveMessage(":jtv!jtv@jtv.tmi.twitch.tv PRIVMSG (HOSTED):(HOSTER) is now hosting you for (VIEWERS_TOTAL) viewers.");
        }

        private async Task ReceivedTwitchConnected()
        {
            await _mockClient.ReceiveMessage($":tmi.twitch.tv 001 {TWITCH_BOT_USERNAME} :Welcome, GLHF!");
            await _mockClient.ReceiveMessage($":tmi.twitch.tv 002 {TWITCH_BOT_USERNAME} :Your host is tmi.twitch.tv");
            await _mockClient.ReceiveMessage($":tmi.twitch.tv 003 {TWITCH_BOT_USERNAME} :This server is rather new");
            await _mockClient.ReceiveMessage($":tmi.twitch.tv 004 {TWITCH_BOT_USERNAME} :-");
            await _mockClient.ReceiveMessage($":tmi.twitch.tv 375 {TWITCH_BOT_USERNAME} :-");
            await _mockClient.ReceiveMessage($":tmi.twitch.tv 372 {TWITCH_BOT_USERNAME} :You are in a maze of twisty passages, all alike.");
            await _mockClient.ReceiveMessage($":tmi.twitch.tv 376 {TWITCH_BOT_USERNAME} :>");
            await _mockClient.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/membership");
            await _mockClient.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/commands");
            await _mockClient.ReceiveMessage(":tmi.twitch.tv CAP * ACK :twitch.tv/tags");
        }

        private async Task ReceivedRoomState()
        {
            await _mockClient.ReceiveMessage($"@broadcaster-lang=;r9k=0;slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #{TWITCH_CHANNEL}");
        }

        #endregion

        #region Modified Assert
        //TL;DR: Extracted version of XUNIT with
        //modification to accept new event Handler

        public partial class Assert
        {

            /// <summary>
            /// Verifies that a event with the exact event args (and not a derived type) is raised.
            /// </summary>
            /// <typeparam name="T">The type of the event arguments to expect</typeparam>
            /// <param name="attach">Code to attach the event handler</param>
            /// <param name="detach">Code to detach the event handler</param>
            /// <param name="testCode">A delegate to the code to be tested</param>
            /// <returns>The event sender and arguments wrapped in an object</returns>
            /// <exception cref="RaisesException">Thrown when the expected event was not raised.</exception>
            public static async Task<RaisedEvent<T>> RaisesAsync<T>(Action<AsyncEventHandler<T>> attach, Action<AsyncEventHandler<T>> detach, Func<Task> testCode)
            {
                var raisedEvent = await RaisesAsyncInternal(attach, detach, testCode);

                if (raisedEvent == null)
                    throw new RaisesException(typeof(T));

                if (raisedEvent.Arguments != null && !raisedEvent.Arguments.GetType().Equals(typeof(T)))
                    throw new RaisesException(typeof(T), raisedEvent.Arguments.GetType());

                return raisedEvent;
            }

            /// <summary>
            /// Verifies that an event with the exact or a derived event args is raised.
            /// </summary>
            /// <typeparam name="T">The type of the event arguments to expect</typeparam>
            /// <param name="attach">Code to attach the event handler</param>
            /// <param name="detach">Code to detach the event handler</param>
            /// <param name="testCode">A delegate to the code to be tested</param>
            /// <returns>The event sender and arguments wrapped in an object</returns>
            /// <exception cref="RaisesException">Thrown when the expected event was not raised.</exception>
            public static async Task<RaisedEvent<T>> RaisesAnyAsync<T>(Action<AsyncEventHandler<T>> attach, Action<AsyncEventHandler<T>> detach, Func<Task> testCode)
            {
                var raisedEvent = await RaisesAsyncInternal(attach, detach, testCode);

                if (raisedEvent == null)
                    throw new RaisesException(typeof(T));

                return raisedEvent;
            }

#if XUNIT_NULLABLE
		static async Task<RaisedEvent<T>?> RaisesAsyncInternal<T>(Action<EventHandler<T>> attach, Action<EventHandler<T>> detach, Func<Task> testCode)
#else
            static async Task<RaisedEvent<T>> RaisesAsyncInternal<T>(Action<AsyncEventHandler<T>> attach, Action<AsyncEventHandler<T>> detach, Func<Task> testCode)
#endif
            {
                NotNull(attach);
                NotNull(detach);
                NotNull(testCode);

#if XUNIT_NULLABLE
			RaisedEvent<T>? raisedEvent = null;
			void handler(object? s, T args) => raisedEvent = new RaisedEvent<T>(s, args);
#else
                RaisedEvent<T> raisedEvent = null;
                AsyncEventHandler<T> value = (object s, T args) =>
                {
                    raisedEvent = new RaisedEvent<T>(s, args);
                    return Task.CompletedTask;
                };
                AsyncEventHandler<T> handler = value;
#endif
                attach(handler);
                await testCode();
                detach(handler);
                return raisedEvent;
            }

            /// <summary>
            /// Represents a raised event after the fact.
            /// </summary>
            /// <typeparam name="T">The type of the event arguments.</typeparam>
            public class RaisedEvent<T>
            {
                /// <summary>
                /// The sender of the event.
                /// </summary>
#if XUNIT_NULLABLE
			public object? Sender { get; }
#else
                public object Sender { get; }
#endif

                /// <summary>
                /// The event arguments.
                /// </summary>
                public T Arguments { get; }

                /// <summary>
                /// Creates a new instance of the <see cref="RaisedEvent{T}" /> class.
                /// </summary>
                /// <param name="sender">The sender of the event.</param>
                /// <param name="args">The event arguments</param>
#if XUNIT_NULLABLE
			public RaisedEvent(object? sender, T args)
#else
                public RaisedEvent(object sender, T args)
#endif
                {
                    Sender = sender;
                    Arguments = args;
                }
            }


#if XUNIT_NULLABLE
		public static void False([DoesNotReturnIf(parameterValue: true)] bool condition)
#else
            public static void False(bool condition)
#endif
            {
                False((bool?)condition, null);
            }

            /// <summary>
            /// Verifies that the condition is false.
            /// </summary>
            /// <param name="condition">The condition to be tested</param>
            /// <exception cref="FalseException">Thrown if the condition is not false</exception>
#if XUNIT_NULLABLE
		public static void False([DoesNotReturnIf(parameterValue: true)] bool? condition)
#else
            public static void False(bool? condition)
#endif
            {
                False(condition, null);
            }

            /// <summary>
            /// Verifies that the condition is false.
            /// </summary>
            /// <param name="condition">The condition to be tested</param>
            /// <param name="userMessage">The message to show when the condition is not false</param>
            /// <exception cref="FalseException">Thrown if the condition is not false</exception>
#if XUNIT_NULLABLE
		public static void False([DoesNotReturnIf(parameterValue: true)] bool condition, string? userMessage)
#else
            public static void False(bool condition, string userMessage)
#endif
            {
                False((bool?)condition, userMessage);
            }

            /// <summary>
            /// Verifies that the condition is false.
            /// </summary>
            /// <param name="condition">The condition to be tested</param>
            /// <param name="userMessage">The message to show when the condition is not false</param>
            /// <exception cref="FalseException">Thrown if the condition is not false</exception>
#if XUNIT_NULLABLE
		public static void False([DoesNotReturnIf(parameterValue: true)] bool? condition, string? userMessage)
#else
            public static void False(bool? condition, string userMessage)
#endif
            {
                if (!condition.HasValue || condition.GetValueOrDefault())
                    throw new FalseException(userMessage, condition);
            }

            /// <summary>
            /// Verifies that an expression is true.
            /// </summary>
            /// <param name="condition">The condition to be inspected</param>
            /// <exception cref="TrueException">Thrown when the condition is false</exception>
#if XUNIT_NULLABLE
		public static void True([DoesNotReturnIf(parameterValue: false)] bool condition)
#else
            public static void True(bool condition)
#endif
            {
                True((bool?)condition, null);
            }

            /// <summary>
            /// Verifies that an expression is true.
            /// </summary>
            /// <param name="condition">The condition to be inspected</param>
            /// <exception cref="TrueException">Thrown when the condition is false</exception>
#if XUNIT_NULLABLE
		public static void True([DoesNotReturnIf(parameterValue: false)] bool? condition)
#else
            public static void True(bool? condition)
#endif
            {
                True(condition, null);
            }

            /// <summary>
            /// Verifies that an expression is true.
            /// </summary>
            /// <param name="condition">The condition to be inspected</param>
            /// <param name="userMessage">The message to be shown when the condition is false</param>
            /// <exception cref="TrueException">Thrown when the condition is false</exception>
#if XUNIT_NULLABLE
		public static void True([DoesNotReturnIf(parameterValue: false)] bool condition, string? userMessage)
#else
            public static void True(bool condition, string userMessage)
#endif
            {
                True((bool?)condition, userMessage);
            }

            /// <summary>
            /// Verifies that an expression is true.
            /// </summary>
            /// <param name="condition">The condition to be inspected</param>
            /// <param name="userMessage">The message to be shown when the condition is false</param>
            /// <exception cref="TrueException">Thrown when the condition is false</exception>
#if XUNIT_NULLABLE
		public static void True([DoesNotReturnIf(parameterValue: false)] bool? condition, string? userMessage)
#else
            public static void True(bool? condition, string userMessage)
#endif
            {
                if (!condition.HasValue || !condition.GetValueOrDefault())
                    throw new TrueException(userMessage, condition);
            }

            /// <summary>
            /// Verifies that a string contains a given sub-string, using the current culture.
            /// </summary>
            /// <param name="expectedSubstring">The sub-string expected to be in the string</param>
            /// <param name="actualString">The string to be inspected</param>
            /// <exception cref="ContainsException">Thrown when the sub-string is not present inside the string</exception>
#if XUNIT_NULLABLE
		public static void Contains(string expectedSubstring, string? actualString)
#else
            public static void Contains(string expectedSubstring, string actualString)
#endif
            {
                Contains(expectedSubstring, actualString, StringComparison.CurrentCulture);
            }

            /// <summary>
            /// Verifies that a string contains a given sub-string, using the given comparison type.
            /// </summary>
            /// <param name="expectedSubstring">The sub-string expected to be in the string</param>
            /// <param name="actualString">The string to be inspected</param>
            /// <param name="comparisonType">The type of string comparison to perform</param>
            /// <exception cref="ContainsException">Thrown when the sub-string is not present inside the string</exception>
#if XUNIT_NULLABLE
		public static void Contains(string expectedSubstring, string? actualString, StringComparison comparisonType)
#else
            public static void Contains(string expectedSubstring, string actualString, StringComparison comparisonType)
#endif
            {
                NotNull(expectedSubstring);

                if (actualString == null || actualString.IndexOf(expectedSubstring, comparisonType) < 0)
                    throw new ContainsException(expectedSubstring, actualString);
            }

            /// <summary>
            /// Verifies that a string does not contain a given sub-string, using the current culture.
            /// </summary>
            /// <param name="expectedSubstring">The sub-string which is expected not to be in the string</param>
            /// <param name="actualString">The string to be inspected</param>
            /// <exception cref="DoesNotContainException">Thrown when the sub-string is present inside the string</exception>
#if XUNIT_NULLABLE
		public static void DoesNotContain(string expectedSubstring, string? actualString)
#else
            public static void DoesNotContain(string expectedSubstring, string actualString)
#endif
            {
                DoesNotContain(expectedSubstring, actualString, StringComparison.CurrentCulture);
            }

            /// <summary>
            /// Verifies that a string does not contain a given sub-string, using the current culture.
            /// </summary>
            /// <param name="expectedSubstring">The sub-string which is expected not to be in the string</param>
            /// <param name="actualString">The string to be inspected</param>
            /// <param name="comparisonType">The type of string comparison to perform</param>
            /// <exception cref="DoesNotContainException">Thrown when the sub-string is present inside the given string</exception>
#if XUNIT_NULLABLE
		public static void DoesNotContain(string expectedSubstring, string? actualString, StringComparison comparisonType)
#else
            public static void DoesNotContain(string expectedSubstring, string actualString, StringComparison comparisonType)
#endif
            {
                NotNull(expectedSubstring);

                if (actualString != null && actualString.IndexOf(expectedSubstring, comparisonType) >= 0)
                    throw new DoesNotContainException(expectedSubstring, actualString);
            }

            /// <summary>
            /// Verifies that a string starts with a given string, using the current culture.
            /// </summary>
            /// <param name="expectedStartString">The string expected to be at the start of the string</param>
            /// <param name="actualString">The string to be inspected</param>
            /// <exception cref="ContainsException">Thrown when the string does not start with the expected string</exception>
#if XUNIT_NULLABLE
		public static void StartsWith(string? expectedStartString, string? actualString)
#else
            public static void StartsWith(string expectedStartString, string actualString)
#endif
            {
                StartsWith(expectedStartString, actualString, StringComparison.CurrentCulture);
            }

            /// <summary>
            /// Verifies that a string starts with a given string, using the given comparison type.
            /// </summary>
            /// <param name="expectedStartString">The string expected to be at the start of the string</param>
            /// <param name="actualString">The string to be inspected</param>
            /// <param name="comparisonType">The type of string comparison to perform</param>
            /// <exception cref="ContainsException">Thrown when the string does not start with the expected string</exception>
#if XUNIT_NULLABLE
		public static void StartsWith(string? expectedStartString, string? actualString, StringComparison comparisonType)
#else
            public static void StartsWith(string expectedStartString, string actualString, StringComparison comparisonType)
#endif
            {
                if (expectedStartString == null || actualString == null || !actualString.StartsWith(expectedStartString, comparisonType))
                    throw new StartsWithException(expectedStartString, actualString);
            }

            /// <summary>
            /// Verifies that a string ends with a given string, using the current culture.
            /// </summary>
            /// <param name="expectedEndString">The string expected to be at the end of the string</param>
            /// <param name="actualString">The string to be inspected</param>
            /// <exception cref="ContainsException">Thrown when the string does not end with the expected string</exception>
#if XUNIT_NULLABLE
		public static void EndsWith(string? expectedEndString, string? actualString)
#else
            public static void EndsWith(string expectedEndString, string actualString)
#endif
            {
                EndsWith(expectedEndString, actualString, StringComparison.CurrentCulture);
            }

            /// <summary>
            /// Verifies that a string ends with a given string, using the given comparison type.
            /// </summary>
            /// <param name="expectedEndString">The string expected to be at the end of the string</param>
            /// <param name="actualString">The string to be inspected</param>
            /// <param name="comparisonType">The type of string comparison to perform</param>
            /// <exception cref="ContainsException">Thrown when the string does not end with the expected string</exception>
#if XUNIT_NULLABLE
		public static void EndsWith(string? expectedEndString, string? actualString, StringComparison comparisonType)
#else
            public static void EndsWith(string expectedEndString, string actualString, StringComparison comparisonType)
#endif
            {
                if (expectedEndString == null || actualString == null || !actualString.EndsWith(expectedEndString, comparisonType))
                    throw new EndsWithException(expectedEndString, actualString);
            }

            /// <summary>
            /// Verifies that a string matches a regular expression.
            /// </summary>
            /// <param name="expectedRegexPattern">The regex pattern expected to match</param>
            /// <param name="actualString">The string to be inspected</param>
            /// <exception cref="MatchesException">Thrown when the string does not match the regex pattern</exception>
#if XUNIT_NULLABLE
		public static void Matches(string expectedRegexPattern, string? actualString)
#else
            public static void Matches(string expectedRegexPattern, string actualString)
#endif
            {
                NotNull(expectedRegexPattern);

                if (actualString == null || !Regex.IsMatch(actualString, expectedRegexPattern))
                    throw new MatchesException(expectedRegexPattern, actualString);
            }

            /// <summary>
            /// Verifies that a string matches a regular expression.
            /// </summary>
            /// <param name="expectedRegex">The regex expected to match</param>
            /// <param name="actualString">The string to be inspected</param>
            /// <exception cref="MatchesException">Thrown when the string does not match the regex</exception>
#if XUNIT_NULLABLE
		public static void Matches(Regex expectedRegex, string? actualString)
#else
            public static void Matches(Regex expectedRegex, string actualString)
#endif
            {
                NotNull(expectedRegex);

                if (actualString == null || !expectedRegex.IsMatch(actualString))
                    throw new MatchesException(expectedRegex.ToString(), actualString);
            }

            /// <summary>
            /// Verifies that a string does not match a regular expression.
            /// </summary>
            /// <param name="expectedRegexPattern">The regex pattern expected not to match</param>
            /// <param name="actualString">The string to be inspected</param>
            /// <exception cref="DoesNotMatchException">Thrown when the string matches the regex pattern</exception>
#if XUNIT_NULLABLE
		public static void DoesNotMatch(string expectedRegexPattern, string? actualString)
#else
            public static void DoesNotMatch(string expectedRegexPattern, string actualString)
#endif
            {
                NotNull(expectedRegexPattern);

                if (actualString != null && Regex.IsMatch(actualString, expectedRegexPattern))
                    throw new DoesNotMatchException(expectedRegexPattern, actualString);
            }

            /// <summary>
            /// Verifies that a string does not match a regular expression.
            /// </summary>
            /// <param name="expectedRegex">The regex expected not to match</param>
            /// <param name="actualString">The string to be inspected</param>
            /// <exception cref="DoesNotMatchException">Thrown when the string matches the regex</exception>
#if XUNIT_NULLABLE
		public static void DoesNotMatch(Regex expectedRegex, string? actualString)
#else
            public static void DoesNotMatch(Regex expectedRegex, string actualString)
#endif
            {
                NotNull(expectedRegex);

                if (actualString != null && expectedRegex.IsMatch(actualString))
                    throw new DoesNotMatchException(expectedRegex.ToString(), actualString);
            }

            /// <summary>
            /// Verifies that two strings are equivalent.
            /// </summary>
            /// <param name="expected">The expected string value.</param>
            /// <param name="actual">The actual string value.</param>
            /// <exception cref="EqualException">Thrown when the strings are not equivalent.</exception>
#if XUNIT_NULLABLE
		public static void Equal(string? expected, string? actual)
#else
            public static void Equal(string expected, string actual)
#endif
            {
                Equal(expected, actual, false, false, false);
            }

            /// <summary>
            /// Verifies that two strings are equivalent.
            /// </summary>
            /// <param name="expected">The expected string value.</param>
            /// <param name="actual">The actual string value.</param>
            /// <param name="ignoreCase">If set to <c>true</c>, ignores cases differences. The invariant culture is used.</param>
            /// <param name="ignoreLineEndingDifferences">If set to <c>true</c>, treats \r\n, \r, and \n as equivalent.</param>
            /// <param name="ignoreWhiteSpaceDifferences">If set to <c>true</c>, treats spaces and tabs (in any non-zero quantity) as equivalent.</param>
            /// <exception cref="EqualException">Thrown when the strings are not equivalent.</exception>
#if XUNIT_NULLABLE
		public static void Equal(
			string? expected,
			string? actual,
			bool ignoreCase = false,
			bool ignoreLineEndingDifferences = false,
			bool ignoreWhiteSpaceDifferences = false)
#else
            public static void Equal(
                string expected,
                string actual,
                bool ignoreCase = false,
                bool ignoreLineEndingDifferences = false,
                bool ignoreWhiteSpaceDifferences = false)
#endif
            {
#if XUNIT_SPAN
			if (expected == null && actual == null)
				return;
			if (expected == null || actual == null)
				throw new EqualException(expected, actual, -1, -1);

			Equal(expected.AsSpan(), actual.AsSpan(), ignoreCase, ignoreLineEndingDifferences, ignoreWhiteSpaceDifferences);
#else
                // Start out assuming the one of the values is null
                int expectedIndex = -1;
                int actualIndex = -1;
                int expectedLength = 0;
                int actualLength = 0;

                if (expected == null)
                {
                    if (actual == null)
                        return;
                }
                else if (actual != null)
                {
                    // Walk the string, keeping separate indices since we can skip variable amounts of
                    // data based on ignoreLineEndingDifferences and ignoreWhiteSpaceDifferences.
                    expectedIndex = 0;
                    actualIndex = 0;
                    expectedLength = expected.Length;
                    actualLength = actual.Length;

                    while (expectedIndex < expectedLength && actualIndex < actualLength)
                    {
                        char expectedChar = expected[expectedIndex];
                        char actualChar = actual[actualIndex];

                        if (ignoreLineEndingDifferences && IsLineEnding(expectedChar) && IsLineEnding(actualChar))
                        {
                            expectedIndex = SkipLineEnding(expected, expectedIndex);
                            actualIndex = SkipLineEnding(actual, actualIndex);
                        }
                        else if (ignoreWhiteSpaceDifferences && IsWhiteSpace(expectedChar) && IsWhiteSpace(actualChar))
                        {
                            expectedIndex = SkipWhitespace(expected, expectedIndex);
                            actualIndex = SkipWhitespace(actual, actualIndex);
                        }
                        else
                        {
                            if (ignoreCase)
                            {
                                expectedChar = Char.ToUpperInvariant(expectedChar);
                                actualChar = Char.ToUpperInvariant(actualChar);
                            }

                            if (expectedChar != actualChar)
                            {
                                break;
                            }

                            expectedIndex++;
                            actualIndex++;
                        }
                    }
                }

                if (expectedIndex < expectedLength || actualIndex < actualLength)
                {
                    throw new EqualException(expected, actual, expectedIndex, actualIndex);
                }
#endif
            }
            static bool IsLineEnding(char c)
            {
                return c == '\r' || c == '\n';
            }

            static bool IsWhiteSpace(char c)
            {
                return c == ' ' || c == '\t';
            }

            static int SkipLineEnding(string value, int index)
            {
                if (value[index] == '\r')
                {
                    ++index;
                }
                if (index < value.Length && value[index] == '\n')
                {
                    ++index;
                }

                return index;
            }

            static int SkipWhitespace(string value, int index)
            {
                while (index < value.Length)
                {
                    switch (value[index])
                    {
                        case ' ':
                        case '\t':
                            index++;
                            break;

                        default:
                            return index;
                    }
                }

                return index;
            }

            /// <summary>
            /// Verifies that an object reference is not null.
            /// </summary>
            /// <param name="object">The object to be validated</param>
            /// <exception cref="NotNullException">Thrown when the object reference is null</exception>
#if XUNIT_NULLABLE
		public static void NotNull([NotNull] object? @object)
#else
            public static void NotNull(object @object)
#endif
            {
                if (@object == null)
                    throw new NotNullException();
            }

            /// <summary>
            /// Verifies that an object reference is null.
            /// </summary>
            /// <param name="object">The object to be inspected</param>
            /// <exception cref="NullException">Thrown when the object reference is not null</exception>
#if XUNIT_NULLABLE
		public static void Null([MaybeNull] object? @object)
#else
            public static void Null(object @object)
#endif
            {
                if (@object != null)
                    throw new NullException(@object);
            }

            /// <summary>
            /// Indicates that the test should immediately fail.
            /// </summary>
            /// <param name="message">The failure message</param>
#if XUNIT_NULLABLE
		[DoesNotReturn]
#endif
            public static void Fail(string message)
            {
                NotNull(message);

                throw new FailException(message);
            }

#if XUNIT_SPAN
		/// <summary>
		/// Verifies that two arrays of unmanaged type T are equal, using Span&lt;T&gt;.SequenceEqual.
		/// </summary>
		/// <typeparam name="T">The type of items whose arrays are to be compared</typeparam>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The value to be compared against</param>
		/// <exception cref="EqualException">Thrown when the arrays are not equal</exception>
		/// <remarks>
		/// If Span&lt;T&gt;.SequenceEqual fails, a call to Assert.Equal(object, object) is made,
		/// to provide a more meaningful error message.
		/// </remarks>
#if XUNIT_NULLABLE
		public static void Equal<T>([AllowNull] T[] expected, [AllowNull] T[] actual)
#else
		public static void Equal<T>(T[] expected, T[] actual)
#endif
			where T : unmanaged, IEquatable<T>
		{
			if (expected == null && actual == null)
				return;

			// Call into Equal<object> so we get proper formatting of the sequence
			if (expected == null || actual == null || !expected.AsSpan().SequenceEqual(actual))
				Equal<object>(expected, actual);
		}
#endif

            /// <summary>
            /// Verifies that two <see cref="double"/> values are equal, within the number of decimal
            /// places given by <paramref name="precision"/>. The values are rounded before comparison.
            /// </summary>
            /// <param name="expected">The expected value</param>
            /// <param name="actual">The value to be compared against</param>
            /// <param name="precision">The number of decimal places (valid values: 0-15)</param>
            /// <exception cref="EqualException">Thrown when the values are not equal</exception>
            public static void Equal(double expected, double actual, int precision)
            {
                var expectedRounded = Math.Round(expected, precision);
                var actualRounded = Math.Round(actual, precision);

                if (!Object.Equals(expectedRounded, actualRounded))
                    throw new EqualException(
                        string.Format(CultureInfo.CurrentCulture, "{0} (rounded from {1})", expectedRounded, expected),
                        string.Format(CultureInfo.CurrentCulture, "{0} (rounded from {1})", actualRounded, actual)
                    );
            }

            /// <summary>
            /// Verifies that two <see cref="double"/> values are equal, within the number of decimal
            /// places given by <paramref name="precision"/>. The values are rounded before comparison.
            /// The rounding method to use is given by <paramref name="rounding" />
            /// </summary>
            /// <param name="expected">The expected value</param>
            /// <param name="actual">The value to be compared against</param>
            /// <param name="precision">The number of decimal places (valid values: 0-15)</param>
            /// <param name="rounding">Rounding method to use to process a number that is midway between two numbers</param>
            public static void Equal(double expected, double actual, int precision, MidpointRounding rounding)
            {
                var expectedRounded = Math.Round(expected, precision, rounding);
                var actualRounded = Math.Round(actual, precision, rounding);

                if (!Object.Equals(expectedRounded, actualRounded))
                    throw new EqualException(
                        string.Format(CultureInfo.CurrentCulture, "{0} (rounded from {1})", expectedRounded, expected),
                        string.Format(CultureInfo.CurrentCulture, "{0} (rounded from {1})", actualRounded, actual)
                    );
            }

            /// <summary>
            /// Verifies that two <see cref="double"/> values are equal, within the tolerance given by
            /// <paramref name="tolerance"/> (positive or negative).
            /// </summary>
            /// <param name="expected">The expected value</param>
            /// <param name="actual">The value to be compared against</param>
            /// <param name="tolerance">The allowed difference between values</param>
            /// <exception cref="ArgumentException">Thrown when supplied tolerance is invalid</exception>"
            /// <exception cref="EqualException">Thrown when the values are not equal</exception>
            public static void Equal(double expected, double actual, double tolerance)
            {
                if (double.IsNaN(tolerance) || double.IsNegativeInfinity(tolerance) || tolerance < 0.0)
                    throw new ArgumentException("Tolerance must be greater than or equal to zero", nameof(tolerance));

                if (!(double.Equals(expected, actual) || Math.Abs(expected - actual) <= tolerance))
                    throw new EqualException(
                        string.Format(CultureInfo.CurrentCulture, "{0:G17}", expected),
                        string.Format(CultureInfo.CurrentCulture, "{0:G17}", actual)
                    );
            }

            /// <summary>
            /// Verifies that two <see cref="float"/> values are equal, within the tolerance given by
            /// <paramref name="tolerance"/> (positive or negative).
            /// </summary>
            /// <param name="expected">The expected value</param>
            /// <param name="actual">The value to be compared against</param>
            /// <param name="tolerance">The allowed difference between values</param>
            /// <exception cref="ArgumentException">Thrown when supplied tolerance is invalid</exception>"
            /// <exception cref="EqualException">Thrown when the values are not equal</exception>
            public static void Equal(float expected, float actual, float tolerance)
            {
                if (float.IsNaN(tolerance) || float.IsNegativeInfinity(tolerance) || tolerance < 0.0)
                    throw new ArgumentException("Tolerance must be greater than or equal to zero", nameof(tolerance));

                if (!(float.Equals(expected, actual) || Math.Abs(expected - actual) <= tolerance))
                    throw new EqualException(
                        string.Format(CultureInfo.CurrentCulture, "{0:G9}", expected),
                        string.Format(CultureInfo.CurrentCulture, "{0:G9}", actual)
                    );
            }

            /// <summary>
            /// Verifies that two <see cref="decimal"/> values are equal, within the number of decimal
            /// places given by <paramref name="precision"/>. The values are rounded before comparison.
            /// </summary>
            /// <param name="expected">The expected value</param>
            /// <param name="actual">The value to be compared against</param>
            /// <param name="precision">The number of decimal places (valid values: 0-28)</param>
            /// <exception cref="EqualException">Thrown when the values are not equal</exception>
            public static void Equal(decimal expected, decimal actual, int precision)
            {
                var expectedRounded = Math.Round(expected, precision);
                var actualRounded = Math.Round(actual, precision);

                if (expectedRounded != actualRounded)
                    throw new EqualException(
                        string.Format(CultureInfo.CurrentCulture, "{0} (rounded from {1})", expectedRounded, expected),
                        string.Format(CultureInfo.CurrentCulture, "{0} (rounded from {1})", actualRounded, actual)
                    );
            }

            /// <summary>
            /// Verifies that two <see cref="DateTime"/> values are equal, within the precision
            /// given by <paramref name="precision"/>.
            /// </summary>
            /// <param name="expected">The expected value</param>
            /// <param name="actual">The value to be compared against</param>
            /// <param name="precision">The allowed difference in time where the two dates are considered equal</param>
            /// <exception cref="EqualException">Thrown when the values are not equal</exception>
            public static void Equal(DateTime expected, DateTime actual, TimeSpan precision)
            {
                var difference = (expected - actual).Duration();
                if (difference > precision)
                {
                    throw new EqualException(
                        string.Format(CultureInfo.CurrentCulture, "{0} ", expected),
                        string.Format(CultureInfo.CurrentCulture, "{0} difference {1} is larger than {2}",
                            actual,
                            difference.ToString(),
                            precision.ToString()
                        ));
                }
            }

#if XUNIT_SPAN
		/// <summary>
		/// Verifies that two arrays of unmanaged type T are not equal, using Span&lt;T&gt;.SequenceEqual.
		/// </summary>
		/// <typeparam name="T">The type of items whose arrays are to be compared</typeparam>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The value to be compared against</param>
		/// <exception cref="NotEqualException">Thrown when the arrays are equal</exception>
#if XUNIT_NULLABLE
		public static void NotEqual<T>([AllowNull] T[] expected, [AllowNull] T[] actual)
#else
		public static void NotEqual<T>(T[] expected, T[] actual)
#endif
			where T : unmanaged, IEquatable<T>
		{
			// Call into NotEqual<object> so we get proper formatting of the sequence
			if (expected == null && actual == null)
				NotEqual<object>(expected, actual);
			if (expected == null || actual == null)
				return;
			if (expected.AsSpan().SequenceEqual(actual))
				NotEqual<object>(expected, actual);
		}
#endif

            /// <summary>
            /// Verifies that two <see cref="double"/> values are not equal, within the number of decimal
            /// places given by <paramref name="precision"/>.
            /// </summary>
            /// <param name="expected">The expected value</param>
            /// <param name="actual">The value to be compared against</param>
            /// <param name="precision">The number of decimal places (valid values: 0-15)</param>
            /// <exception cref="EqualException">Thrown when the values are equal</exception>
            public static void NotEqual(double expected, double actual, int precision)
            {
                var expectedRounded = Math.Round(expected, precision);
                var actualRounded = Math.Round(actual, precision);

                if (Object.Equals(expectedRounded, actualRounded))
                    throw new NotEqualException(
                        string.Format(CultureInfo.CurrentCulture, "{0} (rounded from {1})", expectedRounded, expected),
                        string.Format(CultureInfo.CurrentCulture, "{0} (rounded from {1})", actualRounded, actual)
                    );
            }

            /// <summary>
            /// Verifies that two <see cref="decimal"/> values are not equal, within the number of decimal
            /// places given by <paramref name="precision"/>.
            /// </summary>
            /// <param name="expected">The expected value</param>
            /// <param name="actual">The value to be compared against</param>
            /// <param name="precision">The number of decimal places (valid values: 0-28)</param>
            /// <exception cref="EqualException">Thrown when the values are equal</exception>
            public static void NotEqual(decimal expected, decimal actual, int precision)
            {
                var expectedRounded = Math.Round(expected, precision);
                var actualRounded = Math.Round(actual, precision);

                if (expectedRounded == actualRounded)
                    throw new NotEqualException(
                        string.Format(CultureInfo.CurrentCulture, "{0} (rounded from {1})", expectedRounded, expected),
                        string.Format(CultureInfo.CurrentCulture, "{0} (rounded from {1})", actualRounded, actual)
                    );
            }



            #endregion

        }
    }
}
