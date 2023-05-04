﻿using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Utils;
using ic = DSharpPlus.SlashCommands.InteractionContext;

namespace HeadPats.Commands.Slash.Admin; 

public class Admin : ApplicationCommandModule {

    [SlashCommand("UserInfo", "Displays information about a user", false)]
    [SlashRequirePermissions(Permissions.ManageMessages)]
    public async Task UserInfo(ic c,
        
        [Option("User", "User to get info about")] DiscordUser? user,
        [Option("UserID", "User ID to get info about")] string userId = "") {
;
        var staticCmdUser = 
            user is not null && !string.IsNullOrWhiteSpace(userId) ? 
            user : 
            await c.Client.GetUserAsync(ulong.Parse(userId.Replace("<@", "").Replace(">", "")), true);
        
        DiscordMember m;
        try {
            m = await c.Guild.GetMemberAsync(user!.Id);
        }
        catch {
            await c.CreateResponseAsync("User is not in the server, I cannot provide any information about them.");
            return;
        }

        var e = new DiscordEmbedBuilder();
        e.WithTimestamp(DateTime.Now);
        e.WithTitle("User Information");
        e.WithDescription($"`{user.Username}` - {user.Id}");
        e.AddField("Created", $"{m.CreationTimestamp:F}", true);
        e.AddField("Join", $"{m.JoinedAt:F}", true);

        var sb = new StringBuilder();
        foreach (var r in m.Roles) {
            sb.AppendLine($"{r.Emoji ?? ""}{r.Name}");
        }

        e.AddField($"Roles ({m.Roles.Count()})", sb.ToString());
        e.WithThumbnail(user.GetAvatarUrl(ImageFormat.Auto));
        e.WithColor(Colors.HexToColor("F771A3"));
        
        await c.CreateResponseAsync(e.Build());
    }
}