using DSharpPlus;
using DSharpPlus.SlashCommands;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Managers;

namespace HeadPats.Commands.ContextMenu.User; 

public class Love : ApplicationCommandModule {
    [ContextMenu(ApplicationCommandType.UserContextMenu, "Hug")]
    public async Task Hug(ContextMenuContext ctx) {
        var target = ctx.TargetMember.Username + "#" + ctx.TargetMember.Discriminator;
        var author = ctx.User.Username;
        if (ctx.TargetMember.Id == Vars.ClientId)
            await ctx.CreateResponseAsync($"I got hugs from {author.ReplaceTheNames()}?! Thankies~");
        else if (ctx.TargetMember.Id == ctx.User.Id)
            await ctx.CreateResponseAsync("You cant give yourself hugs, but I'll gladly give you some!");
        else 
            await ctx.CreateResponseAsync($"{author.ReplaceTheNames()} hugged {target.ReplaceTheNamesWithTags()}!");
    }

    [ContextMenu(ApplicationCommandType.UserContextMenu, "Pat")]
    public async Task PatInline(ContextMenuContext c) {
        await using var db = new Context();
        var checkGuild = db.Guilds.AsQueryable()
            .Where(u => u.GuildId.Equals(c.Guild.Id)).ToList().FirstOrDefault();
        
        var checkUser = db.Users.AsQueryable()
            .Where(u => u.UserId.Equals(c.User.Id)).ToList().FirstOrDefault();
        
        if (checkUser is null) {
            var newUser = new Users {
                UserId = c.TargetUser.Id,
                UsernameWithNumber = $"{c.TargetUser.Username}#{c.TargetUser.Discriminator}",
                PatCount = 0,
                CookieCount = 0,
                IsUserBlacklisted = 0
            };
            Logger.Log("Added user to database");
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
        
        var target = c.TargetMember.Username + "#" + c.TargetMember.Discriminator;
        var author = c.User.Username;
        if (c.TargetMember.IsBot)
            await c.CreateResponseAsync("You cannot give bots headpats.", true);
        else if (c.TargetMember.Id == c.User.Id)
            await c.CreateResponseAsync("You cannot give yourself headpats.", true);
        else 
            await c.CreateResponseAsync($"{author.ReplaceTheNames()} patted {target.ReplaceTheNamesWithTags()}!");
        UserControl.AddPatToUser(c.TargetUser.Id, 1, true, c.Guild.Id);
    }
}