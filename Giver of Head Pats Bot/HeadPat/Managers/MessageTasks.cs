using DSharpPlus.Entities;

namespace HeadPats.Managers; 

public static class MessageTasks {
    /// <summary>
    /// Meant to be used with a DiscordMessage Task to delete the message after a given amount of seconds
    /// </summary>
    /// <param name="message">this</param>
    /// <param name="seconds">time until deletion</param>
    /// <param name="reason">deletion reason (for Audit Log)</param>
    /// <returns>an Asynchronous Task</returns>
    public static Task<DiscordMessage> DeleteAfter(this Task<DiscordMessage> message, int seconds, string reason = "") {
        Task.Delay(TimeSpan.FromSeconds(seconds)).ContinueWith(async _ => await message.GetAwaiter().GetResult().DeleteAsync(reason));
        return Task.FromResult(message.GetAwaiter().GetResult());
    }
}