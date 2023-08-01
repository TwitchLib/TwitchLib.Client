using TwitchLib.Client.Models;
using Xunit;

namespace TwitchLib.Client.Test;

public class CommandInfoTest
{
    [Theory]
    [InlineData("")]
    [InlineData("!")]
    [InlineData("! command")]
    public void ParsingFailAndReturnNull(string s)
    {
        Assert.False(CommandInfo.TryParse(s, out CommandInfo commandInfo));
        Assert.Null(commandInfo);
    }
}
