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
            em.WithDescription($"Left server: `{e.Guild.Name}` ({e.Guild.Id})");
            em.AddField("Created", $"{e.Guild.CreationTimestamp:F}", true);
            em.AddField("Joined", $"{e.Guild.JoinedAt:F}", true);
            em.AddField("IsLarge", $"{e.Guild.IsLarge}", true);
            em.AddField("Description", $"{e.Guild.Description}");
            em.AddField("Owner", $"{e.Guild.Owner.Username}#{e.Guild.Owner.Discriminator}");
            var sbr = new StringBuilder();
            foreach (var r in e.Guild.Roles)
                sbr.AppendLine(r.Value.Name);
            em.AddField("Roles", $"{sbr.ToString()[..1023]}");
            var sbf = new StringBuilder();
            foreach (var f in e.Guild.Features)
                sbf.AppendLine(f);
            em.AddField("Features", $"{sbf.ToString()[..1023]}");
            em.WithThumbnail(e.Guild.IconUrl);

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
            em.AddField("Created", $"{e.Guild.CreationTimestamp:F}", true);
            em.AddField("Joined", $"{e.Guild.JoinedAt:F}", true);
            em.AddField("IsLarge", $"{e.Guild.IsLarge}", true);
            em.AddField("Description", $"{e.Guild.Description}");
            em.AddField("Owner", $"{e.Guild.Owner.Username}#{e.Guild.Owner.Discriminator}");
            var sbr = new StringBuilder();
            foreach (var r in e.Guild.Roles)
                sbr.AppendLine(r.Value.Name);
            em.AddField("Roles", $"{sbr.ToString()[..1023]}");
            var sbf = new StringBuilder();
            foreach (var f in e.Guild.Features)
                sbf.AppendLine(f);
            em.AddField("Features", $"{sbf.ToString()[..1023]}");
            em.WithThumbnail(e.Guild.IconUrl);

            await sender.SendMessageAsync(Program.GeneralLogChannel, em.Build());
        } catch (Exception ex) {
            Logger.SendLog(ex);
        }
    }
}