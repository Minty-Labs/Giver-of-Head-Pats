using Newtonsoft.Json;

namespace HeadPats.Utils;

public partial class Media {
    [JsonProperty("gif")]
    public string gif { get; }
    
    [JsonProperty("poster")]
    public string poster { get; }
}

public partial class Bunny {
    [JsonProperty("thisServed")]
    public int thisServed { get; }
    
    [JsonProperty("totalServed")]
    public int totalServed { get; }
    
    [JsonProperty("id")]
    public string id { get; }
    
    [JsonProperty("media")]
    public Media media { get; }
    
    [JsonProperty("source")]
    public string source { get; }
}

public static class BunnyJson {
    public static Bunny? BunnyData;

    public static void GetData(string data) => BunnyData = JsonConvert.DeserializeObject<Bunny>(data);

    public static string? GetImage() => BunnyData?.media.gif;

    public static string? GetIdNumber() => BunnyData?.id;
}