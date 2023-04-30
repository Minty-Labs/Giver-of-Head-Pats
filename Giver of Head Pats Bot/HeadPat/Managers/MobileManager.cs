using HarmonyLib;
using Serilog;

namespace HeadPats.Managers;

public class MobileManager { // Thanks Dubya
    public static void CreateMobilePatch() {
        try {
            Log.Debug("Attempting to patch Client as mobile");
            var harmony = new Harmony("mobilePatch"); 
            var mOriginal = AppDomain.CurrentDomain.GetAssemblies()
                .Single(assembly => assembly.GetName().Name == "DSharpPlus")
                .GetTypes().Single(x => x.Name == "ClientProperties")
                .GetProperty("Browser")
                ?.GetGetMethod();
            var mPostfix = new HarmonyMethod(AccessTools.Method(typeof(MobileManager), nameof(MobilePatch)));
            harmony.Patch(mOriginal, postfix: mPostfix);
            Log.Debug("MobilePatch Success: You are on \"" + "Discord iOS" + "\"");
        }
        catch (Exception e) {
            Log.Error("Failed Mobile Patch\n{0}", e);
        }
    }
        
    public static void MobilePatch(ref string __result) => __result = "Discord iOS";
}