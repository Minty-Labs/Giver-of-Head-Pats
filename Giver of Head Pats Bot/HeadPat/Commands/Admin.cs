// using System.Text;
// using DSharpPlus;
// using DSharpPlus.CommandsNext;
// using DSharpPlus.CommandsNext.Attributes;
// using DSharpPlus.Entities;
// using DSharpPlus.SlashCommands;
// using HeadPats.Data;
// using HeadPats.Utils;
// using cc = DSharpPlus.CommandsNext.CommandContext;
// using ic = DSharpPlus.SlashCommands.InteractionContext;
//
// namespace HeadPats.Commands; 
//
// public class Admin : BaseCommandModule {
//     public Admin() => Logger.Loadodule("AdminCommands");
//
//     [Command("GetPresences"), Aliases("gp"), Description("Get the presences of all users in the server containing a specific world or phrase.")]
//     [RequirePermissions(Permissions.BanMembers)]
//     public async Task GetPresences(cc c, [RemainingText] string input) {
//         var guild = c.Guild;
//         var users = await guild.GetAllMembersAsync();
//         var sb = new StringBuilder();
//         foreach (var user in users) {
//             if (user == null) continue;
//             var presence = user.Presence;
//             if (presence == null) continue;
//             var activity = presence.Activity;
//             if (activity == null) continue;
//             var richPresence = activity.RichPresence;
//             if (richPresence == null) continue;
//             if (!richPresence.Application.Name.ToLower().Contains(input.ToLower())) continue;
//             
//             sb.AppendLine($"{user.Id} - {user.Username}#{user.Discriminator} - {richPresence.Application.Name}");
//         }
//
//         if (string.IsNullOrWhiteSpace(sb.ToString())) {
//             await c.RespondAsync($"No user presences found with {input}.");
//             return;
//         }
//         
//         using var ms = new MemoryStream();
//         await using var sw = new StreamWriter(ms);
//         await sw.WriteLineAsync(sb.ToString());
//         
//         await sw.FlushAsync();
//         ms.Seek(0, SeekOrigin.Begin);
//
//         var builder = new DiscordMessageBuilder();
//         builder.WithFile($"Presences_containing_{input}.txt", ms);
//         await builder.WithReply(c.Message.Id).SendAsync(c.Channel);
//     }
// }