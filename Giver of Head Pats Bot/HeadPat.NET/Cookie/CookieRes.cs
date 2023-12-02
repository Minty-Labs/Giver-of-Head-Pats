using System.Text.Json.Serialization;

namespace HeadPats.Cookie;

public class CookieRes {
    [JsonPropertyName("checksum")] public string? Checksum { get; set; }
    [JsonPropertyName("path")] public string? Path { get; set; }
}