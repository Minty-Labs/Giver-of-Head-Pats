using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HeadPats.Utils;
using cc = DSharpPlus.CommandsNext.CommandContext;
using ic = DSharpPlus.SlashCommands.InteractionContext;

namespace HeadPats.Commands; 

public class Random_Utility : BaseCommandModule {
    public Random_Utility() => Logger.Loadodule("Random / Utility");

    [Command("Color"), Description("Shows you the color from the given input")]
    public async Task GiveColor(cc c,
        [Description("Color type, RGB or or HEX")] string type,
        [RemainingText, Description("Color value (no #, no rgb() | rgb separated by spaces or commas)")] string values = "") {
        if (string.IsNullOrWhiteSpace(type)) {
            await c.RespondAsync("Please specify a color type (`rgb` or `hex`)");
            return;
        }
        if (string.IsNullOrWhiteSpace(values)) {
            await c.RespondAsync("Please input a color value\nExamples: `fd3ac1` or `148, 78, 36` or `48 128 71`");
            return;
        }

        const string baseUrl = "https://c.devminer.xyz/512/512";
        var em = new DiscordEmbedBuilder();

        if (type.ToLower().Contains("rgb")) {
            var tempRgb = values.ToLower().ReplaceAll("abcdefghijklmnopqrstuvwxyz()#!", "");
            var hasCommas = tempRgb.Contains(',');
            var newRgb = tempRgb.Split(hasCommas ? ',' : ' ');
            var r = float.Parse(newRgb[0]);
            var g = float.Parse(newRgb[1]);
            var b = float.Parse(newRgb[2]);
            em.WithColor(new DiscordColor(r / 255, g / 255, b / 255)); // DiscordColor is picky and wants decimal values *pukes in math*
            em.WithImageUrl($"{baseUrl}/rgb/{r}/{g}/{b}");
            em.WithTitle($"rgb({r}, {g}, {b})");
        }
        else {
            var newHex = values.Replace("#", "").ToLower();
            em.WithColor(Colors.HexToColor(newHex));
            em.WithImageUrl($"{baseUrl}/hex/{newHex}");
            em.WithTitle($"#{newHex}");
        }
        
        em.WithFooter("Powered by devminer.xyz");
        await c.RespondAsync(em.Build());
    }
}