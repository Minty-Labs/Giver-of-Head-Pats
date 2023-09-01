﻿using System.Text.Json.Serialization;
using HeadPats.Commands.Slash.Contributors;
namespace HeadPats.Configuration.Classes;

public class DailyPat {
    [JsonPropertyName("User ID")] public ulong UserId { get; set; }
    [JsonPropertyName("User Name")] public string? UserName { get; set; }
    [JsonPropertyName("Set Epoch Time")] public long SetEpochTime { get; set; }
}