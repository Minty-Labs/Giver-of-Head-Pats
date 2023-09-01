using System.Text.Json.Serialization;
using HeadPats.Commands.Slash.Contributors;
namespace HeadPats.Configuration.Classes;

public class IrlQuotes {
    public bool Enabled { get; set; }
    [JsonPropertyName("Channel ID")] public ulong ChannelId { get; set; }
    [JsonPropertyName("Set Epoch Time")] public long SetEpochTime { get; set; }
}