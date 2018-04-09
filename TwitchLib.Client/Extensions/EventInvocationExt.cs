using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace TwitchLib.Client.Extensions
{
    public static class EventInvocationExt
    {
        public static void InvokeOnBeingHosted(this TwitchClient client, string channel, string botUsername, string hostedByChannel, int viewers, bool isAutoHosted)
        {
            var model = new OnBeingHostedArgs()
            {
                BeingHostedNotification = new BeingHostedNotification(channel, botUsername, hostedByChannel, viewers, isAutoHosted)
            };
            client._raiseEvent("OnBeingHosted", model);
        }

        public static void InvokeChannelStateChanged(this TwitchClient client, string channel, bool r9k, bool rituals, 
            bool subOnly, int slowMode, bool emoteOnly, string broadcasterLanguage, TimeSpan followersOnly, bool mercury, string roomId)
        {
            var state = new ChannelState(r9k, rituals, subOnly, slowMode, emoteOnly, broadcasterLanguage, channel, followersOnly, mercury, roomId);
            var model = new OnChannelStateChangedArgs()
            {
                Channel = channel,
                ChannelState = state
            };
            client._raiseEvent("OnChannelStateChanged", model);
        }

        public static void InvokeChatCleared(this TwitchClient client, string channel)
        {
            var model = new OnChatClearedArgs()
            {
                Channel = channel
            };
            client._raiseEvent("OnChatCleared", model);
        }
    }
}
