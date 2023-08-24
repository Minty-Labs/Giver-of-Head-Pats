using DSharpPlus.Entities;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Utils;
using Serilog;

namespace HeadPats.Managers.Loops;

public static class StatusLoop {    
    private static int _tempPatCount;
    
    public static async Task Update(Context db) {
        var tempPatCount = db.Overall.AsQueryable().ToList().First().PatCount;

        if (tempPatCount == _tempPatCount) return;
        if (Program.Client == null) return;
        
        var discordActivity = new DiscordActivity($"{tempPatCount} head pats given | hp!help", StringUtils.GetActivityType(Config.Base.ActivityType));
        await Program.Client.UpdateStatusAsync(discordActivity, StringUtils.GetUserStatus(Config.Base.UserStatus));
        // Log.Debug("Updated Status");
    }
}