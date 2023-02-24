using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using HeadPats.Commands;
// using HeadPats.Commands.Modules;

namespace HeadPats.Managers;
internal static class Commands {
    public static void Register(CommandsNextExtension? c) {
        c?.RegisterCommands<Basic>();
        
        // c?.RegisterCommands<MelonLoaderBlacklist.ProtectCommands>();
    }

    public static void Register(SlashCommandsExtension? s) {
        if (s == null) Logger.Error("Slash commands extension is null");

// #if !DEBUG
        s?.RegisterCommands<SlashOwner>();
        Logger.Log("[Owner] Owner Slash commands registered");
        s?.RegisterCommands<SlashConfigCommands>();
        Logger.Log("[Owner] Config Slash commands registered");
        s?.RegisterCommands<BlacklistCommands>();
        Logger.Log("[Owner] Blacklist Slash commands registered");
        
        s?.RegisterCommands<LoveSlash>();
        Logger.Log("Love Slash commands registered");
        s?.RegisterCommands<Utility_Random>();
        Logger.Log("Random / Utility Slash commands registered");
        s?.RegisterCommands<ReplyApplication>();
        Logger.Log("Reply Slash commands registered");
        s?.RegisterCommands<SlashBasic>();
        Logger.Log("Basic Slash commands registered");
        s?.RegisterCommands<Contributors>();
        Logger.Log("Contributor Slash commands registered");
        s?.RegisterCommands<Admin>();
        Logger.Log("Admin Slash commands registered");
// #endif

        // s?.RegisterCommands<Moderation>();
        // Logger.Log("Moderation Slash commands registered");
    }
}
