using System.Drawing;
using DSharpPlus.Entities;
using System.Globalization;

namespace HeadPats.Utils; 

public class Colors {
    public static DiscordColor Random {
        get {
            Random rnd = new();
            return ConvertColor(Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)));
        }
    }
    public static readonly DiscordColor WarnYellow = new(252, 185, 0);
    public static readonly DiscordColor Purplism = new(38, 0, 99);
    
    private static DiscordColor ConvertColor(Color color) {
        byte.TryParse(color.R.ToString(), out var r);
        byte.TryParse(color.G.ToString(), out var g);
        byte.TryParse(color.B.ToString(), out var b);
        return new DiscordColor(r, g, b);
    }
    
    public static DiscordColor HexToColor(string hexColor) {
        if (hexColor.IndexOf('#') != -1)
            hexColor = hexColor.Replace("#", "");
        var num1 = int.Parse(hexColor[..2], NumberStyles.AllowHexSpecifier) / (double)byte.MaxValue;
        var num2 = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier) / (float)byte.MaxValue;
        var num3 = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier) / (float)byte.MaxValue;
        
        return new DiscordColor((float)num1, num2, num3);
    }
    
    public static string ColorToHex(Color baseColor, bool hash = false) {
        var str = Convert.ToInt32(baseColor.R * byte.MaxValue).ToString("X2") +
                     Convert.ToInt32(baseColor.G * byte.MaxValue).ToString("X2") +
                     Convert.ToInt32(baseColor.B * byte.MaxValue).ToString("X2");
        if (hash) str = "#" + str;
        return str;
    }

    public static DiscordColor GetRandomCookieColor() {
        var hexList = new List<string> {
            "B9A685", "825540", "EFC36B", "DBA875", "DFC2A1", "D19E68", "9D7B57", "A65D3F", "ECA599", "372022", "B8413D"
        };
        return HexToColor(hexList[new Random().Next(hexList.Count)]);
    }
}