using System;
using Xunit;
using TwitchLib.Communication.Events;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace TwitchLib.Client.Test;

//TL;DR: XUNIT with modification to accept async event Handler
public partial class MyAssert
{
    /// <summary>
    /// Verifies that a event with the exact event args (and not a derived type) is raised.
    /// </summary>
    /// <typeparam name="T">The type of the event arguments to expect</typeparam>
    /// <param name="attach">Code to attach the event handler</param>
    /// <param name="detach">Code to detach the event handler</param>
    /// <param name="testCode">A delegate to the code to be tested</param>
    /// <returns>The event sender and arguments wrapped in an object</returns>
    /// <exception cref="RaisesException">Thrown when the expected event was not raised.</exception>
    public static async Task<RaisedEvent<T>> RaisesAsync<T>(Action<AsyncEventHandler<T>> attach, Action<AsyncEventHandler<T>> detach, Func<Task> testCode)
    {
        var raisedEvent = await RaisesAsyncInternal(attach, detach, testCode);
        
        if (raisedEvent == null)
            throw RaisesException.ForNoEvent(typeof(T));

        if (raisedEvent.Arguments != null && !raisedEvent.Arguments.GetType().Equals(typeof(T)))
            throw RaisesException.ForIncorrectType(typeof(T), raisedEvent.Arguments.GetType());

        return raisedEvent;
    }

    /// <summary>
    /// Verifies that an event with the exact or a derived event args is raised.
    /// </summary>
    /// <typeparam name="T">The type of the event arguments to expect</typeparam>
    /// <param name="attach">Code to attach the event handler</param>
    /// <param name="detach">Code to detach the event handler</param>
    /// <param name="testCode">A delegate to the code to be tested</param>
    /// <returns>The event sender and arguments wrapped in an object</returns>
    /// <exception cref="RaisesException">Thrown when the expected event was not raised.</exception>
    public static async Task<RaisedEvent<T>> RaisesAnyAsync<T>(Action<AsyncEventHandler<T>> attach, Action<AsyncEventHandler<T>> detach, Func<Task> testCode)
    {
        var raisedEvent = await RaisesAsyncInternal(attach, detach, testCode);

        if (raisedEvent == null)
            throw RaisesException.ForNoEvent(typeof(T));

        return raisedEvent;
    }

    static async Task<RaisedEvent<T>?> RaisesAsyncInternal<T>(Action<AsyncEventHandler<T>> attach, Action<AsyncEventHandler<T>> detach, Func<Task> testCode)
    {
        Assert.NotNull(attach);
        Assert.NotNull(detach);
        Assert.NotNull(testCode);

        RaisedEvent<T>? raisedEvent = null;
        AsyncEventHandler<T> value = (object? s, T args) =>
        {
            raisedEvent = new RaisedEvent<T>(s, args);
            return Task.CompletedTask;
        };
        AsyncEventHandler<T> handler = value;
        attach(handler);
        await testCode();
        detach(handler);
        return raisedEvent;
    }

    /// <summary>
    /// Represents a raised event after the fact.
    /// </summary>
    /// <typeparam name="T">The type of the event arguments.</typeparam>
    public class RaisedEvent<T>
    {
        /// <summary>
        /// The sender of the event.
        /// </summary>
        public object? Sender { get; }

        /// <summary>
        /// The event arguments.
        /// </summary>
        public T Arguments { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="RaisedEvent{T}" /> class.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The event arguments</param>
        public RaisedEvent(object? sender, T args)
        {
            Sender = sender;
            Arguments = args;
        }
    }
}
