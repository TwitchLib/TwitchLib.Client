using TwitchLib.Communication.Events;

namespace TwitchLib.Client.Extensions;

/// <summary>
/// Extends logic for handling events.
/// </summary>
internal static class EventInvocationExtensions
{
    /// <summary>
    /// Invokes the event handler when it is not null. Returns a completed task otherwise.
    /// </summary>
    public static Task TryInvoke<TEventArgs>(this AsyncEventHandler<TEventArgs>? eventHandler, object? sender, TEventArgs eventArgs)
    {
        return eventHandler?.Invoke(sender, eventArgs) ?? Task.CompletedTask;
    }
}
