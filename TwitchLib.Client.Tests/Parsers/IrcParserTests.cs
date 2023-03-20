using TwitchLib.Client.Enums.Internal;
using TwitchLib.Client.Parsers;

using Xunit;

namespace TwitchLib.Client.Tests.Parsers;

public class IrcParserTests
{
    [Theory]
    [InlineData("PRIVMSG", IrcCommand.PrivMsg)]
    [InlineData("NOTICE", IrcCommand.Notice)]
    [InlineData("PING", IrcCommand.Ping)]
    [InlineData("PONG", IrcCommand.Pong)]
    [InlineData("CLEARCHAT", IrcCommand.ClearChat)]
    [InlineData("CLEARMSG", IrcCommand.ClearMsg)]
    [InlineData("USERSTATE", IrcCommand.UserState)]
    [InlineData("GLOBALUSERSTATE", IrcCommand.GlobalUserState)]
    [InlineData("NICK", IrcCommand.Nick)]
    [InlineData("JOIN", IrcCommand.Join)]
    [InlineData("PART", IrcCommand.Part)]
    [InlineData("PASS", IrcCommand.Pass)]
    [InlineData("CAP", IrcCommand.Cap)]
    [InlineData("001", IrcCommand.RPL_001)]
    [InlineData("002", IrcCommand.RPL_002)]
    [InlineData("003", IrcCommand.RPL_003)]
    [InlineData("004", IrcCommand.RPL_004)]
    [InlineData("353", IrcCommand.RPL_353)]
    [InlineData("366", IrcCommand.RPL_366)]
    [InlineData("372", IrcCommand.RPL_372)]
    [InlineData("375", IrcCommand.RPL_375)]
    [InlineData("376", IrcCommand.RPL_376)]
    [InlineData("WHISPER", IrcCommand.Whisper)]
    [InlineData("SERVERCHANGE", IrcCommand.ServerChange)]
    [InlineData("RECONNECT", IrcCommand.Reconnect)]
    [InlineData("ROOMSTATE", IrcCommand.RoomState)]
    [InlineData("USERNOTICE", IrcCommand.UserNotice)]
    [InlineData("MODE", IrcCommand.Mode)]
    [InlineData("", IrcCommand.Unknown)]
    [InlineData("a", IrcCommand.Unknown)]
    [InlineData("0", IrcCommand.Unknown)]
    [InlineData("*", IrcCommand.Unknown)]
    [InlineData(null, IrcCommand.Unknown)]
    public void GetIrcCommandFromStringTest(string commandAsString, IrcCommand expectedCommand)
    {
        IrcCommand command = IrcParser.GetIrcCommandFromString(commandAsString);
        Assert.Equal(expectedCommand, command);
    }
}
