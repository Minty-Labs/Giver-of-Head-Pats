using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Managers;
using HeadPats.Utils;
//using cc = DSharpPlus.CommandsNext.CommandContext;
using ic = DSharpPlus.SlashCommands.InteractionContext;
using TaskScheduler = HeadPats.Managers.TaskScheduler;

namespace HeadPats.Commands; 

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
    
    [SlashCommand("AddGIFBlacklist", "Adds a GIF URL to a blacklist not to be shown in commands")]
    [SlashRequireOwner]
    public async Task AddBlacklistGif(ic c, [Option("URL", "URL of GIF you want to add to the blacklist", true)] string url) 
        => await BlacklistedNekosLifeGifs.AddBlacklist(c, url);

    [SlashCommand("RemoveGIFBlacklist", "Removes a GIF URL from the blacklist to be shown in commands")]
    [SlashRequireOwner]
    public async Task RemoveBlacklistGif(ic c, [Option("URL", "URL of GIF you want to remove from blacklist", true)] string url) 
        => await BlacklistedNekosLifeGifs.RemoveBlacklist(c, url);
    
    [SlashCommand("BlacklistUserFromPatCommand", "Blacklists a user from the pat command")]
    [SlashRequireOwner]
    public async Task BlacklistUserFromPatCommand(ic c, 
        [Option("MentionedUser", "Looks for User ID", true)] string mentionedUser,
        
        [Choice("Add", "add")]
        [Choice("Remove", "remove")]
        [Option("Action", "Add or Remove user to or from Blacklist")] string value) {
        if (string.IsNullOrWhiteSpace(mentionedUser) || string.IsNullOrWhiteSpace(value)) {
            await c.CreateResponseAsync("Incorrect command format! Please use the command like this:\n`/BlacklistUserFromPatCommand [@user] [add/remove]`", true);
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

        var valueIsTrue = value.ToLower().Contains('t');

        if (checkUser == null) {
            var newUser = new Users {
                UserId = discordUser.Id,
                UsernameWithNumber = $"{discordUser.Username}#{discordUser.Discriminator}",
                PatCount = 0,
                CookieCount = 0,
                IsUserBlacklisted = valueIsTrue ? 1 : 0
            };
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