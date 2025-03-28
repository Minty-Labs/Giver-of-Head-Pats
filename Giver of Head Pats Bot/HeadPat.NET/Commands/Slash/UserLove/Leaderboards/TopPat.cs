﻿using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using HeadPats.Data;
using HeadPats.Managers;
using HeadPats.Utils;
using Serilog;

namespace HeadPats.Commands.Slash.UserLove.Leaderboards; 

[IntegrationType(ApplicationIntegrationType.GuildInstall)]
public class TopPat : InteractionModuleBase<SocketInteractionContext> {
    [SlashCommand("toppat", "Get the Guild's top head pat leaderboard")]
    public async Task HeadPatLeaderboard([Summary("keywords", "Key words")] string keyWords = "") {
        await using var db = new Context();
        var logger = Log.ForContext("SourceContext", "Command - TopPat");
        var guildPats = 0;
        try { guildPats = db.Guilds.AsQueryable().ToList().FirstOrDefault(g => g.GuildId == Context.Guild.Id)!.PatCount; }
        catch { logger.Error("Guilds DataSet is Empty"); }

        var globalPats = 0;
        try { globalPats = db.Overall.AsQueryable().ToList().First().PatCount; }
        catch { logger.Error("Server DataSet is Empty"); }

        var newUserList = db.Users.AsQueryable().ToList().OrderBy(p => -p.PatCount);
        var patPercentage = globalPats == 0 ? 0 : (float) guildPats / globalPats * 100;
        var guildUserList = Context.Guild.Users.ToDictionary(user => user.Id);

        if (keyWords!.ToLower().Equals("server")) {
            var strings = new StringBuilder();
            strings.AppendLine($"Top 50 that are in this server.\n" +
                               $"- Server Pats: {MarkdownUtils.ToBold(guildPats.ToString("N0"))} ({(globalPats == 0 ? "NaN" : $"{patPercentage:F}")}% of global)\n" +
                               $"- Global Pats: {MarkdownUtils.ToBold(globalPats.ToString("N0"))}");
            
            var counter = 1;
            foreach (var u in newUserList) {
                if (counter >= 51) continue;
                if (!guildUserList.ContainsKey(u.UserId)) continue;
                strings.AppendLine($"`{counter}.` {
                    (u.UsernameWithNumber.Contains('#') ? 
                        u.UsernameWithNumber.Split('#')[0].ReplaceName(u.UserId) : 
                        u.UsernameWithNumber.ReplaceName(u.UserId))
                } - Total Pats: {MarkdownUtils.ToBold(u.PatCount.ToString("N0"))}");
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
            sb.AppendLine($"`{max}.` {
                (u.UsernameWithNumber.Contains('#') ? 
                    u.UsernameWithNumber.Split('#')[0].ReplaceName(u.UserId) : 
                    u.UsernameWithNumber.ReplaceName(u.UserId))
            } - Total Pats: {MarkdownUtils.ToBold(u.PatCount.ToString("N0"))}");
            max++;
        }

        var temp = sb.ToString();
        
        var embed = new EmbedBuilder {
            Timestamp = DateTime.Now,
            Title = "Head Pat Leaderboard",
            Color = Colors.Random,
            Footer = new EmbedFooterBuilder {
                Text = $"Synced across all servers • {Vars.Name} (v{Vars.VersionStr})"
            }
        }
            .AddField("Statistics for " + Context.Guild.Name, $"{(string.IsNullOrWhiteSpace(temp) ? "Data is Empty" : $"{temp}")}\n" +
                                                               $"Total Server Pats {MarkdownUtils.ToBold(guildPats.ToString("N0"))} ({(globalPats == 0 ? "NaN" : $"{patPercentage:F}")}% of global)")
            .AddField("Global Stats", $"Total Pats: {MarkdownUtils.ToBold(globalPats.ToString("N0"))}");
        
        await RespondAsync(embed: embed.Build());
    }
}