using Discord;
using Michiru.Configuration;
using Michiru.Utils;
using Quartz;

namespace Michiru.Managers.Jobs; 

public static class RotatingStatus {
    private static int _listEntry;
    
    public static async Task Update() {
        if (!Config.Base.RotatingStatus.Enabled) return;

        var totalStatuses = Config.Base.RotatingStatus.Statuses.Count;
        var status = Config.Base.RotatingStatus.Statuses[_listEntry];
        await Program.Instance.Client.SetStatusAsync(StringUtils.GetUserStatus(status.UserStatus));
        await Program.Instance.Client.SetActivityAsync(new CustomStatusGame(status.ActivityText.GetStatusVariable()));
        _listEntry++;
        if (_listEntry >= totalStatuses) _listEntry = 0;
    }

    private static string GetStatusVariable(this string input) {
        return input.Replace("%bangers%", $"{Config.GetBangerNumber()}")
                .Replace("%pm%", $"{Config.GetPersonalizedMemberCount()}")
                .Replace("%users%", $"{Program.Instance.Client.Guilds.Sum(guild => guild.MemberCount)}")
                .Replace("%os%", Vars.IsWindows ? "Windows" : "Linux")
            ;
    }
}

public class RotatingStatusJob : IJob {
    public async Task Execute(IJobExecutionContext context) {
        try {
            await RotatingStatus.Update();
        }
        catch (Exception err) {
            await ErrorSending.SendErrorToLoggingChannelAsync($"Rotating Status:\n{err}");
        }
    }
}