using Newtonsoft.Json;
using cc = DSharpPlus.CommandsNext.CommandContext;

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
        Logger.Log("Saved VRChat Protection JSON: BaseProtection");
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
    
    private static bool DoesModExist(string modName) => Base.ModNames?.FirstOrDefault(x => x == modName.ToLower())?.ToString() == modName.ToLower();
    
    private static bool DoesAuthorExist(string authorName) => Base.AuthorNames?.FirstOrDefault(x => x == authorName.ToLower())?.ToString() == authorName.ToLower();
    
    private static bool DoesPluginExist(string pluginName) => Base.PluginNames?.FirstOrDefault(x => x == pluginName.ToLower())?.ToString() == pluginName.ToLower();

    public static async Task AddUser(cc c, string userName, ulong userId, Roles role) {
        if (DoesUserExist(userId)) {
            await c.RespondAsync("User already exists.");
            return;
        }

        var item = new Users {
            UserName = userName,
            UserId = userId,
            Role = role
        };
        Base.Users?.Add(item);
        await c.RespondAsync($"Added {userName} as {role}.");
        Logger.LogEvent($"{c.User.Username} added user {{ {item.UserName} : {item?.UserId} : {item?.Role} }}");
        Save();
    }
    
    public static bool ErroredOnRemove { get; set; }
    public static Exception? ErroredException { get; set; }
    
    public static async Task RemoveUser(cc c, ulong userId) {
        if (!DoesUserExist(userId)) {
            await c.RespondAsync("Cannot remove user that does not exists.");
            return;
        }
        try {
            var user = Base.Users?.Single(x => x.UserId == userId);

            if (user != null) 
                Base.Users?.Remove(user);
            await c.RespondAsync("Removed user.");
            Logger.LogEvent($"{c.User.Username} removed user {{ {user?.UserName} : {user?.UserId} : {user?.Role} }}");
        } catch (Exception e) {
            ErroredOnRemove = true;
            ErroredException = e;
            Logger.SendLog(e);
            await c.RespondAsync($"An error has occured:\n```\n{e.Message}\n```");
        }
        Save();
    }
    
    public static async Task AddMod(cc c, string modName) {
        if (DoesModExist(modName)) {
            await c.RespondAsync("Mod already exists.");
            return;
        }
        Base.ModNames?.Add(modName);
        Save();
        await c.RespondAsync($"Added {modName}");
        Logger.LogEvent($"{c.User.Username} added mod {{ {modName} }}");
    }
    
    public static async Task RemoveMod(cc c, string modName) {
        if (!DoesModExist(modName)) {
            await c.RespondAsync("Cannot remove mod that does not exists.");
            return;
        }
        Base.ModNames?.Remove(modName);
        Save();
        await c.RespondAsync($"Removed {modName}");
        Logger.LogEvent($"{c.User.Username} removed mod {{ {modName} }}");
    }

    public static async Task AddAuthor(cc c, string authorName) {
        if (DoesAuthorExist(authorName)) {
            await c.RespondAsync("Author already exists.");
            return;
        }
        Base.AuthorNames?.Add(authorName);
        Save();
        await c.RespondAsync($"Added {authorName}");
        Logger.LogEvent($"{c.User.Username} added author {{ {authorName} }}");
    }
    
    public static async Task RemoveAuthor(cc c, string authorName) {
        if (!DoesAuthorExist(authorName)) {
            await c.RespondAsync("Cannot remove author that does not exists.");
            return;
        }
        Base.AuthorNames?.Remove(authorName);
        Save();
        await c.RespondAsync($"Removed {authorName}");
        Logger.LogEvent($"{c.User.Username} removed author {{ {authorName} }}");
    }

    public static async Task AddPlugin(cc c, string pluginName) {
        if (DoesPluginExist(pluginName)) {
            await c.RespondAsync("Plugin already exists.");
            return;
        }
        Base.PluginNames?.Add(pluginName);
        Save();
        await c.RespondAsync($"Added {pluginName}");
        Logger.LogEvent($"{c.User.Username} added plugin {{ {pluginName} }}");
    }
    
    public static async Task RemovePlugin(cc c, string pluginName) {
        if (!DoesPluginExist(pluginName)) {
            await c.RespondAsync("Cannot remove plugin that does not exists.");
            return;
        }
        Base.PluginNames?.Remove(pluginName);
        Save();
        await c.RespondAsync($"Removed {pluginName}");
        Logger.LogEvent($"{c.User.Username} removed plugin {{ {pluginName} }}");
    }
}