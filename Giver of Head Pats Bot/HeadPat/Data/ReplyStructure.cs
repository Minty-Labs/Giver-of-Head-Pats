using Newtonsoft.Json;

namespace HeadPats.Data;

public class ReplyBase {
    [JsonProperty("Replies")]
    public List<Reply>? Replies { get; set; }
}

public class Reply {
    [JsonProperty("GuildID")]
    public ulong GuildId { get; set; }
    
    [JsonProperty("Trigger")]
    public string? Trigger { get; set; }
    
    [JsonProperty("Response")]
    public string? Response { get; set; }
    
    [JsonProperty("RequireOnlyTriggerText")]
    public bool OnlyTrigger { get; set; }
    
    [JsonProperty("DeleteTrigger")]
    public bool DeleteTrigger { get; set; }
    
    [JsonProperty("Delete Trigger If Is Only In Message")]
    public bool DeleteTriggerIfIsOnlyInMessage { get; set; }
}

public static class ReplyStructure {
    public static ReplyBase Base { get; set; } = Load();

    public static void CreateFile() {
        if (File.Exists($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Responses.json")) return;
        Base = new ReplyBase {
            Replies = new List<Reply> {
                new () {
                    GuildId = 0,
                    Trigger = "creeper",
                    Response = "Awwwww man!",
                    OnlyTrigger = false,
                    DeleteTrigger = false,
                    DeleteTriggerIfIsOnlyInMessage = false
                }
            }
        };
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Responses.json",
            JsonConvert.SerializeObject(Base, Formatting.Indented));
        Logger.Log("Created JSON: Replies");
        Save();
    }

    private static ReplyBase Load() {
        CreateFile();
        var j = JsonConvert.DeserializeObject<ReplyBase>(File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Responses.json"));
        return j ?? throw new Exception();
    }

    private static void Save() {
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Responses.json",
            JsonConvert.SerializeObject(Base, Formatting.Indented));
        Logger.Log("Saved JSON: Replies");
    }

    public static string GetResponse(string? trigger, ulong guildId) => Base.Replies?.FirstOrDefault(x => x.Trigger == trigger && x.GuildId == guildId)?.Response ?? "{{NULL}}";

    public static string GetInfo(string? trigger, ulong guildId) => Base.Replies?.FirstOrDefault(x => x.Trigger == trigger && x.GuildId == guildId)?.OnlyTrigger.ToString() ?? "{{NULL}}";
    
    public static string GetsDeleted(string? trigger, ulong guildId) => Base.Replies?.FirstOrDefault(x => x.Trigger == trigger && x.GuildId == guildId)?.DeleteTrigger.ToString() ?? "{{NULL}}";
    
    public static string GetsDeletedIfAlone(string? trigger, ulong guildId) => Base.Replies?.FirstOrDefault(x => x.Trigger == trigger && x.GuildId == guildId)?.DeleteTriggerIfIsOnlyInMessage.ToString() ?? "{{NULL}}";
    
    private static bool DoesTriggerExist(string? trigger, ulong guildId) => Base.Replies?.FirstOrDefault(x => x.Trigger == trigger && x.GuildId == guildId)?.Trigger == trigger;

    public static List<string?>? GetListOfTriggers() => Base.Replies?.Select(x => x.Trigger).ToList() ?? null;

    public static List<Reply>? GetListOfReplies() => Base.Replies ?? null;

    public static List<string> ListOfReplies() {
        var list = new List<string>();
        Base.Replies?.ForEach(r => {
            if (r.Response != null)
                list.Add(r.Response);
        });
        return list;
    }
    
    public static void AddValue(ulong guildId, string trigger, string response,
        bool requireOnlyTriggerText = false,
        bool deleteTrigger = false,
        bool deleteTriggerIfIsOnlyInMessage = false) {
        
        if (DoesTriggerExist(trigger, guildId)) {
            Logger.Log("Removing duplicate trigger");
            var itemToRemove = Base.Replies?.Single(t => string.Equals(t.Trigger, trigger, StringComparison.CurrentCultureIgnoreCase));
            if (itemToRemove != null) Base.Replies?.Remove(itemToRemove);
        }

        var item = new Reply {
            GuildId = guildId,
            Trigger = trigger,
            Response = response,
            OnlyTrigger = requireOnlyTriggerText,
            DeleteTrigger = deleteTrigger,
            DeleteTriggerIfIsOnlyInMessage = deleteTriggerIfIsOnlyInMessage
        };
        Base.Replies?.Add(item);
        Save();
    }

    public static bool ErroredOnRemove { get; set; }
    public static Exception? ErroredException { get; set; }

    public static void RemoveValue(ulong guildId, string trigger) {
        if (!DoesTriggerExist(trigger, guildId)) return;
        try {
            var reply = Base.Replies?.Single(x => x.Trigger == trigger && x.GuildId == guildId);

            if (reply != null)
                Base.Replies?.Remove(reply);
        }
        catch (Exception e) {
            ErroredOnRemove = true;
            ErroredException = e;
            Logger.SendLog(e);
        }
        Save();
    }
}