using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HeadPats.Data;
using HeadPats.Managers;
using HeadPats.Utils;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NekosSharp;
using cc = DSharpPlus.CommandsNext.CommandContext;
using ic = DSharpPlus.SlashCommands.InteractionContext;

namespace HeadPats.Commands; 

public class Basic : BaseCommandModule {
    public Basic() => Logger.Loadodule("BasicCommands");
    
    private string FooterText(string extra = "")
        => $"{BuildInfo.Name} (v{BuildInfo.Version}) • {BuildInfo.BuildDate}{(string.IsNullOrWhiteSpace(extra) ? "" : $" • {extra}")}";
    
    [Command("ping"), Description("Shows bot's latency from you <-> discord <-> you.")]
    public async Task Ping(cc c) => await c.RespondAsync($":ping_pong: Pong > {c.Client.Ping}ms");
    
    [Command("stats"), Description("Shows the bot status including server status and bot stats")]
    public async Task Stats(cc c) {
        var ram = GC.GetTotalMemory(false) / 1024 / 1024;
        //var cpu = "x";
        const string platform = "Windows";
        const string discordNetVer = BuildInfo.DSharpVer;
        const string mintApiVer = BuildInfo.MintApiVer;
        var tempNow = DateTime.Now;
        var days = tempNow.Subtract(BuildInfo.StartTime).Days;
        var hours = tempNow.Subtract(BuildInfo.StartTime).Hours;
        var minutes = tempNow.Subtract(BuildInfo.StartTime).Minutes;
        var seconds = tempNow.Subtract(BuildInfo.StartTime).Seconds;

        var e = new DiscordEmbedBuilder();
        e.WithTitle($"{BuildInfo.Name} Stats");
        e.WithColor(DiscordColor.Teal);

        e.AddField("Number of Commands", $"{Program.Commands?.RegisteredCommands.Count + Program.Slash?.RegisteredCommands.Count + 1}", true);
        e.AddField("Ping", $"{c.Client.Ping}ms", true);
        e.AddField("Usage", $"Currently using **{ram}MB** of RAM\nRunning on **{platform}**", true);
        e.AddField("Current Uptime", $"{days} Days : {hours} Hours : {minutes} Minutes : {seconds} Seconds");
        e.AddField("Bot Versions Info", $"DSharpPlus: **v{discordNetVer}** \nBot: **v{BuildInfo.Version}** \nMintAPI: **v{mintApiVer}** \nBuild Date: **{BuildInfo.BuildDateShort}**");
        //e.AddField("Server Info", $"Location: **Finland** \nServer: **Hetzner** \nMax RAM: **4 GB** \nOS: **{platform} 11 (Debian GNU)**");
        e.AddField("Server Info", $"Location: **South Carolina, USA** \nServer: **[Sypher](https://mintlily.lgbt/pc)** \nMax RAM: **32 GB** \nOS: **{platform} 10 (21H1)**");
        
        e.WithTimestamp(DateTime.Now);
        e.WithFooter(FooterText());
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
        e.WithFooter(FooterText("Powered by randomfox.ca"));
        httpClient.Dispose();
        await c.RespondAsync(e.Build());
    }

    private async Task OutputBaseCommand(cc c, string embedTitle, string? imageUrl, string colorHex) {
        var e = new DiscordEmbedBuilder();
        e.WithTitle(embedTitle);
        e.WithImageUrl(imageUrl);
        e.WithColor(Colors.HexToColor(colorHex));
        e.WithFooter(FooterText("Powered by nekos.life"));
        await c.Client.SendMessageAsync(c.Message.Channel, e.Build());
    }

    [Command("Neko"), Description("Summon a picture or GIF of a SFW neko")]
    public async Task Neko(cc c) {
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 1);
        var neko = num1 == 1 ? Program.NekoClient?.Image.Neko() : Program.NekoClient?.Image.NekoGif();

        await OutputBaseCommand(c, "Random Neko", neko?.Result.ImageUrl, "42F4A1");
    }
    
    [Command("Smug"), Description("Summon a picture or GIF of a smug face")]
    public async Task Smug(cc c) {
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 1);
        var neko = num1 == 1 ? Program.NekoClient?.Image.Smug() : Program.NekoClient?.Image_v3.SmugGif();

        await OutputBaseCommand(c, "", neko?.Result.ImageUrl, "804A13");
    }
    
    [Command("Cat"), Description("Summon a picture or GIF of a cat")]
    public async Task Cat(cc c) {
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 1);
        var neko = num1 == 1 ? Program.NekoClient?.Misc.Cat() : Program.NekoClient?.Misc_v3.Cat();

        await OutputBaseCommand(c, "", neko?.Result.ImageUrl, "FFFF00");
    }
    
    [Command("Cry"), Aliases("crying"), Description("Summon a picture or GIF of a crying post")]
    public async Task Cry(cc c) {
        NekoLoveJson.NekoData = null;
        var httpClient = new HttpClient();
        var content = await httpClient.GetStringAsync("https://neko-love.xyz/api/v1/cry");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0");
        NekoLoveJson.GetData(content);

        if (NekoLoveJson.ImageHasValidExtension()) {
            var e = new DiscordEmbedBuilder();
            e.WithTitle("`*`_cry_`*`");
            e.WithColor(Colors.HexToColor("58CBCF"));
            e.WithImageUrl(NekoLoveJson.GetImage());
            e.WithFooter(FooterText("Powered by neko-love.xyz"));
            httpClient.Dispose();
            await c.RespondAsync(e.Build());
        }
    }

    [Command("TopPat"), Aliases("lb", "leaderboard", "tp"), Description("Shows the leaderbord for most headpats")]
    public async Task TopPat(cc c) {
        await using var db = new Context();
        var usersList = db.Users.AsQueryable().ToList();
        var guildList = db.Guilds.AsQueryable().ToList();
        var serverList = db.Overall.AsQueryable().ToList();
        
        var guildPats = 0;
        try { guildPats = guildList.FirstOrDefault(g => g.GuildId == c.Guild.Id)!.PatCount; } catch { Logger.Error("Guilds DataSet is Empty"); }
        
        var serverPats = 0;
        try { serverPats = serverList.First().PatCount; } catch { Logger.Error("Server DataSet is Empty"); }
        
        var max = 1;
        var sb = new StringBuilder();
        var newUserList = usersList.OrderBy(p => -p.PatCount);
        foreach (var u in newUserList) {
            if (max >= 10) continue;
            if (!c.Guild.Members.Keys.Contains(u.UserId)) continue;
            sb.AppendLine($"`{max}.` {u.UsernameWithNumber} - Total Pats: **{u.PatCount}**");
            max++;
        }
        
        var e = new DiscordEmbedBuilder();
        e.WithTitle("Head Pat Leaderboard");
        e.WithColor(Colors.HexToColor("DFFFDD"));
        e.WithFooter($"Synced across all servers • {BuildInfo.Name} (v{BuildInfo.Version})");
        e.AddField("Current Server Stats", 
            $"{(string.IsNullOrWhiteSpace(sb.ToString()) ? "Data is Empty" : $"{sb}")}\n\nTotal Server Pats **{guildPats}**");
        e.AddField("Global Stats", $"Total Pats: **{serverPats}**");
        e.WithTimestamp(DateTime.Now);
        await c.RespondAsync(e.Build());
    }
}

public class BasicSlashCommands : ApplicationCommandModule {
    public BasicSlashCommands() => Logger.Loadodule("BasicSlashCommands");
    
    private readonly string _footerText = $"{BuildInfo.Name} (v{BuildInfo.Version}) • {BuildInfo.BuildDate}";
    
    [SlashCommand("ping", "Outputs the bot's latency to discord.")]
    public async Task Ping(ic c) => await c.CreateResponseAsync($":ping_pong: Pong > {c.Client.Ping}ms");

    [SlashCommand("stats", "Shows the bot status including server status and bot stats")]
    public async Task Stats(ic c) {
        var ram = GC.GetTotalMemory(false) / 1024 / 1024;
        //var cpu = "x";
        const string platform = "Windows";
        const string discordNetVer = BuildInfo.DSharpVer;
        const string mintApiVer = BuildInfo.MintApiVer;
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