using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace TwitchLib.Client.Helpers
{
    internal static class RaiseEventHelper
    {
        [SuppressMessage("Style", "IDE0058")]
        public static void RaiseEvent(object eventProvider, string eventName, object args)
        {
            object[] arguments = args == null ? new object[] { eventProvider, new EventArgs() } : new[] { eventProvider, args };
            FieldInfo fieldInfo = eventProvider.GetType().GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic);
            MulticastDelegate multicastDelegate = fieldInfo.GetValue(eventProvider) as MulticastDelegate;
            foreach (Delegate @delegate in multicastDelegate.GetInvocationList())
            {
                // IDE0058
                @delegate.Method.Invoke(@delegate.Target, arguments);
            }


#pragma warning disable CS0162
            if (false)
            {
                // for those who come here one time
                // and think about something like that
                EventInfo eventInfo = eventProvider.GetType().GetEvent(eventName);
                MethodInfo methodInfo = eventInfo.GetRaiseMethod(true);
                methodInfo.Invoke(eventProvider, arguments);
                // try it and take a look at
                // https://stackoverflow.com/questions/14885325
                // in March 2023 it didnt work
                // perhaps one day,
                // in a far future,
                // when im gone,
                // there might be a possibility,
                // it could work
                // :D
            }
#pragma warning restore CS0162
        }
    }
}