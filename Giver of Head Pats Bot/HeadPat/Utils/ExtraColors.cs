using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using DSharpPlus;
using DSharpPlus.Entities;
using System.Net;
using System.Globalization;
using System.IO;

namespace HeadPats.Utils; 

public class Colors {
    public static DiscordColor RANDOM {
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
        var num1 = int.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier) / (double)byte.MaxValue;
        var num2 = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier) / (float)byte.MaxValue;
        var num3 = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier) / (float)byte.MaxValue;
        double num4 = num2;
        double num5 = num3;
        return new DiscordColor((float)num1, (float)num4, (float)num5);
    }
    
    public static string ColorToHex(Color baseColor, bool hash = false) {
        var str = Convert.ToInt32(baseColor.R * byte.MaxValue).ToString("X2") +
                     Convert.ToInt32(baseColor.G * byte.MaxValue).ToString("X2") +
                     Convert.ToInt32(baseColor.B * byte.MaxValue).ToString("X2");
        if (hash) str = "#" + str;
        return str;
    }
    /*
    public static Color GetNearestColor(string bitMapURL) {
        Color inputColor = GetMostUsedColor(bitMapURL);
        var inputRed = Convert.ToDouble(inputColor.R);
        var inputGreen = Convert.ToDouble(inputColor.G);
        var inputBlue = Convert.ToDouble(inputColor.B);
        var colors = new List<Color>();
        foreach (var knownColor in Enum.GetValues(typeof(KnownColor))) {
            var color = Color.FromKnownColor((KnownColor)knownColor);
            if (!color.IsSystemColor)
                colors.Add(color);
        }
        var nearestColor = Color.Empty;
        var distance = 500.0;
        foreach (var color in colors) {
            // Compute Euclidean distance between the two colors
            var testRed = Math.Pow(Convert.ToDouble(color.R) - inputRed, 2.0);
            var testGreen = Math.Pow(Convert.ToDouble(color.G) - inputGreen, 2.0);
            var testBlue = Math.Pow(Convert.ToDouble(color.B) - inputBlue, 2.0);
            var tempDistance = Math.Sqrt(testBlue + testGreen + testRed);
            if (tempDistance == 0.0)
                return color;
            if (tempDistance < distance) {
                distance = tempDistance;
                nearestColor = color;
            }
        }
        return nearestColor;
    }

        private static Color GetMostUsedColor(string bitMapURL) {
            Bitmap bitMap;
            var httpClient = new HttpClient();
            var bytes = httpClient.GetByteArrayAsync(bitMapURL).GetAwaiter().GetResult();
            Stream s = new MemoryStream(bytes);
            //Stream s = wc.OpenRead(bitMapURL);
            Bitmap bmp = new(s);
            bitMap = bmp;
            httpClient.Dispose();

            var colorIncidence = new Dictionary<int, int>();
            for (var x = 0; x < bitMap.Size.Width; x++)
                for (var y = 0; y < bitMap.Size.Height; y++) {
                    var pixelColor = bitMap.GetPixel(x, y).ToArgb();
                    if (colorIncidence.Keys.Contains(pixelColor))
                        colorIncidence[pixelColor]++;
                    else
                        colorIncidence.Add(pixelColor, 1);
                }
            return Color.FromArgb(colorIncidence.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value).First().Key);
        }
        */
}