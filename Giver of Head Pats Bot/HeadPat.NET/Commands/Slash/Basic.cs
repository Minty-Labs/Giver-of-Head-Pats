using Discord;
using Discord.Interactions;
using HeadPats.Configuration;
using HeadPats.Utils;
using HeadPats.Utils.ExternalApis;

namespace HeadPats.Commands.Slash; 

public class Basic : InteractionModuleBase<SocketInteractionContext> {
    [SlashCommand("about", "Shows a message that describes the bot")]
    public async Task About() {
        var bot = Program.Instance.GetUser(Vars.IsWindows || Vars.IsDebug ? 495714488897503232 : Vars.ClientId);
        var embed = new EmbedBuilder {
            Color = Colors.HexToColor("00ffaa"),
            Description = "Hi, I am the **Giver of Head Pats**. I am here to give others head pats, hug, cuddles, and more. I am always expanding in what I can do. " +
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
                                              $"Add to Your Guild: [Invite Link]({Vars.InviteLink}) \n" +
                                              $"Need Support? [Join the Support Sever]({Vars.SupportServer}) \n" +
                                              "Privacy Policy: [Link (`mintylabs.dev`)](https://mintylabs.dev/gohp/privacy-policy) \n" +
                                              "Terms of Service: [Link (`mintylabs.dev`)](https://mintylabs.dev/gohp/terms) \n");
        embed.AddField("Uptime", "```" + (DateTime.UtcNow - Vars.StartTime).ToString(@"dd\.hh\:mm\:ss") + "```");
        var embed2 = new EmbedBuilder {
            Title = "Contributors",
            Description = "These are the Contributors of this bot's project, as I must give credit where its due.",
            Color = Colors.HexToColor("00FFBF"),
            ThumbnailUrl = bot.GetAvatarUrl(),
            Footer = new EmbedFooterBuilder { Text = "If you would like to be added to this list, please contact me." }
        };
        Config.Base.Contributors!.ForEach(contributor => embed2.AddField(contributor.UserName, contributor.Info!.Replace("<br>", "\n")));
        
        var supporterEmbed = new EmbedBuilder {
            Title = "Patreon Supporters",
            Url = "https://www.patreon.com/MintLily",
            Description = "These are the Supporters of this bot's project",
            Color = Colors.HexToColor("8A698D"),
            ThumbnailUrl = "https://i.mintlily.lgbt/Lily_2022_Alternate_pfp.png",
            Footer = new EmbedFooterBuilder { Text = "If you would like to be added to this list, please contact me." }
        };
        try {
            var cutie = Program.Instance.PatreonClientInstance.CutieTier;
            var megaCutie = Program.Instance.PatreonClientInstance.MegaCutieTier;
            var adorable = Program.Instance.PatreonClientInstance.AdorableTier;
            supporterEmbed.AddField("Cutie", cutie is null || cutie.Count > 0 ? "None" : string.Join(',', cutie));
            supporterEmbed.AddField("Mega Cutie", megaCutie is null || megaCutie.Count > 0  ? "None" : string.Join(',', megaCutie));
            supporterEmbed.AddField("Adorable", adorable is null || adorable.Count > 0 ? "None" : string.Join(',', adorable));
        }
        catch (Exception ex) {
            supporterEmbed.AddField("Uh oh!", "Nothing loaded.\n```" + ex.Message + "```");
        }
        
        await RespondAsync(embeds: new [] { embed.Build(), embed2.Build(), supporterEmbed.Build() });
    }

    [SlashCommand("support", "Get the support server invite link")]
    public async Task Support() => await RespondAsync($"Need support? Join the Support Sever at {Vars.SupportServer}", ephemeral: true);

    [SlashCommand("invite", "Get the bot invite link")]
    public async Task Invite() => await RespondAsync($"Want to invite me to your guild? Add me here:\n  {Vars.InviteLink}", ephemeral: true);

    [SlashCommand("flipcoin", "Flip a coin")]
    public async Task FlipCoin() => await RespondAsync($"The coin flip result is **{(new Random().Next(0, 1) == 0 ? "Heads" : "Tails")}**");
}