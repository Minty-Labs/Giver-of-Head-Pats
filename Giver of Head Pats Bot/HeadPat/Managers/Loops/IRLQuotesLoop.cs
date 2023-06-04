using DSharpPlus.Entities;
using HeadPats.Configuration;
using HeadPats.Utils;
using Serilog;

namespace HeadPats.Managers.Loops; 

public static class IrlQuotesLoop {
    public static void SendQuote(long currentEpoch, Random random) {
        var updated = false;
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", Vars.FakeUserAgent);
        var quoteList = httpClient.GetStringAsync("https://raw.githubusercontent.com/Minty-Labs/Giver-of-Head-Pats/main/Media/IRLQuotes.txt").GetAwaiter().GetResult();
        var quotes = quoteList.Split('\n');
        var quote = quotes[random.Next(0, quotes.Length)];

        foreach (var guildParam in Config.Base.GuildSettings!) {
            if (guildParam.IrlQuotes is null) continue;
            if (!guildParam.IrlQuotes.Enabled) continue;
            if (guildParam.IrlQuotes.ChannelId is 0) continue;
            if (guildParam.IrlQuotes.SetEpochTime > currentEpoch)
                continue;
            
            var guild = Program.Client!.GetGuildAsync(guildParam.GuildId).GetAwaiter().GetResult();
            var channel = guild!.GetChannel(guildParam.IrlQuotes.ChannelId);

            var embed = new DiscordEmbedBuilder {
                Description = quote,
                Color = Colors.Random
            }.Build();
            
            channel!.SendMessageAsync(embed).GetAwaiter().GetResult();
            
            guildParam.IrlQuotes.SetEpochTime += 86400;
            updated = true;
        }
        
        if (updated) {
            Config.Save();
        }
    }
}