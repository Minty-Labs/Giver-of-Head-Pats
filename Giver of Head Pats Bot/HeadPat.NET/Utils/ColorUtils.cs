using System.Drawing;
using System.Globalization;

namespace HeadPats.Utils; 

public static class Colors {
    public static string RandomColorHex => ColorToHex(Random);
    public static readonly Discord.Color WarnYellow = new(252, 185, 0);
    public static readonly Discord.Color Purplism = new(38, 0, 99);
    public static readonly Discord.Color Yellow = new(255, 255, 0);
    
    private static Discord.Color ConvertColor(Color color) {
        byte.TryParse(color.R.ToString(), out var r);
        byte.TryParse(color.G.ToString(), out var g);
        byte.TryParse(color.B.ToString(), out var b);
        return new Discord.Color(r, g, b);
    }
    
    public static Discord.Color Random {
        get {
            Random rnd = new();
            return ConvertColor(Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255)));
        }
    }
    
    public static Discord.Color HexToColor(string hexColor) {
        if (hexColor.IndexOf('#') != -1)
            hexColor = hexColor.Replace("#", "");
        var num1 = int.Parse(hexColor[..2], NumberStyles.AllowHexSpecifier) / (double)byte.MaxValue;
        var num2 = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier) / (float)byte.MaxValue;
        var num3 = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier) / (float)byte.MaxValue;
        
        return new Discord.Color((float)num1, num2, num3);
    }
    
    public static string ColorToHex(Color baseColor, bool addHash = false) {
        var str = Convert.ToInt32(baseColor.R * byte.MaxValue).ToString("X2") +
                  Convert.ToInt32(baseColor.G * byte.MaxValue).ToString("X2") +
                  Convert.ToInt32(baseColor.B * byte.MaxValue).ToString("X2");
        return addHash ? "#" + str : str;
    }

    public static Discord.Color GetRandomCookieColor() {
        var hexList = new List<string> {
            "B9A685", "825540", "EFC36B", "DBA875", "DFC2A1", "D19E68", "9D7B57", "A65D3F", "ECA599", "372022", "B8413D"
        };
        return HexToColor(hexList[new Random().Next(hexList.Count)]);
    }
}