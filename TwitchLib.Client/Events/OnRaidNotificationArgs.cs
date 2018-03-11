using System;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Events
{
    public class OnRaidNotificationArgs : EventArgs
    {
        public RaidNotification RaidNotificaiton;
        public string Channel;
    }
}
