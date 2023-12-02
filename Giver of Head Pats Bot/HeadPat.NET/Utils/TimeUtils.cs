namespace HeadPats.Utils; 

public static class TimeConverter {
    /*
     Hour 19 (for Autumn/Winter)
     Hour 20 (for Spring/Summer)
     */
    /// <summary>
    /// Calculates the total seconds from the given DateTime (Based on EST)
    /// </summary>
    /// <param name="dateTime">DateTime</param>
    /// <returns>integer of seconds</returns>
    public static int GetSecondsFromUnixTime(this DateTime dateTime) => (int)dateTime.Subtract(new DateTime(1969, 12, 31, 19, 00, 00)).TotalSeconds;
    
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
}