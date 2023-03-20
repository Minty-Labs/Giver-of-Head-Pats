namespace HeadPats.Utils; 

public static class TimeConverter {
    /*
     Hour 19 (for Autumn/Winter)
     Hour 20 (for Spring/Summer)
     */
    /// <summary>
    /// Calculates the total seconds from the given DateTime
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static int GetSecondsFromUnixTime(this DateTime dateTime) => (int)dateTime.Subtract(new DateTime(1969, 12, 31, 20, 00, 00)).TotalSeconds;

    /// <summary>
    /// Calculates the total seconds from the given DateTimeOffset
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <returns></returns>
    public static int GetSecondsFromUnixTime(this DateTimeOffset dateTimeOffset) 
        => (int)dateTimeOffset.DateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
}
