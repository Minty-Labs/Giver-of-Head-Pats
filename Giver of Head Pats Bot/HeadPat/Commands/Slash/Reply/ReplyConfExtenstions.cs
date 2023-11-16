// using HeadPats.Configuration;
// using HeadPats.Managers;
// using Serilog;
// using HeadPats.Configuration.Classes;
//
// namespace HeadPats.Commands.Slash.Reply; 
//
// public class ReplyConfExtensions {
//     private static bool DoesTriggerExist(string? trigger, ulong guildId) => Config.GuildSettings(guildId)!.Replies!.FirstOrDefault(x => x.Trigger == trigger)?.Trigger == trigger;
//     
//     public static void AddValue(ulong guildId, string trigger, string response,
//         bool requireOnlyTriggerText = false,
//         bool deleteTrigger = false,
//         bool deleteTriggerIfIsOnlyInMessage = false) {
//         
//         if (DoesTriggerExist(trigger, guildId)) {
//             Log.Debug("Removing duplicate trigger");
//             var itemToRemove = Config.GuildSettings(guildId)!.Replies!.Single(t => string.Equals(t.Trigger, trigger, StringComparison.CurrentCultureIgnoreCase));
//             if (itemToRemove != null) Config.GuildSettings(guildId)!.Replies!.Remove(itemToRemove);
//         }
//
//         var item = new HeadPats.Configuration.Classes.Reply {
//             Trigger = trigger,
//             Response = response,
//             OnlyTrigger = requireOnlyTriggerText,
//             DeleteTrigger = deleteTrigger,
//             DeleteTriggerIfIsOnlyInMessage = deleteTriggerIfIsOnlyInMessage
//         };
//         Config.GuildSettings(guildId)!.Replies!.Add(item);
//         Config.Save();
//     }
//     
//     public static bool ErroredOnRemove { get; set; }
//     public static Exception? ErroredException { get; set; }
//     
//     public static void RemoveValue(ulong guildId, string trigger) {
//         if (!DoesTriggerExist(trigger, guildId)) return;
//         try {
//             var reply = Config.GuildSettings(guildId)!.Replies!.Single(x => x.Trigger == trigger);
//
//             if (reply != null)
//                 Config.GuildSettings(guildId)!.Replies!.Remove(reply);
//         }
//         catch (Exception e) {
//             ErroredOnRemove = true;
//             ErroredException = e;
//             DSharpToConsole.SendErrorToLoggingChannel(e);
//         }
//         Config.Save();
//     }
//     
//     public static string GetResponse(string? trigger, ulong guildId) => Config.GuildSettings(guildId)!.Replies!.FirstOrDefault(x => x.Trigger == trigger)?.Response ?? "{{NULL}}";
//
//     public static string GetInfo(string? trigger, ulong guildId) => Config.GuildSettings(guildId)!.Replies!.FirstOrDefault(x => x.Trigger == trigger)?.OnlyTrigger.ToString() ?? "{{NULL}}";
//     
//     public static string GetsDeleted(string? trigger, ulong guildId) => Config.GuildSettings(guildId)!.Replies!.FirstOrDefault(x => x.Trigger == trigger)?.DeleteTrigger.ToString() ?? "{{NULL}}";
//     
//     public static string GetsDeletedIfAlone(string? trigger, ulong guildId) => Config.GuildSettings(guildId)!.Replies!.FirstOrDefault(x => x.Trigger == trigger)?.DeleteTriggerIfIsOnlyInMessage.ToString() ?? "{{NULL}}";
// }