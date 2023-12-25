using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using HeadPats.Configuration.Classes;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Managers;
using Serilog;

namespace HeadPats.Commands.ContextMenu; 


public class ContextMenuLove : InteractionModuleBase { //<SocketInteractionContext<SocketUserCommand>> {
    
    [UserCommand("Hug")]
    public async Task ContextMenuHug(IUser user) {
        var target = Program.Instance.GetUser(user.Id);
        var author = Context.User;
        if (target!.Id == Vars.ClientId)
            await RespondAsync($"I got hugs from {author.Username.ReplaceName(author.Id)}?! Thankies~");
        else if (target.Id == Context.User.Id)
            await RespondAsync("You cant give yourself hugs, but I'll gladly give you some!");
        else
            await RespondAsync($"{author.Username.ReplaceName(author.Id)} hugged {target.Username.ReplaceName(target.Id)}!");
    }

    [UserCommand("Pat")]
    public async Task ContextMenuPat(IUser user) {
        await using var db = new Context();
        var checkGuild = db.Guilds.AsQueryable()
            .Where(u => u.GuildId.Equals(Context.Guild.Id)).ToList().FirstOrDefault();
        
        if (checkGuild is null) {
            var newGuild = new Guilds {
                Name = Context.Guild.Name,
                GuildId = Context.Guild.Id,
                DataDeletionTime = 0,
                PatCount = 0,
                DailyPatChannelId = 0
            };
            Log.Information("Added guild to database from Context menu Pat Command");
            db.Guilds.Add(newGuild);
        }
        
        var checkUser = db.Users.AsQueryable()
            .Where(u => u.UserId.Equals(user.Id)).ToList().FirstOrDefault();
        
        if (checkUser is null) {
            var newUser = new Users {
                UserId = user.Id,
                Username = $"{user.Username}",
                NickName = "",
                PatCount = 0,
                CookieCount = 0,
                Blacklisted = false
            };
            Log.Debug("Added user to database from Context menu Pat Command");
            db.Users.Add(newUser);
        }
        
        if (user.IsBot)
            await RespondAsync("You cannot give bots headpats.", ephemeral: true);
        else if (Context.User.Id == user.Id)
            await RespondAsync("You cannot give yourself headpats.", ephemeral: true);
        else 
            await RespondAsync($"{Context.User.Username.ReplaceName(Context.User.Id)} patted {user.Username.ReplaceName(user.Id)}!");
        UserControl.AddPatToUser(user.Id, 1, true, Context.Guild.Id);
    }
}