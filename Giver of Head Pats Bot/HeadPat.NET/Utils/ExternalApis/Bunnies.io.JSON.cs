using System.Text.Json;
using System.Text.Json.Serialization;

namespace HeadPats.Utils.ExternalApis;

public partial class Media {
    [JsonPropertyName("gif")]
    public string gif { get; }
    
    [JsonPropertyName("poster")]
    public string poster { get; }
}

public partial class Bunny {
    [JsonPropertyName("thisServed")]
    public int thisServed { get; }
    
    [JsonPropertyName("totalServed")]
    public int totalServed { get; }
    
    [JsonPropertyName("id")]
    public string id { get; }
    
    [JsonPropertyName("media")]
    public Media media { get; }
    
    [JsonPropertyName("source")]
    public string source { get; }
}

public static class BunnyJson {
    public static Bunny? BunnyData;

    public static void GetData(string data) => BunnyData = JsonSerializer.Deserialize<Bunny>(data);

    public static string? GetImage() => BunnyData?.media.gif;

    public static string? GetIdNumber() => BunnyData?.id;
}