using NekosSharp;
using Newtonsoft.Json;

namespace HeadPats.Data;

public class ReplyBase {
    public ReplyBase(List<Reply> replys) => Replys = replys;

    public List<Reply> Replys { get; internal set; }
}

public class Reply {
    public Reply(string trigger, string response, bool isGlobal) {
        Trigger = trigger;
        Response = response;
        IsGlobal = isGlobal;
    }

    public string Trigger { get; set; }
    public string Response { get; set; }
    public bool IsGlobal { get; set; }
}

public static class ReplyStructure {
    public static ReplyBase ReplyBase { get; private set; } = Load();

    private static void CreateFile() {
        if (File.Exists($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Responses.json")) return;
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Responses.json",
            JsonConvert.SerializeObject(new ReplyBase (new List<Reply> { new ("", "", false) })));
        Save();
    }

    private static ReplyBase Load() {
        CreateFile();
        var j = JsonConvert.DeserializeObject<ReplyBase>($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Responses.json");
        return j ?? throw new Exception();
    }

    private static void Save() => File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Responses.json",
        JsonConvert.SerializeObject(ReplyBase, Formatting.Indented));

    public static string GetResponse(string trigger) => ReplyBase.Replys.FirstOrDefault(x => x.Trigger == trigger)?.Response ?? "No Response";
    
    public static string GetIsGlobal(string trigger) => ReplyBase.Replys.FirstOrDefault(x => x.Trigger == trigger)?.IsGlobal.ToString() ?? "um";

    public static List<string> GetListOfTriggers() => ReplyBase.Replys.Select(x => x.Trigger).ToList();

    public static void AddValue(string trigger, string response, bool isGlobal = false) {
        var item = new Reply(trigger, response, isGlobal);
        ReplyBase.Replys.Add(item);
        Save();
    }
}