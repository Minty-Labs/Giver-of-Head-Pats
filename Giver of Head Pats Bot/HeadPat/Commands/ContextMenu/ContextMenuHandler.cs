using DSharpPlus.SlashCommands;
using Serilog;

namespace HeadPats.Commands.ContextMenu; 

public static class ContextMenuHandler {
    public static void Register(SlashCommandsExtension c) {
        c.RegisterCommands<User.Love>();
        Log.Information("[Commands.ContextMenu] Love Registered");
    }
}