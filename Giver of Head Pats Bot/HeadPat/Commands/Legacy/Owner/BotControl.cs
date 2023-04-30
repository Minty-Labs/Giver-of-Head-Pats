using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HeadPats.Managers;

namespace HeadPats.Commands.Legacy.Owner; 

public class BotControl : BaseCommandModule  {

    [Command("ListGuilds"), Description("Lists all the guilds the bot is in"), RequireOwner]
    public async Task ListGuilds(CommandContext c) {
        var guilds = c.Client.Guilds;
        var sb = new StringBuilder();
        sb.AppendLine($"Guild Count: {guilds.Count}");
        foreach (var g in guilds) {
            sb.AppendLine(g.Value.Name);
            sb.AppendLine(g.Key.ToString());
            sb.AppendLine();
        }

        var overLimit = sb.ToString().Length > 2000;
        var f = sb.ToString();

        if (overLimit) {
            using var ms = new MemoryStream();
            await using var sw = new StreamWriter(ms);
            await sw.WriteAsync(f);
            await sw.FlushAsync();
            ms.Seek(0, SeekOrigin.Begin);
            var builder = new DiscordMessageBuilder();
            builder.AddFile("Guilds.txt", ms);
            await c.RespondAsync(builder);
            return;
        }
        await c.RespondAsync(f);
    }
    
    [Command("LeaveGuild"), Description("Leaves a guild"), RequireOwner]
    public async Task LeaveGuild(CommandContext c, [Description("Guild ID to leave from")] string guildId) {
        if (string.IsNullOrWhiteSpace(guildId)) {
            await c.RespondAsync("Please provide a guild ID.").DeleteAfter(3);
            return;
        }

        var id = ulong.Parse(guildId);
        var guild = await c.Client.GetGuildAsync(id);

        await guild.LeaveAsync();
        await c.RespondAsync($"Left the server: {guild.Name}");
    }
    
    [Command("SwapApis"), Description("Swaps the API used for image commands"), RequireOwner]
    public async Task SwapApis(CommandContext c) {
        Vars.UseCookieApi = !Vars.UseCookieApi;
        await c.RespondAsync($"API changed to: {(Vars.UseCookieApi ? "Cookie" : "Fluxpoint")}");
    }
    
}