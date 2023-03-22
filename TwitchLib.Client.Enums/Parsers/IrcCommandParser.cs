using System;

namespace TwitchLib.Client.Enums.Parsers
{
    public static class IrcCommandParser
    {
        /// <summary>
        ///     tries to parse the given <paramref name="cmd"/> as <see cref="IrcCommand"/>
        ///     <br></br>
        ///     <br></br>
        ///     case-in-sensitive
        /// </summary>
        /// <param name="cmd">
        ///     <see cref="IrcCommand"/> as string
        /// </param>
        /// <returns>
        ///     the matching <see cref="IrcCommand"/>
        ///     <br></br>
        ///     <br></br>
        ///     <see cref="IrcCommand.Unknown"/>, if no <see cref="IrcCommand"/> matches the given <paramref name="cmd"/>
        /// </returns>
        public static IrcCommand GetIrcCommandFromString(string cmd)
        {
            IrcCommand result;
            // first try to parse with prefix "rpl_"
            // otherwise the parser interprets the numeric string as ordinal
            bool parsed = Enum.TryParse<IrcCommand>($"rpl_{cmd}", true, out result);
            if (!parsed) parsed = Enum.TryParse<IrcCommand>(cmd, true, out result);
            if (!parsed) result = IrcCommand.Unknown;
            return result;
        }
    }
}
