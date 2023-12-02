using Discord;
using Discord.WebSocket;
using HeadPats.Configuration;
using HeadPats.Modules;
using Serilog;

namespace HeadPats.Events; 

public class UserLeft : EventModule {
    protected override string EventName => "UserLeft";
    protected override string Description => "Handles the OnMemberLeave event.";
    
    public override void Initialize(DiscordSocketClient client) {
        client.UserLeft += OnUserLeft;
    }

    private Task OnUserLeft(SocketGuild arg1, SocketUser arg2) {
        var guildSettings = Config.Base.GuildSettings!.FirstOrDefault(g => g.GuildId == arg1.Id);
        if (guildSettings is not null && guildSettings.DailyPatChannelId == 0) return Task.CompletedTask;
        try {
            var pattedUser = guildSettings!.DailyPats!.FirstOrDefault(x => x.UserId == arg2.Id);
            if (pattedUser == null) return Task.CompletedTask;
            guildSettings.DailyPats!.Remove(pattedUser);
            Config.Save();
            Log.Information("Removed {User} ({UserId}) from the daily pats list for {GuildName} ({GuildId}), because they left the guild.", arg2.Username, arg2.Id , arg1.Name, arg1.Id);
        }
        catch {
            // ignored
        }
        return Task.CompletedTask;
    }
}