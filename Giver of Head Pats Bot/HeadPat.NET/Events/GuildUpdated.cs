using Discord;
using Discord.WebSocket;
using HeadPats.Configuration;
using HeadPats.Modules;
using HeadPats.Utils;

namespace HeadPats.Events; 

public class GuildUpdated : EventModule {
    protected override string EventName => "GuildUpdated";
    protected override string Description => "Handles the GuildUpdated events.";
    
    public override void Initialize(DiscordSocketClient client) {
        client.GuildUpdated += OnGuildUpdated;
    }

    private ulong _pennysGuildWatcherChannelId = 0;
    private ulong _pennysGuildWatcherGuildId = 0;
    
    private Task OnGuildUpdated(SocketGuild arg1, SocketGuild arg2) {
        if (_pennysGuildWatcherGuildId == 0) _pennysGuildWatcherGuildId = Config.Base.PennysGuildWatcher.GuildId;
        if (arg1.Id != _pennysGuildWatcherGuildId) return Task.CompletedTask;
        if (arg1.Name == arg2.Name) return Task.CompletedTask;
        if (_pennysGuildWatcherChannelId == 0) _pennysGuildWatcherChannelId = Config.Base.PennysGuildWatcher.ChannelId;
        var channel = arg1.GetTextChannel(_pennysGuildWatcherChannelId);
        if (channel is null) return Task.CompletedTask;
        // var currentTime = DateTime.UtcNow;
        var daysNumber = DateTime.UtcNow.Subtract(Config.Base.PennysGuildWatcher.LastUpdateTime.UnixTimeStampToDateTime()).Days;
        var embed = new EmbedBuilder {
            Title = "Guild Name Updated",
            Description = $"It has been {(daysNumber < 1 ? "less than a day" : (daysNumber == 1 ? "1 day" : $"{daysNumber} days"))} since the last time the guild name was updated.",
            Color = Colors.HexToColor("0091FF")
        };
        embed.AddField("Old Name", arg1.Name);
        embed.AddField("New Name", arg2.Name);
        channel.SendMessageAsync(embed: embed.Build());
        return Task.CompletedTask;
    }
}