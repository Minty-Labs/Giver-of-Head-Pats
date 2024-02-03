namespace Michiru.Utils; 

public static class TimeConverter {
    
    /// <summary>
    /// Calculates the total seconds from the given DateTime (Based on UTC)
    /// </summary>
    /// <param name="dateTime">DateTime</param>
    /// <returns>integer of seconds</returns>
    public static int GetSecondsFromUtcUnixTime(this DateTime dateTime) => (int)dateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

    /// <summary>
    /// Calculates the total seconds from the given DateTimeOffset (Based on UTC)
    /// </summary>
    /// <param name="dateTimeOffset">DateTimeOffset</param>
    /// <returns>integer of seconds</returns>
    public static int GetSecondsFromUtcUnixTime(this DateTimeOffset dateTimeOffset) => (int)dateTimeOffset.DateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
    
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
}