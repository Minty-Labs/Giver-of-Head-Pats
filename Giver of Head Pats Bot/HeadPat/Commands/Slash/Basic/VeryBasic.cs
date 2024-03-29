﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HeadPats.Configuration;
using HeadPats.Utils;

namespace HeadPats.Commands.Slash.Basic; 

public class VeryBasic : ApplicationCommandModule {
    [SlashCommand("About", "Shows a message that describes the bot")]
    public async Task About(InteractionContext c) {
        var e = new DiscordEmbedBuilder();
        e.WithColor(Colors.HexToColor("00ffaa"));
        e.WithDescription("Hi, I am the **Giver of Head Pats**. I am here to give others head pats, hug, cuddles, and more. I am always expanding in what I can do. " +
                          $"At the moment you can see what I can do by starting to type a slash (`/`)\n" + //running the `{Config.Base.Prefix}help` command.\n" +
                          "I hope I will be the perfect caregiver for your guild.");
        e.AddField("Bot Creator Information", "Website: https://mintylabs.dev/gohp \n" +
                                              "Donate: https://ko-fi.com/MintLily \n" +
                                              "Open-Source: https://github.com/Minty-Labs/Giver-of-Head-Pats \n" +
                                              $"Add to Your Guild: [Invite Link]({Vars.InviteLink}) \n" +
                                              $"Need Support? [Join the Support Sever]({Vars.SupportServer}) \n" +
                                              "Privacy Policy: [Link](https://mintylabs.dev/gohp/privacy-policy) \n" +
                                              "Terms of Service: [Link](https://mintylabs.dev/gohp/terms) \n");
        e.WithTimestamp(DateTime.Now);
        var u = await c.Client.GetUserAsync(Vars.ClientId, true);
        e.WithThumbnail(u.AvatarUrl);
        e.WithAuthor("MintLily", "https://mintlily.lgbt/", "https://mintlily.lgbt/assets/img/Lily.png");
        var bot = await c.Client.GetUserAsync(Vars.ClientId);
        var embed = new DiscordEmbedBuilder {
            Title = "Contributors",
            Description = "These are the Contributors of this bot's project, as I must give credit where its due.",
            Color = DiscordColor.Aquamarine,
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = bot.GetAvatarUrl(ImageFormat.Auto) },
            Footer = new DiscordEmbedBuilder.EmbedFooter { Text = $"{Vars.Name} (v{Vars.Version}) | {Vars.BuildDate}" }
        };
        foreach (var co in Config.Base.Contributors!)
            embed.AddField(co.UserName, co.Info!.Replace("<br>", "\n"));
        var builder = new DiscordMessageBuilder();
        builder.AddEmbed(e.Build());
        builder.AddEmbed(embed.Build());
        await c.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(builder));
    }

    [SlashCommand("Support", "Get the support server invite link")]
    public async Task Support(InteractionContext c)
        => await c.CreateResponseAsync($"Need support? Join the Support Sever at {Vars.SupportServer}", true);

    [SlashCommand("Invite", "Get the bot invite link")]
    public async Task Invite(InteractionContext c)
        => await c.CreateResponseAsync($"Want to invite me to your guild? Add me here:\n  {Vars.InviteLink}", true);

    [SlashCommand("FlipCoin", "Flip a coin")]
    public async Task FlipCoin(InteractionContext c) 
        => await c.CreateResponseAsync($"The coin flip result is **{(new Random().Next(0, 1) == 0 ? "Heads" : "Tails")}**");
}