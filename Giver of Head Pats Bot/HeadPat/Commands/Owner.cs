using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Managers;
using HeadPats.Utils;
using cc = DSharpPlus.CommandsNext.CommandContext;
using ic = DSharpPlus.SlashCommands.InteractionContext;

namespace HeadPats.Commands; 

public class Owner : BaseCommandModule {
    public Owner() => Logger.Loadodule("OwnerCommands");

    private void FooterText(DiscordEmbedBuilder em, string extraText = "") {
        em.WithTimestamp(DateTime.Now);
        em.WithFooter($"{(string.IsNullOrWhiteSpace(extraText) ? "" : $"{extraText}")}");
    }

    // [Command("ForceRegisterSlashCommands")]
    // [RequireOwner]
    // public async Task RegOwner(cc c) {
    //     var s = Program.Slash;
    //     if (s == null) Logger.Log("SlashCommandsExtension is null");
    //     s?.RegisterCommands<BasicSlashCommands>();
    //     s?.RegisterCommands<SlashOwner>();
    //     await c.RespondAsync("Done");
    // }

    [Command("ChangeActivity"), Aliases("ca"), Description("Change the bot\'s Activity")]
    [RequireOwner]
    public async Task ChangeActivity(cc c, [Description("Online Status (Online, Idle, etc)")] string status,
        [Description("Activity (playing, watching, etc)")] string activityType, 
        [RemainingText, Description("Extra Text to add after the activity")] string args = "") {
        if (activityType.ToLower().Contains("stream") && !args.Contains("%http")) {
            await c.RespondAsync("Streaming needs a URL: Something like `%http...`");
            return;
        }
        
        var getStatus = status switch {
            "offline"   => UserStatus.Offline,
            "invisible" => UserStatus.Invisible,
            "dnd"       => UserStatus.DoNotDisturb,
            "idle"      => UserStatus.Idle,
            "online"    => UserStatus.Online,
            _           => UserStatus.Offline
        };

        var getActivity = activityType switch {
            "play"    => ActivityType.Playing,
            "listen"  => ActivityType.ListeningTo,
            "watch"   => ActivityType.Watching,
            "stream"  => ActivityType.Streaming,
            "compete" => ActivityType.Competing,
            "other"   => ActivityType.Custom,
            _         => ActivityType.Playing
        };
        
        var arg = args.Split('%');
        var url = !args.Contains("%http") ? "" : arg[1];
        var name = string.IsNullOrWhiteSpace(args) ? BuildInfo.Config.Game : arg[0];

        BuildInfo.Config.Game = name;
        BuildInfo.Config.ActivityType = Program.GetActivityAsString(getActivity);
        BuildInfo.Config.StreamingUrl = string.IsNullOrWhiteSpace(url) ? "" : url;
        Configuration.Save();
        
        await c.Client.UpdateStatusAsync(new DiscordActivity {
            Name = name,
            ActivityType = getActivity,
            StreamUrl = string.IsNullOrWhiteSpace(url) ? "" : url
        }, getStatus);

        var color = status switch {
            "offline"   => "747F8D",
            "invisible" => "747F8D",
            "dnd"       => "ED4245",
            "idle"      => "FAA81A",
            "online"    => "3BA55D",
            _           => "FFFFFF"
        };

        var changeToStreamColor = activityType == "stream" && status is not ("offline" or "invisible");
        var changeToCompeteColor = activityType == "compete" && status is not ("offline" or "invisible");
        if (changeToStreamColor) color = "593695";
        if (changeToCompeteColor) color = "C69164";
        
        var e = new DiscordEmbedBuilder();
        e.WithTitle("Changed Status");
        e.WithColor(Colors.HexToColor(color));
        e.WithDescription($"Game: {name}\nActivityType: {Program.GetActivityAsString(getActivity)}\n{(string.IsNullOrWhiteSpace(url) ? "" : $"Stream URL: {url}")}");
        FooterText(e);
        await c.RespondAsync(e.Build());
    }

    [Command("Guilds"), Aliases("listguilds", "lg"), Description("Lists all guilds the bot is in. [Owner]")]
    [RequireOwner]
    public async Task ListGuilds(cc c) {
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

        await c.RespondAsync(overLimit ? f[..1999] : f);
        if (overLimit)
            await c.Client.SendMessageAsync(c.Message.Channel, f[1999..3999]);
    }

    [Command("LeaveGuild"), Aliases("leave"), Description("Forces the bot to leave a guild")]
    [RequireOwner]
    public async Task LeaveGuild(cc c, [Description("Guild ID to leave from")] string guildId = "") {
        if (string.IsNullOrWhiteSpace(guildId)) {
            await c.RespondAsync("Please provide a guild ID.");
            return;
        }

        var id = ulong.Parse(guildId);
        var guild = await c.Client.GetGuildAsync(id);

        await guild.LeaveAsync();
        await c.RespondAsync($"Left the server: {guild.Name}");
    }

    [Command("DeregisterSlash"), Description("Removes and deregisters a slash command")]
    [RequireOwner]
    public async Task DeregisterSlash(cc c, [Description("You should not use this command")] string command) {
        var cmdList = await c.Client.GetGlobalApplicationCommandsAsync();
        ulong cmdId;
        try {
            cmdId = cmdList.FirstOrDefault(c => c.Name.ToLower().Equals(command.ToLower()))!.ApplicationId;
        }
        catch {
            await c.RespondAsync("Failed to resolve application from given command name");
            return;
        }

        try {
            await c.Client.DeleteGlobalApplicationCommandAsync(cmdId);
        }
        catch (Exception ex) {
            await c.RespondAsync("Failed to remove slash command:");
            var e = new DiscordEmbedBuilder();
            e.WithDescription($"``` \n{ex}\n```");
            await c.RespondAsync(e.Build());
            return;
        }

        await c.RespondAsync($"Removed the application command: {command}");
    }

    [Command("GetPresence"), Aliases("gp"), Description("Gets users with the given presence")]
    [RequireOwner] // Made by Eric van Fandenfart
    public async Task GetPresence(cc c, [Description("Text to search presences with")] string activity = "") {
        if (string.IsNullOrWhiteSpace(activity)) {
            await c.RespondAsync("activity field was empty");
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
                await c.RespondAsync(sb.ToString());
                sb.Clear();
            }
            if (sb.Length > 0)
                await c.RespondAsync(sb.ToString());
            else
                await c.RespondAsync($"No user had the presence of {activity}");
        }
        catch (Exception ex) {
            await c.RespondAsync($"```\n{ex.Message}\n```");
        }
    }

    [Command("BlacklistUserFromPatCommand"), Aliases("blufpc"), Description("Blacklists a user from the pat command")]
    [RequireOwner]
    public async Task BlacklistUserFromPatCommand(cc c, [Description("Looks for User ID")] string mentionedUser = "",
        [Description("Boolean as text; Add(t) or remove(f) blacklist")] string value = "") {
        if (string.IsNullOrWhiteSpace(mentionedUser) || string.IsNullOrWhiteSpace(value)) {
            await c.RespondAsync($"Incorrect command format! Please use the command like this:\n`{BuildInfo.Config.Prefix}BlacklistUserFromPatCommand [@user] [true/false]`");
            return;
        }
        
        var getUserIdFromMention = mentionedUser.Replace("<@", "").Replace(">", "");

        await using var db = new Context();
        var checkUser = db.Users.AsQueryable()
            .Where(u => u.UserId.Equals(getUserIdFromMention)).ToList().FirstOrDefault();

        var discordUser = await c.Client.GetUserAsync(ulong.Parse(getUserIdFromMention));

        if (discordUser == null) {
            await c.RespondAsync("Discord user not found! Getting user as guild member...");
            discordUser = await c.Guild.GetMemberAsync(ulong.Parse(getUserIdFromMention), true);
            
            if (discordUser == null) {
                await c.RespondAsync("Discord user as a guild member not found! Stopping command.");
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
            await c.RespondAsync("User is now blacklisted from the pat command.");
            return;
        }
        
        await c.RespondAsync("User is no longer blacklisted from the pat command.");
    }

    [Command("AddGIFBlacklist"), Aliases("addblacklist", "agb"), Description("Adds a GIF URL to a blacklist not to be shown in commands")]
    [RequireOwner]
    public async Task AddBlacklistGif(cc c, [RemainingText, Description("URL to be blacklisted")] string url) 
        => await BlacklistedNekosLifeGifs.AddBlacklist(c, url);
    
    [Command("RemoveGIFBlacklist"), Aliases("removeblacklist", "rgb"), Description("Removes a GIF URL from the blacklist to be shown in commands")]
    [RequireOwner]
    public async Task RemoveBlacklistGif(cc c, [RemainingText, Description("URL to be removed")] string url) 
        => await BlacklistedNekosLifeGifs.RemoveBlacklist(c, url);
}

// public class RequireUserIdAttribute : SlashCheckBaseAttribute {
//     public ulong UserId;
//
//     public RequireUserIdAttribute(ulong userId) => UserId = userId;
//
//     public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) => ctx.User.Id == UserId;
// }

// public class SlashOwner : ApplicationCommandModule {
//     public SlashOwner() => Logger.Loadodule("OwnerCommands");
//     
//     private string FooterText(string extra = "")
//         => $"{BuildInfo.Name} (v{BuildInfo.Version}) • {BuildInfo.BuildDate}{(string.IsNullOrWhiteSpace(extra) ? "" : $" • {extra}")}";
//
//     [SlashCommand("activity", "Change the bot\'s Activity")]
//     public async Task ChangeActivity(ic c,
//         [Choice("Offline", "off")]
//         [Choice("Invisible", "in")]
//         [Choice("Do not Disturb", "d")]
//         [Choice("Idle", "id")]
//         [Choice("Online", "on")]
//         [Option("UserStatus", "Change User Status")] string userStatus,
//         
//         [Choice("Playing", "play")]
//         [Choice("Listening", "listen")]
//         [Choice("Watching", "watch")]
//         [Choice("Streaming", "stream")]
//         [Choice("Competing", "compete")]
//         [Choice("OtherText", "other")]
//         [Option("ActivityType", "Change Activity Type")] string activityType,
//         
//         [Option("ExtraText", "text")] string? args = "") {
//         if (c.Member.Id != 167335587488071682) {
//             await c.CreateResponseAsync("You cannot run this command.");
//             return;
//         }
//         
//         if (string.IsNullOrWhiteSpace(args)) {
//             await c.CreateResponseAsync("Please select an argument.");
//             return;
//         }
//
//         var getStatus = userStatus switch {
//             "off" => UserStatus.Offline,
//             "in"  => UserStatus.Invisible,
//             "d"   => UserStatus.DoNotDisturb,
//             "id"  => UserStatus.Idle,
//             "on"  => UserStatus.Online,
//             _     => UserStatus.Offline
//         };
//
//         var getActivity = activityType switch {
//             "play" => ActivityType.Playing,
//             "listen" => ActivityType.ListeningTo,
//             "watch" => ActivityType.Watching,
//             "stream" => ActivityType.Streaming,
//             "compete" => ActivityType.Competing,
//             "other" => ActivityType.Custom,
//             _ => ActivityType.Playing
//         };
//
//         var arg = args.Split('%');
//         var url = string.IsNullOrWhiteSpace(args) ? "https://twitch.tv/MintyLily" : arg[1];
//         var name = string.IsNullOrWhiteSpace(args) ? BuildInfo.Config.Game : arg[0];
//         
//         await c.Client!.UpdateStatusAsync(new DiscordActivity {
//             Name = name,
//             ActivityType = getActivity,
//             StreamUrl = url
//         }, getStatus);
//
//         BuildInfo.Config.Game = name;
//         BuildInfo.Config.ActivityType = Program.GetActivityAsString(getActivity);
//         BuildInfo.Config.StreamingUrl = url;
//         Configuration.Save();
//
//         var color = userStatus switch {
//             "off" => "747F8D",
//             "in"  => "747F8D",
//             "d"   => "ED4245",
//             "id"  => "FAA81A",
//             "on"  => "3BA55D",
//             _ => "FFFFFF"
//         };
//
//         var changeToStreamColor = activityType == "stream" && userStatus is not ("off" or "in");
//         
//         var e = new DiscordEmbedBuilder();
//         e.WithTitle("Changed Status");
//         e.WithColor(Colors.HexToColor(changeToStreamColor ? "593695" : color));
//         e.WithDescription($"Game: {name}\nActivityType: {Program.GetActivityAsString(getActivity)}\n{(string.IsNullOrWhiteSpace(url) ? "" : $"Stream URL: {url}")}");
//         e.WithFooter(FooterText());
//         await c.CreateResponseAsync(e.Build());
//     }
// }