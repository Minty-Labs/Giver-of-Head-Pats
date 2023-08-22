using DSharpPlus.Entities;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Utils;
using Serilog;

namespace HeadPats.Managers.Loops;

public static class StatusLoop {    
    private static int _tempPatCount;
    
    public static void Update(Context db) {
        var tempPatCount = db.Overall.AsQueryable().ToList().First().PatCount;

        if (tempPatCount == _tempPatCount) return;
        Program.Client!.UpdateStatusAsync(new DiscordActivity {
            Name = $"{tempPatCount} head pats | hp!help",
            ActivityType = StringUtils.GetActivityType(Config.Base.ActivityType)
        }, StringUtils.GetUserStatus(Config.Base.UserStatus)).GetAwaiter().GetResult();
        Log.Debug("Updated Status");
    }
}