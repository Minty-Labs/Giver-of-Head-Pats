using System.ComponentModel.DataAnnotations;
using Discord;
using HeadPats.Utils;
using Serilog;
using static HeadPats.Program;

namespace HeadPats.Data.Models; 

public class Users {
    [Key] public ulong UserId { get; set; }
    public string Username { get; set; }
    public string? NickName { get; set; }
    public long PatCount { get; set; }
    public long CookieCount { get; set; }
    public bool Blacklisted { get; set; }
}

public static class UserControl {
    private static readonly ILogger Logger = Log.ForContext("SourceContext", "Database - UserControl");
    
    public static void AddPatToUser(ulong userId, int numberOfPats, bool addToGuild = true, ulong guildToAddPatTo = 0, IUser? user = null) {
        using var db = new Context();
        var dbUser = db.Users.AsQueryable()
            .Where(u => u.UserId.Equals(userId)).ToList().FirstOrDefault();
        
        var dbGuild = db.Guilds.AsQueryable()
            .Where(u => u.GuildId.Equals(guildToAddPatTo)).ToList().FirstOrDefault();
        
        if (dbUser == null) {
            var newUser = new Users {
                UserId = userId,
                Username = user!.Username ?? "",
                PatCount = numberOfPats,
                CookieCount = 0,
                Blacklisted = false
            };
            Logger.Debug("Added user to database");
            db.Users.Add(newUser);
        }
        else {
            dbUser.PatCount += numberOfPats;
            db.Users.Update(dbUser);
        }

        if (addToGuild && guildToAddPatTo != 0) {
            if (dbGuild == null) {
                var newGuild = new Guilds {
                    GuildId = guildToAddPatTo,
                    PatCount = numberOfPats,
                    Name = "",
                    DataDeletionTime = 0,
                    DailyPatChannelId = 0
                };
                Logger.Debug("Added guild to database");
                db.Guilds.Add(newGuild);
            }
            else {
                dbGuild.PatCount += numberOfPats;
                db.Guilds.Update(dbGuild);
            }
        }

        // Update Global Pat Count
        var patCount = db.GlobalVariables.AsQueryable().ToList().FirstOrDefault(x => x.Name.Equals("PatCount"));
        var beforePatCount = patCount?.Value.AsInt();
        var modifiedPatCount = beforePatCount + numberOfPats;
        patCount!.Value = modifiedPatCount.ToString()!;
        db.GlobalVariables.Update(patCount);
        db.SaveChanges();
    }

    public static void AddCookieToUser(ulong userId, int cookiesToAdd) {
        using var db = new Context();
        var checkUser = db.Users.AsQueryable()
            .Where(u => u.UserId.Equals(userId)).ToList().FirstOrDefault();
        
        var user = Instance.Client.GetUserAsync(userId).GetAwaiter().GetResult();
        
        if (checkUser == null) {
            var newUser = new Users {
                UserId = userId,
                Username = user!.Username ?? "",
                PatCount = 0,
                CookieCount = cookiesToAdd,
                Blacklisted = false,
            };
            Logger.Debug("Added user to database");
            db.Users.Add(newUser);
        }
        else {
            checkUser.CookieCount += cookiesToAdd;
            db.Users.Update(checkUser);
        }
        
        db.SaveChanges();
    }
}