﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HeadPats.Managers;
using HeadPats.Utils;
using cc = DSharpPlus.CommandsNext.CommandContext;

namespace HeadPats.Commands; 

public class Basic : BaseCommandModule {
    public Basic() => Logger.Loadodule("BasicCommands");
    
    private readonly string _footerText = $"{BuildInfo.Name} (v{BuildInfo.Version}) • {BuildInfo.BuildDate}";
    
    [Command("ping"), Description("Shows bot's latency from you <-> discord <-> you.")]
    public async Task Ping(cc c) => await c.RespondAsync($":ping_pong: Pong > {c.Client.Ping}ms");
    
    [Command("stats"), Description("Shows the bot status including server status and bot stats")]
    public async Task Stats(cc c) {
        var ram = GC.GetTotalMemory(false) / 1024 / 1024;
        //var cpu = "x";
        var platform = "Windows";
        var discordNetVer = BuildInfo.DSharpVer;
        var mintApiVer = BuildInfo.MintAPIVer;
        var tempNow = DateTime.Now;
        var days = tempNow.Subtract(BuildInfo.StartTime).Days;
        var hours = tempNow.Subtract(BuildInfo.StartTime).Hours;
        var minutes = tempNow.Subtract(BuildInfo.StartTime).Minutes;
        var seconds = tempNow.Subtract(BuildInfo.StartTime).Seconds;

        var e = new DiscordEmbedBuilder();
        e.WithTitle($"{BuildInfo.Name} Stats");
        e.WithColor(DiscordColor.Teal);

        e.AddField("Number of Commands", $"{Program.Commands?.RegisteredCommands.Count + Program.Slash?.RegisteredCommands.Count}", true);
        e.AddField("Ping", $"{c.Client.Ping}ms", true);
        e.AddField("Usage", $"Currently using **{ram}MB** of RAM\nRunning on **{platform}**", true);
        e.AddField("Current Uptime", $"{days} Days : {hours} Hours : {minutes} Minutes : {seconds} Seconds");
        e.AddField("Bot Versions Info", $"DSharpPlus: **v{discordNetVer}** \nBot: **v{BuildInfo.Version}** \nMintAPI: **v{mintApiVer}** \nBuild Date: **{BuildInfo.BuildDateShort}**");
        //e.AddField("Server Info", $"Location: **Finland** \nServer: **Hetzner** \nMax RAM: **4 GB** \nOS: **{platform} 11 (Debian GNU)**");
        e.AddField("Server Info", $"Location: **South Carolina, USA** \nServer: **[Sypher](https://mintlily.lgbt/pc)** \nMax RAM: **32 GB** \nOS: **{platform} 10 (21H1)**");
        
        e.WithTimestamp(DateTime.Now);
        e.WithFooter(_footerText);
        await c.RespondAsync(e.Build());
    }

    [Command("flipcoin"), Aliases("fc", "coinflip"), Description("Flip a coin")]
    public async Task CoinFlip(cc c) {
        var rnd = new Random();
        var point = rnd.Next(0, 1);
        await c.RespondAsync($"The coin flip result is **{(point == 0 ? "Heads" : "Tails")}**");
    }

    [Command("inspirobot"), Aliases("ib"), Description("Generates a random inspirational quote created by an AI.")]
    public async Task InspiroBot(cc c) {
        var content = "";
        var httpClient = new HttpClient();
        content = await httpClient.GetStringAsync("https://inspirobot.me/api?generate=true&oy=vey"); // out puts an image URL link
        if (string.IsNullOrWhiteSpace(content)) {
            httpClient.Dispose();
            await c.RespondAsync("Failed to get an image.");
            await Task.Delay(10000);
            await c.Message.DeleteAsync();
            return;
        }
        var e = new DiscordEmbedBuilder();
        e.WithTitle("Got your image!");
        e.WithColor(Colors.Random);
        e.WithImageUrl(content);
        e.WithFooter($"{BuildInfo.Name} (v{BuildInfo.Version}) • Powered by inspirobot.me");
        httpClient.Dispose();
        await c.RespondAsync(e.Build());
    }

    [Command("jsonsave"), Description("DEBUG: Saves the JSON Configuration file.")]
    [RequirePermissions(Permissions.Administrator)]
    public async Task SaveJson(cc c) => Configuration.Save();

    [Command("meme"), Description("Grabs a random meme image from one of 5 meme subreddits")]
    public async Task Meme(cc c) {
        start:

        var rnd = new Random();
        var point = rnd.Next(0, 8);
        var subreddit = TheData.MemeSubreddits[point];
        
        TheData.RedditData = null;
        var httpClient = new HttpClient();
        var content = await httpClient.GetStringAsync($"https://www.reddit.com/r/{subreddit}/random/.json");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0");
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
        
        var e = new DiscordEmbedBuilder();
        e.WithTitle(TheData.GetTitle());
        e.WithColor(Colors.Random);
        //e.WithUrl(TheData.GetPostUrl()); // It no likey
        e.WithImageUrl(image);
        e.WithFooter($"{BuildInfo.Name} (v{BuildInfo.Version}) • Powered by Reddit from r/{subreddit}");
        httpClient.Dispose();
        await c.RespondAsync(e.Build());
    }

    [Command("fox"), Aliases("f", "floof"), Description("Summon a random fox picture")]
    public async Task Fox(cc c) {
        RandomFoxJson.FoxData = null;
        var httpClient = new HttpClient();
        var content = await httpClient.GetStringAsync("https://randomfox.ca/floof/");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0");
        RandomFoxJson.GetData(content);

        var foxCount = await RandomFoxHtml.GetFoxCount();
        
        var e = new DiscordEmbedBuilder();
        e.WithTitle($"{RandomFoxJson.GetImageNumber()} / {foxCount}");
        e.WithColor(Colors.HexToColor("AC5F25"));
        e.WithImageUrl(RandomFoxJson.GetImage());
        e.WithFooter($"{BuildInfo.Name} (v{BuildInfo.Version}) • Powered by randomfox.ca");
        httpClient.Dispose();
        await c.RespondAsync(e.Build());
    }
}

public class BasicSlashCommands : ApplicationCommandModule {
    public BasicSlashCommands() => Logger.Loadodule("BasicSlashCommands");
    
    private readonly string _footerText = $"{BuildInfo.Name} (v{BuildInfo.Version}) • {BuildInfo.BuildDate}";
    
    [SlashCommand("ping", "Outputs the bot's latency to discord.")]
    public async Task Ping(InteractionContext  c) => await c.CreateResponseAsync($":ping_pong: Pong > {c.Client.Ping}ms");

    [SlashCommand("stats", "Shows the bot status including server status and bot stats")]
    public async Task Stats(InteractionContext c) {
        var ram = GC.GetTotalMemory(false) / 1024 / 1024;
        //var cpu = "x";
        var platform = "Windows";
        var discordNetVer = BuildInfo.DSharpVer;
        var mintApiVer = BuildInfo.MintAPIVer;
        var tempNow = DateTime.Now;
        var days = tempNow.Subtract(BuildInfo.StartTime).Days;
        var hours = tempNow.Subtract(BuildInfo.StartTime).Hours;
        var minutes = tempNow.Subtract(BuildInfo.StartTime).Minutes;
        var seconds = tempNow.Subtract(BuildInfo.StartTime).Seconds;

        var e = new DiscordEmbedBuilder();
        e.WithTitle($"{BuildInfo.Name} Stats");
        e.WithColor(DiscordColor.Teal);

        e.AddField("Number of Commands", $"{Program.Commands?.RegisteredCommands.Count + Program.Slash?.RegisteredCommands.Count}", true);
        e.AddField("Ping", $"{c.Client.Ping}ms", true);
        e.AddField("Usage", $"Currently using **{ram}MB** of RAM\nRunning on **{platform}**", true);
        e.AddField("Current Uptime", $"{days} Days : {hours} Hours : {minutes} Minutes : {seconds} Seconds");
        e.AddField("Bot Versions Info", $"DSharpPlus: **v{discordNetVer}** \nBot: **v{BuildInfo.Version}** \nMintAPI: **v{mintApiVer}** \nBuild Date: **{BuildInfo.BuildDateShort}**");
        //e.AddField("Server Info", $"Location: **Finland** \nServer: **Hetzner** \nMax RAM: **4 GB** \nOS: **{platform} 11 (Debian GNU)**");
        e.AddField("Server Info", $"Location: **South Carolina, USA** \nServer: **[Sypher](https://mintlily.lgbt/pc)** \nMax RAM: **32 GB** \nOS: **{platform} 10 (21H1)**");
        
        e.WithTimestamp(DateTime.Now);
        e.WithFooter(_footerText);
        await c.CreateResponseAsync(e.Build());
    }
}