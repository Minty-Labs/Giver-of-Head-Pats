using System.Text;
using Discord;
using Discord.Interactions;
using HeadPats.Commands.Preexecution;
using HeadPats.Configuration;
using HeadPats.Configuration.Classes;
using HeadPats.Data;
using HeadPats.Utils;

namespace HeadPats.Commands.Slash.Owner;

public class ConfigControl : InteractionModuleBase<SocketInteractionContext> {
    [Group("config", "Owner only commands - Controls the config"), RequireUser(167335587488071682)]
    public class Commands : InteractionModuleBase<SocketInteractionContext> {
        public enum ApiSet {
            [ChoiceDisplay("Unsplash Access Key")] UnsplashAccessKey = 1,
            [ChoiceDisplay("Unsplash Secret Key")] UnsplashSecretKey = 2,
            [ChoiceDisplay("CookieAPI Key")] CookieClientApiKey = 3,
            [ChoiceDisplay("FluxpointAPI Key")] FluxpointApiKey = 4,
            [ChoiceDisplay("Patreon Client ID")] PatreonClientId = 5,
            [ChoiceDisplay("Patreon Access Token")] PatreonAccessToken = 6,
            [ChoiceDisplay("Patreon Refresh Token")] PatreonRefreshToken = 7
        }

        public enum NameReplacementAction {
            [ChoiceDisplay("Add")] Add = 1,
            [ChoiceDisplay("Update")] Update = 2,
            [ChoiceDisplay("Remove")] Remove = 3,
            [ChoiceDisplay("List")] List = 4
        }

        public enum RotatingStatusPreAction {
            [ChoiceDisplay("Enable")] Enable = 1,
            [ChoiceDisplay("Disable")] Disable = 2,
            [ChoiceDisplay("List")] List = 3,
            [ChoiceDisplay("Next")] Next = 4
        }

        public enum RotatingStatusAction {
            [ChoiceDisplay("Add")] Add = 1,
            [ChoiceDisplay("Update")] Update = 2,
            [ChoiceDisplay("Remove")] Remove = 3
        }
        
        // [SlashCommand("setapikey", "Sets an API key for the bot")]
        // public async Task SetApiKey() {
        //     var modal = new ModalBuilder {
        //         Title = "API Key",
        //         CustomId = "setapikey",
        //     }
        //     .AddComponents( );
        // }

        [SlashCommand("setapikey", "Sets an API key for the bot")]
        public async Task SetApiKey(ApiSet apiSet, [Summary(description: "The API Key")] string key) {
            switch (apiSet) {
                case ApiSet.UnsplashAccessKey:
                    Config.Base.Api.ApiKeys.UnsplashAccessKey = key;
                    break;
                case ApiSet.UnsplashSecretKey:
                    Config.Base.Api.ApiKeys.UnsplashSecretKey = key;
                    break;
                case ApiSet.CookieClientApiKey:
                    Config.Base.Api.ApiKeys.CookieClientApiKey = key;
                    break;
                case ApiSet.FluxpointApiKey:
                    Config.Base.Api.ApiKeys.FluxpointApiKey = key;
                    break;
                case ApiSet.PatreonClientId:
                    Config.Base.Api.PatreonClientData.PatreonClientId = key;
                    break;
                case ApiSet.PatreonAccessToken:
                    Config.Base.Api.PatreonClientData.PatreonAccessToken = key;
                    break;
                case ApiSet.PatreonRefreshToken:
                    Config.Base.Api.PatreonClientData.PatreonRefreshToken = key;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(apiSet), apiSet, null);
            }

            Config.Save();
            await RespondAsync($"Set {apiSet} to {key}", ephemeral: true);
        }

        [SlashCommand("namereplacement", "Adds, updates, removes, or lists name replacements")]
        public async Task NameReplacement(NameReplacementAction action, [Summary(description: "User's ID")] string userId, [Summary("From Guild", "Get user from guild")] string guildId, [Summary("Replacement Name", "The new replacement name")] string name) {
            var replacements = Config.Base.NameReplacements;
            var guildIdUlong = ulong.Parse(guildId);
            var userIdUlong = ulong.Parse(userId);
            var guild = Context.Client.GetGuild(guildIdUlong);
            await guild.DownloadUsersAsync();
            var user = Program.Instance.GetGuildUser(guildIdUlong, userIdUlong);
            if (user is null) {
                await RespondAsync("User not found in guild.", ephemeral: true);
                return;
            }
            switch (action) {
                case NameReplacementAction.Add:
                    var addedReplacement = new NameReplacement {
                        UserId = userIdUlong,
                        BeforeName = user.Username,
                        Replacement = name
                    };
                    replacements!.Add(addedReplacement);
                    await RespondAsync($"Added ({addedReplacement.UserId}) {MarkdownUtils.ToBold(addedReplacement.BeforeName)} -> {MarkdownUtils.ToBold(addedReplacement.Replacement)}");
                    break;
                case NameReplacementAction.Update:
                    var item = replacements!.Single(r => r.UserId == userIdUlong);
                    var tempName = item.Replacement;
                    item.Replacement = name;
                    await RespondAsync($"Updated ({item.UserId}) {MarkdownUtils.ToBold(tempName!)} -> {MarkdownUtils.ToBold(item.Replacement)}");
                    break;
                case NameReplacementAction.Remove:
                    var removedItem = replacements!.Single(r => r.UserId == userIdUlong);
                    replacements!.Remove(removedItem);
                    await RespondAsync($"Removed ({removedItem.UserId}) {MarkdownUtils.ToBold(removedItem.BeforeName!)} -> {MarkdownUtils.ToBold(removedItem.Replacement!)}");
                    break;
                case NameReplacementAction.List:
                    var sb = new StringBuilder();
                    if (replacements != null) {
                        sb.AppendLine($"Count: {replacements.Count}");
                        foreach (var replacement in replacements)
                            sb.AppendLine($"({replacement.UserId}) {MarkdownUtils.ToBold(replacement.BeforeName!)} -> {MarkdownUtils.ToBold(replacement.Replacement!)}");
                    }

                    await RespondAsync(sb.ToString());
                    return;
                default: throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }

            Config.Save();
        }

        [SlashCommand("rotatingstatus", "Enables, disables, lists, or goes to the next rotating status")]
        public async Task RotatingStatus(RotatingStatusPreAction preAction) {
            switch (preAction) {
                case RotatingStatusPreAction.Enable:
                    Config.Base.RotatingStatus.Enabled = true;
                    await RespondAsync("Enabled Rotating Status", ephemeral: true);
                    break;
                case RotatingStatusPreAction.Disable:
                    Config.Base.RotatingStatus.Enabled = false;
                    await RespondAsync("Disabled Rotating Status", ephemeral: true);
                    break;
                case RotatingStatusPreAction.List:
                    var sb = new StringBuilder();
                    sb.AppendLine(string.Join("\n", Config.Base.RotatingStatus.Statuses.Select((x, i) => $"[{i} - {x.ActivityType} - {x.UserStatus}] {x.ActivityText}")));
                    await RespondAsync(sb.ToString());
                    return;
                case RotatingStatusPreAction.Next: {
                    await using var db = new Context();
                    await Managers.Loops.RotatingStatus.Update(db);
                    await RespondAsync("Skipped to next status.");
                    return;
                }
                default: throw new ArgumentOutOfRangeException(nameof(preAction), preAction, null);
            }

            Config.Save();
        }

        [SlashCommand("modifyrotatingstatus", "Adds, updates, or removes a rotating status")]
        public async Task ModifyRotatingStatus(RotatingStatusAction action,
            [Summary(description: "ex. Playing, Watching, Custom, ...")]
            string activityType = "$XX",
            [Summary(description: "ex. Online, Idle, ...")]
            string userStatus = "$XX",
            [Summary(description: "Actual Status Text")]
            string activityText = "$XX",
            [Summary(description: "Status ID")] string statusId = "$XX") {
            switch (action) {
                case RotatingStatusAction.Add:
                    var status = new Status {
                        Id = Config.Base.RotatingStatus.Statuses.Count + 1,
                        ActivityText = activityText,
                        ActivityType = activityType,
                        UserStatus = userStatus
                    };
                    Config.Base.RotatingStatus.Statuses.Add(status);
                    await RespondAsync($"Added [{status.Id} - {status.ActivityType} - {status.UserStatus}] {status.ActivityText}");
                    break;
                case RotatingStatusAction.Update:
                    var statusUpdate = Config.Base.RotatingStatus.Statuses.Single(s => s.Id == int.Parse(statusId));
                    var tempActivityText = statusUpdate.ActivityText;
                    var tempActivityType = statusUpdate.ActivityType;
                    var tempUserStatus = statusUpdate.UserStatus;
                    statusUpdate.ActivityText = activityText;
                    statusUpdate.ActivityType = activityType;
                    statusUpdate.UserStatus = userStatus;
                    await RespondAsync(
                        $"Old\n" +
                        $"[{statusUpdate.Id} - {tempActivityType} - {tempUserStatus}] {tempActivityText}\n" +
                        $"New:\n" +
                        $"[{statusUpdate.Id} - {statusUpdate.ActivityType} - {statusUpdate.UserStatus}] {statusUpdate.ActivityText}");
                    break;
                case RotatingStatusAction.Remove:
                    var statusRemoval = Config.Base.RotatingStatus.Statuses.Single(s => s.Id == int.Parse(statusId));
                    await RespondAsync($"Removed [{statusRemoval.Id} - {statusRemoval.ActivityType} - {statusRemoval.UserStatus}] {statusRemoval.ActivityText}");
                    Config.Base.RotatingStatus.Statuses.Remove(statusRemoval);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        
            Config.Save();
        }
        
        [SlashCommand("cleanguildsfromconfig", "Cleans the guilds from the config that are not in the guilds list")]
        public async Task CleanGuildsFromConfig() {
            var guilds = Context.Client.Guilds;
            var configGuildSettings = Config.Base.GuildSettings!;
            var sb = new StringBuilder();
            sb.AppendLine("Guilds:");
        
            foreach (var guild in configGuildSettings.Where(guild => guilds.All(g => g.Id != guild.GuildId))) {
                configGuildSettings.Remove(guild);
                sb.AppendLine($"Removed {guild.GuildName} ({guild.GuildId}) from the config.");
            }

            await RespondAsync(sb.ToString(), ephemeral: true);
        }
    }
}