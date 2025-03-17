using System.Text;
using Discord;
using Discord.Interactions;
using HeadPats.Data;
using HeadPats.Managers;
using HeadPats.Utils;
using Serilog;

namespace HeadPats.Commands.Slash.UserLove.Leaderboards; 

[IntegrationType(ApplicationIntegrationType.GuildInstall)]
public class TopCookie : InteractionModuleBase<SocketInteractionContext> {
    [SlashCommand("topcookie", "Get the Guild's top cookie leaderboard")]
    public async Task CookieLeaderboard([Summary("keywords", "Key words")] string keyWords = "") {
        await using var db = new Context();
        var logger = Log.ForContext("SourceContext", "Command - TopCookie");

        var globalCookie = 0;
        try { globalCookie = db.Overall.AsQueryable().ToList().First().PatCount; }
        catch { logger.Error("DataSet is Empty"); }
        
        
        var newUserList = db.Users.AsQueryable().ToList().OrderBy(p => -p.PatCount);
        var guildUserList = Context.Guild.Users.ToDictionary(user => user.Id);
        
        
        if (keyWords!.ToLower().Equals("server")) {
            var strings = new StringBuilder();
            strings.AppendLine("Top 50 that are in this server.");
            var counter = 1;
            foreach (var u in newUserList) {
                if (counter >= 51) continue;
                if (!guildUserList.ContainsKey(u.UserId)) continue;
                strings.AppendLine($"`{counter}.` {(u.UsernameWithNumber.Contains('#') ? u.UsernameWithNumber.Split('#')[0].ReplaceName(u.UserId) : u.UsernameWithNumber.ReplaceName(u.UserId))} - Total Cookies: {MarkdownUtils.ToBold(u.CookieCount.ToString("N0"))}");
                counter++;
            }

            await RespondAsync(strings.ToString());
            return;
        }

        var max = 1;
        var sb = new StringBuilder();

        foreach (var u in newUserList) {
            if (max >= 11) continue;
            if (!guildUserList.ContainsKey(u.UserId)) continue;
            sb.AppendLine($"`{max}.` {(u.UsernameWithNumber.Contains('#') ? u.UsernameWithNumber.Split('#')[0].ReplaceName(u.UserId) : u.UsernameWithNumber.ReplaceName(u.UserId))} - Total Cookies: {MarkdownUtils.ToBold(u.CookieCount.ToString("N0"))}");
            max++;
        }

        var temp = sb.ToString();

        var embed = new EmbedBuilder {
            Timestamp = DateTime.Now,
            Title = "Cookie Leaderboard",
            Color = Colors.GetRandomCookieColor(),
            Footer = new EmbedFooterBuilder {
                Text = $"{Vars.Name} (v{Vars.VersionStr})"
            }
        };
        embed.AddField("Current Server Stats",
            $"{(string.IsNullOrWhiteSpace(temp) ? "Data is Empty" : $"{temp}")}");
        await RespondAsync(embed: embed.Build());
    }
}