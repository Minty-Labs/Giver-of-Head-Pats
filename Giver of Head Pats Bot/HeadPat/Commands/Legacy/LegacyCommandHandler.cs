using DSharpPlus.CommandsNext;
using HeadPats.Commands.Legacy.Basic;
using HeadPats.Commands.Legacy.NSFW;
using HeadPats.Commands.Legacy.Owner;

namespace HeadPats.Commands.Legacy; 

public static class LegacyCommandHandler {
    public static void Register(CommandsNextExtension c) {
        c.RegisterCommands<HeadPats.Commands.Legacy.Admin.Admin>();
        Logger.Log("[Commands.Legacy] Admin Registered");
        
        c.RegisterCommands<Information>();
        Logger.Log("[Commands.Legacy] Information Registered");
        c.RegisterCommands<Salad>();
        Logger.Log("[Commands.Legacy] Salad Registered");
        c.RegisterCommands<FunnyCommands>();
        Logger.Log("[Commands.Legacy] NSFW Registered");
        
        c.RegisterCommands<BlacklistControl>();
        Logger.Log("[Commands.Legacy.Owner] BlacklistControl Registered");
        c.RegisterCommands<BotControl>();
        Logger.Log("[Commands.Legacy.Owner] BotControl Registered");
        c.RegisterCommands<ConfigControl>();
        Logger.Log("[Commands.Legacy.Owner] ConfigControl Registered");
        c.RegisterCommands<UserControl>();
        Logger.Log("[Commands.Legacy.Owner] UserControl Registered");
    }
}