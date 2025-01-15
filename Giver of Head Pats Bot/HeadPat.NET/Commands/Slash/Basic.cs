using Discord;
using Discord.Interactions;
using HeadPats.Commands.Preexecution;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Utils;
using HeadPats.Utils.ExternalApis;

namespace HeadPats.Commands.Slash;

[IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall), CommandContextType(InteractionContextType.Guild, InteractionContextType.PrivateChannel, InteractionContextType.BotDm)]
public class Basic : InteractionModuleBase<SocketInteractionContext> {
    [SlashCommand("about", "Shows a message that describes the bot")]
    public async Task About(bool ephemeral = false) {
        var bot = await Context.Client.GetUserAsync(Vars.IsWindows || Vars.IsDebug ? 495714488897503232 : Vars.ClientId);
        var embed = new EmbedBuilder {
            Color = Colors.HexToColor("00ffaa"),
            Description = $"Hi, I am the {MarkdownUtils.ToBold("Giver of Head Pats")}. I am here to give others head pats, hug, cuddles, and more. I am always expanding in what I can do. " +
                          "At the moment you can see what I can do by starting to type a slash (`/`)\n" +
                          "I hope I will be the perfect caregiver for your guild.",
            Timestamp = DateTime.Now,
            Author = new EmbedAuthorBuilder {
                Name = "MintLily",
                IconUrl = "https://mintlily.lgbt/assets/img/Lily.png",
                Url = "https://mintlily.lgbt/"
            },
            ThumbnailUrl = bot!.GetAvatarUrl()
        };
        embed.AddField("Bot Creator Information", "Website: https://mintylabs.dev/gohp \n" +
                                                  "Donate: https://ko-fi.com/MintLily \n" +
                                                  "Patreon: https://www.patreon.com/MintLily \n" +
                                                  "Open-Source: https://github.com/Minty-Labs/Giver-of-Head-Pats \n" +
                                                  $"Add to Your Guild: {MarkdownUtils.MakeLink("Invite Link", Vars.InviteLink)}\n" +
                                                  $"Need Support? {MarkdownUtils.MakeLink("Join the Support Sever", Vars.SupportServer)}\n" +
                                                  $"Privacy Policy: {MarkdownUtils.MakeLink($"Link {MarkdownUtils.ToCodeBlockSingleline("mintylabs.dev")}", "https://mintylabs.dev/gohp/privacy-policy")}\n" +
                                                  $"Terms of Service: {MarkdownUtils.MakeLink($"Link {MarkdownUtils.ToCodeBlockSingleline("mintylabs.dev")}", "https://mintylabs.dev/gohp/terms")}\n");
        embed.AddField("Uptime", "```" + (DateTime.UtcNow - Vars.StartTime).ToString(@"dd\.hh\:mm\:ss") + "```");
        var embed2 = new EmbedBuilder {
            Title = "Contributors",
            Description = "These are the Contributors of this bot's project, as I must give credit where its due.",
            Color = Colors.HexToColor("00FFBF"),
            ThumbnailUrl = bot.GetAvatarUrl(),
            Footer = new EmbedFooterBuilder { Text = "If you would like to be added to this list, please contact me." }
        };

        Embed[] embeds;

        Config.Base.Contributors!.ForEach(contributor => embed2.AddField(contributor.UserName, contributor.Info!.Replace("<br>", "\n")));

        List<string>? cutie = [], megaCutie = [], adorable = [];
        try {
            cutie = PatronLogic.Instance.CutieTier;
            megaCutie = PatronLogic.Instance.MegaCutieTier;
            adorable = PatronLogic.Instance.AdorableTier;
        }
        catch {
            // ignored
        }

        var cutieBool = cutie is null || cutie.Count > 0;
        var megaCutieBool = megaCutie is null || megaCutie.Count > 0;
        var adorableBool = adorable is null || adorable.Count > 0;

        if (!cutieBool && !megaCutieBool && !adorableBool) {
            var supporterEmbed = new EmbedBuilder {
                Title = "Patreon Supporters",
                Url = "https://www.patreon.com/MintLily",
                Description = "These are the Supporters of this bot's project",
                Color = Colors.HexToColor("8A698D"),
                ThumbnailUrl = "https://i.mintlily.lgbt/Lily_2022_Alternate_pfp.png",
                Footer = new EmbedFooterBuilder { Text = "If you would like to be added to this list, please contact me." }
            };
            supporterEmbed.AddField("Cutie", cutieBool ? "None" : string.Join(',', cutie!));
            supporterEmbed.AddField("Mega Cutie", megaCutieBool ? "None" : string.Join(',', megaCutie!));
            supporterEmbed.AddField("Adorable", adorableBool ? "None" : string.Join(',', adorable!));
            embeds = [embed.Build(), embed2.Build(), supporterEmbed.Build()];
        }
        else {
            embeds = [embed.Build(), embed2.Build()];
        }

        await RespondAsync(embeds: embeds, ephemeral: ephemeral);
    }

    [SlashCommand("support", "Get the support server invite link")]
    public async Task Support() => await RespondAsync($"Need support? Join the Support Sever at {Vars.SupportServer}", ephemeral: true);

    [SlashCommand("invite", "Get the bot invite link")]
    public async Task Invite() => await RespondAsync($"Want to invite me to your guild? Add me here:\n{Vars.InviteLink}", ephemeral: true);

    [SlashCommand("flipcoin", "Flip a coin")]
    public async Task FlipCoin(bool ephemeral = false) => await RespondAsync($"The coin flip result is **{(new Random().Next(0, 1) == 0 ? "Heads" : "Tails")}**", ephemeral: ephemeral);

    [SlashCommand("opencmd", "Runs very specific commands set by the owner"), RateLimit(30, 5)]
    public async Task OpenCommand([Summary("Command", "The command to run")] string command) {
        switch (command) {
            case "stats":
            case "status": {
                await using var db = new Context();
                var embed = new EmbedBuilder {
                    Title = "Bot Stats",
                    Description = $"{Context.User.Mention} is cute!",
                    Color = Colors.HexToColor("9fffe3"),
                    ThumbnailUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Footer = new EmbedFooterBuilder {
                        Text = $"v{Vars.VersionStr}"
                    },
                    Timestamp = DateTime.Now
                }
                    .AddField("Global Pat Count", $"{db.Overall.AsQueryable().ToList().First().PatCount:N0}")
                    .AddField("Guild Count", $"{Program.Instance.Client.Guilds.Count}")
                    .AddField("Patreon Pledge Count", $"{PatronLogic.Instance.MemberCount}")
                    // .AddField("Build Time", $"{Vars.BuildTime.ToUniversalTime().ConvertToDiscordTimestamp(TimestampFormat.LongDateTime)}\n{Vars.BuildTime.ToUniversalTime().ConvertToDiscordTimestamp(TimestampFormat.RelativeTime)}")
                    .AddField("Start Time", $"{Vars.StartTime.ConvertToDiscordTimestamp(TimestampFormat.LongDateTime)}\n{Vars.StartTime.ConvertToDiscordTimestamp(TimestampFormat.RelativeTime)}")
                    .AddField("OS", Vars.IsWindows ? "Windows" : "Linux", true)
                    .AddField("Discord.NET Version", Vars.DNetVer, true)
                    .AddField("System .NET Version", Environment.Version, true)
                    .AddField("Links", $"{MarkdownUtils.MakeLink("GitHub", "https://github.com/Minty-Labs/Giver-of-Head-Pats")} | " +
                                       $"{MarkdownUtils.MakeLink("Privacy Policy", "https://mintylabs.dev/gohp/privacy-policy")} | {MarkdownUtils.MakeLink("Terms of Service", "https://mintylabs.dev/gohp/terms")} | " +
                                       $"{MarkdownUtils.MakeLink("Donate", "https://ko-fi.com/MintLily")} | {MarkdownUtils.MakeLink("Patreon", "https://www.patreon.com/MintLily")} | {MarkdownUtils.MakeLink("Developer Server", Vars.SupportServer)}\n");
                await RespondAsync(embed: embed.Build());
                break;
            }
            case "peppermint":
            case "mintcraft":
            case "pepper mint":
            case "mint craft":
            case "mc":
            case "minecraft": {
                try {
                    var mcServer = await Program.Instance.FluxpointClient.Minecraft.GetMinecraftServerAsync("mc.mili.lgbt");
                    var embed = new EmbedBuilder {
                            Title = "Minecraft Server",
                            Description = $"Server is currently {(mcServer.online ? "online" : "offline")}",
                            Color = Colors.HexToColor("00D200"),
                            ThumbnailUrl = mcServer.icon ?? "https://i.mintlily.lgbt/null.jpg",
                        }
                        .AddField("IP", "mc.mili.lgbt")
                        .AddField("Player Count", $"{mcServer.playersOnline} / {mcServer.playersMax}")
                        .AddField("Version", mcServer.version)
                        .AddField("MOTD", mcServer.motd)
                        .AddField("Available Platforms", "Java Edition, Bedrock Edition (Xbox, PlayStation, Switch, iOS, Android, Windows 10/11)")
                        .AddField("Players", $"{(mcServer.players.Length > 0 ? string.Join(", ", mcServer.players)[..512] : "No players online")}")
                        .AddField("Miscellaneous Info", $"code:{mcServer.code}|success:{mcServer.success}|message:{mcServer.message}");
                    await RespondAsync(embed: embed.Build());
                }
                catch {
                    await RespondAsync("Failed to get server status", ephemeral: true);
                }
                break;
            }
            case "force download users":
                if (Context.User.Id is 167335587488071682)
                    await Context.Client.DownloadUsersAsync([Context.Guild]);
                break;
            default:
                await RespondAsync("Invalid or unknown command", ephemeral: true);
                break;
        }
    }
}