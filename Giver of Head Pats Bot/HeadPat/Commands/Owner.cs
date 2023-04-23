using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Managers;
using ic = DSharpPlus.SlashCommands.InteractionContext;
using TaskScheduler = HeadPats.Managers.TaskScheduler;

namespace HeadPats.Commands;

public class ClassicOwner : BaseCommandModule {
    public ClassicOwner() => Logger.LoadModule("ClassicOwner");
    
    [Command("MultiKick"), Aliases("mk"), Description("Kicks multiple users at once"), RequireOwner]
    public async Task MultiKick(CommandContext c, [Description("List of users separated by commas")] string userIds,
        [Description("Reason for kick"), RemainingText] string reason = "No reason provided.") {
        if (string.IsNullOrWhiteSpace(userIds)) {
            await c.RespondAsync("Please provide one or more user IDs, separated by commas.");
            return;
        }
        
        var ulongList = userIds.Split(',').Select(ulong.Parse).ToList();
        var num = 0;
        foreach (var id in ulongList) {
            if (num != 0) // Instantly do the first, delay the rest
                await Task.Delay(TimeSpan.FromSeconds(2));

            var user = await c.Guild.GetMemberAsync(id);
            await user.RemoveAsync(reason);
            num++;
        }
        
        await c.RespondAsync($"Finished kicking {num} users.");
    }
    
    [Command("AddToFullBlacklist"), Aliases("atfb"), Description("Adds a guild to the full blacklist"), RequireOwner]
    public async Task AddToFullBlacklist(CommandContext c, [Description("Guild ID")] ulong guildId) {
        Vars.Config.FullBlacklistOfGuilds!.Add(guildId);
        Configuration.Save();
        await c.RespondAsync($"Added {guildId} to the full blacklist.");
    }
    
    [Command("ToggleGifs"), RequireOwner]
    public async Task ToggleGifs(CommandContext c) {
        Vars.EnableGifs = !Vars.EnableGifs;
        await c.RespondAsync($"Gifs are now {(Vars.EnableGifs ? "enabled" : "disabled")}.");
    }
    
    [Command("SetCookieApiKey"), RequireOwner]
    public async Task SetCookieApiKey(CommandContext c, [Description("API Key")] string apiKey) {
        Vars.Config.CookieClientApiKey = apiKey;
        Configuration.Save();
        await c.RespondAsync("Set API Key.");
    }
}

public class RequireUserIdAttribute : SlashCheckBaseAttribute {
    public ulong UserId;

    public RequireUserIdAttribute(ulong userId) => UserId = userId;

    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) => ctx.User.Id == UserId;
}

public class SlashOwner : ApplicationCommandModule {
    
    [SlashCommand("UpdatePresence", "Updates the bot's Presence", false)]
    [SlashRequireOwner]
    public async Task UpdatePresence(ic c,
        [Choice("false", "false")]
        [Choice("true", "true")]
        [Option("clearAndInvokeNew", "Clears Timers and restarts the Scheduled Task")] string clearAndInvokeNew,
        
        [Choice("false", "false")]
        [Choice("true", "true")]
        [Option("disableAndSetStaticStatus", "Ignores first boolean, switches to static status")] string disableAndSetStaticStatus = "false") {
        if (disableAndSetStaticStatus.Equals("true")) {
            TaskScheduler.StopStatusLoop();
            await Task.Delay(1000);
            TaskScheduler.NormalDiscordActivity();
            await c.CreateResponseAsync("Set status to static default text > `lots of cuties | hp!help`", true);
            return;
        }

        if (clearAndInvokeNew.Equals("true") && disableAndSetStaticStatus.Equals("false")) {
            TaskScheduler.StopStatusLoop();
            await Task.Delay(1000);
            TaskScheduler.StartStatusLoop();
        }

        await c.CreateResponseAsync("Reset status scheduled task", true);
    }
    
    [SlashCommand("GetPresence", "Gets users with the given presence", false)]
    [SlashRequireOwner] // Made by Eric van Fandenfart
    public async Task GetPresence(ic c, [Option("Activity", "Text to search presences with", true)] string activity) {
        if (string.IsNullOrWhiteSpace(activity)) {
            await c.CreateResponseAsync("activity field was empty", true);
            return;
        }
        try
        {
            await c.Guild.RequestMembersAsync(presences: true);
            List<DiscordMember> users = new();
            foreach (var member in c.Guild.Members) {
                if (member.Value?.Presence?.Activities == null)
                    continue;

                foreach (var dcactivity in member.Value.Presence.Activities) {
                    if (dcactivity?.Name == null)
                        continue;
                    if (dcactivity.Name.Contains(activity, StringComparison.CurrentCultureIgnoreCase)) 
                        users.Add(member.Value);
                }
            }
            var sb = new StringBuilder();
            for (var i = 0; i < users.Count; i++) {
                sb.Append($"<@{users[i].Id}>\r\n");
                if (i == 0 || 1 % 20 != 0) continue;
                await c.CreateResponseAsync(sb.ToString());
                sb.Clear();
            }
            if (sb.Length > 0)
                await c.CreateResponseAsync(sb.ToString());
            else
                await c.CreateResponseAsync($"No user had the presence of {activity}");
        }
        catch (Exception ex) {
            await c.CreateResponseAsync($"```\n{ex.Message}\n```");
        }
    }
    
    [SlashCommand("Guilds", "Lists all guilds the bot is in.", false)]
    [SlashRequireOwner]
    public async Task ListGuilds(ic c) {
        var guilds = c.Client.Guilds;
        var sb = new StringBuilder();
        sb.AppendLine($"Guild Count: {guilds.Count}");
        foreach (var g in guilds) {
            sb.AppendLine(g.Value.Name);
            sb.AppendLine(g.Key.ToString());
            sb.AppendLine();
        }

        var overLimit = sb.ToString().Length > 2000;
        var f = sb.ToString();

        if (overLimit) {
            using var ms = new MemoryStream();
            await using var sw = new StreamWriter(ms);
            await sw.WriteAsync(f);
            await sw.FlushAsync();
            ms.Seek(0, SeekOrigin.Begin);
            await c.CreateResponseAsync(new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddFile("Guilds.txt", ms)));
            return;
        }
        await c.CreateResponseAsync(f);
    }

    [SlashCommand("LeaveGuild", "Forces the bot to leave a guild")]
    [SlashRequireOwner]
    public async Task LeaveGuild(ic c, [Option("GuildID", "Guild ID to leave from", true)] string guildId) {
        if (string.IsNullOrWhiteSpace(guildId)) {
            await c.CreateResponseAsync("Please provide a guild ID.", true);
            return;
        }

        var id = ulong.Parse(guildId);
        var guild = await c.Client.GetGuildAsync(id);

        await guild.LeaveAsync();
        await c.CreateResponseAsync($"Left the server: {guild.Name}");
    }
}

[SlashCommandGroup("Blacklist", "Blacklist related commands")]
public class BlacklistCommands : ApplicationCommandModule {
    [SlashCommand("AddGIF", "Adds a GIF URL to a blacklist not to be shown in commands", false)]
    [SlashRequireOwner]
    public async Task AddBlacklistGif(ic c, [Option("URL", "URL of GIF you want to add to the blacklist", true)] string url) 
        => await BlacklistedNekosLifeGifs.AddBlacklist(c, url);

    [SlashCommand("RemoveGIF", "Removes a GIF URL from the blacklist to be shown in commands", false)]
    [SlashRequireOwner]
    public async Task RemoveBlacklistGif(ic c, [Option("URL", "URL of GIF you want to remove from blacklist", true)] string url) 
        => await BlacklistedNekosLifeGifs.RemoveBlacklist(c, url);
    
    [SlashCommand("UserFromPatCommand", "Blacklists a user from the pat command", false)]
    [SlashRequireOwner]
    public async Task BlacklistUserFromPatCommand(ic c, 
        [Option("UserId", "Looks for User ID", true)] string mentionedUser,
        
        [Choice("Add", "add")]
        [Choice("Remove", "remove")]
        [Option("Action", "Add or Remove user to or from Blacklist")] string value) {
        if (string.IsNullOrWhiteSpace(mentionedUser) || string.IsNullOrWhiteSpace(value)) {
            await c.CreateResponseAsync("Incorrect command format! Please use the command like this:\n`/BlacklistUserFromPatCommand [@user] [\"add\"/\"remove\"]`", true);
            return;
        }
        
        var getUserIdFromMention = mentionedUser.Replace("<@", "").Replace(">", "");

        await using var db = new Context();
        var checkUser = db.Users.AsQueryable()
            .Where(u => u.UserId.Equals(getUserIdFromMention)).ToList().FirstOrDefault();

        var discordUser = await c.Client.GetUserAsync(ulong.Parse(getUserIdFromMention));

        if (discordUser == null) {
            await c.CreateResponseAsync("Discord user not found! Getting user as guild member...", true);
            discordUser = await c.Guild.GetMemberAsync(ulong.Parse(getUserIdFromMention), true);
            
            if (discordUser == null) {
                await c.CreateResponseAsync("Discord user as a guild member not found! Stopping command.", true);
                return;
            }
        }

        var valueIsTrue = value.ToLower().Equals("add");

        if (checkUser == null) {
            var newUser = new Users {
                UserId = discordUser.Id,
                UsernameWithNumber = $"{discordUser.Username}#{discordUser.Discriminator}",
                PatCount = 0,
                CookieCount = 0,
                IsUserBlacklisted = valueIsTrue ? 1 : 0
            };
            Logger.Log("Added user to database");
            db.Users.Add(newUser);
            db.Users.Update(checkUser!);
        }
        else {
            checkUser.IsUserBlacklisted = valueIsTrue ? 1 : 0;
            db.Users.Update(checkUser);
        }

        if (valueIsTrue) {
            await c.CreateResponseAsync("User is now blacklisted from the pat command.");
            return;
        }
        
        await c.CreateResponseAsync("User is no longer blacklisted from the pat command.");
    }

    [SlashCommand("AddGuild", "Adds a guild to the blacklist", false)]
    [SlashRequireOwner]
    public async Task AddGuildToBlacklist(ic c,
        [Option("GuildId", "Guild ID to add to blacklist", true)] string guildId,
        [Option("Commands", "List the commands to block (separate by comma)", true)] string commands) 
        => await BlacklistedCmdsGuilds.AddBlacklist(c, guildId, commands);
    
    [SlashCommand("RemoveGuild", "Removes a guild from the blacklist", false)]
    [SlashRequireOwner]
    public async Task RemoveGuildFromBlacklist(ic c,
        [Option("GuildId", "Guild ID to remove from blacklist", true)] string guildId) 
        => await BlacklistedCmdsGuilds.RemoveBlacklist(c, guildId);
}

[SlashCommandGroup("Config", "Config related commands")]
public class SlashConfigCommands : ApplicationCommandModule {
    [SlashCommand("updateunsplashaccesskey", "Updates the unsplash api key", false)]
    [SlashRequireOwner]
    public async Task UpdateUnsplashAccessKey(ic c, [Option("AccessKey", "Unsplash Access Key", true)] string accessKey) {
        Vars.Config.UnsplashAccessKey = accessKey;
        await c.CreateResponseAsync("Updated Unsplash Access Key", true);
        Configuration.Save();
    }
        
    [SlashCommand("updateunsplashsecretkey", "Updates the unsplash secret key", false)]
    [SlashRequireOwner]
    public async Task UpdateUnsplashSecretKey(ic c, [Option("SecretKey", "Unsplash Secret Key", true)] string secretKey) {
        Vars.Config.UnsplashSecretKey = secretKey;
        await c.CreateResponseAsync("Updated Unsplash Secret Key", true);
        Configuration.Save();
    }
}