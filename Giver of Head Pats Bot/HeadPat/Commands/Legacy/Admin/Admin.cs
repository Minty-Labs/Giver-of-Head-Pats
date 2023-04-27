using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HeadPats.Data;
using HeadPats.Data.Models;

namespace HeadPats.Commands.Legacy.Admin; 

public class Admin : BaseCommandModule {
    [Command("BlacklistRoleFromPatCommand"), Description("Blacklists a role from the pat command")]
    [RequirePermissions(Permissions.ManageRoles & Permissions.ManageMessages)]
    public async Task BlacklistRoleFromPatCommand(CommandContext c, [Description("Role to blacklist")] DiscordRole role,
        [Description("add|remove")] string value) {

        if (c.Member!.Permissions != Permissions.ManageRoles) {
            await c.RespondAsync("You do not have permission to use this command.");
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
            await c.RespondAsync($"The role, **{role.Name}**, is now blacklisted from the pat command.");
            return;
        }
        
        await c.RespondAsync("The role is no longer blacklisted from the pat command.");
    }
}