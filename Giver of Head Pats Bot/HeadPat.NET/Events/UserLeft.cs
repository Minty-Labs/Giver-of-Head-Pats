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
        var guildConfig = DailyPatConfig.Base!.Guilds!.FirstOrDefault(g => g.GuildId == arg1.Id);
        if (guildConfig is not null && guildConfig.DailyPatChannelId == 0) return Task.CompletedTask;
        try {
            var pattedUser = guildConfig!.Users!.FirstOrDefault(x => x.UserId == arg2.Id);
            if (pattedUser == null) return Task.CompletedTask;
            guildConfig.Users!.Remove(pattedUser);
            Config.Save();
            Log.Information("Removed {User} ({UserId}) from the daily pats list for {GuildName} ({GuildId}), because they left the guild.", arg2.Username, arg2.Id , arg1.Name, arg1.Id);
        }
        catch {
            // ignored
        }
        return Task.CompletedTask;
    }
}