using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using HeadPats.Configuration.Classes;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Managers;
using HeadPats.Utils;
using Serilog;

namespace HeadPats.Commands.ContextMenu; 

[IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall), CommandContextType(InteractionContextType.Guild, InteractionContextType.PrivateChannel)]
public class ContextMenuLove : InteractionModuleBase {
    
    [UserCommand("Hug")]
    public async Task ContextMenuHug(IUser user) {
        var target = await Context.Client.GetUserAsync(user.Id);
        var author = Context.User;
        if (target!.Id == Vars.ClientId)
            await RespondAsync($"I got hugs from {author.Username.ReplaceName(author.Id)}?! Thankies~");
        else if (target.Id == Context.User.Id)
            await RespondAsync("You cant give yourself hugs, but I'll gladly give you some!", ephemeral: true);
        else
            await RespondAsync($"{author.Username.ReplaceName(author.Id)} hugged {target.Username.ReplaceName(target.Id)}!");
    }

    [UserCommand("Pat")]
    public async Task ContextMenuPat(IUser user) {
        var logger = Log.ForContext("SourceContext", "CONTEXTMENU:PAT");
        
        await using var db = new Context();
        // check if action is ran in a guild or not
        var ranInGuild = Context.Guild is not null;
        if (ranInGuild) {
            var checkGuild = db.Guilds.AsQueryable()
                .Where(u => u.GuildId.Equals(Context.Guild!.Id)).ToList().FirstOrDefault();
        
            if (checkGuild is null) {
                var newGuild = new Guilds {
                    GuildId = Context.Guild!.Id,
                    HeadPatBlacklistedRoleId = 0,
                    PatCount = 0
                };
                logger.Information("Added guild to database from Context menu Pat Command");
                db.Guilds.Add(newGuild);
            }
        }
        
        var checkUser = db.Users.AsQueryable()
            .Where(u => u.UserId.Equals(user.Id)).ToList().FirstOrDefault();
        
        if (checkUser is null) {
            var newUser = new Users {
                UserId = user.Id,
                UsernameWithNumber = $"{user.Username}",
                PatCount = 0,
                CookieCount = 0,
                IsUserBlacklisted = 0
            };
            logger.Debug("Added user to database from Context menu Pat Command");
            db.Users.Add(newUser);
        }
        
        if (user.IsBot)
            await RespondAsync("You cannot give bots headpats.", ephemeral: true);
        else if (Context.User.Id == user.Id)
            await RespondAsync("You cannot give yourself headpats.", ephemeral: true);
        else {
            if (ranInGuild)
                await RespondAsync(PatUtils.GetRandomPatMessageTemplate(Context.User.Mention, user.Username.ReplaceName(user.Id)));
            else
                await RespondAsync(PatUtils.GetRandomUserAppPatMessageTemplate(user.Username.ReplaceName(user.Id)));
        }
        
        if (ranInGuild)
            UserControl.AddPatToUser(user.Id, 1, true, Context.Guild!.Id);
        else
            UserControl.AddPatToUser(user.Id, 1);
    }
}