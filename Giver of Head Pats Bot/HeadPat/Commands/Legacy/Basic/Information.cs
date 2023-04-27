using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HeadPats.Utils;

namespace HeadPats.Commands.Legacy.Basic; 

public class Information : BaseCommandModule {
    
    [Command("ping"), Description("Shows bot's latency from you <-> discord <-> you.")]
    public async Task Ping(CommandContext c) => await c.RespondAsync($":ping_pong: Pong > {c.Client.Ping}ms");
    
    [Command("stats"), Description("Shows the bot status including server status and bot stats")]
    public async Task Stats(CommandContext c) {
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
        e.AddField("APIs", "Unsplash\nCookie\nFluxpoint");

        e.WithTimestamp(DateTime.Now);
        await c.RespondAsync(e.Build());
    }
}