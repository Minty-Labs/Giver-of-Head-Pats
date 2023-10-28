using DSharpPlus.CommandsNext;
using HeadPats.Commands.Legacy.Basic;
// using HeadPats.Commands.Legacy.NSFW;
using HeadPats.Commands.Legacy.Owner;
using Serilog;

namespace HeadPats.Commands.Legacy; 

public static class LegacyCommandHandler {
    public static void RegisterLegacyCommands(CommandsNextExtension c) {
        c.RegisterCommands<HeadPats.Commands.Legacy.Admin.Admin>();
        Log.Information("[Commands.Legacy] Admin Registered");
        
        c.RegisterCommands<Information>();
        Log.Information("[Commands.Legacy] Information Registered");
        // c.RegisterCommands<Salad>();
        // Log.Information("[Commands.Legacy] Salad Registered");
        // c.RegisterCommands<FunnyCommands>();
        // Log.Information("[Commands.Legacy] NSFW Registered");
        
        c.RegisterCommands<Commission.Banger.BangerAdmin>();
        Log.Information("[Commands.Legacy.Commission.Banger] BangerAdmin Registered");
        c.RegisterCommands<Commission.Banger.BangerAnyone>();
        Log.Information("[Commands.Legacy.Commission.Banger] BangerAnyone Registered");
        
        c.RegisterCommands<Commission.PersonalizedMembers.PersonalizationAdmin>();
        Log.Information("[Commands.Legacy.Commission.PersonalizedMembers] PersonalizationAdmin Registered");
        c.RegisterCommands<Commission.PersonalizedMembers.PersonalizationAnyone>();
        Log.Information("[Commands.Legacy.Commission.PersonalizedMembers] PersonalizationAnyone Registered");
        
        c.RegisterCommands<BlacklistControl>();
        Log.Information("[Commands.Legacy.Owner] BlacklistControl Registered");
        c.RegisterCommands<BotControl>();
        Log.Information("[Commands.Legacy.Owner] BotControl Registered");
        c.RegisterCommands<ConfigControl>();
        Log.Information("[Commands.Legacy.Owner] ConfigControl Registered");
        c.RegisterCommands<UserControl>();
        Log.Information("[Commands.Legacy.Owner] UserControl Registered");
    }
}