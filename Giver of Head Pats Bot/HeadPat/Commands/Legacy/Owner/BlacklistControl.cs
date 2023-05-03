using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Managers;
using Serilog;

namespace HeadPats.Commands.Legacy.Owner; 

public class BlacklistControl : BaseCommandModule {

    [Command("blacklist"), Description("Control blacklists of various types."), RequireOwner]
    public async Task BlacklistControlCommand(CommandContext c, string type, string action, string value) {
        // blacklist (user|url|guild) (add|remove|list) (value)
        if (string.IsNullOrWhiteSpace(type)) {
            await c.RespondAsync("Please provide a type of blacklist to control.");
            return;
        }
        
        if (type.ToLower() is "user" or "url" or "guild") {
            await c.RespondAsync("Please provide a valid type of blacklist to control.");
            return;
        }
        
        if (string.IsNullOrWhiteSpace(action)) {
            await c.RespondAsync("Please provide an action to perform.");
            return;
        }
        
        if (action.ToLower() is "add" or "remove" or "list") {
            await c.RespondAsync("Please provide a valid action to perform.");
            return;
        }
        
        if (string.IsNullOrWhiteSpace(value)) {
            await c.RespondAsync("Please provide a value to add or remove.");
            return;
        }
        
        switch (type.ToLower()) {
            case "user":
                switch (action.ToLower()) {
                    case "add": {
                        await using var db = new Context();
                        var getUserIdFromMention = value.Replace("<@", "").Replace(">", "");
                        
                        var checkUser = db.Users.AsQueryable()
                            .Where(u => u.UserId.Equals(getUserIdFromMention)).ToList().FirstOrDefault();
                        
                        var user = await c.Client.GetUserAsync(ulong.Parse(value));
                        
                        if (checkUser == null) {
                            var newUser = new Users {
                                UserId = user.Id,
                                UsernameWithNumber = $"{user.Username}#{user.Discriminator}",
                                PatCount = 0,
                                CookieCount = 0,
                                IsUserBlacklisted = 1
                            };
                            Log.Debug("Added user to database");
                            db.Users.Add(newUser);
                            db.Users.Update(checkUser!);
                        }
                        else {
                            checkUser.IsUserBlacklisted = 1;
                            db.Users.Update(checkUser);
                        }

                        await db.SaveChangesAsync();
                        
                        await c.RespondAsync($"Added {value} to user blacklist.");
                        break;
                    }
                    case "remove": {
                        await using var db = new Context();
                        var getUserIdFromMention = value.Replace("<@", "").Replace(">", "");
                        
                        var checkUser = db.Users.AsQueryable()
                            .Where(u => u.UserId.Equals(getUserIdFromMention)).ToList().FirstOrDefault();
                        
                        var user = await c.Client.GetUserAsync(ulong.Parse(value));
                        
                        if (checkUser == null) {
                            var newUser = new Users {
                                UserId = user.Id,
                                UsernameWithNumber = $"{user.Username}#{user.Discriminator}",
                                PatCount = 0,
                                CookieCount = 0,
                                IsUserBlacklisted = 0
                            };
                            Log.Debug("Added user to database");
                            db.Users.Add(newUser);
                            db.Users.Update(checkUser!);
                        }
                        else {
                            checkUser.IsUserBlacklisted = 0;
                            db.Users.Update(checkUser);
                        }

                        await db.SaveChangesAsync();
                        
                        await c.RespondAsync($"Removed {value} from user blacklist.");
                        break;
                    }
                    case "list": {
                        await using var db = new Context();
                        var userList = new List<string>();

                        foreach (var u in db.Users.AsQueryable())
                            userList.Add($"{u.UsernameWithNumber} - {u.UserId} - {u.IsUserBlacklisted}");
                        
                        await c.RespondAsync($"User blacklist: {string.Join(", ", userList)}");
                        break;
                    }
                }

                break;
            case "url":
                switch (action.ToLower()) {
                    case "add":
                        Config.Base.Api.ApiMediaUrlBlacklist!.Add(value);
                        await c.RespondAsync($"Added {value} to url blacklist.");
                        break;
                    case "remove":
                        Config.Base.Api.ApiMediaUrlBlacklist!.Remove(value);
                        await c.RespondAsync($"Removed {value} from url blacklist.");
                        break;
                    case "list":
                        await c.RespondAsync($"Url blacklist: {string.Join(", ", Config.Base.Api.ApiMediaUrlBlacklist!)}");
                        break;
                }

                break;
            case "guild":
                switch (action.ToLower()) {
                    case "add":
                        Config.Base.FullBlacklistOfGuilds!.Add(ulong.Parse(value));
                        await c.RespondAsync($"Added {value} to guild blacklist.");
                        break;
                    case "remove":
                        Config.Base.FullBlacklistOfGuilds!.Remove(ulong.Parse(value));
                        await c.RespondAsync($"Removed {value} from guild blacklist.");
                        break;
                    case "list":
                        await c.RespondAsync($"Guild blacklist: {string.Join(", ", Config.Base.FullBlacklistOfGuilds!)}");
                        break;
                }

                break;
        }
        
        Config.Save();
    }

}