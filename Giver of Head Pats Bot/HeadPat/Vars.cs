using System.Diagnostics;

namespace HeadPats; 

public static class Vars {
    public const string DSharpVer = "4.4.2-stable";
    public const string Name = "Giver of Head Pats";
    public const ulong ClientId = 489144212911030304;
#if DEBUG
    public const ulong TestGuildId = 279459962843955201;
    public const string Version = "4.5.0-dev2";
    public static readonly DateTime BuildTime = DateTime.Now;
    public const bool IsDebug = true;
#else
    public const string Version = "2023.7.2"; // Year.Month.Revision
    public static readonly DateTime BuildTime = new(2023, 7, 1, 21, 00, 00); // (year, month, day, hour, min, sec)
    public const bool IsDebug = false;
#endif
    public static string BuildDate { get; } = $"{BuildTime:F}";
    public static DateTime StartTime { get; set; }
    public static bool IsWindows { get; set; }
    public static readonly string InviteLink = $"https://discord.com/api/oauth2/authorize?client_id={ClientId}&permissions=1240977501264&scope=bot%20applications.commands";
    public const string SupportServer = "https://discord.gg/Qg9eVB34sq";
    public const ulong SupportServerId = 1083619886980403272;
    public static Process? ThisProcess { get; set; }
    public static bool UseCookieApi { get; set; } = true;
    public const string FakeUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:102.0) Gecko/20100101 Firefox/102.0";
}