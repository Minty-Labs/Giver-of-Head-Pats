namespace HeadPats.Utils; 

public static class TimeConverter {
    /* hour: 19 (for Daylight Savings) */
    public static Int32 GetUnixTime(DateTime time) => (Int32)time.Subtract(new DateTime(1969, 12, 31, 19, 00, 00)).TotalSeconds;
}