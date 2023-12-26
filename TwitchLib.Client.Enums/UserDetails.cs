using System;

namespace TwitchLib.Client.Enums;

[Flags]
public enum UserDetails
{
    None = 0,
    Moderator = 1,
    Turbo = 2,
    Subscriber = 4,
    Vip = 8,

    Partner = 16,
    Staff = 32,
}
