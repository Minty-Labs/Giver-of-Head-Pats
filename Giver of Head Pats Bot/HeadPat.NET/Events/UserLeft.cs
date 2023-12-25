using Discord;
using Discord.WebSocket;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Modules;
using Serilog;

namespace HeadPats.Events; 

public class UserLeft : EventModule {
    protected override string EventName => "UserLeft";
    protected override string Description => "Handles the OnMemberLeave event.";
    
    public override void Initialize(DiscordSocketClient client) {
        client.UserLeft += OnUserLeft;
    }

    private static async Task OnUserLeft(SocketGuild arg1, SocketUser arg2) {
        var logger = Log.ForContext("SourceContext", "Event - UserLeave");
        await using var db = new Context();
        var dbGuild = db.Guilds.AsQueryable().FirstOrDefault(g => g.GuildId == arg1.Id);
        if (dbGuild is not null && dbGuild.DailyPatChannelId == 0) return;
        try {
            var pattedUser = db!.DailyPats!.AsQueryable().FirstOrDefault(x => x.GuildId == arg1.Id && x.UserId == arg2.Id);
            if (pattedUser == null) return;
            db.DailyPats.Remove(pattedUser);
            await db.SaveChangesAsync();
            logger.Information("Removed {User} ({UserId}) from the daily pats list for {GuildName} ({GuildId}).", arg2.Username, arg2.Id , arg1.Name, arg1.Id);
        }
        catch {
            // ignored
        }
    }
}