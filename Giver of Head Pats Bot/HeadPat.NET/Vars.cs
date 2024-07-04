using System.Reflection;
using Discord.WebSocket;

namespace HeadPats; 

public static class Vars {
    public static string DNetVer { get; } = Assembly.GetAssembly(typeof(DiscordSocketClient))!.GetName().Version!.ToString(3);
    public const string Name = "Giver of Head Pats";
    private static readonly Version VersionObj = new (5, 3, 1); // Major.Feature.Minor
    public const ulong ClientId = 489144212911030304;
    public const int TargetConfigVersion = 1;
    
    public static readonly string VersionStr = VersionObj.ToString(3) + (IsDebug ? "-dev" : "");
    public static readonly DateTime BuildTime = IsDebug ? DateTime.UtcNow : new DateTime(2024, 7, 4, 19, 50, 20);
    public const bool IsDebug = false;
    public static string BuildDate { get; } = $"{BuildTime:F}";
    public static DateTime StartTime { get; set; }
    public static bool IsWindows { get; set; }
    public static readonly string InviteLink = $"https://discord.com/api/oauth2/authorize?client_id={ClientId}&permissions=1240977501264&scope=bot%20applications.commands";
    public const string SupportServer = "https://discord.gg/Qg9eVB34sq";
    public const ulong SupportServerId = 1083619886980403272;
    public static bool UseLocalImages { get; set; } = true;
    public static readonly string BotUserAgent = $"Mozilla/5.0 {(IsWindows ? "(Windows NT 10.0; Win64; x64; rv:115.0)" : "(X11; Linux x86_64)")} (compatible; {Name}/{VersionObj.ToString(3)}; +https://discordapp.com)";
}