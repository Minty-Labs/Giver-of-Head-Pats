namespace HeadPats.Utils; 

public static class TimeConverter {
    public static Int32 GetUnixTime(DateTime time) => (Int32)time.Subtract(new DateTime(1969, 12, 31, 20, 00, 00)).TotalSeconds;
}