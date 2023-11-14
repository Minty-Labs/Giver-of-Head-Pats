using System.Diagnostics;

namespace HeadPats; 

public static class Vars {
    public const string DSharpVer = "5.0.0-nightly-01839";
    public const string Name = "Giver of Head Pats";
    public const ulong ClientId = 489144212911030304;
    
    public const string Version = "2023.11.1" + (IsDebug ? "-dev" : ""); // Year.Month.Revision
    public static readonly DateTime BuildTime = IsDebug ? DateTime.Now : new(2023, 11, 13, 21, 39, 00); // (year, month, day, hour, min, sec)
    public const bool IsDebug = false;
    public static string BuildDate { get; } = $"{BuildTime:F}";
    public static DateTime StartTime { get; set; }
    public static bool IsWindows { get; set; }
    public static readonly string InviteLink = $"https://discord.com/api/oauth2/authorize?client_id={ClientId}&permissions=1240977501264&scope=bot%20applications.commands";
    public const string SupportServer = "https://discord.gg/Qg9eVB34sq";
    public const ulong SupportServerId = 1083619886980403272;
    public static Process? ThisProcess { get; set; }
    public static bool UseCookieApi { get; set; } = true;
    public const string FakeUserAgent = $"{Name} v{Version} (https://github.com/Minty-Labs/Giver-of-Head-Pats)";
}