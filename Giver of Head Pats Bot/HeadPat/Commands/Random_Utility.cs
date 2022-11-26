﻿using ColorHelper;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HeadPats.Utils;
using cc = DSharpPlus.CommandsNext.CommandContext;
using ic = DSharpPlus.SlashCommands.InteractionContext;

namespace HeadPats.Commands; 

public class Random_Utility : BaseCommandModule {
    public Random_Utility() => Logger.Loadodule("Random / Utility");

    [Command("Color"), Description("Shows you the color from the given input")]
    public async Task GiveColor(cc c,
        [Description("Color type, RGB, HEX, CMYK, HSV, HSL")] string type,
        [RemainingText, Description("Color value (no #, no rgb() | non-HEX separated by spaces or commas)")] string values = "") {
        if (string.IsNullOrWhiteSpace(type)) {
            await c.RespondAsync("Please specify a color type\nTypes: `RGB, HEX, CMYK, HSV, HSL`");
            return;
        }
        if (string.IsNullOrWhiteSpace(values)) {
            await c.RespondAsync("Please input a color value\nExamples: `fd3ac1` or `148, 78, 36` or `48 128 71` etc.");
            return;
        }

        const string baseUrl = "https://c.devminer.xyz/256/256";
        var em = new DiscordEmbedBuilder();

        var tempColorValue = values.ToLower().ReplaceAll("abcdefghijklmnopqrstuvwxyz()#!%°", "");
        var hasCommas = tempColorValue.Contains(',');
        var newColorValue = tempColorValue.Split(hasCommas ? ',' : ' ');

        switch (type) {
            case "rgb": 
                var r = float.Parse(newColorValue[0]);
                var g = float.Parse(newColorValue[1]);
                var b = float.Parse(newColorValue[2]);
                em.WithColor(new DiscordColor(r / 255, g / 255, b / 255)); // DiscordColor is picky and wants decimal values *pukes in math*
                var rgbToHex = ColorConverter.RgbToHex(new RGB((byte)r, (byte)g, (byte)b));
                em.WithImageUrl($"{baseUrl}/rgb/{r}/{g}/{b}");
                em.WithDescription($"#{rgbToHex}");
                em.WithTitle($"rgb({r}, {g}, {b})");
                break;
            case "hex": 
                var newHex = values.Replace("#", "").ToLower();
                em.WithColor(Colors.HexToColor(newHex));
                em.WithImageUrl($"{baseUrl}/hex/{newHex}");
                em.WithTitle($"#{newHex}");
                break;
            case "cmyk":
                var _c = float.Parse(newColorValue[0]);
                var m = float.Parse(newColorValue[1]);
                var y = float.Parse(newColorValue[2]);
                var k = float.Parse(newColorValue[3]);
                var cmykToHex = ColorConverter.CmykToHex(new CMYK((byte)_c, (byte)m, (byte)y, (byte)k));
                em.WithColor(Colors.HexToColor(cmykToHex.ToString()));
                em.WithImageUrl($"{baseUrl}/hex/{cmykToHex}");
                em.WithDescription($"#{cmykToHex}");
                em.WithTitle($"cmyk({_c}, {m}, {y}, {k})");
                break;
            case "hsv":
                var h = float.Parse(newColorValue[0]);
                var s = float.Parse(newColorValue[1]);
                var v = float.Parse(newColorValue[2]);
                var hsvToHex = ColorConverter.HsvToHex(new HSV((int)h, (byte)s, (byte)v));
                em.WithColor(Colors.HexToColor(hsvToHex.ToString()));
                em.WithImageUrl($"{baseUrl}/hex/{hsvToHex}");
                em.WithDescription($"#{hsvToHex}");
                em.WithTitle($"hsv({h}, {s}, {v})");
                break;
            case "hsl":
                var _h = float.Parse(newColorValue[0]);
                var _s = float.Parse(newColorValue[1]);
                var l = float.Parse(newColorValue[2]);
                var hslToHex = ColorConverter.HslToHex(new HSL((int)_h, (byte)_s, (byte)l));
                em.WithColor(Colors.HexToColor(hslToHex.ToString()));
                em.WithImageUrl($"{baseUrl}/hex/{hslToHex}");
                em.WithDescription($"#{hslToHex}");
                em.WithTitle($"hsl({_h}, {_s}, {l})");
                break;
        }
        
        em.WithFooter("Powered by devminer.xyz");
        await c.RespondAsync(em.Build());
    }
}

public class Utility_Random : ApplicationCommandModule {

    [SlashCommand("Color", "Shows you the color from the given input")]
    public async Task GiveColor(ic c,
        [Choice("HEX", "hex")]
        [Choice("RGB", "rgb")]
        [Choice("CMYK", "cmyk")]
        [Choice("HSV", "hsv")]
        [Choice("HSL", "hsl")]
        [Option("Type", "Choose between a type of color")] string type,
        
        [Option("ColorValue", "Color values non-hex separate with spaces or commas")] string values = "") {
        if (string.IsNullOrWhiteSpace(values)) {
            await c.CreateResponseAsync("You must in a color value\nExamples: `fd3ac1` or `148, 78, 36` or `48 128 71` etc.", true);
            return;
        }
        
        const string baseUrl = "https://c.devminer.xyz/256/256";
        var em = new DiscordEmbedBuilder();
        
        var tempColorValue = values.ToLower().ReplaceAll("abcdefghijklmnopqrstuvwxyz()#!%°", "");
        var hasCommas = tempColorValue.Contains(',');
        var newColorValue = tempColorValue.Split(hasCommas ? ',' : ' ');

        switch (type) {
            case "rgb": 
                var r = float.Parse(newColorValue[0]);
                var g = float.Parse(newColorValue[1]);
                var b = float.Parse(newColorValue[2]);
                em.WithColor(new DiscordColor(r / 255, g / 255, b / 255)); // DiscordColor is picky and wants decimal values *pukes in math*
                var rgbToHex = ColorConverter.RgbToHex(new RGB((byte)r, (byte)g, (byte)b));
                em.WithImageUrl($"{baseUrl}/rgb/{r}/{g}/{b}");
                em.WithDescription($"#{rgbToHex}");
                em.WithTitle($"rgb({r}, {g}, {b})");
                break;
            case "hex": 
                var newHex = values.Replace("#", "").ToLower();
                em.WithColor(Colors.HexToColor(newHex));
                em.WithImageUrl($"{baseUrl}/hex/{newHex}");
                em.WithTitle($"#{newHex}");
                break;
            case "cmyk":
                var _c = float.Parse(newColorValue[0]);
                var m = float.Parse(newColorValue[1]);
                var y = float.Parse(newColorValue[2]);
                var k = float.Parse(newColorValue[3]);
                var cmykToHex = ColorConverter.CmykToHex(new CMYK((byte)_c, (byte)m, (byte)y, (byte)k));
                em.WithColor(Colors.HexToColor(cmykToHex.ToString()));
                em.WithImageUrl($"{baseUrl}/hex/{cmykToHex}");
                em.WithDescription($"#{cmykToHex}");
                em.WithTitle($"cmyk({_c}, {m}, {y}, {k})");
                break;
            case "hsv":
                var h = float.Parse(newColorValue[0]);
                var s = float.Parse(newColorValue[1]);
                var v = float.Parse(newColorValue[2]);
                var hsvToHex = ColorConverter.HsvToHex(new HSV((int)h, (byte)s, (byte)v));
                em.WithColor(Colors.HexToColor(hsvToHex.ToString()));
                em.WithImageUrl($"{baseUrl}/hex/{hsvToHex}");
                em.WithDescription($"#{hsvToHex}");
                em.WithTitle($"hsv({h}, {s}, {v})");
                break;
            case "hsl":
                var _h = float.Parse(newColorValue[0]);
                var _s = float.Parse(newColorValue[1]);
                var l = float.Parse(newColorValue[2]);
                var hslToHex = ColorConverter.HslToHex(new HSL((int)_h, (byte)_s, (byte)l));
                em.WithColor(Colors.HexToColor(hslToHex.ToString()));
                em.WithImageUrl($"{baseUrl}/hex/{hslToHex}");
                em.WithDescription($"#{hslToHex}");
                em.WithTitle($"hsl({_h}, {_s}, {l})");
                break;
        }
        
        em.WithFooter("Powered by devminer.xyz");
        await c.CreateResponseAsync(em.Build());
    }
}




























