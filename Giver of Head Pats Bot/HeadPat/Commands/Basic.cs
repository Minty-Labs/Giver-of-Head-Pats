﻿using System.Net.Http.Headers;
using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using HeadPats.Data;
using HeadPats.Handlers;
using HeadPats.Managers;
using HeadPats.Utils;
using Newtonsoft.Json;
using cc = DSharpPlus.CommandsNext.CommandContext;
using ic = DSharpPlus.SlashCommands.InteractionContext;

namespace HeadPats.Commands; 

public class Basic : BaseCommandModule {
    public Basic() => Logger.LoadModule("BasicCommands");
    
    [Command("ping"), Description("Shows bot's latency from you <-> discord <-> you.")]
    public async Task Ping(cc c) => await c.RespondAsync($":ping_pong: Pong > {c.Client.Ping}ms");
    
    [Command("stats"), Description("Shows the bot status including server status and bot stats")]
    public async Task Stats(cc c) {
        var ram = GC.GetTotalMemory(false) / 1024 / 1024;
        var tempNow = DateTime.Now;
        var days = tempNow.Subtract(Vars.StartTime).Days;
        var hours = tempNow.Subtract(Vars.StartTime).Hours;
        var minutes = tempNow.Subtract(Vars.StartTime).Minutes;
        var seconds = tempNow.Subtract(Vars.StartTime).Seconds;

        var e = new DiscordEmbedBuilder();
        e.WithTitle($"{Vars.Name} Stats");
        e.WithColor(DiscordColor.Teal);

        e.AddField("Number of Commands", $"{Program.Commands?.RegisteredCommands.Count + Program.Slash?.RegisteredCommands.Count}", true);
        e.AddField("Ping", $"{c.Client.Ping}ms", true);
        e.AddField("Usage", $"Currently using **{ram}MB** of RAM\nRunning on **{(Vars.IsWindows ? "Windows" : "Linux")}**", true);
        e.AddField("Current Uptime", $"{days} Days : {hours} Hours : {minutes} Minutes : {seconds} Seconds");
        e.AddField("Bot Versions Info", $"DSharpPlus: **v{Vars.DSharpVer}** \nBot: **v{Vars.Version}** \nBuild Date: {Vars.BuildTime:F} - <t:{Vars.BuildTime.GetSecondsFromUnixTime()}:R>");
        e.AddField("Server Info", "Location: **Finland** \nServer: **Hetzner** \nMax RAM: **16 GB** \nOS: **Debian 11**");

        e.WithTimestamp(DateTime.Now);
        await c.RespondAsync(e.Build());
    }
    
    [Command("Cry"), Aliases("crying"), Description("Summon a picture or GIF of a crying post")]
    public async Task Cry(cc c) {
        NekoLoveJson.NekoData = null;
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0");
        var content = await httpClient.GetStringAsync("https://neko-love.xyz/api/v1/cry");
        NekoLoveJson.GetData(content);

        if (NekoLoveJson.ImageHasValidExtension()) {
            var e = new DiscordEmbedBuilder();
            e.WithTitle("`*`_cry_`*`");
            e.WithColor(Colors.HexToColor("58CBCF"));
            e.WithImageUrl(NekoLoveJson.GetImage());
            e.WithFooter("Powered by neko-love.xyz");
            e.WithTimestamp(DateTime.Now);
            
            httpClient.Dispose();
            await c.RespondAsync(e.Build());
        }
    }

    [Command("TopPat"), Aliases("lb", "leaderboard", "tp"), Description("Shows the leaderboard for most headpats")]
    public async Task TopPat(cc c) => await c.RespondAsync("Use slash commands instead. `/TopPat`");

    [Command("Salad"), Description("Summon a picture of salad")]
    [Cooldown(50, 3600, CooldownBucketType.Guild)]
    [LockCommandForOnlyMintyLabs]
    public async Task Salad(cc c) {
        if (string.IsNullOrWhiteSpace(Vars.Config.UnsplashAccessKey) || string.IsNullOrWhiteSpace(Vars.Config.UnsplashSecretKey)) {
            await c.RespondAsync("The bot owner has not set up the Unsplash API keys yet. Therefore, this command cannot be used at the moment.").DeleteAfter(10);
            await c.Message.DeleteAsync();
            return;
        }
        
        var unsplashApiUrl = $"https://api.unsplash.com/photos/random/?query=salad&count=1&client_id={Vars.Config.UnsplashAccessKey}";
        if (UnsplashApiJson.unsplashApi != null) UnsplashApiJson.unsplashApi.Clear();
        UnsplashApiJson.unsplashApi = null;
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36");
        var content = await httpClient.GetStringAsync(unsplashApiUrl);
        var logged = JsonConvert.SerializeObject(content, Formatting.Indented);
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

    /*[Command("SwapLikeMethod")]
    [RequireOwner]
    public async Task SLM(cc c) {
        UnsplashApiJson.tempMethod = !UnsplashApiJson.tempMethod;
        await c.RespondAsync($"Done. Using {(UnsplashApiJson.tempMethod ? "Action" : "Boolean")}").DeleteAfter(5);
        await c.Message.DeleteAsync();
    }*/
}

public static class BasicCmdUtils {
    public static async Task OutputBaseCommand(ic c, string embedTitle, string? imageUrl, string colorHex, string footerText) {
        var e = new DiscordEmbedBuilder();
        e.WithTitle(embedTitle);
        e.WithImageUrl(imageUrl);
        e.WithColor(Colors.HexToColor(colorHex));
        e.WithTimestamp(DateTime.Now);
        await c.CreateResponseAsync(e.Build());
    }
}

public class SlashBasic : ApplicationCommandModule {
    
    [SlashCommand("About", "Shows a message that describes the bot")]
    public async Task About(ic c) {
        var e = new DiscordEmbedBuilder();
        e.WithColor(Colors.HexToColor("00ffaa"));
        e.WithDescription("Hi, I am the **Giver of Head Pats**. I am here to give others head pats, hug, cuddles, and more. I am always expanding in what I can do. " +
                          $"At the moment you can see what I can do by running the `{Vars.Config.Prefix}help` command.\n" +
                          "I was recently rewritten from Javascript to C#. So if things seem broken or missing from the older version, don't worry, they'll be fixed " +
                          "or added in the near future.\nI hope I will be the perfect caregiver for your guild.");
        e.AddField("Bot Creator Information", "Website: https://mintlily.lgbt/ \n" +
                                              "Donate: https://ko-fi.com/MintLily \n" +
                                              "Open-Source: https://git.ellyvr.dev/Lily/giver-of-head-pats \n" +
                                              $"Add to Your Guild: [Invite Link]({Vars.InviteLink}) \n" +
                                              "Need Support? [Create an Issue](https://git.ellyvr.dev/Lily/giver-of-head-pats/-/issues/new) \n" +
                                              "Privacy Policy: [Link](https://mintlily.lgbt/gohp/privacy)");
        e.WithTimestamp(DateTime.Now);
        var u = await c.Client.GetUserAsync(Vars.ClientId, true);
        e.WithThumbnail(u.AvatarUrl);
        e.WithAuthor("MintLily#0001", "https://mintlily.lgbt/", "https://mintlily.lgbt/assets/img/Lily_Art_Headshot_pfp_x1024.png");
        await c.CreateResponseAsync(e.Build());
    }

    [SlashCommand("Support", "Get the support server invite link")]
    public async Task Support(ic c)
        => await c.CreateResponseAsync("Need support? Create an issue on our GitLab:\n  https://git.ellyvr.dev/Lily/giver-of-head-pats/-/issues/new", true);

    [SlashCommand("Invite", "Get the bot invite link")]
    public async Task Invite(ic c)
        => await c.CreateResponseAsync($"Want to invite me to your guild? Add me here:\n  {Vars.InviteLink}", true);

    [SlashCommand("FlipCoin", "Flip a coin")]
    public async Task FlipCoin(ic c) 
        => await c.CreateResponseAsync($"The coin flip result is **{(new Random().Next(0, 1) == 0 ? "Heads" : "Tails")}**");
    
    [SlashCommandGroup("Summon", "Summon a picture of various options")]
    public class SummonPicture : ApplicationCommandModule {
        [SlashCommand("Cat", "Cat pics are always nice")]
        public async Task Cat(ic c) {
            var neko = new Random().Next(0, 1) == 1 ? Program.NekoClient?.Misc.Cat() : Program.NekoClient?.Misc_v3.Cat();

            await BasicCmdUtils.OutputBaseCommand(c, "", neko?.Result.ImageUrl, "FFFF00", "nekos.life");
        }

        [SlashCommand("Bunny", "Bunnies are mega adorable")]
        public async Task Bunny(ic c) {
            start:
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var content = await httpClient.GetStringAsync("https://api.bunnies.io/v2/loop/random/?media=gif,png");
            // Logger.Log($"Data: {content}");
            var id = content.Split("\"id\":\"")[1].Split("\"")[0];
            var url = content.Split("\"gif\":\"")[1].Split("\"")[0];
            var source = content.Split("\"source\":\"")[1].Split("\"")[0];
            // Logger.Log(id);
            // Logger.Log(url);

            if (string.IsNullOrWhiteSpace(url)) {
                httpClient.Dispose();
                goto start;
            }

            var e = new DiscordEmbedBuilder();
            if (source != "unknown") {
                e.WithAuthor("Source", source);
            }

            e.WithTitle($"Bunny #{id}");
            e.WithColor(Colors.HexToColor("#B88F64"));
            e.WithImageUrl(url);
            e.WithFooter("Powered by Bunnies.io");
            httpClient.Dispose();
            await c.CreateResponseAsync(e.Build());
            // BunnyJson.BunnyData = null;
        }

        [SlashCommand("Fox", "Foxes are cute")]
        public async Task Fox(ic c) {
            RandomFoxJson.FoxData = null;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0");
            var content = await httpClient.GetStringAsync("https://randomfox.ca/floof/");
            RandomFoxJson.GetData(content);

            var foxCount = await RandomFoxHtml.GetFoxCount();

            var e = new DiscordEmbedBuilder();
            e.WithTitle($"{RandomFoxJson.GetImageNumber()} / {foxCount}");
            e.WithColor(Colors.HexToColor("AC5F25"));
            e.WithImageUrl(RandomFoxJson.GetImage());
            e.WithFooter("Powered by randomfox.ca");
            httpClient.Dispose();
            await c.CreateResponseAsync(e.Build());
            RandomFoxJson.FoxData = null;
        }
        
        [SlashCommand("Neko", "Summon a picture or GIF of a SFW neko")]
        public async Task Neko(ic c) {
            var neko = new Random().Next(0, 1) == 1 ? Program.NekoClient?.Image.Neko() : Program.NekoClient?.Image.NekoGif();
            await BasicCmdUtils.OutputBaseCommand(c, "Random Neko", neko?.Result.ImageUrl, "42F4A1", "Powered by nekos.life");
        }
    }

    [SlashCommand("TopPat", "Get the top pat leaderboard")]
    public async Task PatLeaderboard(ic c,
        [Option("KeyWords", "Key words (can be empty)")]
        string? keyWords = "") {
        await using var db = new Context();

        var guildPats = 0;
        try { guildPats = db.Guilds.AsQueryable().ToList().FirstOrDefault(g => g.GuildId == c.Guild.Id)!.PatCount; }
        catch { Logger.Error("[TopPat] Guilds DataSet is Empty"); }

        var globalPats = 0;
        try { globalPats = db.Overall.AsQueryable().ToList().First().PatCount; }
        catch { Logger.Error("[TopPat] Server DataSet is Empty"); }

        var newUserList = db.Users.AsQueryable().ToList().OrderBy(p => -p.PatCount);

        if (keyWords!.ToLower().Equals("server")) {
            var strings = new StringBuilder();
            strings.AppendLine($"Top 50 that are in this server. - Server Pats: **{guildPats}** - Global Pats: **{globalPats}**");
            var counter = 1;
            foreach (var u in newUserList) {
                if (counter >= 51) continue;
                if (!c.Guild.Members.Keys.Contains(u.UserId)) continue;
                strings.AppendLine($"`{counter}.` {u.UsernameWithNumber.Split('#')[0]} - Total Pats: **{u.PatCount}**");
                counter++;
            }

            await c.CreateResponseAsync(strings.ToString());
            return;
        }

        var max = 1;
        var sb = new StringBuilder();

        foreach (var u in newUserList) {
            if (max >= 11) continue;
            if (!c.Guild.Members.Keys.Contains(u.UserId)) continue;
            sb.AppendLine($"`{max}.` {u.UsernameWithNumber} - Total Pats: **{u.PatCount}**");
            max++;
        }

        var temp = sb.ToString().ReplaceTheNamesWithTags();

        var e = new DiscordEmbedBuilder();
        e.WithTitle("Head Pat Leaderboard");
        e.WithColor(Colors.HexToColor("DFFFDD"));
        e.WithFooter($"Synced across all servers • {Vars.Name} (v{Vars.Version})");
        e.AddField("Current Server Stats",
            $"{(string.IsNullOrWhiteSpace(temp) ? "Data is Empty" : $"{temp}")}\nTotal Server Pats **{guildPats}**");
        e.AddField("Global Stats", $"Total Pats: **{globalPats}**");
        e.WithTimestamp(DateTime.Now);
        await c.CreateResponseAsync(e.Build());
    }

    [SlashCommand("Meme", "Get a random meme from one of nine subreddits")]
    public async Task Meme(ic c) {
        var counter = 0;
        start:
        var subreddit = TheData.MemeSubreddits[new Random().Next(0, 8)];

        TheData.RedditData = null;
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0");
        var content = await httpClient.GetStringAsync($"https://www.reddit.com/r/{subreddit}/random/.json");
        //await c.RespondAsync(string.Concat(new [] {
        //    "```json\n",
        //    content[..1979],
        //    "```"
        //}));
        try {
            TheData.GetData(content);
        }
        catch {
            httpClient.Dispose();
            goto start;
        }

        if (TheData.IsNsfw()) {
            httpClient.Dispose();
            goto start;
        }

        var image = TheData.GetImageUrl();
        if (string.IsNullOrWhiteSpace(image)) {
            httpClient.Dispose();
            goto start;
        }

        try {
            counter += 1;
            var e = new DiscordEmbedBuilder();
            e.WithTitle(TheData.GetTitle()?.Replace("&amp;", "&").Replace("&ndash;", "\u2013").Replace("&mdash;", "\u2014"));
            e.WithColor(Colors.Random);
            //e.WithUrl(TheData.GetPostUrl()); // It no likey
            e.WithImageUrl(image);
            e.WithFooter($"Powered by Reddit from r/{subreddit}");
            httpClient.Dispose();
            await c.CreateResponseAsync(e.Build());
        }
        catch {
            if (counter >= 3)
                return;
            goto start;
        }
        TheData.RedditData = null;
    }

    [SlashCommand("InspiroBot", "Lets an AI create an inspirational quote with image")]
    public async Task InspiroBot(ic c) {
        var httpClient = new HttpClient();
        var content = await httpClient.GetStringAsync("https://inspirobot.me/api?generate=true&oy=vey"); // out puts an image URL link
        if (string.IsNullOrWhiteSpace(content)) {
            httpClient.Dispose();
            await c.CreateResponseAsync("Failed to get an image.", true);
            return;
        }
        var e = new DiscordEmbedBuilder();
        e.WithTitle("Got your image!");
        e.WithColor(Colors.Random);
        e.WithImageUrl(content);
        e.WithFooter($"Powered by inspirobot.me");
        httpClient.Dispose();
        await c.CreateResponseAsync(e.Build());
    }
}