using Newtonsoft.Json;

namespace HeadPats.Data;

public class ContributorBase {
    public List<Contributor> Base { get; set; }
}

public class Contributor {
    public string UserName { get; set; }
    public string Info { get; set; }
}

public class ContributorStructure {
    public static ContributorBase Base { get; set; } = Load();

    public static void CreateFile() {
        if (File.Exists($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Contributors.json")) return;
        Base = new ContributorBase {
            Base = new List<Contributor> {
                new () {
                    UserName = "MintLily",
                    Info = "Bot Owner, Creator, Maintainer"
                }
            }
        };
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Contributors.json",
            JsonConvert.SerializeObject(Base, Formatting.Indented));
        Logger.Log("Created Responses JSON: ContributorBase");
        Save();
    }

    private static ContributorBase Load() {
        CreateFile();
        var j = JsonConvert.DeserializeObject<ContributorBase>(File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Contributors.json"));
        return j ?? throw new Exception();
    }

    private static void Save() {
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Contributors.json",
            JsonConvert.SerializeObject(Base, Formatting.Indented));
        Logger.Log("Saved Responses JSON: ContributorBase");
    }
    
    private static bool DoesUserNameExist(string? name) => Base.Base.FirstOrDefault(n => n.UserName == name)?.UserName == name;
    
    public static void AddValue(string userName, string info) {
        if (DoesUserNameExist(userName)) {
            Logger.Log("Removing duplicate user");
            var itemToRemove = Base.Base.Single(u => string.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase));
            Base.Base.Remove(itemToRemove);
        }

        var item = new Contributor {
            UserName = userName,
            Info = info
        };
        Base.Base.Add(item);
        Save();
    }
    
    public static bool ErroredOnRemove;
    public static Exception? ErroredException;
    
    public static void RemoveValue(string userName) {
        if (!DoesUserNameExist(userName)) return;
        try {
            var contributor = Base.Base.Single(u => u.UserName == userName);

            Base.Base.Remove(contributor);
        }
        catch (Exception e) {
            ErroredOnRemove = true;
            ErroredException = e;
            Logger.SendLog(e);
        }
        Save();
    }
}