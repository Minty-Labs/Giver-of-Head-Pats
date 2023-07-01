using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using HeadPats.Configuration;
using HeadPats.Handlers.CommandAttributes;
using HeadPats.Utils;

namespace HeadPats.Commands.Slash.Admin; 

public class IrlQuoteCmds : ApplicationCommandModule {
    
    [SlashCommandGroup("IRLQuotes", "IRL Quote commands"), Hidden]
    public class IrlQuotesCommands : ApplicationCommandModule {
        
        [SlashCommand("setchannel", "Sets the channel where IRL quotes are sent", false), CustomSlashRequirePermissions(Permissions.ManageGuild)]
        public async Task SetQuoteChannel(InteractionContext c, 
            [Option("Channel", "Channel to set as the quote channel")] DiscordChannel? channel,
            [Option("ChannelID", "Channel to set as the quote channel")] string channelId = "") {

            var doingChannelMention = true;
            var guildSettings = Config.GuildSettings(c.Guild.Id);
        
            if (channel is null && !string.IsNullOrWhiteSpace(channelId)) {
                doingChannelMention = false;
                var id = ulong.Parse(channelId);
                DiscordChannel? discordChannel = null;
                bool failed;
                try {
                    discordChannel = c.Guild.GetChannel(id);
                }
                catch {
                    failed = true;
                    if (discordChannel is null || failed) {
                        var builder = new DiscordInteractionResponseBuilder {
                            Content = "The provided ID may not be an actual channel, please double check that it is in fact a channel.\nYou may also run this command with mentioning the channel.",
                            IsEphemeral = true
                        };
                        await c.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
                        return;
                    }
                }
            }
        
            guildSettings!.IrlQuotes.ChannelId = doingChannelMention ? channel!.Id : ulong.Parse(channelId);
            Config.Save();
            var tempStr = doingChannelMention ? channel!.Mention : $"<#{channelId}>";
            await c.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Set the quotes channel to {tempStr}"));
        }
        
        [SlashCommand("toggle", "Toggles IRL quotes"), CustomSlashRequirePermissions(Permissions.ManageGuild)]
        public async Task ToggleQuotes(InteractionContext c, 
            [Option("Enabled", "Whether to enable or disable IRL quotes"),
                Choice("true", "true"),
                Choice("false", "false")
            ] string enabled) {
            var guildSettings = Config.GuildSettings(c.Guild.Id)!;
            guildSettings.IrlQuotes.Enabled = enabled.AsBool();
            guildSettings.IrlQuotes.SetEpochTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Config.Save();
            await c.CreateResponseAsync($"IRL Quotes are now {(guildSettings.IrlQuotes.Enabled ? "enabled" : "disabled")}.");
        }
        
    }
}