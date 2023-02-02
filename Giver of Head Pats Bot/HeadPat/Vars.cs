using System.Diagnostics;
using HeadPats.Managers;

namespace HeadPats; 

public static class Vars {
    public const string DSharpVer = "4.3.0-stable";
    public const string Name = "Giver of Head Pats";
    public const ulong ClientId = 489144212911030304;
    public const ulong TestGuildId = 279459962843955201;
#if DEBUG
    public const string Version = "4.5.0-dev2";
    public static readonly DateTime BuildTime = DateTime.Now;
    public static bool IsDebug = true;
#elif !DEBUG
    public const string Version = "4.11.0";
    public static readonly DateTime BuildTime = new(2023, 2, 2, 17, 54, 00); // (year, month, day, hour, min, sec)
    public static bool IsDebug = false;
#endif
    public static string BuildDateShort = $"{BuildTime.Day} {GetMonth(BuildTime.Month)} @ {BuildTime.Hour}:{ChangeSingleNumber(BuildTime.Minute)}";
    public static string BuildDate = $"Last Updated: {BuildDateShort}";
    public static DateTime StartTime = new();
    public static bool IsWindows;
    public static readonly string InviteLike = $"https://discord.com/api/oauth2/authorize?client_id={ClientId}&permissions=1240977501264&scope=bot%20applications.commands";

    private static string GetMonth(int month) {
        return month switch {
            1 => "January",
            2 => "February",
            3 => "March",
            4 => "April",
            5 => "May",
            6 => "June",
            7 => "July",
            8 => "August",
            9 => "September",
            10 => "October",
            11 => "November",
            12 => "December",
            _ => ""
        };
    }

    private static string ChangeSingleNumber(int num) {
        return num switch {
            0 => "00",
            1 => "01",
            2 => "02",
            3 => "03",
            4 => "04",
            5 => "05",
            6 => "06",
            7 => "07",
            8 => "08",
            9 => "09",
            _ => num.ToString()
        };
    }
        
    public static readonly Config Config = Configuration.TheConfig;
    public static Process? ThisProcess { get; set; }
}