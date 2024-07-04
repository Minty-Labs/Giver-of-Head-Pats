// using HeadPats.Configuration;
// using Serilog;
//
// namespace HeadPats.Managers;
//
// public class FindActiveGuilds {
//     public static void Start() {
//         var configGuildSettings = Config.Base.GuildSettings;
//         if (configGuildSettings is null) return;
//         var needsUpdating = false;
//         foreach (var guild in configGuildSettings) {
//             try {
//                 var guildVar = Program.Instance.GetGuild(guild.GuildId);
//                 if (guildVar is not null) continue;
//                 var guildFromConfig = configGuildSettings.Single(x => x.GuildId == guild.GuildId);
//                 guildFromConfig.DailyPatChannelId = 0;
//                 guildFromConfig.DailyPats?.Clear();
//                 if (guildFromConfig.DataDeletionTime == 0)
//                     guildFromConfig.DataDeletionTime = DateTimeOffset.UtcNow.AddDays(28).ToUnixTimeSeconds();
//                 needsUpdating = true;
//                 Log.Debug("Guild {guildId} not found, skipping and removing guild from config", guild.GuildId);
//             }
//             catch { /*ignore*/}
//         }
//         if (needsUpdating) Config.Save();
//     }
// }