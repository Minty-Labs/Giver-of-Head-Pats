using DSharpPlus.SlashCommands;

namespace HeadPats.Commands.ContextMenu; 

public static class ContextMenuHandler {
    public static void Register(SlashCommandsExtension c) {
        c.RegisterCommands<User.Love>();
        Logger.Log("[Commands.ContextMenu] Love Registered");
    }
}