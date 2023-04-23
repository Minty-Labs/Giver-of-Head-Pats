/*using Newtonsoft.Json;

namespace HeadPats.Utils; 

public static class NekoLoveJson {
    public static NekoLove? NekoData;

    public static void GetData(string data) => NekoData = JsonConvert.DeserializeObject<NekoLove>(data);

    public static string? GetImage() => NekoData?.Url;
    
    public static bool ImageHasValidExtension() => GetImage()!.Contains(".png") || GetImage()!.Contains(".jpg") || GetImage()!.Contains(".jpeg") || GetImage()!.Contains(".gif");

    public static int? GetStatusCode() => NekoData?.Code;
}

public partial class NekoLove {
    [JsonProperty("code")]
    public int? Code { get; set; }
    [JsonProperty("url")]
    public string? Url { get; set; }
}*/