using HarmonyLib;
using Pastel;

namespace HeadPats.Managers;

public class MobileManager { // Thanks Dubya
    public static void CreateMobilePatch() {
        try {
            Logger.Log("Attempting to patch Client as mobile");
            var harmony = new Harmony("mobilePatch"); 
            var mOriginal = AppDomain.CurrentDomain.GetAssemblies()
                .Single(assembly => assembly.GetName().Name == "DSharpPlus")
                .GetTypes().Single(x => x.Name == "ClientProperties")
                .GetProperty("Browser")
                ?.GetGetMethod();
            var mPostfix = new HarmonyMethod(AccessTools.Method(typeof(MobileManager), nameof(MobilePatch)));
            harmony.Patch(mOriginal, postfix: mPostfix);
            Logger.Log("MobilePatch Success: You are on " + "\"Discord iOS\"".Pastel("00ff00"));
        }
        catch (Exception e) {
            Logger.Error("Failed Mobile Patch" + e);
        }
    }
        
    public static void MobilePatch(ref string __result) => __result = "Discord iOS";
}