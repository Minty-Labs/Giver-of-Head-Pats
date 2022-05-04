using System.ComponentModel.DataAnnotations;

namespace HeadPats.Data.Models; 

public class Guilds {
    [Key] public ulong GuildId { get; set; }
    public int PatCount { get; set; }
    public bool AutoResponses { get; set; }
}

public static class GuildControl {
    public static void ChangeAutoResponseAction(ulong guildId, bool result) {
        using var db = new Context();
        var checkGuild = db.Guilds.AsQueryable()
            .Where(u => u.GuildId.Equals(guildId)).ToList().FirstOrDefault();

        if (checkGuild == null) {
            var newGuild = new Guilds {
                GuildId = 0,
                PatCount = 0,
                AutoResponses = result
            };
            db.Guilds.Add(newGuild);
        }
        else {
            checkGuild.AutoResponses = result;
            db.Guilds.Update(checkGuild);
        }

        db.SaveChanges();
    }
}