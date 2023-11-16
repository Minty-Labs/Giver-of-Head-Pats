using DSharpPlus;
using DSharpPlus.EventArgs;
using HeadPats.Configuration;
using HeadPats.Modules;
using Serilog;

namespace HeadPats.Handlers.Events; 

public class OnMemberLeave : EventModule {
    protected override string EventName => "OnMemberLeave";
    protected override string Description => "Handles the OnMemberLeave event.";
    
    public override void Initialize(DiscordClient client) {
        client.GuildMemberRemoved += OnGuildMemberRemoved;
    }

    private static Task OnGuildMemberRemoved(DiscordClient sender, GuildMemberRemoveEventArgs args) {
        var guildSettings = Config.Base.GuildSettings!.FirstOrDefault(g => g.GuildId == args.Guild.Id);
        if (guildSettings is not null && guildSettings.DailyPatChannelId == 0) return Task.CompletedTask;
        try {
            var pattedUser = guildSettings!.DailyPats!.FirstOrDefault(x => x.UserId == args.Member.Id);
            // if (pattedUser == null) return Task.CompletedTask;
            guildSettings.DailyPats!.Remove(pattedUser);
            Config.Save();
            Log.Information("Removed {User} from the daily pats list for {GuildName} ({GuildId}), because they left the guild.", args.Member.Id, args.Guild.Name, args.Guild.Id);
        }
        catch {
            // ignored
        }
        return Task.CompletedTask;
    }
}