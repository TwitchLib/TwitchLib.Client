using System.Diagnostics.CodeAnalysis;

namespace TwitchLib.Client.Models;

/// <summary>Object representing a command received via Twitch chat.</summary>
public class CommandInfo
{
    /// <summary>Property representing the command identifier (ie command prefix).</summary>
    public string Identifier { get; }

    /// <summary>Property representing the actual command (without the command prefix).</summary>
    public string Name { get; }

    /// <summary>Property representing all arguments received in a string form.</summary>
    public string ArgumentsAsString { get; }

    /// <summary>Property representing all arguments received in a List form.</summary>
    public List<string> ArgumentsAsList { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandInfo"/> class.
    /// </summary>
    public CommandInfo(string identifier, string name) : this(identifier, name, string.Empty, new())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandInfo"/> class.
    /// </summary>
    public CommandInfo(string identifier, string name, string argumentsAsString, List<string> argumentsAsList)
    {
        Identifier = identifier;
        Name = name;
        ArgumentsAsString = argumentsAsString;
        ArgumentsAsList = argumentsAsList;
    }

    /// <summary>
    /// Tries to parse a message with specified command identifier into a value.
    /// </summary>
    /// <returns>true if s was successfully parsed; otherwise, false.</returns>
#if NETSTANDARD2_0
    internal static bool TryParse(string commandIdentifier, ReadOnlySpan<char> message, out CommandInfo result)
#else
    internal static bool TryParse(string commandIdentifier, ReadOnlySpan<char> message, [MaybeNullWhen(false)] out CommandInfo result)
#endif
    {
        result = default!;
        if(!message.StartsWith(commandIdentifier.AsSpan()))
            return false;

        message = message.Slice(commandIdentifier.Length);
        if (message.IsEmpty || message[0] == ' ') // if string contains only the identifier or the first char after identifier is space, then it is invalid input
            return false;
        var indexOfSpace = message.IndexOf(' ');
        if (indexOfSpace == -1)
        {
            var name = message.ToString();
            result = new(commandIdentifier, name);
        }
        else
        {
            var name = message.Slice(0, indexOfSpace).ToString();
            message = message.Slice(indexOfSpace + 1).TrimStart();
            var argumentsAsString = message.ToString();
            result = new(commandIdentifier, name, argumentsAsString, ParseArgumentsToList(message));
        }
        return true;

        static List<string> ParseArgumentsToList(ReadOnlySpan<char> s)
        {
            int index;
            var arguments = new List<string>();
            while (!s.IsEmpty)
            {
                bool isQuote = s[0] == '"';
                if (s[0] == '"')
                {
                    s = s.Slice(1);
                    index = s.IndexOf('"');
                }
                else
                {
                    index = s.IndexOfAny('"', ' ');
                }
                if (index == -1)
                {
                    arguments.Add(s.ToString());
                    s = default;
                }
                else
                {
                    arguments.Add(s.Slice(0, index).ToString());
                    if (!isQuote && s[index] == '"') // s"txt"   we dont want remove quote after s
                        index--;
                    s = s.Slice(index + 1);
                }
                s = s.TrimStart();
            }
            return arguments;
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return ArgumentsAsString.Length == 0
            ? $"{Identifier}{Name}"
            : $"{Identifier}{Name} {ArgumentsAsString}";
    }
}
