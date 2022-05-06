using System.ComponentModel;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Emzi0767.Utilities;
using HeadPats.Managers;
using HeadPats.Utils;
using cc = DSharpPlus.CommandsNext.CommandContext;
using ic = DSharpPlus.SlashCommands.InteractionContext;

namespace HeadPats.Commands; 

public class Owner : BaseCommandModule {
    public Owner() => Logger.Loadodule("OwnerCommands");
    
    private string FooterText(string extra = "")
        => $"{BuildInfo.Name} (v{BuildInfo.Version}) • {BuildInfo.BuildDate}{(string.IsNullOrWhiteSpace(extra) ? "" : $" • {extra}")}";

    [Command("ForceRegisterSlashCommands")]
    [RequireOwner]
    public async Task RegOwner(cc c) {
        var s = Program.Slash;
        s?.RegisterCommands<BasicSlashCommands>();
        s?.RegisterCommands<SlashOwner>();
        await c.RespondAsync("Done");
    }
}

// public class RequireUserIdAttribute : SlashCheckBaseAttribute {
//     public ulong UserId;
//
//     public RequireUserIdAttribute(ulong userId) => UserId = userId;
//
//     public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) => ctx.User.Id == UserId;
// }

public class SlashOwner : ApplicationCommandModule {
    public SlashOwner() => Logger.Loadodule("OwnerCommands");
    
    private string FooterText(string extra = "")
        => $"{BuildInfo.Name} (v{BuildInfo.Version}) • {BuildInfo.BuildDate}{(string.IsNullOrWhiteSpace(extra) ? "" : $" • {extra}")}";

    [SlashCommand("activity", "Change the bot\'s Activity")]
    public async Task ChangeActivity(ic c,
        [Choice("Offline", "off")]
        [Choice("Invisible", "in")]
        [Choice("Do not Disturb", "d")]
        [Choice("Idle", "id")]
        [Choice("Online", "on")]
        [Option("User Status", "Change User Status")] string userStatus,
        
        [Choice("Playing", "play")]
        [Choice("Listening", "listen")]
        [Choice("Watching", "watch")]
        [Choice("Streaming", "stream")]
        [Choice("Competing", "compete")]
        [Choice("Other Text", "other")]
        [Option("Activity Type", "Change Activity Type")] string activityType,
        
        string? args = "") {
        if (c.Member.Id != 167335587488071682) {
            await c.CreateResponseAsync("You cannot run this command.");
            return;
        }
        
        if (string.IsNullOrWhiteSpace(args)) {
            await c.CreateResponseAsync("Please select an argument.");
            return;
        }

        var getStatus = userStatus switch {
            "off" => UserStatus.Offline,
            "in"  => UserStatus.Invisible,
            "d"   => UserStatus.DoNotDisturb,
            "id"  => UserStatus.Idle,
            "on"  => UserStatus.Online,
            _     => UserStatus.Offline
        };

        var getActivity = activityType switch {
            "play" => ActivityType.Playing,
            "listen" => ActivityType.ListeningTo,
            "watch" => ActivityType.Watching,
            "stream" => ActivityType.Streaming,
            "compete" => ActivityType.Competing,
            "other" => ActivityType.Custom,
            _ => ActivityType.Playing
        };

        var arg = args.Split('%');
        var url = string.IsNullOrWhiteSpace(args) ? "https://twitch.tv/MintyLily" : arg[1];
        var name = string.IsNullOrWhiteSpace(args) ? BuildInfo.Config.Game : arg[0];
        
        await c.Client!.UpdateStatusAsync(new DiscordActivity {
            Name = name,
            ActivityType = getActivity,
            StreamUrl = url
        }, getStatus);

        BuildInfo.Config.Game = name;
        BuildInfo.Config.ActivityType = Program.GetActivityAsString(getActivity);
        BuildInfo.Config.StreamingUrl = url;
        Configuration.Save();

        var color = userStatus switch {
            "off" => "747F8D",
            "in"  => "747F8D",
            "d"   => "ED4245",
            "id"  => "FAA81A",
            "on"  => "3BA55D",
            _ => "FFFFFF"
        };

        var changeToStreamColor = activityType == "stream" && userStatus is not ("off" and "in");
        
        var e = new DiscordEmbedBuilder();
        e.WithTitle("Changed Status");
        e.WithColor(Colors.HexToColor(changeToStreamColor ? "593695" : color));
        e.WithDescription($"Game: {name}\nActivityType: {Program.GetActivityAsString(getActivity)}\n{(string.IsNullOrWhiteSpace(url) ? "" : $"Stream URL: {url}")}");
        e.WithFooter(FooterText());
        await c.CreateResponseAsync(e.Build());
    }
}