﻿using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HeadPats.Data;
using HeadPats.Managers;
using HeadPats.Utils;
using Serilog;

namespace HeadPats.Commands.Slash.UserLove; 

public class Leaderboards : ApplicationCommandModule {
    [SlashCommand("TopPat", "Get the top pat leaderboard")]
    public async Task PatLeaderboard(InteractionContext c,
        [Option("KeyWords", "Key words (can be empty)")]
        string? keyWords = "") {
        await using var db = new Context();

        var guildPats = 0;
        try { guildPats = db.Guilds.AsQueryable().ToList().FirstOrDefault(g => g.GuildId == c.Guild.Id)!.PatCount; }
        catch { Log.Error("[TopPat] Guilds DataSet is Empty"); }

        var globalPats = 0;
        try { globalPats = db.Overall.AsQueryable().ToList().First().PatCount; }
        catch { Log.Error("[TopPat] Server DataSet is Empty"); }

        var newUserList = db.Users.AsQueryable().ToList().OrderBy(p => -p.PatCount);

        if (keyWords!.ToLower().Equals("server")) {
            var strings = new StringBuilder();
            strings.AppendLine($"Top 50 that are in this server. - Server Pats: **{guildPats}** - Global Pats: **{globalPats}**");
            var counter = 1;
            foreach (var u in newUserList) {
                if (counter >= 51) continue;
                if (!c.Guild.Members.Keys.Contains(u.UserId)) continue;
                strings.AppendLine($"`{counter}.` {(u.UsernameWithNumber.Contains('#') ? u.UsernameWithNumber.Split('#')[0].ReplaceName(u.UserId) : u.UsernameWithNumber.ReplaceName(u.UserId))} - Total Pats: **{u.PatCount}**");
                counter++;
            }

            await c.CreateResponseAsync(strings.ToString());
            return;
        }

        var max = 1;
        var sb = new StringBuilder();

        foreach (var u in newUserList) {
            if (max >= 11) continue;
            if (!c.Guild.Members.Keys.Contains(u.UserId)) continue;
            sb.AppendLine($"`{max}.` {(u.UsernameWithNumber.Contains('#') ? u.UsernameWithNumber.Split('#')[0].ReplaceName(u.UserId) : u.UsernameWithNumber.ReplaceName(u.UserId))} - Total Pats: **{u.PatCount}**");
            max++;
        }

        var temp = sb.ToString();

        var e = new DiscordEmbedBuilder();
        e.WithTitle("Head Pat Leaderboard");
        e.WithColor(Colors.HexToColor("DFFFDD"));
        e.WithFooter($"Synced across all servers • {Vars.Name} (v{Vars.Version})");
        e.AddField("Current Server Stats",
            $"{(string.IsNullOrWhiteSpace(temp) ? "Data is Empty" : $"{temp}")}\nTotal Server Pats **{guildPats}**");
        e.AddField("Global Stats", $"Total Pats: **{globalPats}**");
        e.WithTimestamp(DateTime.Now);
        // await c.CreateResponseAsync(e.Build());
        await c.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(e.Build()));
    }
}