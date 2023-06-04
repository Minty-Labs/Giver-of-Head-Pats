using DSharpPlus.Entities;
using HeadPats.Configuration;
using HeadPats.Utils;

namespace HeadPats.Managers.Loops; 

public static class IrlQuotesLoop {
    public static void SendQuote(long currentEpoch) {
        var updated = false;
        var httpClient = new HttpClient();
        var quoteList = httpClient.GetStringAsync("https://raw.githubusercontent.com/Minty-Labs/Giver-of-Head-Pats/main/Media/IRLQuotes.txt").GetAwaiter().GetResult();
        var quotes = quoteList.Split("\n");
        var quote = quotes[new Random().Next(quotes.Length)];

        foreach (var guildParam in Config.Base.GuildSettings!) {
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