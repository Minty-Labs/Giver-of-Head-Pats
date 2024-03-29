﻿using DSharpPlus;
using DSharpPlus.SlashCommands;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Managers;
using Serilog;

namespace HeadPats.Commands.ContextMenu.User; 

public class Love : ApplicationCommandModule {
    [ContextMenu(ApplicationCommandType.UserContextMenu, "Hug")]
    public async Task Hug(ContextMenuContext ctx) {
        var target = ctx.TargetMember;
        var author = ctx.User;
        if (ctx.TargetMember.Id == Vars.ClientId)
            await ctx.CreateResponseAsync($"I got hugs from {author.Username.ReplaceName(ctx.User.Id)}?! Thankies~");
        else if (ctx.TargetMember.Id == ctx.User.Id)
            await ctx.CreateResponseAsync("You cant give yourself hugs, but I'll gladly give you some!");
        else 
            await ctx.CreateResponseAsync($"{author.Username.ReplaceName(author.Id)} hugged {target.Username.ReplaceName(target.Id)}!");
    }

    [ContextMenu(ApplicationCommandType.UserContextMenu, "Pat")]
    public async Task PatInline(ContextMenuContext c) {
        await using var db = new Context();
        var checkGuild = db.Guilds.AsQueryable()
            .Where(u => u.GuildId.Equals(c.Guild.Id)).ToList().FirstOrDefault();
        
        if (checkGuild is null) {
            var newGuild = new Guilds {
                GuildId = c.Guild.Id,
                HeadPatBlacklistedRoleId = 0,
                PatCount = 0
            };
            Log.Information("Added guild to database from Context menu Pat Command");
            db.Guilds.Add(newGuild);
        }
        
        var checkUser = db.Users.AsQueryable()
            .Where(u => u.UserId.Equals(c.TargetUser.Id)).ToList().FirstOrDefault();
        
        if (checkUser is null) {
            var newUser = new Users {
                UserId = c.TargetUser.Id,
                UsernameWithNumber = $"{c.TargetUser.Username}",
                PatCount = 0,
                CookieCount = 0,
                IsUserBlacklisted = 0
            };
            Log.Debug("Added user to database from Context menu Pat Command");
            db.Users.Add(newUser);
        }

        var isRoleBlackListed = c.Member!.Roles.Any(x => x.Id == checkGuild!.HeadPatBlacklistedRoleId && checkGuild.HeadPatBlacklistedRoleId != 0);

        if (isRoleBlackListed) {
            await c.CreateResponseAsync("This role is not allowed to use this command. This was set by a server administrator.", true);
            return;
        }
        
        var isUserBlackListed = checkUser!.IsUserBlacklisted == 1;
        
        if (isUserBlackListed) {
            await c.CreateResponseAsync("You are not allowed to use this command. This was set by a bot developer.", true);
            return;
        }
        
        var target = c.TargetMember;
        var author = c.User;
        if (c.TargetMember.IsBot)
            await c.CreateResponseAsync("You cannot give bots headpats.", true);
        else if (c.TargetMember.Id == c.User.Id)
            await c.CreateResponseAsync("You cannot give yourself headpats.", true);
        else 
            await c.CreateResponseAsync($"{author.Username.ReplaceName(author.Id)} patted {target.Username.ReplaceName(target.Id)}!");
        UserControl.AddPatToUser(c.TargetUser.Id, 1, true, c.Guild.Id);
    }
}