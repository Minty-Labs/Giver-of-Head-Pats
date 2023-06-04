using DSharpPlus.Entities;
using HeadPats.Configuration;
using HeadPats.Data;

namespace HeadPats.Managers.Loops;

public static class StatusLoop {    
    private static int _tempPatCount;
    
    public static void Update(Context db) {
        var tempPatCount = db.Overall.AsQueryable().ToList().First().PatCount;

        if (tempPatCount != _tempPatCount) {
            Program.Client!.UpdateStatusAsync(new DiscordActivity {
                Name = $"{tempPatCount} head pats | hp!help",
                ActivityType = ActivityType.Watching
            }, UserStatus.Online).GetAwaiter().GetResult();
            // Log.Debug("Updated Status");
        }
    }
}