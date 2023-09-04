using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using HeadPats.Configuration;
using HeadPats.Handlers;
using HeadPats.Managers;
using HeadPats.Utils;

namespace HeadPats.Commands.Legacy.Basic; 

public class Salad : BaseCommandModule {
    [Command("Salad"), Description("Summon a picture of salad"), Cooldown(50, 3600, CooldownBucketType.Guild), LockCommandForLilysComfyCorner]
    public async Task SaladCommand(CommandContext c) {
        if (string.IsNullOrWhiteSpace(Config.Base.Api.ApiKeys.UnsplashAccessKey) || string.IsNullOrWhiteSpace(Config.Base.Api.ApiKeys.UnsplashSecretKey)) {
            await c.RespondAsync("The bot owner has not set up the Unsplash API keys yet. Therefore, this command cannot be used at the moment.").DeleteAfter(10);
            await c.Message.DeleteAsync();
            return;
        }
        
        var unsplashApiUrl = $"https://api.unsplash.com/photos/random/?query=salad&count=1&client_id={Config.Base.Api.ApiKeys.UnsplashAccessKey}";
        if (UnsplashApiJson.unsplashApi != null) UnsplashApiJson.unsplashApi.Clear();
        UnsplashApiJson.unsplashApi = null;
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", Vars.FakeUserAgent);
        var content = await httpClient.GetStringAsync(unsplashApiUrl);
        // var logged = JsonConvert.SerializeObject(content, Formatting.Indented);
        // Logger.Log(logged);
        httpClient.Dispose();
        UnsplashApiJson.GetData(content);
        await c.Message.DeleteAsync();
        
        var unsplashSaladUrlLink = UnsplashApiJson.GetImage();
        var unsplashSaladPostTime = UnsplashApiJson.GetCreatedAt();
        var unsplashSaladPostAuthor = UnsplashApiJson.GetAuthorName();
        var unsplashSaladPostAuthorProfileLink = UnsplashApiJson.GetAuthorProfileLink();
        var unsplashSaladPostAuthorProfileImage = UnsplashApiJson.GetAuthorProfileImage();
        var imageLinkCounter = UnsplashApiJson.GetLikes();
        var imageDownloadCounter = UnsplashApiJson.GetDownloadCount();
        var imageId = UnsplashApiJson.GetImageId();
        
        /*UnsplashApiJson.DownloadImage = null;
        var url = $"https://api.unsplash.com/photos/{imageId}/download?client_id={BuildInfo.Config.UnsplashAccessKey}";
        var http = new HttpClient();
        var dlContent = await http.GetStringAsync(url);
        UnsplashApiJson.DownloadImageMethod(dlContent);
        http.Dispose();*/

        var saladEmbed = new DiscordEmbedBuilder {
            Title = "I got you a salad!",
            Description = $"[Direct Photo Link]({UnsplashApiJson.GetDownloadImageLink()})\n" +
                          $"{UnsplashApiJson.GetPostDescription() ?? UnsplashApiJson.GetPostAltDescription() ?? ""}",
            ImageUrl = unsplashSaladUrlLink,
            Color = UnsplashApiJson.GetColor(),
            Timestamp = unsplashSaladPostTime,
            Footer = new DiscordEmbedBuilder.EmbedFooter {
                Text = $"Powered by Unsplash | Photo by {unsplashSaladPostAuthor}"
            },
            Author = new DiscordEmbedBuilder.EmbedAuthor {
                IconUrl = unsplashSaladPostAuthorProfileImage,
                Url = unsplashSaladPostAuthorProfileLink + $"?utm_source={Vars.Name.Replace(" ", "_")}_-_Discord_Bot&utm_medium=referral",
                // {BuildInfo.Name.Replace(" ", "_")} refers to the bot's name, but with underscores instead of spaces. Being "Giver of Head Pats"
                Name = unsplashSaladPostAuthor
            }
        };
        
        var builder = new DiscordMessageBuilder();
        builder.WithEmbed(saladEmbed.Build());
        
        var likeButton = new DiscordButtonComponent(ButtonStyle.Primary, "like_image", $"{imageLinkCounter}", false, new DiscordComponentEmoji("❤️"));
        var downloadCountButton = new DiscordButtonComponent(ButtonStyle.Secondary, "download_count", $"{imageDownloadCounter}", true, new DiscordComponentEmoji("💾"));
        //var downloadImageButton = new DiscordButtonComponent(ButtonStyle.Primary, "dlImage", "Download Image", false, new DiscordComponentEmoji("↗️"));
        builder.AddComponents(likeButton, downloadCountButton);
        var message = await builder.SendAsync(c.Channel);
        var args = await c.Client.GetInteractivity().WaitForButtonAsync(message, TimeSpan.FromMinutes(2.5f));
        var likedOnce = false;
        start:
        // var isOriginalCmdAuthor = args.Result.User == c.User;
        if (!args.TimedOut) {
            await args.Result.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
            args = await c.Client.GetInteractivity().WaitForButtonAsync(message, TimeSpan.FromMinutes(2.5f));
            await Task.Delay(450);
        }
        
        if (args.TimedOut) {
            var timedOutButton = new DiscordButtonComponent(ButtonStyle.Danger, "timeout", "Timed Out!", true, new DiscordComponentEmoji("⏰"));
            builder.ClearComponents();
            var newBuilder = new DiscordMessageBuilder();
            newBuilder.WithContent(builder.Content);
            newBuilder.AddEmbeds(builder.Embeds);
            newBuilder.AddComponents(timedOutButton, downloadCountButton);
            await message.ModifyAsync(newBuilder);
            await Task.Delay(900);
        }
        else if (args.Result.Id == "like_image" && !likedOnce/* && isOriginalCmdAuthor*/) {
            likedOnce = true;
            var liked = UnsplashApiJson.LikeImage(imageId);
            await Task.Delay(TimeSpan.FromSeconds(1));
            if (liked) {
                var likeButton2 = new DiscordButtonComponent(ButtonStyle.Primary, "like_image", $"{imageLinkCounter + 1}", true, new DiscordComponentEmoji("❤️"));
                builder.ClearComponents();
                var newBuilder = new DiscordMessageBuilder();
                newBuilder.WithContent(builder.Content);
                newBuilder.AddEmbeds(builder.Embeds);
                newBuilder.AddComponents(likeButton2, downloadCountButton);
                await message.ModifyAsync(newBuilder);
            }
            else {
                await c.RespondAsync("Something went wrong. Please try again later.").DeleteAfter(10);
            }
            await Task.Delay(900);
        }
        else {
            await c.RespondAsync("Image was already liked."/*"Only the original command author can like the image."*/).DeleteAfter(10);
            goto start;
        }
        await Task.Delay(450);
    }
}