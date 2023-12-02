using System.Text.Json.Serialization;

namespace HeadPats.Configuration.Classes; 

public class PatreonClientData {
    [JsonPropertyName("Campaign ID")] public string? CampaignId { get; set; }
    [JsonPropertyName("Patreon Client ID")] public string? PatreonClientId { get; set; }
    [JsonPropertyName("Patreon Access Token")] public string? PatreonAccessToken { get; set; }
    [JsonPropertyName("Patreon Refresh Token")] public string? PatreonRefreshToken { get; set; }
}