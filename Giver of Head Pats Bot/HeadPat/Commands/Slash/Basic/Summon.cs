using System.Net.Http.Headers;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HeadPats.Utils;

namespace HeadPats.Commands.Slash.Basic; 

public class Summon : ApplicationCommandModule {
    
    [SlashCommandGroup("Summon", "Summon a picture of various options")]
    public class SummonPicture : ApplicationCommandModule {

        [SlashCommand("Bunny", "Bunnies are mega adorable")]
        public async Task Bunny(InteractionContext c) {
            start:
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", Vars.FakeUserAgent);
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
        public async Task Fox(InteractionContext c) {
            RandomFoxJson.FoxData = null;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", Vars.FakeUserAgent);
            var content = await httpClient.GetStringAsync("https://randomfox.ca/floof/");
            RandomFoxJson.GetData(content);

            var foxCount = await RandomFoxHtml.GetFoxCount();

            var e = new DiscordEmbedBuilder();
            e.WithAuthor("Source", "https://randomfox.ca/");
            e.WithTitle($"{RandomFoxJson.GetImageNumber()} / {foxCount}");
            e.WithColor(Colors.HexToColor("AC5F25"));
            e.WithImageUrl(RandomFoxJson.GetImage());
            e.WithFooter("Powered by randomfox.ca");
            httpClient.Dispose();
            await c.CreateResponseAsync(e.Build());
            RandomFoxJson.FoxData = null;
        }
        
        [SlashCommand("Neko", "Summon a picture of a neko (SFW)")]
        public async Task Neko(InteractionContext c) {
            var embed = new DiscordEmbedBuilder {
                Title = "Neko",
                Color = Colors.Random,
                Footer = new DiscordEmbedBuilder.EmbedFooter {
                    Text = "Powered by FluxpointAPI"
                },
                ImageUrl = (await Program.FluxpointClient!.Sfw.GetNekoAsync()).file
            };
            await c.CreateResponseAsync(embed.Build());
        }
        
        [SlashCommand("Cat", "Summon a picture of a cat")]
        public async Task Cat(InteractionContext c) {
            var embed = new DiscordEmbedBuilder {
                Title = "Kitty",
                Color = Colors.Random,
                Footer = new DiscordEmbedBuilder.EmbedFooter {
                    Text = "Powered by FluxpointAPI"
                },
                ImageUrl = (await Program.FluxpointClient!.Animal.GetCatAsync()).file
            };
            await c.CreateResponseAsync(embed.Build());
        }
    }
    
    [SlashCommand("Meme", "Get a random meme from one of nine subreddits")]
    public async Task Meme(InteractionContext c) {
        var counter = 0;
        start:
        var subreddit = TheData.MemeSubreddits[new Random().Next(0, 8)];

        TheData.RedditData = null;
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", Vars.FakeUserAgent);
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
            e.WithAuthor("Reddit", $"https://www.reddit.com/r/{subreddit}");
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
    public async Task InspiroBot(InteractionContext c) {
        var httpClient = new HttpClient();
        var content = await httpClient.GetStringAsync("https://inspirobot.me/api?generate=true&oy=vey"); // out puts an image URL link
        if (string.IsNullOrWhiteSpace(content)) {
            httpClient.Dispose();
            await c.CreateResponseAsync("Failed to get an image.", true);
            return;
        }
        var e = new DiscordEmbedBuilder();
        e.WithAuthor("InspiroBot", "https://inspirobot.me/");
        e.WithTitle("Got your image!");
        e.WithColor(Colors.Random);
        e.WithImageUrl(content);
        e.WithFooter("Powered by inspirobot.me");
        httpClient.Dispose();
        await c.CreateResponseAsync(e.Build());
    }
}