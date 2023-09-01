using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HeadPats.Configuration;
using HeadPats.Handlers.CommandAttributes;
using HeadPats.Managers;
using HeadPats.Configuration.Classes;

namespace HeadPats.Commands.Slash.Admin; 

public class DailyPatCmds : ApplicationCommandModule {
    
    [SlashCommandGroup("DailyPat", "Daily pat commands"), Hidden]
    public class DailyPats : ApplicationCommandModule {
        
        [SlashCommand("setpatchannel", "Sets the channel where daily pats are sent", false), CustomSlashRequirePermissions(Permissions.ManageGuild)]
        public async Task SetDailyPatChannel(InteractionContext c, 
            [Option("Channel", "Channel to set as the daily pat channel")] DiscordChannel? channel,
            [Option("ChannelID", "Channel to set as the daily pat channel")] string channelId = "") {

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
        
            guildSettings!.DailyPatChannelId = doingChannelMention ? channel!.Id : ulong.Parse(channelId);
            Config.Save();
            var tempStr = doingChannelMention ? channel!.Mention : $"<#{channelId}>";
            await c.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Set the daily pat channel to {tempStr}"));
        }
        
        private static bool _doesItExist(SnowflakeObject user, ulong guildId) => Config.GuildSettings(guildId)!.DailyPats!.Any(u => u.UserId == user.Id); 
    
        [SlashCommand("add", "Sets the daily pat to user"), CustomSlashRequirePermissions(Permissions.ManageGuild)]
        public async Task SetDailyPat(InteractionContext c, 
            [Option("Member", "The mentioned member to receive daily pats")] SnowflakeObject user,
            [Option("ManualEpochTime", "Manually Set an Epoch time; if empty, defaults to NOW")] long manualSetEpochTime = 0) {
        
            if (_doesItExist(user, c.Guild.Id)) {
                await c.CreateResponseAsync("User already has a daily pat set.", ephemeral: true);
                return;
            }

            var member = (DiscordMember)user;
        
            var dailyPat = new DailyPat {
                UserId = user.Id,
                UserName = member.Username,
                SetEpochTime = manualSetEpochTime == 0 ? DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 86400 : manualSetEpochTime + 86400
            };
            var guildSettings = Config.GuildSettings(c.Guild.Id);
        
            guildSettings!.DailyPats!.Add(dailyPat);
            Config.Save();
            await c.CreateResponseAsync($"Set daily pat to {member.Username}.");
        }
    
        [SlashCommand("remove", "Removes the daily pat from user"), CustomSlashRequirePermissions(Permissions.ManageGuild)]
        public async Task RemoveDailyPat(InteractionContext c, [Option("Member", "The mentioned member to remove")] SnowflakeObject user) {
            if (!_doesItExist(user, c.Guild.Id)) {
                await c.CreateResponseAsync("User does not have a daily pat set.", ephemeral: true);
                return;
            }
            var guildSettings = Config.GuildSettings(c.Guild.Id);
            var member = (DiscordMember)user;
        
            var dailyPat = guildSettings!.DailyPats!.Single(u => u.UserId == user.Id);
            guildSettings!.DailyPats!.Remove(dailyPat);
            Config.Save();
            await c.CreateResponseAsync($"Removed daily pat from {member.Username}.");
        }
    
        [SlashCommand("list", "Lists all daily pats"), CustomSlashRequirePermissions(Permissions.ManageGuild)]
        public async Task ListDailyPats(InteractionContext c) {
            var sb = new StringBuilder();
            var guildSettings = Config.GuildSettings(c.Guild.Id);
            foreach (var dailyPat in guildSettings!.DailyPats!) {
                sb.AppendLine($"{dailyPat.UserName.ReplaceName(dailyPat.UserId)} ({dailyPat.UserId}) - {dailyPat.SetEpochTime}");
            }
            await c.CreateResponseAsync(sb.ToString());
        }
    }
}