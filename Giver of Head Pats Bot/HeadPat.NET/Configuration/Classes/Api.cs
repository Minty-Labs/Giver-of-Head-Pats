using System.Text.Json.Serialization;
namespace HeadPats.Configuration.Classes;

public class Api {
    [JsonPropertyName("API Keys")] public ApiKeys ApiKeys { get; set; }
    [JsonPropertyName("Patreon API Data")] public PatreonClientData PatreonClientData { get; set; } = new();
    [JsonPropertyName("API Media URL Blacklist")] public List<string>? ApiMediaUrlBlacklist { get; set; }
}

public class ApiKeys {
    [JsonPropertyName("Unsplash Access Key")] public string? UnsplashAccessKey { get; set; } = "";
    [JsonPropertyName("Unsplash Secret Key")] public string? UnsplashSecretKey { get; set; } = "";
    [JsonPropertyName("CookieAPI Key")] public string? CookieClientApiKey { get; set; } = "";
    [JsonPropertyName("FluxpointAPI Key")] public string? FluxpointApiKey { get; set; } = "";
}