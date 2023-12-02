using System.Text;
using Discord;
using Discord.Interactions;
using HeadPats.Commands.Preexecution;
using HeadPats.Configuration;

namespace HeadPats.Commands.Slash.Commission; 

public class Banger : InteractionModuleBase<SocketInteractionContext> {

    [Group("banger", "Banger Commands"), EnabledInDm(false), RequireUser(875251523641294869, 167335587488071682)]
    public class Commands : InteractionModuleBase<SocketInteractionContext> {
        
        [SlashCommand("toggle", "Toggles the banger system")]
        public async Task ToggleBangerSystem([Summary("toggle", "Enable or disable the banger system")] bool enabled) {
            Config.Base.Banger!.Enabled = enabled;
            Config.Save();
            await RespondAsync($"Bangers are now {(enabled ? "enabled" : "disabled")}.");
        }
        
        [SlashCommand("setchannel", "Sets the channel to only bangers")]
        public async Task SetBangerChannel([Summary("channel", "Destination Discord Channel")] ITextChannel channel) {
            if (Config.Base.Banger!.GuildId == 0)
                Config.Base.Banger.GuildId = Context.Guild.Id;
            Config.Base.Banger.ChannelId = channel.Id;
            Config.Save();
            await RespondAsync($"Set Banger channel to {channel.Mention}.");
        }
        
        [SlashCommand("seturlerrormessage", "Changes the error message")]
        public async Task ChangeBangerUrlErrorMessage([Summary("message", "Admin defined error message")] string text) {
            var newText = string.IsNullOrWhiteSpace(text) ? "This URL is not whitelisted." : text;
            Config.Base.Banger!.UrlErrorResponseMessage = newText;
            Config.Save();
            await RespondAsync($"Set Banger URL Error Message to: {newText}");
        }
        
        [SlashCommand("setexterrormessage", "Changes the error message")]
        public async Task ChangeBangerExtErrorMessage([Summary("message", "Admin defined error message")] string text) {
            var newText = string.IsNullOrWhiteSpace(text) ? "This file extension is not whitelisted." : text;
            Config.Base.Banger!.FileErrorResponseMessage = newText;
            Config.Save();
            await RespondAsync($"Set Banger File Extension Error Message to: {newText}");
        }
        
        private static bool _doesItExist(string value, IEnumerable<string> list) => list.Any(x => x.Equals(value, StringComparison.OrdinalIgnoreCase));
        
        [SlashCommand("addurl", "Adds a URL to the whitelist")]
        public async Task AddUrl([Summary("url", "URL to whitelist")] string url) {
            var configBanger = Config.Base.Banger!;
            configBanger.WhitelistedUrls ??= new();
            if (_doesItExist(url, configBanger.WhitelistedUrls)) {
                await RespondAsync("URL already exists in the whitelist.", ephemeral: true);
                return;
            }
            configBanger.WhitelistedUrls.Add(url);
            Config.Save();
            await RespondAsync($"Added {url} to the whitelist.");
        }
        
        [SlashCommand("removeurl", "Removes a URL from the whitelist")]
        public async Task RemoveUrl([Summary("url", "URL to remove from the whitelist")] string url) {
            var configBanger = Config.Base.Banger!;
            configBanger.WhitelistedUrls ??= new();
            if (!_doesItExist(url, configBanger.WhitelistedUrls)) {
                await RespondAsync("URL does not exist in the whitelist.", ephemeral: true);
                return;
            }
            configBanger.WhitelistedUrls.Remove(url);
            Config.Save();
            await RespondAsync($"Removed {url} from the whitelist.");
        }
        
        [SlashCommand("addext", "Adds a file extension to the whitelist")]
        public async Task AddExt([Summary("ext", "File extension to whitelist")] string ext) {
            var configBanger = Config.Base.Banger!;
            configBanger.WhitelistedFileExtensions ??= new();
            if (ext.StartsWith('.'))
                ext = ext[1..];
            if (_doesItExist(ext, configBanger.WhitelistedFileExtensions)) {
                await RespondAsync("File extension already exists in the whitelist.", ephemeral: true);
                return;
            }
            configBanger.WhitelistedFileExtensions.Add(ext);
            Config.Save();
            await RespondAsync($"Added {ext} to the whitelist.");
        }
        
        [SlashCommand("removeext", "Removes a file extension from the whitelist")]
        public async Task RemoveExt([Summary("ext", "File extension to remove from the whitelist")] string ext) {
            var configBanger = Config.Base.Banger!;
            configBanger.WhitelistedFileExtensions ??= new();
            if (ext.StartsWith('.'))
                ext = ext[1..];
            if (!_doesItExist(ext, configBanger.WhitelistedFileExtensions)) {
                await RespondAsync("File extension does not exist in the whitelist.", ephemeral: true);
                return;
            }
            configBanger.WhitelistedFileExtensions.Remove(ext);
            Config.Save();
            await RespondAsync($"Removed {ext} from the whitelist.");
        }
        
        [SlashCommand("listeverything", "Lists all URLs and file extns")]
        public async Task ListUrls() {
            var sb = new StringBuilder();
            sb.AppendLine("```");
            sb.AppendLine("Whitelisted URLs:");
            // foreach (var url in BangerEventListener.WhitelistedUrls!) {
            //     sb.AppendLine($"- {url}");
            // }
            sb.AppendLine();
            sb.AppendLine("Whitelisted File Extensions:");
            // foreach (var ext in BangerEventListener.WhitelistedFileExtensions!) {
            //     sb.AppendLine($"- {ext}");
            // }
            sb.AppendLine("```");
            await RespondAsync(sb.ToString());
        }
    }
}