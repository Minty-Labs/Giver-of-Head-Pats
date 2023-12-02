using Discord;
using HarmonyLib;
using Serilog;

namespace HeadPats.Managers;

public static class MobileManager { // Thanks DubyaDude
    public static void Initialize() {
        try {
            Log.Debug("Attempting to patch Client as mobile");
            var harmony = new Harmony("mobilePatch"); 
            var mOriginal = AppDomain.CurrentDomain.GetAssemblies()
                .Single(assembly => assembly.GetName().Name == "Discord.Net.WebSocket")
                .GetTypes().Single(x => x.Name == "DiscordSocketApiClient")
                .GetMethods().Single(m => m.Name == "SendGatewayAsync");
            var mPrefix = new HarmonyMethod(AccessTools.Method(typeof(MobileManager), nameof(PatchPreSendGatewayAsync)));
            harmony.Patch(mOriginal, prefix: mPrefix);
        }
        catch (Exception e) {
            Log.Error("Failed Mobile Patch\n{0}", e);
        }
    }

    public static void PatchPreSendGatewayAsync(ref object __0, ref object __1, ref RequestOptions __2) {
        if ((byte)__0 != 2) return;
        var type = __1.GetType();
        var property = type.GetProperty("Properties");
        if (property == null) {
            Log.Error("Failed to get Properties :: Failed Mobile Patch");
            return;
        }
        var properties = (IDictionary<string, string>)property.GetValue(__1)!;
        properties!["$browser"] = "Discord iOS";
        property.SetValue(__1, properties);
    }
}