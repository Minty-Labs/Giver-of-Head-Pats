﻿using HarmonyLib;
using Newtonsoft.Json;

namespace HeadPats.Data;

public class NekosLifeGifs {
    [JsonProperty("Blacklisted URLs")] public List<string>? Urls { get; set; }
}

public class BlacklistedNekosLifeGifs {
    public static NekosLifeGifs BlacklistedGifs = Load();

    public static void CreateFile() {
        if (File.Exists($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}BlacklistedGifs.json"))
            return;
        BlacklistedGifs = new NekosLifeGifs {
            Urls = new List<string> { "https://cdn.nekos.life/pat/pat_074.gif" }
        };
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}BlacklistedGifs.json",
            JsonConvert.SerializeObject(BlacklistedGifs, Formatting.Indented));
        Logger.Log("Created JSON: BlacklistedGifs");
        Save();
    }

    private static NekosLifeGifs Load() {
        CreateFile();
        var j = JsonConvert.DeserializeObject<NekosLifeGifs>(File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}BlacklistedGifs.json"));
        return j ?? throw new Exception();
    }

    private static void Save() {
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}BlacklistedGifs.json",
            JsonConvert.SerializeObject(BlacklistedGifs, Formatting.Indented));
        Logger.Log("Saved JSON: BlacklistedGifs");
    }
    
    public static async Task AddBlacklist(DSharpPlus.SlashCommands.InteractionContext cc, string url) {
        if (BlacklistedGifs.Urls!.Any(c => c.Contains(url))) {
            await cc.CreateResponseAsync("URL is already blacklisted.", true);
            return;
        }

        BlacklistedGifs.Urls!.Add(url);
        await cc.CreateResponseAsync($"Added URL to the blacklist: `{url}`");
        Save();
    }
    
    public static async Task RemoveBlacklist(DSharpPlus.SlashCommands.InteractionContext cc, string url) {
        if (BlacklistedGifs.Urls!.Any(c => !c.Contains(url))) {
            await cc.CreateResponseAsync("URL is not blacklisted.", true);
            return;
        }

        BlacklistedGifs.Urls!.Remove(url);
        await cc.CreateResponseAsync($"Removed URL from the blacklist `{url}`");
        Save();
    }
}