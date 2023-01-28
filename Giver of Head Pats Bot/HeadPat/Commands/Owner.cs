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

// public class Owner : BaseCommandModule {
//     public Owner() => Logger.Loadodule("OwnerCommands");
//
//     private void FooterText(DiscordEmbedBuilder em, string extraText = "") {
//         em.WithTimestamp(DateTime.Now);
//         em.WithFooter($"{(string.IsNullOrWhiteSpace(extraText) ? "" : $"{extraText}")}");
//     }
//
//     /*[Command("ChangeActivity"), Aliases("ca"), Description("Change the bot\'s Activity")]
//     [RequireOwner]
//     public async Task ChangeActivity(cc c, [Description("Online Status (Online, Idle, etc)")] string status,
//         [Description("Activity (playing, watching, etc)")] string activityType, 
//         [RemainingText, Description("Extra Text to add after the activity")] string args = "") {
//         if (activityType.ToLower().Contains("stream") && !args.Contains("%http")) {
//             await c.RespondAsync("Streaming needs a URL: Something like `%http...`");
//             return;
//         }
//         
//         var getStatus = status switch {
//             "offline"   => UserStatus.Offline,
//             "invisible" => UserStatus.Invisible,
//             "dnd"       => UserStatus.DoNotDisturb,
//             "idle"      => UserStatus.Idle,
//             "online"    => UserStatus.Online,
//             _           => UserStatus.Offline
//         };
//
//         var getActivity = activityType switch {
//             "play"    => ActivityType.Playing,
//             "listen"  => ActivityType.ListeningTo,
//             "watch"   => ActivityType.Watching,
//             "stream"  => ActivityType.Streaming,
//             "compete" => ActivityType.Competing,
//             "other"   => ActivityType.Custom,
//             _         => ActivityType.Playing
//         };
//         
//         var arg = args.Split('%');
//         var url = !args.Contains("%http") ? "" : arg[1];
//         var name = string.IsNullOrWhiteSpace(args) ? BuildInfo.Config.Game : arg[0];
//
//         BuildInfo.Config.Game = name;
//         BuildInfo.Config.ActivityType = Program.GetActivityAsString(getActivity);
//         BuildInfo.Config.StreamingUrl = string.IsNullOrWhiteSpace(url) ? "" : url;
//         Configuration.Save();
//         
//         await c.Client.UpdateStatusAsync(new DiscordActivity {
//             Name = name,
//             ActivityType = getActivity,
//             StreamUrl = string.IsNullOrWhiteSpace(url) ? "" : url
//         }, getStatus);
//
//         var color = status switch {
//             "offline"   => "747F8D",
//             "invisible" => "747F8D",
//             "dnd"       => "ED4245",
//             "idle"      => "FAA81A",
//             "online"    => "3BA55D",
//             _           => "FFFFFF"
//         };
//
//         var changeToStreamColor = activityType == "stream" && status is not ("offline" or "invisible");
//         var changeToCompeteColor = activityType == "compete" && status is not ("offline" or "invisible");
//         if (changeToStreamColor) color = "593695";
//         if (changeToCompeteColor) color = "C69164";
//         
//         var e = new DiscordEmbedBuilder();
//         e.WithTitle("Changed Status");
//         e.WithColor(Colors.HexToColor(color));
//         e.WithDescription($"Game: {name}\nActivityType: {Program.GetActivityAsString(getActivity)}\n{(string.IsNullOrWhiteSpace(url) ? "" : $"Stream URL: {url}")}");
//         FooterText(e);
//         await c.RespondAsync(e.Build());
//     }*/
//
//     [Command("Guilds"), Aliases("listguilds", "lg"), Description("Lists all guilds the bot is in. [Owner]")]
//     [RequireOwner]
//     public async Task ListGuilds(cc c) {
//         var guilds = c.Client.Guilds;
//         var sb = new StringBuilder();
//         sb.AppendLine($"Guild Count: {guilds.Count}");
//         foreach (var g in guilds) {
//             sb.AppendLine(g.Value.Name);
//             sb.AppendLine(g.Key.ToString());
//             sb.AppendLine();
//         }
//
//         var overLimit = sb.ToString().Length > 2000;
//         var f = sb.ToString();
//
//         await c.RespondAsync(overLimit ? f[..1999] : f);
//         if (overLimit)
//             await c.Client.SendMessageAsync(c.Message.Channel, f[1999..3999]);
//     }
//
//     [Command("LeaveGuild"), Aliases("leave"), Description("Forces the bot to leave a guild")]
//     [RequireOwner]
//     public async Task LeaveGuild(cc c, [Description("Guild ID to leave from")] string guildId = "") {
//         if (string.IsNullOrWhiteSpace(guildId)) {
//             await c.RespondAsync("Please provide a guild ID.");
//             return;
//         }
//
//         var id = ulong.Parse(guildId);
//         var guild = await c.Client.GetGuildAsync(id);
//
//         await guild.LeaveAsync();
//         await c.RespondAsync($"Left the server: {guild.Name}");
//     }
//
//     /*[Command("DeregisterSlash"), Description("Removes and deregisters a slash command")]
//     [RequireOwner]
//     public async Task DeregisterSlash(cc c, [Description("You should not use this command")] string command) {
//         var cmdList = await c.Client.GetGlobalApplicationCommandsAsync();
//         ulong cmdId;
//         try {
//             cmdId = cmdList.FirstOrDefault(c => c.Name.ToLower().Equals(command.ToLower()))!.ApplicationId;
//         }
//         catch {
//             await c.RespondAsync("Failed to resolve application from given command name");
//             return;
//         }
//
//         try {
//             await c.Client.DeleteGlobalApplicationCommandAsync(cmdId);
//         }
//         catch (Exception ex) {
//             await c.RespondAsync("Failed to remove slash command:");
//             var e = new DiscordEmbedBuilder();
//             e.WithDescription($"``` \n{ex}\n```");
//             await c.RespondAsync(e.Build());
//             return;
//         }
//
//         await c.RespondAsync($"Removed the application command: {command}");
//     }*/
//
//     [Command("GetPresence"), Aliases("gp"), Description("Gets users with the given presence")]
//     [RequireOwner] // Made by Eric van Fandenfart
//     public async Task GetPresence(cc c, [Description("Text to search presences with")] string activity = "") {
//         if (string.IsNullOrWhiteSpace(activity)) {
//             await c.RespondAsync("activity field was empty");
//             return;
//         }
//         try
//         {
//             await c.Guild.RequestMembersAsync(presences: true);
//             List<DiscordMember> users = new();
//             foreach (var member in c.Guild.Members) {
//                 if (member.Value?.Presence?.Activities == null)
//                     continue;
//
//                 foreach (var dcactivity in member.Value.Presence.Activities) {
//                     if (dcactivity?.Name == null)
//                         continue;
//                     if (dcactivity.Name.Contains(activity, StringComparison.CurrentCultureIgnoreCase)) 
//                         users.Add(member.Value);
//                 }
//             }
//             var sb = new StringBuilder();
//             for (var i = 0; i < users.Count; i++) {
//                 sb.Append($"<@{users[i].Id}>\r\n");
//                 if (i == 0 || 1 % 20 != 0) continue;
//                 await c.RespondAsync(sb.ToString());
//                 sb.Clear();
//             }
//             if (sb.Length > 0)
//                 await c.RespondAsync(sb.ToString());
//             else
//                 await c.RespondAsync($"No user had the presence of {activity}");
//         }
//         catch (Exception ex) {
//             await c.RespondAsync($"```\n{ex.Message}\n```");
//         }
//     }
//
//     [Command("BlacklistUserFromPatCommand"), Aliases("blufpc"), Description("Blacklists a user from the pat command")]
//     [RequireOwner]
//     public async Task BlacklistUserFromPatCommand(cc c, [Description("Looks for User ID")] string mentionedUser = "",
//         [Description("Boolean as text; Add(t) or remove(f) blacklist")] string value = "") {
//         if (string.IsNullOrWhiteSpace(mentionedUser) || string.IsNullOrWhiteSpace(value)) {
//             await c.RespondAsync($"Incorrect command format! Please use the command like this:\n`{BuildInfo.Config.Prefix}BlacklistUserFromPatCommand [@user] [true/false]`");
//             return;
//         }
//         
//         var getUserIdFromMention = mentionedUser.Replace("<@", "").Replace(">", "");
//
//         await using var db = new Context();
//         var checkUser = db.Users.AsQueryable()
//             .Where(u => u.UserId.Equals(getUserIdFromMention)).ToList().FirstOrDefault();
//
//         var discordUser = await c.Client.GetUserAsync(ulong.Parse(getUserIdFromMention));
//
//         if (discordUser == null) {
//             await c.RespondAsync("Discord user not found! Getting user as guild member...");
//             discordUser = await c.Guild.GetMemberAsync(ulong.Parse(getUserIdFromMention), true);
//             
//             if (discordUser == null) {
//                 await c.RespondAsync("Discord user as a guild member not found! Stopping command.");
//                 return;
//             }
//         }
//
//         var valueIsTrue = value.ToLower().Contains('t');
//
//         if (checkUser == null) {
//             var newUser = new Users {
//                 UserId = discordUser.Id,
//                 UsernameWithNumber = $"{discordUser.Username}#{discordUser.Discriminator}",
//                 PatCount = 0,
//                 CookieCount = 0,
//                 IsUserBlacklisted = valueIsTrue ? 1 : 0
//             };
//             db.Users.Add(newUser);
//             db.Users.Update(checkUser!);
//         }
//         else {
//             checkUser.IsUserBlacklisted = valueIsTrue ? 1 : 0;
//             db.Users.Update(checkUser);
//         }
//
//         if (valueIsTrue) {
//             await c.RespondAsync("User is now blacklisted from the pat command.");
//             return;
//         }
//         
//         await c.RespondAsync("User is no longer blacklisted from the pat command.");
//     }
//
//     [Command("AddGIFBlacklist"), Aliases("addblacklist", "agb"), Description("Adds a GIF URL to a blacklist not to be shown in commands")]
//     [RequireOwner]
//     public async Task AddBlacklistGif(cc c, [RemainingText, Description("URL to be blacklisted")] string url) 
//         => await BlacklistedNekosLifeGifs.AddBlacklist(c, url);
//     
//     [Command("RemoveGIFBlacklist"), Aliases("removeblacklist", "rgb"), Description("Removes a GIF URL from the blacklist to be shown in commands")]
//     [RequireOwner]
//     public async Task RemoveBlacklistGif(cc c, [RemainingText, Description("URL to be removed")] string url) 
//         => await BlacklistedNekosLifeGifs.RemoveBlacklist(c, url);
// }

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

        await c.CreateResponseAsync(overLimit ? f[..1999] : f);
        if (overLimit)
            await c.Client.SendMessageAsync(c.Channel, f[1999..3999]);
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
























