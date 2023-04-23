using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Utils;
using ic = DSharpPlus.SlashCommands.InteractionContext;

namespace HeadPats.Commands; 

public class Admin : ApplicationCommandModule {

    [SlashCommand("InviteInfo", "Gets a basic description about an invite by code", false)]
    [SlashRequirePermissions(Permissions.ManageMessages)]
    public async Task GetInviteInfo(ic c, [Option("Code", "Full link or just invite code", true)] string code) {
        if (c.Member.Permissions != Permissions.ManageMessages) {
            await c.CreateResponseAsync("You do not have permission to use this command.");
            return;
        }
        var hasLink = code.ToLower().Contains("discord.gg") || code.ToLower().Contains(".gg") || code.ToLower().Contains("https://");
        var final = code
            .Replace("https://", "")
            .Replace("discord.gg", "");
        if (string.IsNullOrWhiteSpace(hasLink ? final : code)) {
            await c.CreateResponseAsync("Please provide an invite link or code\nUsage: `-inviteinfo [code]`");
            return;
        }
        var e = new DiscordEmbedBuilder();
        DiscordInvite? inv = null;
        try {
            inv = await c.Client.GetInviteByCodeAsync(hasLink ? final : code, true, true);
        }
        catch (Exception ex) {
            if (ex.ToString().Contains("DSharpPlus.Exceptions.NotFoundException: Not found: 404")) {
                await c.CreateResponseAsync("Invite not found");
                return;
            }
        }
        e.WithTitle($"Invite for {inv!.Guild.Name}");
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
                $"Date Created: {(createdDateFinal.Contains("January 1, 0001") ? "Invalid Creation Date" : createdDateFinal)}\n",
                $"Expires: {inv.ExpiresAt:F}\n",
                $"[Click to Join Server](https://discord.gg/{inv.Code}) {(createdByDiscriminator == "null" ? "(Vanity URL Invite)" : "")}"
            }));

        e.WithThumbnail(inv.Guild.IconUrl);
        e.WithColor(Colors.HexToColor("5C7F90"));
        // e.WithFooter("Invite Inspector Created by SourVodka", "https://cdn.discordapp.com/avatars/211681643235115008/e47f3414381c5cbd1fc305f45dc2ffc8.png?size=2048");
        await c.CreateResponseAsync(e.Build());
    }

    [SlashCommand("UserInfoID", "Displays information about a user", false)]
    [SlashRequirePermissions(Permissions.ManageMessages)]
    public async Task UserInfo(ic c, [Option("UserID", "User ID")] string userId = "") {
        /*if (c.Member.Permissions != Permissions.ManageMessages) {
            await c.CreateResponseAsync("You do not have permission to use this command.");
            return;
        }*/
        if (string.IsNullOrWhiteSpace(userId)) {
            await c.CreateResponseAsync("Please provide a user to get info.");
            return;
        }

        var ul = ulong.Parse(userId.Replace("<@", "").Replace(">", ""));
        DiscordUser? u;
        try {
            u = await c.Client.GetUserAsync(ul, true);
        } catch {
            await c.CreateResponseAsync("The provided member is invalid. Did you put a message ID there instead?");
            return;
        }
        DiscordMember? m;
        try {
            m = await c.Guild.GetMemberAsync(ul);
        } catch {
            await c.CreateResponseAsync("User is not in the server, I cannot provide any information about them.");
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
        
        await c.CreateResponseAsync(e.Build());
    }

    [SlashCommand("UserInfo", "Displays information about a user", false)]
    [SlashRequirePermissions(Permissions.ManageMessages)]
    public async Task UserInfo(ic c, [Option("User", "Mentioned User to get info about", true)] DiscordUser user) {
        /*if (c.Member.Permissions != Permissions.ManageMessages) {
            await c.CreateResponseAsync("You do not have permission to use this command.");
            return;
        }*/
        DiscordMember m;
        try {
            m = await c.Guild.GetMemberAsync(user.Id);
        }
        catch {
            await c.CreateResponseAsync("User is not in the server, I cannot provide any information about them.");
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
        
        await c.CreateResponseAsync(e.Build());
    }

    [SlashCommand("BlacklistRoleFromPatCommand", "Blacklists a role from the pat command", false)]
    [SlashRequirePermissions(Permissions.ManageRoles)]
    public async Task BlacklistRoleFromPatCommand(ic c, [Option("Role", "Role to blacklist", true)] DiscordRole role,
        [Option("Action", "Action to take")]
        [Choice("Add", "add")]
        [Choice("Remove", "remove")]
        string value) {

        if (c.Member.Permissions != Permissions.ManageRoles) {
            await c.CreateResponseAsync("You do not have permission to use this command.");
            return;
        }
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
            await c.CreateResponseAsync($"The role, **{role.Name}**, is now blacklisted from the pat command.");
            return;
        }
        
        await c.CreateResponseAsync("The role is no longer blacklisted from the pat command.");
    }
}