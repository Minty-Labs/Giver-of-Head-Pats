﻿using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Utils;

namespace HeadPats.Commands.Legacy.Admin; 

public class Admin : BaseCommandModule {
    [Command("BlacklistRoleFromPatCommand"), Description("Blacklists a role from the pat command")]
    [RequirePermissions(Permissions.ManageRoles & Permissions.ManageMessages)]
    public async Task BlacklistRoleFromPatCommand(CommandContext c, [Description("Role to blacklist")] DiscordRole role,
        [Description("add|remove")] string value) {

        // if (c.Member!.Permissions != Permissions.ManageRoles) {
        //     await c.RespondAsync("You do not have permission to use this command.");
        //     return;
        // }
        await using var db = new Context();
        var checkGuild = db.Guilds.AsQueryable()
            .Where(u => u.GuildId.Equals(c.Guild.Id)).ToList().FirstOrDefault();
        
        var valueIsTrue = value.ToLower().Equals("add");

        if (checkGuild == null) {
            var newGuild = new Guilds {
                GuildId = c.Guild.Id,
                PatCount = 0,
                HeadPatBlacklistedRoleId = valueIsTrue ? role.Id : 0
            };
            db.Guilds.Add(newGuild);
            db.Guilds.Update(checkGuild!);
        }
        else {
            checkGuild.HeadPatBlacklistedRoleId = valueIsTrue ? role.Id : 0;
            db.Guilds.Update(checkGuild);
        }
        
        if (valueIsTrue) {
            await c.RespondAsync($"The role, **{role.Name}**, is now blacklisted from the pat command.");
            return;
        }
        
        await c.RespondAsync("The role is no longer blacklisted from the pat command.");
    }

    [Command("userinfo"), Priority(0), Description("Gets information about a user"), RequirePermissions(Permissions.ManageMessages)]
    public async Task UserInfo(CommandContext ctx, [Description("User ID")] string userId = "") {
        await UserInfoExt.DoUserIntoAction(ctx, 1, userId);
    }
    
    [Command("userinfo"), Priority(1), Description("Gets information about a user"), RequirePermissions(Permissions.ManageMessages)]
    public async Task UserInfo(CommandContext ctx, [Description("Member")] DiscordMember user) {
        await UserInfoExt.DoUserIntoAction(ctx, 3, member: user);
    }
    
    [Command("userinfo"), Priority(2), Description("Gets information about a user"), RequirePermissions(Permissions.ManageMessages)]
    public async Task UserInfo(CommandContext ctx, [Description("User")] DiscordUser user) {
        await UserInfoExt.DoUserIntoAction(ctx, 2, user: user);
    }
}

public static class UserInfoExt {
    public static async Task DoUserIntoAction(CommandContext ctx, int userInfoType, string userId = "", DiscordUser? user = null, DiscordMember? member = null) {
        DiscordUser? u = null;
        DiscordMember? m = null;
        
        switch (userInfoType) {
            case 1:
                if (string.IsNullOrWhiteSpace(userId)) {
                    await ctx.RespondAsync("Please provide a user to get info.");
                    return;
                }

                var ul = ulong.Parse(userId.Replace("<@", "").Replace(">", ""));
                
                try {
                    u = await ctx.Client.GetUserAsync(ul, true);
                }
                catch {
                    await ctx.RespondAsync("The provided member is invalid. Did you put a message ID there instead?");
                    return;
                }
                
                try {
                    m = await ctx.Guild.GetMemberAsync(ul);
                }
                catch {
                    await ctx.RespondAsync("User is not in the server, I cannot provide any information about them.");
                    return;
                }
                break;
            case 2:
                u = user;
                break;
            case 3:
                m = member;
                break;
        }
        
        var embed = new DiscordEmbedBuilder {
            Title = "User Information",
            Description = $"{m.DisplayName} ({u.Username}) - {u.Id}",
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail {Url = u.GetAvatarUrl(ImageFormat.Auto)},
            Color = Colors.HexToColor("F771A3")
        }
            .AddField("Created", $"{m.CreationTimestamp:F}", true)
            .AddField("Joined", $"{m.JoinedAt:F} (<t:{m.JoinedAt.GetSecondsFromUnixTime()}:R>)", true);

        var sb = new StringBuilder();
        foreach (var r in m.Roles)
            sb.AppendLine($"{r.Emoji ?? ""}{r.Name}");

        if (string.IsNullOrWhiteSpace(sb.ToString()))
            embed.AddField("Roles (0)", "None");
        else
            embed.AddField($"Roles ({m.Roles.Count()})", sb.ToString());
        
        await ctx.RespondAsync(embed.Build());
    }
}