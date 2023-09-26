using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HeadPats.Configuration;
using HeadPats.Handlers;
using HeadPats.Configuration.Classes;
using HeadPats.Data;

namespace HeadPats.Commands.Legacy.Owner; 

public class ConfigControl : BaseCommandModule {
    
    [Command("SetAPIKey"), Description("Set an API key to gain access to a selected API"), RequireOwner]
    public async Task ApiKey(CommandContext c, [Description("(cookie|flux|unsplash|unsplashsecret)")] string type = "", [RemainingText, Description("API Access Key")] string key = "") {
        // SetApiKey (cookie|flux|unsplash|unsplashsecret) (key)
        if (string.IsNullOrWhiteSpace(type)) {
            await c.RespondAsync("Please provide a type of API key to set.");
            return;
        }
        
        if (type.ToLower() is "cookie" or "flux" or "unsplash" or "unsplashsecret") {
            await c.RespondAsync("Please provide a valid type of API key to set.");
            return;
        }

        if (string.IsNullOrWhiteSpace(key)) {
            await c.RespondAsync("Please provide a key to set.");
            return;
        }
        
        await c.Message.DeleteAsync();

        switch (type.ToLower()) {
            case "cookie":
                Config.Base.Api.ApiKeys.CookieClientApiKey = key;
                await c.Client.SendMessageAsync(c.Channel, "Set cookie API key.");
                break;
            case "flux":
                Config.Base.Api.ApiKeys.FluxpointApiKey = key;
                await c.Client.SendMessageAsync(c.Channel, "Set flux API key.");
                break;
            case "unsplash":
                Config.Base.Api.ApiKeys.UnsplashAccessKey = key;
                await c.Client.SendMessageAsync(c.Channel, "Set unsplash API key.");
                break;
            case "unsplashsecret":
                Config.Base.Api.ApiKeys.UnsplashSecretKey = key;
                await c.Client.SendMessageAsync(c.Channel, "Set unsplash API secret.");
                break;
        }

        Config.Save();
    }

    // Not needed anymore, but I'll keep it here for reference
    /*[Command("SetupGuildInfo"), Description("Sets up the new config with any missing guilds"), RequireOwner]
    public async Task SetupGuilds(CommandContext c) {
        var guilds = c.Client.Guilds.Values.ToList();
        var count = 0;
        foreach (var guild in guilds) {
            if (Config.Base.GuildSettings!.FirstOrDefault(g => g.GuildId == guild.Id)!.GuildId == guild.Id) continue;
            var guildConfigItem = new GuildParams {
                GuildName = guild.Name,
                GuildId = guild.Id,
                BlacklistedCommands = new List<string>(),
                Replies = new List<Reply>()
            };
            Config.Base.GuildSettings!.Add(guildConfigItem);
            Config.Save();
            count++;
            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        await c.RespondAsync($"Added {count} guilds to the config.");
    }*/

    [Command("NameReplacement"), Description("Replaces a user's name with a custom name"), RequireAnyOwner]
    public async Task NameReplacements(CommandContext c, [Description("(list|add|remove)")] string action = "", 
        [Description("The User ID of the user to replace the name of")] string userId = "",
        [Description("The New Name to replace the user's current User or Display Name"), RemainingText] string replacementName = "") {
        if (string.IsNullOrWhiteSpace(action)) {
            await c.RespondAsync("Please provide an action to perform. `(list|add|update|remove)`");
            return;
        }

        var replacements = Config.Base.NameReplacements;
        
        if (action.ToLower().Equals("list")) {
            var sb = new StringBuilder();
            if (replacements != null) {
                sb.AppendLine($"Count: {replacements.Count}");
                foreach (var replacement in replacements) 
                    sb.AppendLine($"({replacement.UserId}) **{replacement.BeforeName}** -> **{replacement.Replacement}**");
            }

            await c.RespondAsync(sb.ToString());
            return;
        }

        if (!action.ToLower().Equals("list")) {
            if (string.IsNullOrWhiteSpace(userId)) {
                await c.RespondAsync("Please provide the user ID of the user.");
                return;
            }
            if (string.IsNullOrWhiteSpace(replacementName)) {
                await c.RespondAsync("Please provide a replacement name.");
                return;
            }
        }
        
        var userIdLong = ulong.Parse(userId);
        var user = await c.Client.GetUserAsync(userIdLong);

        switch (action.ToLower()) {
            case "add": {
                var replacement = new NameReplacement {
                    UserId = userIdLong,
                    BeforeName = user.Username,
                    Replacement = replacementName
                };
                replacements!.Add(replacement);
                await c.RespondAsync($"Added ({replacement.UserId}) **{replacement.BeforeName}** -> **{replacement.Replacement}**");
                break;
            }
            case "update": {
                var item = replacements!.Single(r => r.UserId == userIdLong);
                var tempName = item.Replacement;
                item.Replacement = replacementName;
                await c.RespondAsync($"Updated ({item.UserId}) **{tempName}** -> **{item.Replacement}**");
                break;
            }
            case "remove": {
                var item = replacements!.Single(r => r.UserId == userIdLong);
                replacements!.Remove(item);
                await c.RespondAsync($"Removed ({item.UserId}) **{item.BeforeName}** -> **{item.Replacement}**");
                break;
            }
        }
        
        Config.Save();
    }
    
    [Command("rotatingstatus"), Description("Controls the rotating status"), RequireOwner]
    public async Task RotatingStatus(CommandContext c, [Description("(enable|disable|list|next)")] string preAction = "", [Description("(ActivityType UserStatus ActivityText)"), RemainingText] string action = "") {
        if (string.IsNullOrWhiteSpace(preAction)) {
            await c.RespondAsync("Please provide an action to perform. `(enable|disable|list|skip|next)`");
            return;
        }

        switch (preAction.ToLower()) {
            case "enable":
                Config.Base.RotatingStatus.Enabled = true;
                await c.RespondAsync("Enabled rotating status.");
                Config.Save();
                return;
            case "disable":
                Config.Base.RotatingStatus.Enabled = false;
                await c.RespondAsync("Disabled rotating status.");
                Config.Save();
                return;
            case "list":
                var sb = new StringBuilder();
                sb.AppendLine(string.Join("\n", Config.Base.RotatingStatus.Statuses.Select((x, i) => $"[{i} - {x.ActivityType} - {x.UserStatus}] {x.ActivityText}")));
                await c.RespondAsync(sb.ToString());
                return;
            case "bopit":
            case "skip":
            case "next": {
                await using var db = new Context();
                await Managers.Loops.RotatingStatus.Update(db);
                await c.RespondAsync("Skipped to next status.");
                return;
            }
        }
        
        if (string.IsNullOrWhiteSpace(action)) {
            await c.RespondAsync(
                "Please provide an action to perform. `(add|remove|modify)`\n" +
                "Add: `rotatingstatus add (ActivityType|ActivityText|UserStatus)`\n" +
                "Remove: `rotatingstatus remove (StatusID)`\n" +
                "Modify: `rotatingstatus modify (StatusID) (ActivityType|ActivityText|UserStatus)`");
            return;
        }
        
        var actionSplit = action.Split(" ");
        var actionType = actionSplit[0];
        var actionValue = actionSplit[1];
        
        switch (actionType.ToLower()) {
            case "add": {
                var actionValueSplit = actionValue.Split("`");
                var activityType = actionValueSplit[0];
                var userStatus = actionValueSplit[1];
                var activityText = actionValueSplit[2];
                var status = new Status {
                    Id = Config.Base.RotatingStatus.Statuses.Count + 1,
                    ActivityText = activityText,
                    ActivityType = activityType,
                    UserStatus = userStatus
                };
                Config.Base.RotatingStatus.Statuses.Add(status);
                await c.RespondAsync($"Added [{status.Id} - {status.ActivityType} - {status.UserStatus}] {status.ActivityText}");
                break;
            }
            case "remove": {
                var status = Config.Base.RotatingStatus.Statuses.Single(s => s.Id == int.Parse(actionValue));
                await c.RespondAsync($"Removed [{status.Id} - {status.ActivityType} - {status.UserStatus}] {status.ActivityText}");
                Config.Base.RotatingStatus.Statuses.Remove(status);
                break;
            }
            case "modify": {
                var modifyActionSplit = actionValue.Split(" ");
                var id = modifyActionSplit[0];
                var modifyActionNewValues = modifyActionSplit[1];
                var modifyActionNewValuesSplit = modifyActionNewValues.Split("|");
                var activityType = modifyActionNewValuesSplit[0];
                var userStatus = modifyActionNewValuesSplit[1];
                var activityText = modifyActionNewValuesSplit[2];
                
                var status = Config.Base.RotatingStatus.Statuses.Single(s => s.Id == int.Parse(id));
                var tempActivityText = status.ActivityText;
                var tempActivityType = status.ActivityType;
                var tempUserStatus = status.UserStatus;
                status.ActivityText = activityText;
                status.ActivityType = activityType;
                status.UserStatus = userStatus;
                await c.RespondAsync(
                    $"Old\n" +
                    $"[{status.Id} - {tempActivityType} - {tempUserStatus}] {tempActivityText}\n" +
                    $"New:\n" +
                    $"[{status.Id} - {status.ActivityType} - {status.UserStatus}] {status.ActivityText}");
                break;
            }
        }
        
        Config.Save();
    }
    
}