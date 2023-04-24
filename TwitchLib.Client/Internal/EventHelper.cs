using System;
using System.Reflection;

namespace TwitchLib.Client.Internal
{
    internal static class EventHelper
    {
        /// <summary>
        /// Raises the event.
        /// </summary>
        /// <param name="eventProvider">Event provider.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="args">The arguments.</param>
        internal static void RaiseEvent(this object eventProvider, string eventName, object args = null)
        {
            if (eventProvider == null)
            {
                return;
            }
            
            var fieldInfo = eventProvider.GetType().GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (!(fieldInfo?.GetValue(eventProvider) is MulticastDelegate multicastDelegate))
            {
                return;
            }
            
            var arguments = args == null
                ? new[] { eventProvider, EventArgs.Empty }
                : new[] { eventProvider, args };
            
            foreach (var del in multicastDelegate.GetInvocationList())
            {
                del.Method.Invoke(del.Target, arguments);
            }
        }
    }
}