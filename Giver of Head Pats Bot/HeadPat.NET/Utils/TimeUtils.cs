namespace HeadPats.Utils; 

public static class TimeConverter {

    /// <summary>
    /// Converts the current datetime to a unix timestamp and returns it as an integer
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns>integer of seconds</returns>
    public static int GetSeconds(this DateTime dateTime) => (int)new DateTimeOffset(dateTime, TimeSpan.Zero).ToUnixTimeSeconds();
    
    /// <summary>
    /// Converts a unix timestamp to a DateTime
    /// </summary>
    /// <param name="unixTimeStamp">long</param>
    /// <returns>DateTime from unix timestamp</returns>
    public static DateTime UnixTimeStampToDateTime(this long unixTimeStamp) {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }
    
    public static string ConvertToDiscordTimestamp(this DateTimeOffset dateTime, TimestampFormat format) => $"<t:{dateTime.ToUnixTimeSeconds()}:{format.Flag()}>";
    public static string ConvertToDiscordTimestamp(this DateTime dateTime, TimestampFormat format) => $"<t:{new DateTimeOffset(dateTime, TimeSpan.Zero).ToUnixTimeSeconds()}:{format.Flag()}>";

    private static string Flag(this TimestampFormat format) 
        => format switch {
            TimestampFormat.ShortTime => "t",
            TimestampFormat.LongTime => "T",
            TimestampFormat.ShortDate => "d",
            TimestampFormat.LongDate => "D",
            TimestampFormat.ShortDateTime => "f",
            TimestampFormat.LongDateTime => "F",
            TimestampFormat.RelativeTime => "R",
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
    };
}

public enum TimestampFormat : byte {
    /// <summary>
    /// 't:' Short time (e.g. 9:41 PM)
    /// </summary>
    ShortTime,

    /// <summary>
    /// 'T:' Long time (e.g. 9:41:30 PM)
    /// </summary>
    LongTime,

    /// <summary>
    /// 'd:' Short date (e.g. 30/06/2021)
    /// </summary>
    ShortDate,

    /// <summary>
    /// 'D:' Long date (e.g. 30 June 2021)
    /// </summary>
    LongDate,

    /// <summary>
    /// 'f' (default) Short date/time (e.g. 30 June 2021 9:41 PM)
    /// </summary>
    ShortDateTime,

    /// <summary>
    /// 'F:' Long date/time (e.g. Wednesday, June, 30, 2021 9:41 PM)
    /// </summary>
    LongDateTime,

    /// <summary>
    /// 'R:' Relative time (e.g. 2 months ago, in an hour)
    /// </summary>
    RelativeTime
}