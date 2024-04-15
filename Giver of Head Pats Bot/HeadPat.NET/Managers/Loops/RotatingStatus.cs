using Discord;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Utils;

namespace HeadPats.Managers.Loops; 

public static class RotatingStatus {
    private static int _listEntry, _globalPatCount;
    
    public static async Task Update(Context db) {
        if (!Config.Base.RotatingStatus.Enabled) return;
        _globalPatCount = db.Overall.AsQueryable().ToList().First().PatCount;

        var totalStatuses = Config.Base.RotatingStatus.Statuses.Count;
        var status = Config.Base.RotatingStatus.Statuses[_listEntry];
        await Program.Instance.Client.SetStatusAsync(StringUtils.GetUserStatus(status.UserStatus));
        await Program.Instance.Client.SetActivityAsync(new CustomStatusGame(status.ActivityText.GetStatusVariable(_globalPatCount)));
        _listEntry++;
        if (_listEntry >= totalStatuses) _listEntry = 0;
    }
}

public static class StatusUtils {
    public static string GetStatusVariable(this string input, int patCount) {
        return input.Replace("%patCount%", $"{patCount:N0}")
            .Replace("%guildCount%", $"{Program.Instance.Client.Guilds.Count}")
            .Replace("%users%", $"{Program.Instance.Client.Guilds.Sum(guild => guild.MemberCount):N0}")
            .Replace("%os%", Vars.IsWindows ? "Windows" : "Linux")
            ;
    }
}