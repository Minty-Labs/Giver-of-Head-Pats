using ColorHelper;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HeadPats.Utils;
using ColorConverter = ColorHelper.ColorConverter;

namespace HeadPats.Commands.Slash.Basic; 

public class Color : ApplicationCommandModule {
    private const string BaseUrl = "https://c.devminer.xyz/256/256";

    [SlashCommand("Color", "Shows you the color from the given input"), Cooldown(50, 3600, CooldownBucketType.Guild)]
    public async Task GiveColor(InteractionContext c,
        [Choice("HEX", "hex")]
        [Choice("RGB", "rgb")]
        [Choice("CMYK", "cmyk")]
        [Choice("HSV", "hsv")]
        [Choice("HSL", "hsl")]
        [Choice("Random", "random")]
        [Option("Type", "Choose between a type of color")] string type,
        
        [Option("Value", "Color values, non-hex separate with spaces or commas")] string values = "") {
        if (string.IsNullOrWhiteSpace(values) && type != "random") {
            await c.CreateResponseAsync("You must in a color value\nExamples: `fd3ac1` or `148, 78, 36` or `48 128 71` etc.", true);
            return;
        }
        var embed = new DiscordEmbedBuilder();
        
        var tempColorValue = values.ToLower().ReplaceAll("abcdefghijklmnopqrstuvwxyz()#!%°*^");
        var hasCommas = tempColorValue.Contains(',');
        var newColorValue = tempColorValue.Split(hasCommas ? ',' : ' ');

        switch (type) {
            case "rgb": 
                var r = float.Parse(newColorValue[0]);
                var g = float.Parse(newColorValue[1]);
                var b = float.Parse(newColorValue[2]);
                embed.WithColor(new DiscordColor(r / 255, g / 255, b / 255)); // DiscordColor is picky and wants decimal values *pukes in math*
                var rgbToHex = ColorConverter.RgbToHex(new RGB((byte)r, (byte)g, (byte)b));
                embed.WithImageUrl($"{BaseUrl}/rgb/{r}/{g}/{b}");
                embed.WithDescription($"#{rgbToHex}");
                embed.WithTitle($"rgb({r}, {g}, {b})");
                break;
            case "hex": 
                var newHex = values.Replace("#", "").ToLower();
                embed.WithColor(Colors.HexToColor(newHex));
                embed.WithImageUrl($"{BaseUrl}/hex/{newHex}");
                embed.WithTitle($"#{newHex}");
                break;
            case "cmyk":
                var _c = float.Parse(newColorValue[0]);
                var m = float.Parse(newColorValue[1]);
                var y = float.Parse(newColorValue[2]);
                var k = float.Parse(newColorValue[3]);
                var cmykToHex = ColorConverter.CmykToHex(new CMYK((byte)_c, (byte)m, (byte)y, (byte)k));
                embed.WithColor(Colors.HexToColor(cmykToHex.ToString()));
                embed.WithImageUrl($"{BaseUrl}/hex/{cmykToHex}");
                embed.WithDescription($"#{cmykToHex}");
                embed.WithTitle($"cmyk({_c}, {m}, {y}, {k})");
                break;
            case "hsv":
                var h = float.Parse(newColorValue[0]);
                var s = float.Parse(newColorValue[1]);
                var v = float.Parse(newColorValue[2]);
                var hsvToHex = ColorConverter.HsvToHex(new HSV((int)h, (byte)s, (byte)v));
                embed.WithColor(Colors.HexToColor(hsvToHex.ToString()));
                embed.WithImageUrl($"{BaseUrl}/hex/{hsvToHex}");
                embed.WithDescription($"#{hsvToHex}");
                embed.WithTitle($"hsv({h}, {s}, {v})");
                break;
            case "hsl":
                var _h = float.Parse(newColorValue[0]);
                var _s = float.Parse(newColorValue[1]);
                var l = float.Parse(newColorValue[2]);
                var hslToHex = ColorConverter.HslToHex(new HSL((int)_h, (byte)_s, (byte)l));
                embed.WithColor(Colors.HexToColor(hslToHex.ToString()));
                embed.WithImageUrl($"{BaseUrl}/hex/{hslToHex}");
                embed.WithDescription($"#{hslToHex}");
                embed.WithTitle($"hsl({_h}, {_s}, {l})");
                break;
            case "random":
                var randomR = new Random().Next(new RgbRandomColorFilter().minR, new RgbRandomColorFilter().maxR);
                var randomG = new Random().Next(new RgbRandomColorFilter().minG, new RgbRandomColorFilter().maxG);
                var randomB = new Random().Next(new RgbRandomColorFilter().minB, new RgbRandomColorFilter().maxB);
                var randomHex = ColorConverter.RgbToHex(new RGB((byte)randomR, (byte)randomG, (byte)randomB));
                embed.WithColor(Colors.HexToColor(randomHex.ToString()));
                embed.WithImageUrl($"{BaseUrl}/hex/{randomHex}");
                embed.WithDescription($"HEX: #{randomHex}\n" +
                                   $"RGB: {randomR} {randomG} {randomB}\n" +
                                   $"CMYK: {ColorConverter.RgbToCmyk(new RGB((byte)randomR, (byte)randomG, (byte)randomB))}\n" +
                                   $"HSV: {ColorConverter.RgbToHsv(new RGB((byte)randomR, (byte)randomG, (byte)randomB))}\n" +
                                   $"HSL: {ColorConverter.RgbToHsl(new RGB((byte)randomR, (byte)randomG, (byte)randomB))}");
                embed.WithTitle("Your RANDOM Color");
                break;
        }
        
        embed.WithFooter("Powered by devminer.xyz + iamartyom");
        await c.CreateResponseAsync(embed.Build());
    }

    [SlashCommand("Gradient", "Shows you the gradient from the given input"), Cooldown(50, 3600, CooldownBucketType.Guild)]
    public async Task GiveGradient(InteractionContext c,
        [Choice("HEX", "hex")]
        [Choice("RGB", "rgb")]
        [Choice("CMYK", "cmyk")]
        [Choice("HSV", "hsv")]
        [Choice("HSL", "hsl")]
        [Choice("Random", "random")]
        [Option("Type", "Choose between a type of color")] string type,
        
        [Option("ValueL", "Color values, non-hex separate with spaces or commas")] string valuesL = "",
        [Option("ValueR", "must be same format as previous value")] string valuesR = "") {
        if (((!string.IsNullOrWhiteSpace(valuesL) && string.IsNullOrWhiteSpace(valuesR)) || (string.IsNullOrWhiteSpace(valuesL) && !string.IsNullOrWhiteSpace(valuesR))) && type != "random") {
            await c.CreateResponseAsync("You must in a color value\nExamples: `fd3ac1` or `148, 78, 36` or `48 128 71` etc.", true);
            return;
        }

        var embed = new DiscordEmbedBuilder();

        var tempColorValueL = valuesL.ToLower().ReplaceAll("abcdefghijklmnopqrstuvwxyz()#!%°*^");
        var hasCommasL = tempColorValueL.Contains(',');
        var newColorValueL = tempColorValueL.Split(hasCommasL ? ',' : ' ');
        
        var tempColorValueR = valuesR.ToLower().ReplaceAll("abcdefghijklmnopqrstuvwxyz()#!%°*^");
        var hasCommasR = tempColorValueR.Contains(',');
        var newColorValueR = tempColorValueR.Split(hasCommasR ? ',' : ' ');

        switch (type) {
            case "rgb":
                var rL = float.Parse(newColorValueL[0]);
                var gL = float.Parse(newColorValueL[1]);
                var bL = float.Parse(newColorValueL[2]);
                var rR = float.Parse(newColorValueR[0]);
                var gR = float.Parse(newColorValueR[1]);
                var bR = float.Parse(newColorValueR[2]);
                embed.WithImageUrl($"{BaseUrl}/{rL}/{gL}/{bL}/{rR}/{gR}/{bR}");
                embed.WithTitle($"rgb({rL}, {gL}, {bL}) - rgb({rR}, {gR}, {bR})");
                break;
            case "hex":
                var newHexL = valuesL.Replace("#", "").ToLower();
                var newHexR = valuesR.Replace("#", "").ToLower();
                var leftAsRgb = ColorConverter.HexToRgb(new HEX(newHexL));
                var rightAsRgb = ColorConverter.HexToRgb(new HEX(newHexR));
                embed.WithImageUrl($"{BaseUrl}/{leftAsRgb.R}/{leftAsRgb.G}/{leftAsRgb.B}/{rightAsRgb.R}/{rightAsRgb.G}/{rightAsRgb.B}");
                embed.WithTitle($"#{newHexL} - #{newHexR}");
                break;
            case "cmyk":
                var cL = float.Parse(newColorValueL[0]);
                var mL = float.Parse(newColorValueL[1]);
                var yL = float.Parse(newColorValueL[2]);
                var kL = float.Parse(newColorValueL[3]);
                var cR = float.Parse(newColorValueR[0]);
                var mR = float.Parse(newColorValueR[1]);
                var yR = float.Parse(newColorValueR[2]);
                var kR = float.Parse(newColorValueR[3]);
                var leftAsRgbCmyk = ColorConverter.CmykToRgb(new CMYK((byte)cL, (byte)mL, (byte)yL, (byte)kL));
                var rightAsRgbCmyk = ColorConverter.CmykToRgb(new CMYK((byte)cR, (byte)mR, (byte)yR, (byte)kR));
                embed.WithImageUrl($"{BaseUrl}/{leftAsRgbCmyk.R}/{leftAsRgbCmyk.G}/{leftAsRgbCmyk.B}/{rightAsRgbCmyk.R}/{rightAsRgbCmyk.G}/{rightAsRgbCmyk.B}");
                embed.WithTitle($"cmyk({cL}, {mL}, {yL}, {kL}) - cmyk({cR}, {mR}, {yR}, {kR})");
                break;
            case "hsv":
                var hL = float.Parse(newColorValueL[0]);
                var sL = float.Parse(newColorValueL[1]);
                var vL = float.Parse(newColorValueL[2]);
                var hR = float.Parse(newColorValueR[0]);
                var sR = float.Parse(newColorValueR[1]);
                var vR = float.Parse(newColorValueR[2]);
                var leftAsRgbHsv = ColorConverter.HsvToRgb(new HSV((byte)hL, (byte)sL, (byte)vL));
                var rightAsRgbHsv = ColorConverter.HsvToRgb(new HSV((byte)hR, (byte)sR, (byte)vR));
                embed.WithImageUrl($"{BaseUrl}/{leftAsRgbHsv.R}/{leftAsRgbHsv.G}/{leftAsRgbHsv.B}/{rightAsRgbHsv.R}/{rightAsRgbHsv.G}/{rightAsRgbHsv.B}");
                embed.WithTitle($"hsv({hL}, {sL}, {vL}) - hsv({hR}, {sR}, {vR})");
                break;
            case "hsl":
                var _hL = float.Parse(newColorValueL[0]);
                var _sL = float.Parse(newColorValueL[1]);
                var _lL = float.Parse(newColorValueL[2]);
                var _hR = float.Parse(newColorValueR[0]);
                var _sR = float.Parse(newColorValueR[1]);
                var _lR = float.Parse(newColorValueR[2]);
                var leftAsRgbHsl = ColorConverter.HslToRgb(new HSL((byte)_hL, (byte)_sL, (byte)_lL));
                var rightAsRgbHsl = ColorConverter.HslToRgb(new HSL((byte)_hR, (byte)_sR, (byte)_lR));
                embed.WithImageUrl($"{BaseUrl}/{leftAsRgbHsl.R}/{leftAsRgbHsl.G}/{leftAsRgbHsl.B}/{rightAsRgbHsl.R}/{rightAsRgbHsl.G}/{rightAsRgbHsl.B}");
                embed.WithTitle($"hsl({_hL}, {_sL}, {_lL}) - hsl({_hR}, {_sR}, {_lR})");
                break;
            case "random":
                var randomLr = new Random().Next(new RgbRandomColorFilter().minR, new RgbRandomColorFilter().maxR);
                var randomLg = new Random().Next(new RgbRandomColorFilter().minG, new RgbRandomColorFilter().maxG);
                var randomLb = new Random().Next(new RgbRandomColorFilter().minB, new RgbRandomColorFilter().maxB);
                var randomRr = new Random().Next(new RgbRandomColorFilter().minR, new RgbRandomColorFilter().maxR);
                var randomRg = new Random().Next(new RgbRandomColorFilter().minG, new RgbRandomColorFilter().maxG);
                var randomRb = new Random().Next(new RgbRandomColorFilter().minB, new RgbRandomColorFilter().maxB);
                embed.WithImageUrl($"{BaseUrl}/{randomLr}/{randomLg}/{randomLb}/{randomRr}/{randomRg}/{randomRb}");
                embed.WithTitle("Your RANDOM Color");
                embed.WithDescription($"HEX L: #{ColorConverter.RgbToHex(new RGB((byte)randomLr, (byte)randomLg, (byte)randomLb))} - HEX R: #{ColorConverter.RgbToHex(new RGB((byte)randomRr, (byte)randomRg, (byte)randomRb))}\n" +
                                      $"RGB L: {randomLr} {randomLg} {randomLb} - RGB R: {randomRr} {randomRg} {randomRb}\n" +
                                      $"CMYK L: {ColorConverter.RgbToCmyk(new RGB((byte)randomLr, (byte)randomLg, (byte)randomLb))} - CMYK R: {ColorConverter.RgbToCmyk(new RGB((byte)randomRr, (byte)randomRg, (byte)randomRb))}\n" +
                                      $"HSV L: {ColorConverter.RgbToHsv(new RGB((byte)randomLr, (byte)randomLg, (byte)randomLb))} - CMYK R: {ColorConverter.RgbToHsv(new RGB((byte)randomRr, (byte)randomRg, (byte)randomRb))}\n" +
                                      $"HSL L: {ColorConverter.RgbToHsl(new RGB((byte)randomLr, (byte)randomLg, (byte)randomLb))} - CMYK R: {ColorConverter.RgbToHsl(new RGB((byte)randomRr, (byte)randomRg, (byte)randomRb))}");
                break;
        }
        
        embed.WithFooter("Powered by devminer.xyz + iamartyom");
        await c.CreateResponseAsync(embed.Build());
    }
}