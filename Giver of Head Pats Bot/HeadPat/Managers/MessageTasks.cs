using DSharpPlus.Entities;

namespace HeadPats.Managers; 

public static class MessageTasks {
    public static Task<DiscordMessage> DeleteAfter(this Task<DiscordMessage> message, int seconds) {
        Task.Delay(TimeSpan.FromSeconds(seconds)).ContinueWith(async _ => await message.GetAwaiter().GetResult().DeleteAsync());
        return Task.FromResult(message.GetAwaiter().GetResult());
    }
}