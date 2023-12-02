using System.Text.Json;
using System.Text.Json.Serialization;

namespace HeadPats.Utils.ExternalApis; 

public static class RandomFoxJson {
    public static FoxRoot? FoxData;
    public static void GetData(string data) => FoxData = JsonSerializer.Deserialize<FoxRoot>(data);
    public static string? GetImage() => FoxData?.Image?.Replace("\\", "");
    public static string? GetImageNumber() => FoxData?.Link?.Split('=')[1];
}

public partial class FoxRoot {
    [JsonPropertyName("image")]
    public string? Image { get; set; }
    [JsonPropertyName("link")]
    public string? Link { get; set; }
}