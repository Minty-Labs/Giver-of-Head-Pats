using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HeadPats.Utils;
using cc = DSharpPlus.CommandsNext.CommandContext;

namespace HeadPats.MelonLoaderBlacklist;

public class ProtectCommands : BaseCommandModule {
    public ProtectCommands() => Logger.LoadModule("ProtectCommands");

    [Command("Blacklist"), Description("Lists the all the blacklist commands.")]
    [IsOnList]
    public async Task ListCommands(cc c) {
        var e = new DiscordEmbedBuilder();
        var builder = new DiscordMessageBuilder();
        e.WithTitle("Blacklist Commands");
        e.WithDescription("These commands are for all things blacklist.");
        e.WithColor(Colors.HexToColor("FFFFFF"));
        var footerText = ProtectStructure.Base.Users?.FirstOrDefault(x => x.UserId == c.User.Id)?.Role switch {
            Roles.Admin => "Your Role: Admin",
            Roles.Mod => "Your Role: Mod",
            Roles.None => "Your Role: None",
            _ => "Your Role: None"
        };
        e.WithFooter(footerText);
        e.WithTimestamp(DateTime.Now);
        e.AddField("Admin Commands", "`AddUser`, `RemoveUser`");
        e.AddField("Mod Commands", "`AddMod`, `AddAuthor`, `AddPlugin`, `ListUsers`, `ListMods`, `ListAuthors`, `ListPlugins`, `ListEverything`, `RemoveMod`, `RemoveAuthor`, `RemovePlugin`");
        await builder.WithReply(c.Message.Id).WithEmbed(e.Build()).SendAsync(c.Channel);
    }

    [Command("ListUsers"), Description("Lists all users in the authorized to edit the blacklist.")]
    [IsOnList]
    public async Task ListUsers(cc c) {
        var sb = new StringBuilder();
        var list = ProtectStructure.GetListOfUsers();
        foreach (var user in list) {
            sb.AppendLine($"{user.UserName} ({user.UserId})");
            sb.AppendLine($"{user.Role.ToString()}");
            sb.AppendLine();
        }
        
        using var ms = new MemoryStream();
        await using var sw = new StreamWriter(ms);
        await sw.WriteLineAsync(sb.ToString());
        
        await sw.FlushAsync();
        ms.Seek(0, SeekOrigin.Begin);

        var builder = new DiscordMessageBuilder();
        builder.AddFile("Users.txt", ms);
        await builder.WithReply(c.Message.Id).SendAsync(c.Channel);
    }

    [Command("ListMods"), Description("Lists all mods that are on the blacklist.")]
    [IsOnList]
    public async Task ListMods(cc c) {
        var sb = new StringBuilder();
        var list = ProtectStructure.GetAllModsAsList();
        if (list != null) {
            foreach (var mod in list) 
                sb.AppendLine($"{mod}");
        }

        using var ms = new MemoryStream();
        await using var sw = new StreamWriter(ms);
        await sw.WriteLineAsync(sb.ToString());
        
        await sw.FlushAsync();
        ms.Seek(0, SeekOrigin.Begin);

        var builder = new DiscordMessageBuilder();
        builder.AddFile("Mods.txt", ms);
        await builder.WithReply(c.Message.Id).SendAsync(c.Channel);
    }
    
    [Command("ListAuthors"), Description("Lists all authors that are on the blacklist.")]
    [IsOnList]
    public async Task ListAuthors(cc c) {
        var sb = new StringBuilder();
        var list = ProtectStructure.GetAllAuthorsAsList();
        if (list != null) {
            foreach (var author in list) 
                sb.AppendLine($"{author}");
        }

        using var ms = new MemoryStream();
        await using var sw = new StreamWriter(ms);
        await sw.WriteLineAsync(sb.ToString());
        
        await sw.FlushAsync();
        ms.Seek(0, SeekOrigin.Begin);

        var builder = new DiscordMessageBuilder();
        builder.AddFile("Authors.txt", ms);
        await builder.WithReply(c.Message.Id).SendAsync(c.Channel);
    }
    
    [Command("ListPlugins"), Description("Lists all plugins that are on the blacklist.")]
    [IsOnList]
    public async Task ListPlugins(cc c) {
        var sb = new StringBuilder();
        var list = ProtectStructure.GetAllPluginsAsList();
        if (list != null) {
            foreach (var plugin in list) 
                sb.AppendLine($"{plugin}");
        }

        using var ms = new MemoryStream();
        await using var sw = new StreamWriter(ms);
        await sw.WriteLineAsync(sb.ToString());
        
        await sw.FlushAsync();
        ms.Seek(0, SeekOrigin.Begin);

        var builder = new DiscordMessageBuilder();
        builder.AddFile("Plugins.txt", ms);
        await builder.WithReply(c.Message.Id).SendAsync(c.Channel);
    }
    
    [Command("ListEverything"), Description("Lists all users, mods, authors, and plugins that are on the blacklist.")]
    [IsOnList]
    public async Task ListAll(cc c) {
        var sb = new StringBuilder();
        var sb2 = new StringBuilder();
        var sb3 = new StringBuilder();
        var sb4 = new StringBuilder();
        var list = ProtectStructure.GetListOfUsers();
        foreach (var user in list) 
            sb.AppendLine($"{user.UserName} ({user.UserId}) - {user.Role.ToString()}");
        sb.AppendLine();

        var list2 = ProtectStructure.GetAllModsAsList();
        if (list2 != null) {
            foreach (var mod in list2) 
                sb2.AppendLine($"{mod}");
            sb2.AppendLine();
        }

        var list3 = ProtectStructure.GetAllAuthorsAsList();
        if (list3 != null) {
            foreach (var author in list3) 
                sb3.AppendLine($"{author}");
            sb3.AppendLine();
        }

        var list4 = ProtectStructure.GetAllPluginsAsList();
        if (list4 != null) {
            foreach (var plugin in list4) 
                sb4.AppendLine($"{plugin}");
            sb4.AppendLine();
        }

        using var ms = new MemoryStream();
        await using var sw = new StreamWriter(ms);
        await sw.WriteLineAsync("-=- Users -=-");
        await sw.WriteLineAsync(sb.ToString());
        await sw.WriteLineAsync("-=- Mods -=-");
        await sw.WriteLineAsync(sb2.ToString());
        await sw.WriteLineAsync("-=- Authors -=-");
        await sw.WriteLineAsync(sb3.ToString());
        await sw.WriteLineAsync("-=- Plugins -=-");
        await sw.WriteLineAsync(sb4.ToString());
        
        await sw.FlushAsync();
        ms.Seek(0, SeekOrigin.Begin);

        var builder = new DiscordMessageBuilder();
        builder.AddFile("Everything.txt", ms);
        await builder.WithReply(c.Message.Id).SendAsync(c.Channel);
    }
    
    private Roles GetRole(string role)
        => role switch {
            "admin" => Roles.Admin,
            "mod" => Roles.Mod,
            "moderator" => Roles.Mod,
            _ => Roles.None
        };

    [Command("AddUser"), Description("Add a user to the be able to edit the protect list.")]
    [IsAdmin]
    public async Task AddUser(cc c, string userName, string userId, string role) {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(role)) {
            await c.RespondAsync($"Incorrect format. Usage: `{BuildInfo.Config.Prefix}adduser [userName] [userId] [role]`");
            return;
        }
        var uId = ulong.Parse(userId);
        
        await ProtectStructure.AddUser(c, userName, uId, GetRole(role.ToLower()));
    }
    
    [Command("AddUser")]
    [IsAdmin]
    public async Task AddUser(cc c, DiscordUser? user, string role) {
        if (user is null || string.IsNullOrWhiteSpace(role)) {
            await c.RespondAsync($"Incorrect format. Usage: `{BuildInfo.Config.Prefix}adduser [@User] [role]`");
            return;
        }
        
        await ProtectStructure.AddUser(c, user.Username, user.Id, GetRole(role.ToLower()));
    }
    
    [Command("AddUser")]
    [IsAdmin]
    public async Task AddUser(cc c, string userId, string role) {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(role)) {
            await c.RespondAsync($"Incorrect format. Usage: `{BuildInfo.Config.Prefix}adduser [userId] [role]`");
            return;
        }
        var user = await c.Guild.GetMemberAsync(ulong.Parse(userId.Replace("<@", "").Replace(">", "")));
        
        await ProtectStructure.AddUser(c, user.Username, user.Id, GetRole(role.ToLower()));
    }

    [Command("RemoveUser"), Description("Remove a user from the be able to edit the protect list.")]
    [IsAdmin]
    public async Task RemoveUser(cc c, string userId) {
        if (string.IsNullOrWhiteSpace(userId)) {
            await c.RespondAsync($"Please provide a UserID. Usage: `{BuildInfo.Config.Prefix}removeuser [UserID]`");
            return;
        }
        
        await ProtectStructure.RemoveUser(c, ulong.Parse(userId));
    }

    [Command("AddMod"), Description("Add a mod name to the blacklist")]
    [IsAdminOrMod]
    public async Task AddMod(cc c, [RemainingText] string name) {
        if (string.IsNullOrWhiteSpace(name)) {
            await c.RespondAsync($"Please provide a mod name. Usage: `{BuildInfo.Config.Prefix}addmod [name]`");
            return;
        }
        
        await ProtectStructure.AddMod(c, name);
    }
    
    [Command("RemoveMod"), Description("Remove a mod name from the blacklist")]
    [IsAdminOrMod]
    public async Task RemoveMod(cc c, [RemainingText] string name) {
        if (string.IsNullOrWhiteSpace(name)) {
            await c.RespondAsync($"Please provide a mod name. Usage: `{BuildInfo.Config.Prefix}removemod [name]`");
            return;
        }
        
        await ProtectStructure.RemoveMod(c, name);
    }
    
    [Command("AddAuthor"), Description("Add a author name to the blacklist")]
    [IsAdminOrMod]
    public async Task AddAuthor(cc c, [RemainingText] string author) {
        if (string.IsNullOrWhiteSpace(author)) {
            await c.RespondAsync($"Please provide an author name. Usage: `{BuildInfo.Config.Prefix}removeauthor [author]`");
            return;
        }
        
        await ProtectStructure.AddAuthor(c, author);
    }
    
    [Command("RemoveAuthor"), Description("Remove a author name from the blacklist")]
    [IsAdminOrMod]
    public async Task RemoveAuthor(cc c, [RemainingText] string author) {
        if (string.IsNullOrWhiteSpace(author)) {
            await c.RespondAsync($"Please provide an author name. Usage: `{BuildInfo.Config.Prefix}removeauthor [author]`");
            return;
        }
        
        await ProtectStructure.RemoveAuthor(c, author);
    }
    
    [Command("AddPlugin"), Description("Add a plugin name to the blacklist")]
    [IsAdminOrMod]
    public async Task AddPlugin(cc c, [RemainingText] string plugin) {
        if (string.IsNullOrWhiteSpace(plugin)) {
            await c.RespondAsync($"Please provide a plugin name. Usage: `{BuildInfo.Config.Prefix}addplugin [plugin]`");
            return;
        }
        
        await ProtectStructure.AddPlugin(c, plugin);
    }
    
    [Command("RemovePlugin"), Description("Remove a plugin name from the blacklist")]
    [IsAdminOrMod]
    public async Task RemovePlugin(cc c, [RemainingText] string plugin) {
        if (string.IsNullOrWhiteSpace(plugin)) {
            await c.RespondAsync($"Please provide a plugin name. Usage: `{BuildInfo.Config.Prefix}removeplugin [plugin]`");
            return;
        }
        
        await ProtectStructure.RemovePlugin(c, plugin);
    }

    public static bool LookingForAnswer;
    [Command("ResetEntireList"), Description("Reset the entire protect list")]
    [RequireOwner]
    public async Task ResetEntireList(cc c) {
        if (LookingForAnswer) return;
        await c.RespondAsync("Are you sure you want to reset the entire protect list? (y/n)");
    }
}