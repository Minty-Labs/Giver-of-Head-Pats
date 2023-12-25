using System.Text;
using Discord;
using Discord.Interactions;
using HeadPats.Data;
using HeadPats.Managers;
using HeadPats.Utils;
using Serilog;

namespace HeadPats.Commands.Slash.UserLove.Leaderboards; 

public class TopPat : InteractionModuleBase<SocketInteractionContext> {
    [SlashCommand("toppat", "Get the top head pat leaderboard")]
    public async Task HeadPatLeaderboard([Summary("keywords", "Key words")] string keyWords = "") {
        var logger = Log.ForContext("SourceContext", "Command - TopPat");
        await using var db = new Context();
        long guildPats = 0;
        try { guildPats = db.Guilds.AsQueryable().ToList().FirstOrDefault(g => g.GuildId == Context.Guild.Id)!.PatCount; }
        catch { logger.Error("Guilds DataSet is Empty"); }

        var globalPats = 0;
        try { globalPats = Convert.ToInt32(db.GlobalVariables.AsQueryable().ToList().FirstOrDefault(x=> x.Name.Equals("PatCount"))?.Value); }
        catch { logger.Error("GlobalVariable {0} is Empty or does not exist", "PatCount"); }

        var newUserList = db.Users.AsQueryable().ToList().OrderBy(p => -p.PatCount);

        var patPercentage = globalPats == 0 ? 0 : (float) guildPats / globalPats * 100;

        if (keyWords!.ToLower().Equals("server")) {
            var strings = new StringBuilder();
            strings.AppendLine($"Top 50 that are in this server.\n" +
                               $"- Server Pats: **{guildPats}** ({(globalPats == 0 ? "NaN" : $"{patPercentage:F}")}% of global)\n" +
                               $"- Global Pats: **{globalPats}**");
            var counter = 1;
            foreach (var u in newUserList) {
                if (counter >= 51) continue;
                if (Context.Guild.Users.Any(m => m.Id != u.UserId)) continue;
                strings.AppendLine($"`{counter}.` {(u.Username.Contains('#') ? u.Username.Split('#')[0].ReplaceName(u.UserId) : u.Username.ReplaceName(u.UserId))} - Total Pats: **{u.PatCount}**");
                counter++;
            }

            await RespondAsync(strings.ToString());
            return;
        }

        var max = 1;
        var sb = new StringBuilder();

        foreach (var u in newUserList) {
            if (max >= 11) continue;
            if (Context.Guild.Users.Any(m => m.Id != u.UserId)) continue;
            sb.AppendLine($"`{max}.` {(u.Username.Contains('#') ? u.Username.Split('#')[0].ReplaceName(u.UserId) : u.Username.ReplaceName(u.UserId))} - Total Pats: **{u.PatCount}**");
            max++;
        }

        var temp = sb.ToString();
        
        var embed = new EmbedBuilder {
            Timestamp = DateTime.Now,
            Title = "Head Pat Leaderboard",
            Color = Colors.Random,
            Footer = new EmbedFooterBuilder {
                Text = $"Synced across all servers • {Vars.Name} (v{Vars.Version})"
            }
        };
        embed.AddField("Current Server Stats", $"{(string.IsNullOrWhiteSpace(temp) ? "Data is Empty" : $"{temp}")}\nTotal Server Pats **{guildPats}** ({(globalPats == 0 ? "NaN" : $"{patPercentage:F}")}% of global)");
        embed.AddField("Global Stats", $"Total Pats: **{globalPats}**");
        await RespondAsync(embed: embed.Build());
    }
}