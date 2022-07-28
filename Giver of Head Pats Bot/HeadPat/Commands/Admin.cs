using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Utils;
using cc = DSharpPlus.CommandsNext.CommandContext;
using ic = DSharpPlus.SlashCommands.InteractionContext;

namespace HeadPats.Commands; 

public class Admin : BaseCommandModule {
    public Admin() => Logger.Loadodule("AdminCommands");

    [Command("InviteInfo"), Aliases("ii"), Description("Gets a basic description about an invite by code")]
    [RequirePermissions(Permissions.ManageMessages)]
    public async Task GetInviteInfo(cc c, string code) {
        var hasLink = code.ToLower().Contains("discord.gg") || code.ToLower().Contains(".gg") || code.ToLower().Contains("https://");
        var final = code
            .Replace("https://", "")
            .Replace("discord.gg", "");
        if (string.IsNullOrWhiteSpace(hasLink ? final : code)) {
            await c.RespondAsync("Please provide an invite link or code\nUsage: `-inviteinfo [code]`");
            return;
        }
        var e = new DiscordEmbedBuilder();
        var inv = c.Client.GetInviteByCodeAsync(hasLink ? final : code, true, true).Result;
        e.WithTitle($"Invite for {inv.Guild.Name}");
        string createdByName;
        try { createdByName = inv.Inviter.Username; } catch { createdByName = "null"; }
        string createdByDiscriminator;
        try { createdByDiscriminator = inv.Inviter.Discriminator; } catch { createdByDiscriminator = "null"; }

        var createdDateFinal = $"{inv.CreatedAt:F}";
        var uses = $"{inv.Uses}/{inv.MaxUses}";

        // TODO: Fix Inviter & ExpiresAt
        e.WithDescription(string.Concat(new[] {
                $"Guild: {inv.Guild.Name} (ID: {inv.Guild.Id})\n",
                $"Created by: `{createdByName}#{createdByDiscriminator}`\n",
                $"For Channel: #{inv.Channel.Name} (ID: {inv.Channel.Id})\n",
                $"Uses: {(uses.Equals("0/0") ? "Unlimited" : uses)}\n",
                $"Date Created: {(createdDateFinal.Equals("Monday, 01 January 0001 00:00:00") ? "Unknown Creation Date" : createdDateFinal)}\n",
                $"Expires: {inv.ExpiresAt:F}\n",
                $"[Click to Join Server](https://discord.gg/{inv.Code}) {(createdByDiscriminator == "null" ? "(Vanity URL Invite)" : "")}"
            }));

        e.WithThumbnail(inv.Guild.IconUrl);
        e.WithColor(Colors.HexToColor("5C7F90"));
        e.WithFooter("Invite Inspector Created by SourVodka", "https://cdn.discordapp.com/avatars/211681643235115008/e47f3414381c5cbd1fc305f45dc2ffc8.png?size=2048");
        await c.RespondAsync(e.Build());
    }

    [Command("UserInfo"), Aliases("ui", "user-info", "uinfo"), Description("Displays information about a user")]
    [RequirePermissions(Permissions.ManageMessages)]
    public async Task UserInfo(cc c, string userId = "") {
        if (string.IsNullOrWhiteSpace(userId)) {
            await c.RespondAsync("Please provide a user to get info.");
            return;
        }

        var ul = ulong.Parse(userId.Replace("<@", "").Replace(">", ""));
        DiscordUser? u;
        try {
            u = await c.Client.GetUserAsync(ul, true);
        } catch {
            await c.RespondAsync("The provided member is invalid. Did you put a message ID there instead?");
            return;
        }
        DiscordMember? m;
        try {
            m = await c.Guild.GetMemberAsync(ul);
        } catch {
            await c.RespondAsync("User is not in the server, I cannot provide any information about them.");
            return;
        }

        var e = new DiscordEmbedBuilder();
        e.WithTimestamp(DateTime.Now);
        e.WithTitle("User Information");
        e.WithDescription($"`{u.Username}#{u.Discriminator}` - {u.Id}");
        e.AddField("Created", $"{m.CreationTimestamp:F}", true);
        e.AddField("Join", $"{m.JoinedAt:F}", true);

        var sb = new StringBuilder();
        foreach (var r in m.Roles) {
            sb.AppendLine($"{r.Emoji ?? ""}{r.Name}");
        }

        e.AddField($"Roles ({(m.Roles == null ? "null" : $"{m.Roles.Count()}")})", sb.ToString());
        e.WithThumbnail(u.GetAvatarUrl(ImageFormat.Auto));
        e.WithColor(Colors.HexToColor("F771A3"));

        var builder = new DiscordMessageBuilder();
        builder.WithEmbed(e.Build());
        await builder.WithReply(c.Message.Id).SendAsync(c.Channel);
    }

    [Command("UserInfo")]
    [RequirePermissions(Permissions.ManageMessages)]
    public async Task UserInfo(cc c, DiscordUser? user = null) {
        if (user is null) {
            await c.RespondAsync("Please provide a user to get info.");
            return;
        }

        DiscordUser? u;
        try {
            u = await c.Client.GetUserAsync(user.Id, true);
        } catch {
            await c.RespondAsync("The provided member is invalid. Did you put a message ID there instead?");
            return;
        }
        DiscordMember m;
        try {
            m = await c.Guild.GetMemberAsync(user.Id);
        }
        catch {
            await c.RespondAsync("User is not in the server, I cannot provide any information about them.");
            return;
        }

        var e = new DiscordEmbedBuilder();
        e.WithTimestamp(DateTime.Now);
        e.WithTitle("User Information");
        e.WithDescription($"`{user.Username}#{user.Discriminator}` - {user.Id}");
        e.AddField("Created", $"{m.CreationTimestamp:F}", true);
        e.AddField("Join", $"{m.JoinedAt:F}", true);

        var sb = new StringBuilder();
        foreach (var r in m.Roles) {
            sb.AppendLine($"{r.Emoji ?? ""}{r.Name}");
        }

        e.AddField($"Roles ({m.Roles.Count()})", sb.ToString());
        e.WithThumbnail(user.GetAvatarUrl(ImageFormat.Auto));
        e.WithColor(Colors.HexToColor("F771A3"));

        var builder = new DiscordMessageBuilder();
        builder.WithEmbed(e.Build());
        await builder.WithReply(c.Message.Id).SendAsync(c.Channel);
    }

    [Command("BlacklistRoleFromPatCommand"), Aliases("brfp"), Description("Blacklists a role from the pat command")]
    [RequirePermissions(Permissions.ManageRoles)]
    public async Task BlacklistRoleFromPatCommand(cc c, string mentionedRoleOrId = "", string value = "") {
        if (string.IsNullOrWhiteSpace(mentionedRoleOrId) || string.IsNullOrWhiteSpace(value)) {
            await c.RespondAsync($"Incorrect command format! Please use the command like this:\n`{BuildInfo.Config.Prefix}BlacklistRoleFromPatCommand [@role] [true/false]`");
            return;
        }
        
        var getRoleIdFromMention = mentionedRoleOrId.Replace("<@&", "").Replace(">", "");
        
        var role = c.Guild.GetRole(ulong.Parse(getRoleIdFromMention));

        await using var db = new Context();
        var checkGuild = db.Guilds.AsQueryable()
            .Where(u => u.GuildId.Equals(c.Guild.Id)).ToList().FirstOrDefault();
        
        var valueIsTrue = value.ToLower().Contains('t');

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
}