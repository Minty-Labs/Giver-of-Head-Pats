using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HeadPats.Data;
using HeadPats.Utils;
using cc = DSharpPlus.CommandsNext.CommandContext;
using ic = DSharpPlus.SlashCommands.InteractionContext;

namespace HeadPats.Commands; 

public class Basic : BaseCommandModule {
    public Basic() => Logger.Loadodule("BasicCommands");

    private void FooterText(DiscordEmbedBuilder em, string extraText = "") {
        em.WithTimestamp(DateTime.Now);
        em.WithFooter($"{(string.IsNullOrWhiteSpace(extraText) ? "" : $"{extraText}")}");
    }

    [Command("About"), Aliases("info"), Description("Shows a message that describes the bot")]
    public async Task About(cc c) {
        var e = new DiscordEmbedBuilder();
        e.WithColor(Colors.HexToColor("00ffaa"));
        e.WithDescription("Hi, I am the **Giver of Head Pats**. I am here to give others head pats, hug, cuddles, and more. I am always expanding in what I can do. " +
                          $"At the moment you can see what I can do by running the `{BuildInfo.Config.Prefix}help` command.\n" +
                          "I was recently rewritten from Javascript to C#. So if things seem broken or missing from the older version, don't worry, they'll be fixed " +
                          "or added in the near future.\nI hope I will be the perfect caregiver for your guild.");
        e.AddField("Bot Creator Information", "Website: https://mintlily.lgbt/ \n" +
                                              "Donate: https://ko-fi.com/MintLily \n" +
                                              "~~Open-Source~~: https://git.ellyvr.dev/Lily/giver-of-head-pats \n" +
                                              "Add to Your Guild: [Invite Link](https://discord.com/api/oauth2/authorize?client_id=489144212911030304&permissions=1240977501264&scope=bot%20applications.commands) \n" +
                                              "Support Guild: [Invite Link](https://discord.gg/98JExhF)");
        FooterText(e);
        e.WithTimestamp(DateTime.Now);
        var u = await c.Client.GetUserAsync(BuildInfo.ClientId, true);
        e.WithThumbnail(u.AvatarUrl);
        e.WithAuthor("MintLily#0001", "https://mintlily.lgbt/", "https://mintlily.lgbt/assets/img/Lily_Art_Headshot_pfp_x1024.png");
        await c.RespondAsync(e.Build());
    }

    [Command("Support"), Description("Sends a DM for a  server invite link to the bot support server")]
    public async Task Support(cc c) {
        await c.Message.DeleteAsync();
        var message = new DiscordMessageBuilder();
        message.WithContent("Need support or have questions? Join the **Giver of Head Pats** Discord support guild:\n  https://discord.gg/98JExhF");
        var member = await c.Guild.GetMemberAsync(c.Message.Author.Id);
        try {
            var dm = await member.CreateDmChannelAsync();
            await dm.SendMessageAsync(message);
        }
        catch (Exception ee) {
            Logger.SendLog($"{c.Message.Author.Username}#{c.Message.Author.Discriminator} in guild: {c.Guild.Name} ({c.Guild.Id})," +
                           $" ran the command {BuildInfo.Config.Prefix}support\nUser probably had DM disabled.\n" +
                           $"```\n{ee}```");
        }
    }

    [Command("Invite"), Aliases("i"), Description("Sends a DM to invite the bot to a server")]
    public async Task Invite(cc c) {
        await c.Message.DeleteAsync();
        var message = new DiscordMessageBuilder();
        message.WithContent("Want to invite me to your guild? Add me here:\n  https://discord.com/api/oauth2/authorize?client_id=489144212911030304&permissions=1240977501264&scope=bot%20applications.commands");
        var member = await c.Guild.GetMemberAsync(c.Message.Author.Id);
        try {
            var dm = await member.CreateDmChannelAsync();
            await dm.SendMessageAsync(message);
        }
        catch (Exception ee) {
            Logger.SendLog($"{c.Message.Author.Username}#{c.Message.Author.Discriminator} in guild: {c.Guild.Name} ({c.Guild.Id})," +
                           $" ran the command {BuildInfo.Config.Prefix}invite\nUser probably had DM disabled.\n" +
                           $"```\n{ee}```");
        }
    }
    
    [Command("ping"), Description("Shows bot's latency from you <-> discord <-> you.")]
    public async Task Ping(cc c) => await c.RespondAsync($":ping_pong: Pong > {c.Client.Ping}ms");
    
    [Command("stats"), Description("Shows the bot status including server status and bot stats")]
    public async Task Stats(cc c) {
        var ram = GC.GetTotalMemory(false) / 1024 / 1024;
        //var cpu = "x";
        const string platform = "Linux";
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
        e.AddField("Server Info", $"Location: **Finland** \nServer: **Hetzner** \nMax RAM: **4 GB** \nOS: **Debian 11**");
        //e.AddField("Server Info", $"Location: **South Carolina, USA** \nServer: **[Sypher](https://mintlily.lgbt/pc)** \nMax RAM: **32 GB** \nOS: **{platform} 10 (21H1)**");

        FooterText(e);
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
        FooterText(e, "Powered by randomfox.ca");
        httpClient.Dispose();
        await c.RespondAsync(e.Build());
    }

    private async Task OutputBaseCommand(cc c, string embedTitle, string? imageUrl, string colorHex) {
        var e = new DiscordEmbedBuilder();
        e.WithTitle(embedTitle);
        e.WithImageUrl(imageUrl);
        e.WithColor(Colors.HexToColor(colorHex));
        FooterText(e, "Powered by nekos.life");
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
            FooterText(e, "Powered by neko-love.xyz");
            httpClient.Dispose();
            await c.RespondAsync(e.Build());
        }
    }

    [Command("TopPat"), Aliases("lb", "leaderboard", "tp"), Description("Shows the leaderboard for most headpats")]
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
        const string platform = "Linux";
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
        e.AddField("Server Info", $"Location: **Finland** \nServer: **Hetzner** \nMax RAM: **4 GB** \nOS: **Debian 11**");
        //e.AddField("Server Info", $"Location: **South Carolina, USA** \nServer: **[Sypher](https://mintlily.lgbt/pc)** \nMax RAM: **32 GB** \nOS: **{platform} 10 (21H1)**");
        
        e.WithTimestamp(DateTime.Now);
        e.WithFooter(_footerText);
        await c.CreateResponseAsync(e.Build());
    }
}