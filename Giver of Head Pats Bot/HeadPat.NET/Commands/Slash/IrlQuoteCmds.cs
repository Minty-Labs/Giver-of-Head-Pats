// using Discord;
// using Discord.Interactions;
// using HeadPats.Configuration;
//
// namespace HeadPats.Commands.Slash; 
//
// public class IrlQuoteCmds : InteractionModuleBase<SocketInteractionContext> {
//
//     [Group("irlquote", "IRL Quote commands"), EnabledInDm(false), RequireUserPermission(GuildPermission.ManageMessages)]
//     public class Commands : InteractionModuleBase<SocketInteractionContext> {
//
//         [SlashCommand("setchannel", "Sets the channel where IRL quotes are sent")]
//         public async Task SetQuoteChannel([Summary(description: "Channel to set as the quote channel")] ITextChannel channel) {
//             var guildSettings = Config.GuildSettings(Context.Guild.Id);
//             guildSettings!.IrlQuotes.ChannelId = channel.Id;
//             Config.Save();
//             await RespondAsync($"Set the quotes channel to <#{channel.Id}>");
//         }
//         
//         [SlashCommand("toggle", "Toggles IRL quotes")]
//         public async Task ToggleQuotes([Summary("quotetoggle", "Whether to enable or disable IRL quotes")] bool enabled) {
//             var guildSettings = Config.GuildSettings(Context.Guild.Id)!;
//             guildSettings.IrlQuotes.Enabled = enabled;
//             guildSettings.IrlQuotes.SetEpochTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
//             Config.Save();
//             await RespondAsync($"IRL Quotes are now {(guildSettings.IrlQuotes.Enabled ? "enabled" : "disabled")}.");
//         }
//     }
// }