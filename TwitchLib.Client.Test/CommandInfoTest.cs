using TwitchLib.Client.Models;
using Xunit;

namespace TwitchLib.Client.Test;

public class CommandInfoTest
{
    [Theory]
    [InlineData("", "")]
    [InlineData("!", "!")]
    [InlineData("!", "! command")]
    [InlineData("?", "!command")]
    public void ParsingFailAndReturnNull(string commandIdentifier, string message)
    {
        Assert.False(CommandInfo.TryParse(commandIdentifier, message, out var commandInfo));
        Assert.Null(commandInfo);
    }

    [Theory]
    [InlineData("!", "!command", 0)]
    [InlineData("!", "!command arg1", 1)]
    [InlineData("!", "!command arg1 arg2", 2)]
    [InlineData("!", "!command arg1 arg2 arg3 arg4", 4)]
    [InlineData("!", "!command \"arg1 with space\"", 1)]
    [InlineData("!", "!command \"arg1 with space\" \"arg2 with space\"", 2)]
    [InlineData("cmd!", "cmd!command", 0)]
    [InlineData("cmd!", "cmd!command arg1", 1)]
    [InlineData("cmd!", "cmd!command \"arg1 with space\" \"arg2 with space\"", 2)]
    public void Parsing(string commandIdentifier, string message, int argCount)
    {
        Assert.True(CommandInfo.TryParse(commandIdentifier, message, out var commandInfo));
        Assert.NotNull(commandInfo);
        Assert.Equal(argCount, commandInfo.ArgumentsAsList.Count);
    }
}
