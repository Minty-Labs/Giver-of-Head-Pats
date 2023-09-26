using DSharpPlus.Entities;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Utils;

namespace HeadPats.Managers.Loops; 

public static class RotatingStatus {
    private static int _listEntry;
    
    public static async Task Update(Context db) {
        if (!Config.Base.RotatingStatus.Enabled) return;
        if (Program.Client == null) return;

        var totalStatuses = Config.Base.RotatingStatus.Statuses.Count;
        var status = Config.Base.RotatingStatus.Statuses[_listEntry];
        var discordActivity = new DiscordActivity(status.ActivityText.GetStatusVariable(db), StringUtils.GetActivityType(status.ActivityType));
        await Program.Client.UpdateStatusAsync(discordActivity, StringUtils.GetUserStatus(status.UserStatus));
        _listEntry++;
        if (_listEntry >= totalStatuses) _listEntry = 0;
    }
}

public static class StatusUtils {
    public static string GetStatusVariable(this string input, Context db) {
        return input.Replace("%patCount%", $"{db.Overall.AsQueryable().ToList().First().PatCount}")
            .Replace("%guildCount%", $"{Program.Client?.Guilds.Count}")
            .Replace("%users%", $"{Program.Client?.Guilds.Sum(x => x.Value.MemberCount)}")
            .Replace("%os%", Vars.IsWindows ? "Windows" : "Linux")
            ;
    }
}