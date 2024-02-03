using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace Michiru.Managers; 

public static class MessageTasks {
    /// <summary>
    /// Meant to be used with an IMessage Task to delete the message after a given amount of seconds
    /// </summary>
    /// <param name="message">this</param>
    /// <param name="seconds">time until deletion</param>
    /// <param name="reason">deletion reason (for Audit Log)</param>
    /// <returns>an Asynchronous Task</returns>
    /// <exception cref="ArgumentNullException">message is null</exception>
    public static Task<SocketUserMessage> DeleteAfter(this Task<SocketUserMessage> message, int seconds, string reason = "") {
        Task.Delay(TimeSpan.FromSeconds(seconds)).ContinueWith(async _ => await message.GetAwaiter().GetResult().DeleteAsync(new RequestOptions {AuditLogReason = reason}));
        return Task.FromResult(message.GetAwaiter().GetResult());
    }
    
    /// <summary>
    /// Meant to be used with an IUserMessage Task to delete the message after a given amount of seconds
    /// </summary>
    /// <param name="message">this</param>
    /// <param name="seconds">time until deletion</param>
    /// <param name="reason">deletion reason (for Audit Log)</param>
    /// <returns>an Asynchronous Task</returns>
    /// <exception cref="ArgumentNullException">message is null</exception>
    public static Task<IUserMessage> DeleteAfter(this Task<IUserMessage> message, int seconds, string reason = "") {
        Task.Delay(TimeSpan.FromSeconds(seconds)).ContinueWith(async _ => await message.GetAwaiter().GetResult().DeleteAsync(new RequestOptions {AuditLogReason = reason}));
        return Task.FromResult(message.GetAwaiter().GetResult());
    }
    
    /// <summary>
    /// Meant to be used with a RestUserMessage Task to delete the message after a given amount of seconds
    /// </summary>
    /// <param name="message">this</param>
    /// <param name="seconds">time until deletion</param>
    /// <param name="reason">deletion reason (for Audit Log)</param>
    /// <returns>an Asynchronous Task</returns>
    /// <exception cref="ArgumentNullException">message is null</exception>
    public static Task<RestUserMessage> DeleteAfter(this Task<RestUserMessage> message, int seconds, string reason = "") {
        Task.Delay(TimeSpan.FromSeconds(seconds)).ContinueWith(async _ => await message.GetAwaiter().GetResult().DeleteAsync(new RequestOptions {AuditLogReason = reason}));
        return Task.FromResult(message.GetAwaiter().GetResult());
    }
}