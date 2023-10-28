using System.Text;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HeadPats.Configuration;
using HeadPats.Handlers.CommandAttributes;
using HeadPats.Handlers.Events;
using HeadPats.Utils;

namespace HeadPats.Commands.Slash.Commission.Banger; 

public class BangerClass : ApplicationCommandModule {

    [SlashCommandGroup("Banger", "Banger commands", false), SlashBangerCommand(false)]
    public class Banger : ApplicationCommandModule {
        
        [SlashCommand("toggle", "Toggles the banger system"), SlashBangerCommand(true)]
        public async Task ToggleBanger(InteractionContext c, 
            [Option("Enabled", "Enable or disable the banger system"),
             Choice("true", "true"), Choice("false", "false")] string enabled) {
            var guild = Config.GuildSettings(c.Guild.Id);
            var newSet = enabled.AsBool();
            Config.Base.Banger.Enabled = newSet;
            Config.Save();
            await c.CreateResponseAsync($"Bangers are now **{(newSet ? "enabled" : "disabled")}**.");
        }
        
        [SlashCommand("setchannel", "Sets the channel to only bangers"), SlashBangerCommand(true)]
        public async Task SetBangerChannel(InteractionContext c, 
            [Option("Destination", "Destination Discord Channel (mention)", true)] DiscordChannel channel) {
            if (Config.Base.Banger.GuildId == 0)
                Config.Base.Banger.GuildId = c.Guild.Id;
            Config.Base.Banger.ChannelId = channel.Id;
            Config.Save();
            await c.CreateResponseAsync($"Set Banger channel to {channel.Mention}.");
        }
        
        [SlashCommand("seturlerrormessage", "Changes the error message"), SlashBangerCommand(true)]
        public async Task ChangeBangerUrlErrorMessage(InteractionContext c, 
            [Option("Message", "Admin defined error message", true)] string text) {
            var newText = string.IsNullOrWhiteSpace(text) ? "This URL is not whitelisted." : text;
            Config.Base.Banger.UrlErrorResponseMessage = newText;
            Config.Save();
            await c.CreateResponseAsync($"Set Banger URL Error Message to: {newText}");
        }
        
        [SlashCommand("setexterrormessage", "Changes the error message"), SlashBangerCommand(true)]
        public async Task ChangeBangerExtErrorMessage(InteractionContext c, 
            [Option("Message", "Admin defined error message", true)] string text) {
            var newText = string.IsNullOrWhiteSpace(text) ? "This file extension is not whitelisted." : text;
            Config.Base.Banger.FileErrorResponseMessage = newText;
            Config.Save();
            await c.CreateResponseAsync($"Set Banger File Extension Error Message to: {newText}");
        }
        
        private static bool _doesItExist(string value, IEnumerable<string> list) => list.Any(x => x.Equals(value, StringComparison.OrdinalIgnoreCase));
        
        [SlashCommand("addurl", "Adds a URL to the whitelist"), SlashBangerCommand(true)]
        public async Task AddUrl(InteractionContext c, 
            [Option("URL", "URL to whitelist", true)] string url) {
            var guild = Config.GuildSettings(c.Guild.Id);
            var configBanger = Config.Base.Banger;
            if (configBanger.WhitelistedUrls == null) configBanger.WhitelistedUrls = new();
            if (_doesItExist(url, configBanger.WhitelistedUrls)) {
                await c.CreateResponseAsync($"The URL `{url}` is already whitelisted.", true);
                return;
            }
            configBanger.WhitelistedUrls.Add(url);
            Config.Save();
            await c.CreateResponseAsync($"Added `{url}` to the whitelist.");
        }
        
        [SlashCommand("addext", "Adds file extension to whitelist"), SlashBangerCommand(true)]
        public async Task AddExt(InteractionContext c, 
            [Option("Extension", "File extension to whitelist", true)] string ext) {
            var guild = Config.GuildSettings(c.Guild.Id);
            var configBanger = Config.Base.Banger;
            if (configBanger.WhitelistedFileExtensions == null) configBanger.WhitelistedFileExtensions = new();
            if (_doesItExist(ext, configBanger.WhitelistedFileExtensions)) {
                await c.CreateResponseAsync($"The file extension `{ext}` is already whitelisted.", true);
                return;
            }
            configBanger.WhitelistedFileExtensions.Add(ext);
            Config.Save();
            await c.CreateResponseAsync($"Added `{ext}` to the whitelist.");
        }
        
        [SlashCommand("removeurl", "Removes a URL from the whitelist"), SlashBangerCommand(true)]
        public async Task RemoveUrl(InteractionContext c, 
            [Option("URL", "URL to remove from whitelist", true)] string url) {
            var guild = Config.GuildSettings(c.Guild.Id);
            var configBanger = Config.Base.Banger;
            if (configBanger.WhitelistedUrls == null) configBanger.WhitelistedUrls = new();
            if (!_doesItExist(url, configBanger.WhitelistedUrls)) {
                await c.CreateResponseAsync($"The URL `{url}` is not whitelisted.", true);
                return;
            }
            configBanger.WhitelistedUrls.Remove(url);
            Config.Save();
            await c.CreateResponseAsync($"Removed `{url}` from the whitelist.");
        }
        
        [SlashCommand("removeext", "Removes file extension from list"), SlashBangerCommand(true)]
        public async Task RemoveExt(InteractionContext c, 
            [Option("Extension", "File extension to remove from whitelist", true)] string ext) {
            var guild = Config.GuildSettings(c.Guild.Id);
            var configBanger = Config.Base.Banger;
            if (configBanger.WhitelistedFileExtensions == null) configBanger.WhitelistedFileExtensions = new();
            if (!_doesItExist(ext, configBanger.WhitelistedFileExtensions)) {
                await c.CreateResponseAsync($"The file extension `{ext}` is not whitelisted.", true);
                return;
            }
            configBanger.WhitelistedFileExtensions.Remove(ext);
            Config.Save();
            await c.CreateResponseAsync($"Removed `{ext}` from the whitelist.");
        }
        
        [SlashCommand("listeverything", "Lists all URLs and file extns"), SlashBangerCommand(false)]
        public async Task ListEverything(InteractionContext c) {
            var sb = new StringBuilder();
            sb.AppendLine("```");
            sb.AppendLine("Whitelisted URLs:");
            foreach (var url in BangerEventListener.WhitelistedUrls!) {
                sb.AppendLine($"- {url}");
            }
            sb.AppendLine();
            sb.AppendLine("Whitelisted File Extensions:");
            foreach (var ext in BangerEventListener.WhitelistedFileExtensions!) {
                sb.AppendLine($"- {ext}");
            }
            sb.AppendLine("```");
            await c.CreateResponseAsync(sb.ToString());
        }
        
        
    }
    
}