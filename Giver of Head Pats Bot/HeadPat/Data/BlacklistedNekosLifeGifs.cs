using HarmonyLib;
using Newtonsoft.Json;

namespace HeadPats.Data;

public class NekosLifeGifs {
    [JsonProperty("Blacklisted URLs")] public List<string>? Urls { get; set; }
}

public class BlacklistedNekosLifeGifs {
    private static NekosLifeGifs BlacklistedGifs = Load();

    public static List<string> StoredMemoryUrls;

    public static void CreateFile(bool sendCertainStartToSave = false) {
        if (File.Exists($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}BlacklistedGifs.json"))
            return;
        BlacklistedGifs = new NekosLifeGifs {
            Urls = new List<string> { "https://cdn.nekos.life/pat/pat_074.gif" }
        };
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}BlacklistedGifs.json",
            JsonConvert.SerializeObject(BlacklistedGifs, Formatting.Indented));
        Logger.Log("Created BlacklistedURLs JSON: BlacklistedGifs");
        Save(sendCertainStartToSave);
    }

    private static NekosLifeGifs Load() {
        CreateFile();
        var j = JsonConvert.DeserializeObject<NekosLifeGifs>(File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}BlacklistedGifs.json"));
        return j ?? throw new Exception();
    }

    private static void Save(bool runTempOnStart = false) {
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}BlacklistedGifs.json",
            JsonConvert.SerializeObject(BlacklistedGifs, Formatting.Indented));
        Logger.Log("Save BlacklistedURLs JSON: BlacklistedGifs");
        if (!runTempOnStart) return;
        foreach (var i in BlacklistedGifs.Urls!)
            StoredMemoryUrls.Add(i);
    }

    public static async Task AddBlacklist(DSharpPlus.CommandsNext.CommandContext cc, string url) {
        if (StoredMemoryUrls.Any(c => c.Contains(url))) {
            await cc.RespondAsync("URL is already blacklisted.");
            return;
        }

        BlacklistedGifs.Urls!.Add(url);
        StoredMemoryUrls.Add(url);
        await cc.RespondAsync("Added URL to the blacklist");
        Save();
    }

    public static async Task RemoveBlacklist(DSharpPlus.CommandsNext.CommandContext cc, string url) {
        if (StoredMemoryUrls.Any(c => !c.Contains(url))) {
            await cc.RespondAsync("URL is not blacklisted.");
            return;
        }

        BlacklistedGifs.Urls!.Remove(url);
        StoredMemoryUrls.Remove(url);
        await cc.RespondAsync("Removed URL from the blacklist");
        Save();
    }
}