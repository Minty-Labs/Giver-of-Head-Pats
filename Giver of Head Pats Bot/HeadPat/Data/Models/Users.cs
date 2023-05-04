using System.Collections;
using System.ComponentModel.DataAnnotations;
using Serilog;

namespace HeadPats.Data.Models; 

public class Users {
    [Key] public ulong UserId { get; set; }
    public string UsernameWithNumber { get; set; }
    public int PatCount { get; set; }
    public int CookieCount { get; set; }
    public int IsUserBlacklisted { get; set; }
}

public static class UserControl {
    public static void AddPatToUser(ulong userId, int numberOfPats, bool addToGuild = true, ulong guildToAddPatTo = 0) {
        using var db = new Context();
        var checkUser = db.Users.AsQueryable()
            .Where(u => u.UserId.Equals(userId)).ToList().FirstOrDefault();
        
        var checkGuild = db.Guilds.AsQueryable()
            .Where(u => u.GuildId.Equals(guildToAddPatTo)).ToList().FirstOrDefault();
        
        var checkOverall = db.Overall.AsQueryable()
            .Where(u => u.ApplicationId.Equals(Vars.ClientId)).ToList().FirstOrDefault();

        var user = Program.Client?.GetUserAsync(userId).GetAwaiter().GetResult();
        
        if (checkUser == null) {
            var newUser = new Users {
                UserId = userId,
                UsernameWithNumber = $"{user?.Username}",
                PatCount = numberOfPats,
                CookieCount = 0,
                IsUserBlacklisted = 0
            };
            Log.Debug("Added user to database");
            db.Users.Add(newUser);
        }
        else {
            checkUser.PatCount += numberOfPats;
            db.Users.Update(checkUser);
        }

        if (addToGuild) {
            if (checkGuild == null) {
                var newGuild = new Guilds {
                    GuildId = guildToAddPatTo,
                    PatCount = numberOfPats,
                    HeadPatBlacklistedRoleId = 0
                };
                Log.Debug("Added guild to database");
                db.Guilds.Add(newGuild);
            }
            else {
                checkGuild.PatCount += numberOfPats;
                db.Guilds.Update(checkGuild);
            }
        }
        
        if (checkOverall == null) {
            var overall = new Overlord {
                ApplicationId = Vars.ClientId,
                PatCount = 0,
                NsfwCommandsUsed = 0
            };
            db.Overall.Add(overall);
            db.SaveChanges();
        }
        else {
            checkOverall.PatCount += numberOfPats;
            db.Overall.Update(checkOverall);
        }
        
        db.SaveChanges();
    }

    public static void AddCookieToUser(ulong userId, int cookiesToAdd) {
        using var db = new Context();
        var checkUser = db.Users.AsQueryable()
            .Where(u => u.UserId.Equals(userId)).ToList().FirstOrDefault();
        
        var user = Program.Client?.GetUserAsync(userId).GetAwaiter().GetResult();
        
        if (checkUser == null) {
            var newUser = new Users {
                UserId = userId,
                UsernameWithNumber = $"{user?.Username}",
                PatCount = 0,
                CookieCount = cookiesToAdd
            };
            Log.Debug("Added user to database");
            db.Users.Add(newUser);
        }
        else {
            checkUser.CookieCount += cookiesToAdd;
            db.Users.Update(checkUser);
        }
        
        db.SaveChanges();
    }
}