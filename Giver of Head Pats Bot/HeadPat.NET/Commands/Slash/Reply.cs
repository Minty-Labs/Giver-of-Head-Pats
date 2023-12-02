// using System.Text;
// using Discord;
// using Discord.Interactions;
// using HeadPats.Configuration;
// using HeadPats.Utils;
//
// namespace HeadPats.Commands.Slash; 
//
// public class Reply : InteractionModuleBase<SocketInteractionContext> {
//     
//     [Group("reply", "Reply Commands"), EnabledInDm(false), RequireUserPermission(GuildPermission.BanMembers)]
//     public class Commands : InteractionModuleBase<SocketInteractionContext> {
//
//         [SlashCommand("add", "Adds a reply")]
//         public async Task AddReply(
//             [Summary("trigger", "Word or phrase as the trigger")] string trigger,
//             [Summary("response", "Response from trigger (add '<br>' for new lines)")] string response,
//             [Summary("requireonlytriggertext", "Respond ONLY if the message is equal to the trigger?")] bool requireOnlyTriggerText = false,
//             [Summary("deleteTrigger", "Auto Remove the trigger text message?")] bool deleteTrigger = false,
//             [Summary("onlydeleteinmessage", "Deletes trigger message if type alone and is the only text in message")] bool onlydeleteinmessage = false) {
//             
//             ReplyConfigExtensions.AddValue(Context.Guild.Id, trigger, response,
//                 requireOnlyTriggerText,
//                 deleteTrigger,
//                 onlydeleteinmessage);
//             
//             await RespondAsync("Trigger saved!");
//         }
//         
//         [SlashCommand("remove", "Removes a reply")]
//         public async Task RemoveReply([Summary("trigger", "Enter the trigger word or phrase exactly")] string trigger) {
//             ReplyConfigExtensions.RemoveValue(Context.Guild.Id, trigger);
//             if (ReplyConfigExtensions.ErroredOnRemove) {
//                 await RespondAsync($"Either the provided trigger does not exist, or an error has occured.{(Vars.IsDebug ? $"\n[Debug] Error: {ReplyConfigExtensions.ErroredException}" : "")}", ephemeral: true);
//             }
//             else await RespondAsync($"Removed the trigger: {trigger}");
//             ReplyConfigExtensions.ErroredOnRemove = false;
//         }
//         
//         [SlashCommand("list", "Lists the triggers for auto responses")]
//         public async Task ListTriggers() {
//             var legend = new StringBuilder();
//             var list = Config.GuildSettings(Context.Guild.Id)!.Replies!;
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
//                     var r = ReplyConfigExtensions.GetResponse(t.Trigger, Context.Guild.Id);
//                     var i = ReplyConfigExtensions.GetInfo(t.Trigger, Context.Guild.Id);
//                     var d = ReplyConfigExtensions.GetsDeleted(t.Trigger, Context.Guild.Id);
//                     var a = ReplyConfigExtensions.GetsDeletedIfAlone(t.Trigger, Context.Guild.Id);
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
//             await sw.WriteLineAsync($"Triggers for {Context.Guild.Name} ({Context.Guild.Id})");
//             await sw.WriteLineAsync();
//             await sw.WriteLineAsync("-=- Legend -=-");
//             await sw.WriteLineAsync(legend.ToString());
//             await sw.WriteLineAsync();
//             await sw.WriteLineAsync(triggerStr);
//
//             await sw.FlushAsync();
//             ms.Seek(0, SeekOrigin.Begin);
//             
//             await Context.Channel.SendFileAsync(new FileAttachment(ms, $"{Context.Guild.Name} Responses.txt", "File Sent below"));
//         }
//     }
// }