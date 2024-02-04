using System.Reflection;
using Discord.WebSocket;

namespace Michiru;

public static class Vars {
    public static string DNetVer { get; } = Assembly.GetAssembly(typeof(DiscordSocketClient))!.GetName().Version!.ToString(3);
    public const string Name = "Michiru";
    public const ulong ClientId = 477202627285876756;
    public const int TargetConfigVersion = 1;
    
    public const string Version = "1.0.1" + (IsDebug ? "-dev" : ""); // Major.Feature.Minor
    public static readonly DateTime BuildTime = IsDebug ? DateTime.UtcNow : new DateTime(2024, 2, 4, 17, 45, 00);
    public const bool IsDebug = false;
    public static string BuildDate { get; } = $"{BuildTime:F}";
    public static DateTime StartTime { get; set; }
    public static bool IsWindows { get; set; }
    public const string SupportServer = "https://discord.gg/Qg9eVB34sq";
    public const ulong SupportServerId = 1083619886980403272;
    public const string FakeUserAgent = $"{Name}/{Version} (https://github.com/Minty-Labs/Giver-of-Head-Pats)";
}