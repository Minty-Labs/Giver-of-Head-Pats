using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HeadPats.Configuration;
using static HeadPats.Managers.MessageTasks;

namespace HeadPats.Commands.Legacy.Owner; 

public class ConfigControl : BaseCommandModule {
    
    [Command("SetAPIKey"), Description("Set an API key to gain access to a selected API"), RequireOwner]
    public async Task ApiKey(CommandContext c, [Description("(cookie|flux|unsplash|unsplashsecret)")] string type, [RemainingText, Description("API Access Key")] string key) {
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

        switch (type.ToLower()) {
            case "cookie":
                Config.Base.Api.ApiKeys.CookieClientApiKey = key;
                await c.RespondAsync("Set cookie API key.");
                break;
            case "flux":
                Config.Base.Api.ApiKeys.FluxpointApiKey = key;
                await c.RespondAsync("Set flux API key.");
                break;
            case "unsplash":
                Config.Base.Api.ApiKeys.UnsplashAccessKey = key;
                await c.RespondAsync("Set unsplash API key.");
                break;
            case "unsplashsecret":
                Config.Base.Api.ApiKeys.UnsplashSecretKey = key;
                await c.RespondAsync("Set unsplash API secret.");
                break;
        }

        await c.Message.DeleteAsync();
        Config.Save();
    }

    [Command("SetupGuildInfo"), Description("Sets up the new config with any missing guilds"), RequireOwner]
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
    }
    
}