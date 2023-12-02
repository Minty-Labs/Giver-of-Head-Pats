using Discord;
using Discord.WebSocket;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Utils;
using Serilog;
using HeadPats.Configuration.Classes;
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
    internal static int GuildCount = 0;

    private static async Task OnLeaveGuild(SocketGuild e) {
        if (DoNotRunOnStart) return;
        if (Program.Instance.Client.Guilds.Count == GuildCount) return;
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
        
        var guildSettings = Config.Base.GuildSettings!.FirstOrDefault(g => g.GuildId == e.Id);
        if (guildSettings is null) {
            Log.Error("Guild Settings for guild {guildId} is null, cannot delete data.", e.Id);
            return;
        }
        guildSettings.DailyPatChannelId = 0;
        var dailyPats = guildSettings.DailyPats;
        dailyPats?.Clear();
        // var irlQuotes = guildSettings.IrlQuotes;
        // irlQuotes!.Enabled = false;
        // irlQuotes.ChannelId = 0;
        guildSettings.DataDeletionTime = DateTimeOffset.UtcNow.AddDays(28).ToUnixTimeSeconds();
        Log.Information("Cleared Daily Pats and IRL Quote data for guild {guildId}", e.Id);
        Config.Save();
    }

    private static async Task OnJoinGuild(SocketGuild e) {
        if (DoNotRunOnStart) return;
        if (Program.Instance.Client.Guilds.Count == GuildCount) return;
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

        var guildSettings = Config.Base.GuildSettings!.FirstOrDefault(g => g.GuildId == e.Id);
        if (guildSettings is null) {
            // var irlQuotes = new IrlQuotes {
            //     Enabled = false,
            //     ChannelId = 0,
            //     SetEpochTime = 0
            // };
            
            var guildParams = new GuildParams {
                GuildName = e.Name,
                GuildId = e.Id,
                BlacklistedCommands = new List<string>(),
                // Replies = new List<Reply>(),
                DailyPatChannelId = 0,
                DailyPats = new List<DailyPat>(),
                // IrlQuotes = irlQuotes,
                DataDeletionTime = 0
            };

            Config.Base.GuildSettings!.Add(guildParams);
        }
        else guildSettings.DataDeletionTime = 0;
        Config.Save();

        try {
            await using var db = new Context();
            
            var checkGuild = db.Guilds.AsQueryable()
                .Where(u => u.GuildId.Equals(e.Id)).ToList().FirstOrDefault();
            
            var checkUser = db.Users.AsQueryable()
                .Where(u => u.UserId.Equals(e.Owner.Id)).ToList().FirstOrDefault();

            if (checkGuild is null) {
                var newGuild = new Guilds {
                    GuildId = e.Id,
                    HeadPatBlacklistedRoleId = 0,
                    PatCount = 0
                };
                Log.Information("Added guild to database from OnJoinGuild");
                db.Guilds.Add(newGuild);
            }
        
            if (checkUser is null) {
                var newUser = new Users {
                    UserId = e.Owner.Id,
                    UsernameWithNumber = $"{e.Owner.Username}",
                    PatCount = 0,
                    CookieCount = 0,
                    IsUserBlacklisted = 0
                };
                Log.Information("Added user to database from OnJoinGuild");
                db.Users.Add(newUser);
            }
            
            await db.SaveChangesAsync();
        } catch (Exception ex) {
            await DNetToConsole.SendErrorToLoggingChannelAsync(ex);
        }
    }
}