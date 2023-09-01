using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Managers;
using HeadPats.Utils;
using Serilog;
using HeadPats.Configuration.Classes;

namespace HeadPats.Handlers.Events; 

public class OnBotJoinOrLeave {
    public OnBotJoinOrLeave(DiscordClient c) {
        Log.Information("Setting up OnBotJoinOrLeave Event Handler . . .");
        
        c.GuildDeleted += OnLeaveGuild;
        c.GuildCreated += OnJoinGuild;
    }

    private static async Task OnLeaveGuild(DiscordClient sender, GuildDeleteEventArgs e) {
        var em = new DiscordEmbedBuilder();
        em.WithColor(Colors.HexToColor("FF2525"));
        em.WithDescription($"Left server: `{e.Guild.Name}` ({e.Guild.Id})");
        try { em.AddField("Created", $"{e.Guild.CreationTimestamp:F}", true); } catch { em.AddField("Joined", "unknown", true); }
        try { em.AddField("Joined", $"{e.Guild.JoinedAt:F}", true); } catch { em.AddField("Joined", "unknown", true); }
        em.AddField("Members", e.Guild.MemberCount.ToString(), true);
        em.AddField("Description", e.Guild.Description ?? "None");
        em.AddField("Owner", $"{e.Guild.Owner.Username} ({e.Guild.Owner.Id})");
        em.WithThumbnail(e.Guild.IconUrl ?? "https://i.mintlily.lgbt/null.jpg");
        em.WithFooter($"Total servers: {sender.Guilds.Count}");

        await sender.SendMessageAsync(Program.GeneralLogChannel, em.Build());
    }

    private static async Task OnJoinGuild(DiscordClient sender, GuildCreateEventArgs e) {
        var em = new DiscordEmbedBuilder();
        em.WithColor(Colors.HexToColor("42E66C"));
        em.WithDescription($"Joined server: `{e.Guild.Name}` ({e.Guild.Id})");
        try { em.AddField("Created", $"{e.Guild.CreationTimestamp:F}", true); } catch { em.AddField("Joined", "unknown", true); }
        try { em.AddField("Joined", $"{e.Guild.JoinedAt:F}", true); } catch { em.AddField("Joined", "unknown", true); }
        em.AddField("Members", e.Guild.MemberCount.ToString(), true);
        em.AddField("Description", e.Guild.Description ?? "None");
        em.AddField("Owner", $"{e.Guild.Owner.Username} ({e.Guild.Owner.Id})");
        em.WithThumbnail(e.Guild!.IconUrl ?? "https://i.mintlily.lgbt/null.jpg");
        em.WithFooter($"Total servers: {sender.Guilds.Count}");

        await sender.SendMessageAsync(Program.GeneralLogChannel, em.Build());
        if (Config.Base.FullBlacklistOfGuilds!.Contains(e.Guild.Id)) {
            await sender.SendMessageAsync(Program.GeneralLogChannel, $"Leaving guild {e.Guild.Name} ({e.Guild.Id}) because it is blacklisted.");
            await e.Guild.LeaveAsync();
            return;
        }

        if (Config.Base.GuildSettings!.FirstOrDefault(g => g.GuildId == e.Guild.Id) is null) {
            var irlQuotes = new IrlQuotes {
                Enabled = false,
                ChannelId = 0,
                SetEpochTime = 0
            };
            
            var guildParams = new GuildParams {
                GuildName = e.Guild.Name,
                GuildId = e.Guild.Id,
                BlacklistedCommands = new List<string>(),
                Replies = new List<Reply>(),
                DailyPatChannelId = 0,
                DailyPats = new List<DailyPat>(),
                IrlQuotes = irlQuotes
            };
        
            Config.Base.GuildSettings!.Add(guildParams);
            Config.Save();
        }

        try {
            await using var db = new Context();
            
            var checkGuild = db.Guilds.AsQueryable()
                .Where(u => u.GuildId.Equals(e.Guild.Id)).ToList().FirstOrDefault();
            
            var checkUser = db.Users.AsQueryable()
                .Where(u => u.UserId.Equals(e.Guild.Owner.Id)).ToList().FirstOrDefault();

            if (checkGuild is null) {
                var newGuild = new Guilds {
                    GuildId = e.Guild.Id,
                    HeadPatBlacklistedRoleId = 0,
                    PatCount = 0
                };
                Log.Information("Added guild to database from OnJoinGuild");
                db.Guilds.Add(newGuild);
            }
        
            if (checkUser is null) {
                var newUser = new Users {
                    UserId = e.Guild.Owner.Id,
                    UsernameWithNumber = $"{e.Guild.Owner.Username}",
                    PatCount = 0,
                    CookieCount = 0,
                    IsUserBlacklisted = 0
                };
                Log.Information("Added user to database from OnJoinGuild");
                db.Users.Add(newUser);
            }
            
            await db.SaveChangesAsync();
        } catch (Exception ex) {
            await DSharpToConsole.SendErrorToLoggingChannelAsync(ex);
        }
    }
}