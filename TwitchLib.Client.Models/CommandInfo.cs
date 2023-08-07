using System.Diagnostics.CodeAnalysis;

namespace TwitchLib.Client.Models;

/// <summary>Object representing a command received via Twitch chat.</summary>
public class CommandInfo
{
    /// <summary>Property representing the command identifier (ie command prefix).</summary>
    public char Identifier { get; }

    /// <summary>Property representing the actual command (without the command prefix).</summary>
    public string Name { get; }

    /// <summary>Property representing all arguments received in a string form.</summary>
    public string ArgumentsAsString { get; }

    /// <summary>Property representing all arguments received in a List form.</summary>
    public List<string> ArgumentsAsList { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandInfo"/> class.
    /// </summary>
    public CommandInfo(char identifier, string name) : this(identifier, name, string.Empty, new())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandInfo"/> class.
    /// </summary>
    public CommandInfo(char identifier, string name, string argumentsAsString, List<string> argumentsAsList)
    {
        Identifier = identifier;
        Name = name;
        ArgumentsAsString = argumentsAsString;
        ArgumentsAsList = argumentsAsList;
    }

    /// <summary>
    /// Tries to parse a span of characters into a value.
    /// </summary>
    /// <param name="s">The span of characters to parse.</param>
    /// <param name="result">When this method returns, contains the result of successfully parsing s, or an undefined value on failure.</param>
    /// <returns>true if s was successfully parsed; otherwise, false.</returns>
#if NETSTANDARD2_0
    public static bool TryParse(ReadOnlySpan<char> s, out CommandInfo result)
#else
    public static bool TryParse(ReadOnlySpan<char> s, [MaybeNullWhen(false)] out CommandInfo result)
#endif
    {
        result = default!;
        s = s.Trim();
        if (s.IsEmpty)
            return false;
        var commandIdentifier = s[0];
        s = s.Slice(1);
        if (s.IsEmpty || s[0] == ' ') // if string contains only the identifier or the first char after identifier is space, then it is invalid input
            return false;
        var indexOfSpace = s.IndexOf(' ');
        if (indexOfSpace == -1)
        {
            var name = s.ToString();
            result = new(commandIdentifier, name);
        }
        else
        {
            var name = s.Slice(0, indexOfSpace).ToString();
            s = s.Slice(indexOfSpace + 1).TrimStart();
            var argumentsAsString = s.ToString();
            result = new(commandIdentifier, name, argumentsAsString, ParseArgumentsToList(s));
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
