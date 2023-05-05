using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HeadPats.Configuration;
using HeadPats.Handlers;

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
            await c.RespondAsync("Please provide an action to perform. `(list|add|remove)`");
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
            case "remove": {
                var item = replacements!.Single(r => r.UserId == userIdLong);
                replacements!.Remove(item);
                await c.RespondAsync($"Removed ({item.UserId}) **{item.BeforeName}** -> **{item.Replacement}**");
                break;
            }
        }
        
        Config.Save();
    }
    
}