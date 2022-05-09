using HeadPats.Commands;
using NekosSharp;
using Newtonsoft.Json;

namespace HeadPats.Data;

public class ReplyBase {
    [JsonProperty("ResponsesEnabled")]
    public bool Enabled { get; set; }
    
    [JsonProperty("Replys")]
    public List<Reply> Replys { get; internal set; }
}

public class Reply {
    [JsonProperty("Trigger")]
    public string Trigger { get; set; }
    
    [JsonProperty("Response")]
    public string Response { get; set; }
    
    [JsonProperty("IsGlobal")]
    public bool IsGlobal { get; set; }
    
    [JsonProperty("SendToGuild")]
    public ulong GuildId { get; set; }
}

public static class ReplyStructure {
    public static ReplyBase _ReplyBase { get; private set; } = Load();

    public static void CreateFile() {
        if (File.Exists($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Responses.json")) return;
        var r = new List<Reply> {
            new () {
                Trigger = "creeper",
                Response = "Awwwww man!",
                IsGlobal = true,
                GuildId = 0
            }
        };
        _ReplyBase = new ReplyBase();
        _ReplyBase.Replys = r;
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Responses.json",
            JsonConvert.SerializeObject(_ReplyBase, Formatting.Indented));
        Save();
    }

    private static ReplyBase Load() {
        CreateFile();
        var j = JsonConvert.DeserializeObject<ReplyBase>(File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Responses.json"));
        return j ?? throw new Exception();
    }

    private static void Save() => File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Responses.json",
        JsonConvert.SerializeObject(_ReplyBase, Formatting.Indented));

    public static string GetResponse(string trigger) => _ReplyBase.Replys.FirstOrDefault(x => x.Trigger == trigger)?.Response ?? "No Response";
    private static bool DoesTriggerExist(string trigger) => _ReplyBase.Replys.FirstOrDefault(x => x.Trigger == trigger)?.Trigger == trigger;
    
    public static string GetIsGlobal(string trigger) => _ReplyBase.Replys.FirstOrDefault(x => x.Trigger == trigger)?.IsGlobal.ToString() ?? "um";

    public static List<string> GetListOfTriggers() => _ReplyBase.Replys.Select(x => x.Trigger).ToList();

    public static void AddValue(string trigger, string response, bool isGlobal = false, ulong guildId = 0) {
        if (DoesTriggerExist(trigger)) {
            Logger.Log("Removing duplicate trigger");
            var itemToRemove = _ReplyBase.Replys.Single(t => string.Equals(t.Trigger, trigger, StringComparison.CurrentCultureIgnoreCase));
            _ReplyBase.Replys.Remove(itemToRemove);
        }

        var item = new Reply {
            Trigger = trigger,
            Response = response,
            IsGlobal = isGlobal,
            GuildId = guildId
        };
        _ReplyBase.Replys.Add(item);
        Save();
    }
}