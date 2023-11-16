// using System.Text;
// using DSharpPlus;
// using DSharpPlus.CommandsNext.Attributes;
// using DSharpPlus.Entities;
// using DSharpPlus.SlashCommands;
// using HeadPats.Configuration;
// using HeadPats.Handlers.CommandAttributes;
// using HeadPats.Utils;
//
// namespace HeadPats.Commands.Slash.Reply;
//
// public class ReplyApplication : ApplicationCommandModule {
//
//     [SlashCommandGroup("Reply", "Self-creating simple trigger-based command message outputs", false), Cooldown(10, 3600, CooldownBucketType.Guild)]
//     public class Replies : ApplicationCommandModule {
//         [SlashCommand("add", "Adds an auto response for the server", false), CustomSlashRequirePermissions(Permissions.ManageMessages)]
//         public async Task AddReply(InteractionContext c,
//             [Option("Trigger", "Word or phrase as the trigger", true)] string trigger,
//             [Option("Response", "Response from trigger (add '<br>' for new lines)", true)] string response,
//             [Choice("false", "false")] [Choice("true", "true")]
//             [Option("RequireOnlyTriggerText", "Respond ONLY if the message is equal to the trigger?")] string requireOnlyTriggerText = "false",
//             [Choice("false", "false")] [Choice("true", "true")]
//             [Option("DeleteTrigger", "Auto Remove the trigger text message?")] string deleteTrigger = "false",
//             [Option("delTrigIfOnly", "Deletes trigger message if type alone and is the only text in message")] string deleteTriggerIfIsOnlyInMessage = "false") {
//             
//             ReplyConfExtensions.AddValue(c.Guild.Id, trigger, response,
//                 requireOnlyTriggerText.AsBool(),
//                 deleteTrigger.AsBool(),
//                 deleteTriggerIfIsOnlyInMessage.AsBool());
//             
//             await c.CreateResponseAsync("Trigger saved!");
//         }
//     
//         [SlashCommand("Remove", "Removes a trigger response by the provided trigger", false), CustomSlashRequirePermissions(Permissions.ManageMessages)]
//         public async Task RemoveReply(InteractionContext c,
//             [Option("Trigger", "Enter the trigger word or phrase exactly", true)]
//             string trigger) {
//             ReplyConfExtensions.RemoveValue(c.Guild.Id, trigger);
//             if (ReplyConfExtensions.ErroredOnRemove) {
//                 await c.CreateResponseAsync($"Either the provided trigger does not exist, or an error has occured.{(Vars.IsDebug ? $"\n[Debug] Error: {ReplyConfExtensions.ErroredException}" : "")}", true);
//             }
//             else await c.CreateResponseAsync($"Removed the trigger: {trigger}");
//             ReplyConfExtensions.ErroredOnRemove = false;
//         }
//
//         [SlashCommand("List", "Lists the triggers for auto responses"), CustomSlashRequirePermissions(Permissions.ManageMessages)]
//         public async Task ListTriggers(InteractionContext c) {
//             var legend = new StringBuilder();
//             var list = Config.GuildSettings(c.Guild.Id)!.Replies!;
//             var triggers = new StringBuilder();
//             legend.AppendLine("Trigger");
//             legend.AppendLine("Response");
//             legend.AppendLine("Respond only to the trigger alone");
//             legend.AppendLine("Does trigger message auto delete");
//             legend.AppendLine("Does trigger message auto delete if it is the only text in the message");
//             legend.AppendLine("==============================================================================");
//
//             if (list != null) {
//                 foreach (var t in list) {
//                     // if (t.GuildId != c.Guild.Id) continue;
//                     var r = ReplyConfExtensions.GetResponse(t.Trigger, c.Guild.Id);
//                     var i = ReplyConfExtensions.GetInfo(t.Trigger, c.Guild.Id);
//                     var d = ReplyConfExtensions.GetsDeleted(t.Trigger, c.Guild.Id);
//                     var a = ReplyConfExtensions.GetsDeletedIfAlone(t.Trigger, c.Guild.Id);
//
//                     triggers.AppendLine(t.Trigger);
//                     triggers.AppendLine(r);
//                     triggers.AppendLine(i);
//                     triggers.AppendLine(d);
//                     triggers.AppendLine(a);
//                     triggers.AppendLine();
//                 }
//             }
//
//             var triggerStr = triggers.ToString();
//
//             if (string.IsNullOrWhiteSpace(triggerStr) || triggerStr.Equals("\n\n\n\n\n"))
//                 triggerStr = "Guild has no triggers for auto replies.";
//
//             using var ms = new MemoryStream();
//             await using var sw = new StreamWriter(ms);
//             await sw.WriteLineAsync($"Triggers for {c.Guild.Name} ({c.Guild.Id})");
//             await sw.WriteLineAsync();
//             await sw.WriteLineAsync("-=- Legend -=-");
//             await sw.WriteLineAsync(legend.ToString());
//             await sw.WriteLineAsync();
//             await sw.WriteLineAsync(triggerStr);
//
//             await sw.FlushAsync();
//             ms.Seek(0, SeekOrigin.Begin);
//         
//             var builder = new DiscordMessageBuilder();
//             builder.AddFile($"{c.Guild.Name} Responses.txt", ms);
//             // await c.CreateResponseAsync(new DiscordInteractionResponseBuilder(builder));
//             await c.CreateResponseAsync("File Sent below", true);
//             await Program.Client!.SendMessageAsync(c.Channel, builder);
//         }
//     }
// }