using Discord;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Utils;

namespace HeadPats.Managers.Loops; 

public static class RotatingStatus {
    private static int _listEntry;
    
    public static async Task Update(Context db) {
        if (!Config.Base.RotatingStatus.Enabled) return;

        var totalStatuses = Config.Base.RotatingStatus.Statuses.Count;
        var status = Config.Base.RotatingStatus.Statuses[_listEntry];
        await Program.Instance.Client.SetStatusAsync(StringUtils.GetUserStatus(status.UserStatus));
        await Program.Instance.Client.SetActivityAsync(new CustomStatusGame(status.ActivityText.GetStatusVariable(db)));
        _listEntry++;
        if (_listEntry >= totalStatuses) _listEntry = 0;
    }
}

public static class StatusUtils {
    public static string GetStatusVariable(this string input, Context db) {
        return input.Replace("%patCount%", $"{Convert.ToInt32(db.GlobalVariables.AsQueryable().ToList().FirstOrDefault(x=> x.Name.Equals("PatCount")).Value)}")
            .Replace("%guildCount%", $"{Program.Instance.Client.Guilds.Count}")
            .Replace("%users%", $"{Program.Instance.Client.Guilds.Sum(guild => guild.MemberCount)}")
            .Replace("%os%", Vars.IsWindows ? "Windows" : "Linux")
            ;
    }
}