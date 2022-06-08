using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using HeadPats.Utils;

namespace HeadPats.Handlers.Events; 

public class OnBotJoinOrLeave {
    public OnBotJoinOrLeave(DiscordClient c) {
        Logger.Log("Setting up OnBotJoinOrLeave Event Handler . . .");
        
        c.GuildCreated += OnJoinGuild;
        c.GuildDeleted += OnLeaveGuild;
    }

    private async Task OnLeaveGuild(DiscordClient sender, GuildDeleteEventArgs e) {
        try {
            var em = new DiscordEmbedBuilder();
            em.WithColor(Colors.HexToColor("FF2525"));
            em.WithDescription($"Left server: `{e.Guild?.Name}` ({e.Guild?.Id})");
            try { em.AddField("Created", $"{e.Guild?.CreationTimestamp:F}", true); } catch { em.AddField("Joined", "uknown", true); }
            try { em.AddField("Joined", $"{e.Guild?.JoinedAt:F}", true); } catch { em.AddField("Joined", "uknown", true); }
            em.AddField("Members", e.Guild?.MemberCount.ToString(), true);
            em.AddField("Description", e.Guild?.Description ?? "None");
            em.AddField("Owner", $"{e.Guild?.Owner.Username}#{e.Guild?.Owner.Discriminator}");
            em.WithThumbnail(e.Guild?.IconUrl ?? "https://totallywholeso.me/assets/img/team/null.jpg");

            await sender.SendMessageAsync(Program.GeneralLogChannel, em.Build());
        } catch (Exception ex) {
            Logger.SendLog(ex);
        }
    }

    private async Task OnJoinGuild(DiscordClient sender, GuildCreateEventArgs e) {
        try {
            var em = new DiscordEmbedBuilder();
            em.WithColor(Colors.HexToColor("42E66C"));
            em.WithDescription($"Joined server: `{e.Guild.Name}` ({e.Guild.Id})");
            try { em.AddField("Created", $"{e.Guild.CreationTimestamp:F}", true); } catch { em.AddField("Joined", "uknown", true); }
            try { em.AddField("Joined", $"{e.Guild.JoinedAt:F}", true); } catch { em.AddField("Joined", "uknown", true); }
            em.AddField("Members", e.Guild.MemberCount.ToString(), true);
            em.AddField("Description", e.Guild.Description ?? "None");
            em.AddField("Owner", $"{e.Guild.Owner.Username}#{e.Guild.Owner.Discriminator}");
            em.WithThumbnail(e.Guild.IconUrl ?? "https://totallywholeso.me/assets/img/team/null.jpg");

            await sender.SendMessageAsync(Program.GeneralLogChannel, em.Build());
        } catch (Exception ex) {
            Logger.SendLog(ex);
        }
    }
}