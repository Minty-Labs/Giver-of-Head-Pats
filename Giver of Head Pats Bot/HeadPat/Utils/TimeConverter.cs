namespace HeadPats.Utils; 

public static class TimeConverter {
    /*
     1969, 19 (for Daylight Savings)
     1969, 20 (for NO Daylight Savings)
     */
    /// <summary>
    /// Calculates the total seconds from the given DateTime
    /// </summary>
    /// <returns>Total Seconds</returns>
    public static int GetSecondsFromUnixTime(this DateTime dateTime) => (int)dateTime.Subtract(new DateTime(1969, 12, 31, 19, 00, 00)).TotalSeconds;

    /// <summary>
    /// Calculates the total seconds from the given DateTimeOffset
    /// </summary>
    /// <returns>Total Seconds</returns>
    public static int GetSecondsFromUnixTime(this DateTimeOffset dateTimeOffset) 
        => (int)dateTimeOffset.DateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
}