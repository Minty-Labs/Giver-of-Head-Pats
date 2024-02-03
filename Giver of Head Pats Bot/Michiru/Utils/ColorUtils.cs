using System.Drawing;
using System.Globalization;

namespace Michiru.Utils; 

public static class Colors {
    public static Discord.Color Random {
        get {
            Random rnd = new();
            return ConvertColor(Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255)));
        }
    }
    
    public static readonly Discord.Color MichiruPink = new(187, 107, 121);
    
    private static Discord.Color ConvertColor(Color color) {
        byte.TryParse(color.R.ToString(), out var r);
        byte.TryParse(color.G.ToString(), out var g);
        byte.TryParse(color.B.ToString(), out var b);
        return new Discord.Color(r, g, b);
    }
    
    public static Discord.Color HexToColor(string hexColor) {
        if (hexColor.Contains('#'))
            hexColor = hexColor.Replace("#", "");
        var num1 = int.Parse(hexColor[..2], NumberStyles.AllowHexSpecifier) / (double)byte.MaxValue;
        var num2 = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier) / (float)byte.MaxValue;
        var num3 = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier) / (float)byte.MaxValue;
        
        return new Discord.Color((float)num1, num2, num3);
    }
    
    public static string ColorToHex(Color baseColor, bool hash = false) {
        var str = Convert.ToInt32(baseColor.R * byte.MaxValue).ToString("X2") +
                  Convert.ToInt32(baseColor.G * byte.MaxValue).ToString("X2") +
                  Convert.ToInt32(baseColor.B * byte.MaxValue).ToString("X2");
        if (hash) str = "#" + str;
        return str;
    }

    public static Discord.Color GetRandomCookieColor() {
        var hexList = new List<string> {
            "B9A685", "825540", "EFC36B", "DBA875", "DFC2A1", "D19E68", "9D7B57", "A65D3F", "ECA599", "372022", "B8413D"
        };
        return HexToColor(hexList[new Random().Next(hexList.Count)]);
    }
}