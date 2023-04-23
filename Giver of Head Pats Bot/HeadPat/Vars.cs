using System.Diagnostics;
using HeadPats.Managers;

namespace HeadPats; 

public static class Vars {
    public const string DSharpVer = "4.4.0-stable";
    public const string Name = "Giver of Head Pats";
    public const ulong ClientId = 489144212911030304;
    public const ulong TestGuildId = 279459962843955201;
#if DEBUG
    public const string Version = "4.5.0-dev2";
    public static readonly DateTime BuildTime = DateTime.Now;
    public const bool IsDebug = true;
#else
    public const string Version = "2023.4.1"; // Year.Month.Revision
    public static readonly DateTime BuildTime = new(2023, 4, 21, 8, 42, 00); // (year, month, day, hour, min, sec)
    public const bool IsDebug = false;
#endif
    public static string BuildDate { get; } = $"{BuildTime:F}";
    public static DateTime StartTime { get; set; }
    public static bool IsWindows { get; set; }
    public static readonly string InviteLink = $"https://discord.com/api/oauth2/authorize?client_id={ClientId}&permissions=1240977501264&scope=bot%20applications.commands";
    public const string SupportServer = "https://discord.gg/Qg9eVB34sq";

    public static readonly Config Config = Configuration.TheConfig;
    public static Process? ThisProcess { get; set; }
    public static bool EnableGifs { get; set; }
}