using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HeadPats.Managers;

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
                Vars.Config.CookieClientApiKey = key;
                await c.RespondAsync("Set cookie API key.");
                break;
            case "flux":
                Vars.Config.FluxpointApiKey = key;
                await c.RespondAsync("Set flux API key.");
                break;
            case "unsplash":
                Vars.Config.UnsplashAccessKey = key;
                await c.RespondAsync("Set unsplash API key.");
                break;
            case "unsplashsecret":
                Vars.Config.UnsplashSecretKey = key;
                await c.RespondAsync("Set unsplash API secret.");
                break;
        }

        await c.Message.DeleteAsync();
        Configuration.Save();
    }
    
}