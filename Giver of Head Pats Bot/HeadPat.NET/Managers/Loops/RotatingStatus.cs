using Discord;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Utils;
using HeadPats.Utils.ExternalApis;

namespace HeadPats.Managers.Loops; 

public static class RotatingStatus {
    private static int _listEntry, _globalPatCount;
    
    public static async Task Update(/*Context db*/) {
        if (!Config.Base.RotatingStatus.Enabled) return;
        // _globalPatCount = db.Overall.AsQueryable().ToList().First().PatCount;
        var totalStatuses = Config.Base.RotatingStatus.Statuses.Count;
        var status = Config.Base.RotatingStatus.Statuses[_listEntry];
        var client = Program.Instance.Client;

        try {
            await client.SetStatusAsync(status.UserStatus.GetUserStatus());
            await client.SetGameAsync(status.ActivityText, type: status.ActivityType.GetActivityType());
        }
        catch {
            await client.SetStatusAsync(UserStatus.Online);
            await client.SetGameAsync("with your love", type: ActivityType.Competing);
        }
        
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
            .Replace("%patreonCount%", $"{PatronLogic.Instance.MemberCount:N0}")
            ;
    }
    
    /// <summary>
    /// Get Discord ActivityType from string
    /// </summary>
    /// <param name="type">activity as string</param>
    /// <returns>DSharpPlus.Entities.ActivityType</returns>
    public static ActivityType GetActivityType(this string type) {
        return type.ToLower() switch {
            "playing" => ActivityType.Playing,
            "listening" => ActivityType.Listening,
            "watching" => ActivityType.Watching,
            "streaming" => ActivityType.Streaming,
            "competing" => ActivityType.Competing,
            "custom" => ActivityType.CustomStatus,
            _ => ActivityType.CustomStatus
        };
    }

    /// <summary>
    /// Get Discord UserStatus from string
    /// </summary>
    /// <param name="status">status as string</param>
    /// <returns>DSharpPlus.Entities.UserStatus</returns>
    public static UserStatus GetUserStatus(this string status) {
        return status.ToLower() switch {
            "online" => UserStatus.Online,
            "idle" => UserStatus.Idle,
            "dnd" => UserStatus.DoNotDisturb,
            "do_not_disturb" => UserStatus.DoNotDisturb,
            "invisible" => UserStatus.Invisible,
            "offline" => UserStatus.Invisible,
            _ => UserStatus.Online
        };
    }
}