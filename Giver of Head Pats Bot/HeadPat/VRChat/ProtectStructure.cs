using Newtonsoft.Json;

namespace HeadPats.VRChat;
public class BaseProtection {
    [JsonProperty("Users")]
    public List<Users>? Users { get; set; }

    [JsonProperty("ModName")]
    public List<string>? ModNames { get; set; }

    [JsonProperty("PluginName")]
    public List<string>? PluginNames { get; set; }

    [JsonProperty("Author")]
    public List<string>? AuthorNames { get; set; }
}

public class Users {
    [JsonProperty("UserName")]
    public string? UserName { get; set; }

    [JsonProperty("UserId")]
    public ulong UserId { get; set; }

    [JsonProperty("Role")]
    public Roles Role { get; set; }
}

public enum Roles {
    Admin,
    Mod,
    None
}

internal static class ProtectStructure {
    internal static BaseProtection Base { get; set; } = Load();
    
    private static BaseProtection Load() {
        var path = BuildInfo.IsWindows ? 
            $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}VRChat{Path.DirectorySeparatorChar}Protection.json" :
            $"{Path.DirectorySeparatorChar}VRChat{Path.DirectorySeparatorChar}Protection.json";
        CreateFile();
        var j = JsonConvert.DeserializeObject<BaseProtection>(File.ReadAllText(
            path));
        return j ?? throw new Exception();
    }
    
    public static void CreateFile() {
        var path = BuildInfo.IsWindows ? 
            $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}VRChat{Path.DirectorySeparatorChar}Protection.json" :
            $"{Path.DirectorySeparatorChar}VRChat{Path.DirectorySeparatorChar}Protection.json";
        
        // if (!Directory.Exists($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}VRChat"))
        //     Directory.CreateDirectory($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}VRChat");
        
        if (File.Exists(path)) return;
        
        Base = new BaseProtection {
            Users = new List<Users> {
                new() {
                    UserName = "MintLily",
                    UserId = 167335587488071682,
                    Role = Roles.Admin
                }
            },
            ModNames = new List<string> { "astral" },
            PluginNames = new List<string> { "freeloading" },
            AuthorNames = new List<string> { "largestboi" }
        };
        File.WriteAllText(
            path, JsonConvert.SerializeObject(Base, Formatting.Indented));
        Logger.Log("Created VRChat Protection JSON: BaseProtection");
        Save();
    }
    
    private static void Save() {
        var path = BuildInfo.IsWindows ? 
            $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}VRChat{Path.DirectorySeparatorChar}Protection.json" :
            $"{Path.DirectorySeparatorChar}VRChat{Path.DirectorySeparatorChar}Protection.json";
        
        File.WriteAllText(path, JsonConvert.SerializeObject(Base, Formatting.Indented));
        Logger.Log("Created VRChat Protection JSON: BaseProtection");
    }
    
    public static List<string>? GetAllModsAsList() => Base.ModNames;
    
    public static List<string>? GetAllPluginsAsList() => Base.PluginNames;
    
    public static List<string>? GetAllAuthorsAsList() => Base.AuthorNames;
    
    public static List<Users> GetListOfUsers() {
        var list = new List<Users>();
        Base.Users?.ForEach(u => {
            list.Add(u);
        });
        return list;
    }
    
    public static List<string?> GetListOfUserNamesFromUsers(List<Users> users) => (from user in users where user.UserName is not null select user.UserName).ToList();
    
    public static List<ulong> GetListOfUserIdsFromUsers(List<Users> users) => users.Select(user => user.UserId).ToList();
    
    private static bool DoesUserExist(ulong userId) => Base.Users?.FirstOrDefault(x => x.UserId == userId)?.UserId == userId;
    
    public static string? AddedUserResponseString;
    
    public static void AddUser(string userName, ulong userId, Roles role) {
        if (DoesUserExist(userId)) {
            Logger.Log("User already exists.");
            AddedUserResponseString = "User already exists.";
            return;
        }

        var item = new Users {
            UserName = userName,
            UserId = userId,
            Role = role
        };
        Base.Users?.Add(item);
        Save();
    }
    
    public static bool ErroredOnRemove { get; set; }
    public static Exception? ErroredException { get; set; }
    
    public static void RemoveUser(ulong userId) {
        if (!DoesUserExist(userId)) return;
        try {
            var user = Base.Users?.Single(x => x.UserId == userId);

            if (user != null) 
                Base.Users?.Remove(user);
        } catch (Exception e) {
            ErroredOnRemove = true;
            ErroredException = e;
            Logger.SendLog(e);
        }
        Save();
    }
    
    public static void AddMod(string modName) {
        Base.ModNames?.Add(modName);
        Save();
    }
    
    public static void RemoveMod(string modName) {
        Base.ModNames?.Remove(modName);
        Save();
    }

    public static void AddAuthor(string authorName) {
        Base.AuthorNames?.Add(authorName);
        Save();
    }
    
    public static void RemoveAuthor(string authorName) {
        Base.AuthorNames?.Remove(authorName);
        Save();
    }

    public static void AddPlugin(string pluginName) {
        Base.PluginNames?.Add(pluginName);
        Save();
    }
    
    public static void RemovePlugin(string pluginName) {
        Base.PluginNames?.Remove(pluginName);
        Save();
    }
}