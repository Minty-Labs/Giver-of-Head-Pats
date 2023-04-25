using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Managers;
using HeadPats.Utils;

namespace HeadPats.Handlers.Events; 

public class OnBotJoinOrLeave {
    public OnBotJoinOrLeave(DiscordClient c) {
        Logger.Log("Setting up OnBotJoinOrLeave Event Handler . . .");
        
        c.GuildCreated += OnJoinGuild;
        c.GuildDeleted += OnLeaveGuild;
    }

    private static async Task OnLeaveGuild(DiscordClient sender, GuildDeleteEventArgs e) {
        try {
            var em = new DiscordEmbedBuilder();
            em.WithColor(Colors.HexToColor("FF2525"));
            em.WithDescription($"Left server: `{e.Guild.Name}` ({e.Guild.Id})");
            try { em.AddField("Created", $"{e.Guild.CreationTimestamp:F}", true); } catch { em.AddField("Joined", "unknown", true); }
            try { em.AddField("Joined", $"{e.Guild.JoinedAt:F}", true); } catch { em.AddField("Joined", "unknown", true); }
            em.AddField("Members", e.Guild.MemberCount.ToString(), true);
            em.AddField("Description", e.Guild.Description ?? "None");
            em.AddField("Owner", $"{e.Guild.Owner.Username}#{e.Guild.Owner.Discriminator} ({e.Guild.Owner.Id})");
            em.WithThumbnail(e.Guild.IconUrl ?? "https://totallywholeso.me/assets/img/team/null.jpg");
            em.WithFooter($"Total servers: {sender.Guilds.Count}");

            await sender.SendMessageAsync(Program.GeneralLogChannel, em.Build());
            
            await using var db = new Context();
            var checkUser = db.Users.AsQueryable()
                .Where(u => u.UserId.Equals(e.Guild.Owner.Id)).ToList().FirstOrDefault();
        
            if (checkUser is null) {
                var newUser = new Users {
                    UserId = e.Guild.Owner.Id,
                    UsernameWithNumber = $"{e.Guild.Owner.Username}#{e.Guild.Owner.Discriminator}",
                    PatCount = 0,
                    CookieCount = 0,
                    IsUserBlacklisted = 0
                };
                Logger.Log("Added user to database from OnJoinGuild");
                db.Users.Add(newUser);
            }
        } catch (Exception ex) {
            Logger.SendLog(ex);
        }
    }

    private static async Task OnJoinGuild(DiscordClient sender, GuildCreateEventArgs e) {
        try {
            var em = new DiscordEmbedBuilder();
            em.WithColor(Colors.HexToColor("42E66C"));
            em.WithDescription($"Joined server: `{e.Guild.Name}` ({e.Guild.Id})");
            try { em.AddField("Created", $"{e.Guild.CreationTimestamp:F}", true); } catch { em.AddField("Joined", "unknown", true); }
            try { em.AddField("Joined", $"{e.Guild.JoinedAt:F}", true); } catch { em.AddField("Joined", "unknown", true); }
            em.AddField("Members", e.Guild.MemberCount.ToString(), true);
            em.AddField("Description", e.Guild.Description ?? "None");
            em.AddField("Owner", $"{e.Guild.Owner.Username}#{e.Guild.Owner.Discriminator} ({e.Guild.Owner.Id})");
            em.WithThumbnail(e.Guild!.IconUrl ?? "https://totallywholeso.me/assets/img/team/null.jpg");
            em.WithFooter($"Total servers: {sender.Guilds.Count}");

            await sender.SendMessageAsync(Program.GeneralLogChannel, em.Build());

            if (Vars.Config.FullBlacklistOfGuilds!.Contains(e.Guild.Id)) {
                await sender.SendMessageAsync(Program.GeneralLogChannel, $"Leaving guild {e.Guild.Name} ({e.Guild.Id}) because it is blacklisted.");
                await e.Guild.LeaveAsync();
            }
            
        } catch (Exception ex) {
            Logger.SendLog(ex);
        }
    }
}