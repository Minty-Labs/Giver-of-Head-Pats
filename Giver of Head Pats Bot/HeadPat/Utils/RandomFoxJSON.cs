using Newtonsoft.Json;

namespace HeadPats.Utils; 

public static class RandomFoxJson {
    public static FoxRoot? FoxData;

    public static void GetData(string data) => FoxData = JsonConvert.DeserializeObject<FoxRoot>(data);

    public static string? GetImage() => FoxData?.Image?.Replace("\\", "");

    public static string? GetImageNumber() => FoxData?.Link?.Split('=')[1];
}

public partial class FoxRoot {
    [JsonProperty("image")]
    public string? Image { get; set; }
    [JsonProperty("link")]
    public string? Link { get; set; }
}