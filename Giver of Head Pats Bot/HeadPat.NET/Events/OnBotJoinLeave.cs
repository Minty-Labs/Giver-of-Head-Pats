using Discord;
using Discord.WebSocket;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Utils;
using Serilog;
using HeadPats.Managers;
using HeadPats.Modules;

namespace HeadPats.Events; 

public class OnBotJoinOrLeave : EventModule {
    protected override string EventName => "OnBotJoinOrLeave";
    protected override string Description => "Handles On Bot Join and Leave events.";

    public override void Initialize(DiscordSocketClient client) {
        client.GuildUnavailable += OnLeaveGuild;
        client.GuildAvailable += OnJoinGuild;
    }
    
    internal static bool DoNotRunOnStart = true;
    internal static List<ulong>? GuildIds;

    private static async Task OnLeaveGuild(SocketGuild e) {
        if (DoNotRunOnStart) return;
        if (GuildIds is not null && !GuildIds.Contains(e.Id)) return;
        var logger = Log.ForContext("SourceContext", "Event - GuildLeave");
        var em = new EmbedBuilder();
        em.WithColor(Colors.HexToColor("FF2525"));
        em.WithDescription($"Left server: `{e.Name.Sanitize()}` ({e.Id})");
        try { em.AddField("Created", $"{e.CreatedAt:F}", true); } catch { em.AddField("Joined", "unknown", true); }
        // try { em.AddField("Joined", $"{e.:F}", true); } catch { em.AddField("Joined", "unknown", true); }
        em.AddField("Members", $"{e.MemberCount - 1}", true);
        em.AddField("Description", e.Description ?? "None");
        em.AddField("Owner", $"{e.Owner.Username.Sanitize()} ({e.Owner.Id})");
        em.WithThumbnailUrl(e.IconUrl ?? "https://i.mintlily.lgbt/null.jpg");
        em.WithFooter($"Total servers: {Program.Instance.Client.Guilds.Count}");

        await Program.Instance.GeneralLogChannel!.SendMessageAsync(embed: em.Build());
        
        await using var db = new Context();
        var dbGuild = db.Guilds.AsQueryable().FirstOrDefault(g => g.GuildId == e.Id);
        if (dbGuild is null) {
            logger.Error("Guild Settings for guild {guildId} is null, cannot delete data.", e.Id);
            return;
        }
        dbGuild.DailyPatChannelId = 0;
        var guildPats = db.DailyPats.AsQueryable().Where(x => x.GuildId == e.Id).ToList();
        if (guildPats.Count > 0)
            guildPats?.Clear();
        dbGuild.DataDeletionTime = DateTimeOffset.UtcNow.AddDays(28).ToUnixTimeSeconds();
        await db.SaveChangesAsync();
        logger.Information("Cleared Daily Pats and IRL Quote data for guild {guildId}", e.Id);
    }

    private static async Task OnJoinGuild(SocketGuild e) {
        if (DoNotRunOnStart) return;
        if (GuildIds is not null && GuildIds.Contains(e.Id)) return;
        var logger = Log.ForContext("SourceContext", "Event - GuildJoin");
        var em = new EmbedBuilder();
        em.WithColor(Colors.HexToColor("42E66C"));
        em.WithDescription($"Joined server: `{e.Name.Sanitize()}` ({e.Id})");
        try { em.AddField("Created", $"{e.CreatedAt:F}", true); } catch { em.AddField("Joined", "unknown", true); }
        // try { em.AddField("Joined", $"{e.JoinedAt:F}", true); } catch { em.AddField("Joined", "unknown", true); }
        em.AddField("Members", $"{e.MemberCount - 1}", true); // -1 to exclude the bot
        em.AddField("Description", e.Description ?? "None");
        em.AddField("Owner", $"{e.Owner.Username.Sanitize()} ({e.Owner.Id})");
        em.WithThumbnailUrl(e.IconUrl ?? "https://i.mintlily.lgbt/null.jpg");
        em.WithFooter($"Total servers: {Program.Instance.Client.Guilds.Count}");

        if (Config.Base.FullBlacklistOfGuilds!.Contains(e.Id)) {
            await Program.Instance.GeneralLogChannel!.SendMessageAsync($"Leaving guild {e.Name} ({e.Id}) because it is blacklisted.", embed: em.Build());
            await e.LeaveAsync();
            return;
        }
        await Program.Instance.GeneralLogChannel!.SendMessageAsync(embed: em.Build());

        try {
            await using var db = new Context();
            
            var checkGuild = db.Guilds.AsQueryable()
                .Where(u => u.GuildId.Equals(e.Id)).ToList().FirstOrDefault();
            
            var checkUser = db.Users.AsQueryable()
                .Where(u => u.UserId.Equals(e.Owner.Id)).ToList().FirstOrDefault();

            if (checkGuild is null) {
                var newGuild = new Guilds {
                    Name = e.Name,
                    GuildId = e.Id,
                    DataDeletionTime = 0,
                    PatCount = 0,
                    DailyPatChannelId = 0
                };
                logger.Information("Added guild to database");
                db.Guilds.Add(newGuild);
            }
        
            if (checkUser is null) {
                var newUser = new Users {
                    UserId = e.Owner.Id,
                    Username = e.Owner.Username,
                    PatCount = 0,
                    CookieCount = 0,
                    Blacklisted = false
                };
                logger.Information("Added user to database");
                db.Users.Add(newUser);
            }
            
            await db.SaveChangesAsync();
        } catch (Exception ex) {
            await DNetToConsole.SendErrorToLoggingChannelAsync(ex);
        }
    }
}