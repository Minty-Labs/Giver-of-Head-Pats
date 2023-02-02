using HeadPats.Handlers.Events;

namespace HeadPats.Managers; 

public static class AutoRemoveOldDmChannels {
    public static async Task RemoveOldDmChannelsTask() {
        foreach (var channel in MessageCreated.DmCategory!.Children) {
            //if (channel.GuildId == BuildInfo.Config.SupportGuildId) continue;
            if (channel.IsCategory) continue;
            var messages = await channel.GetMessagesAsync(5);
            var timeSpan = messages[0].CreationTimestamp.Subtract(DateTime.Now);
            if (timeSpan.TotalSeconds >= 5259600) // 2 months
                channel.DeleteAsync().GetAwaiter().GetResult();
        }
    }
}