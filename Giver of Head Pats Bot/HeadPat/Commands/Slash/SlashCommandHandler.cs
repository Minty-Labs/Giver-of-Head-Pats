using DSharpPlus.SlashCommands;
using HeadPats.Commands.Slash.Basic;
using HeadPats.Commands.Slash.Reply;
using HeadPats.Commands.Slash.UserLove;
using Serilog;

namespace HeadPats.Commands.Slash; 

public static class SlashCommandHandler {
    public static void Register(SlashCommandsExtension s) {
        // s.RegisterCommands<HeadPats.Commands.Slash.Admin.Admin>();
        // Log.Information("[Commands.Slash.Admin] Admin Registered");
        s.RegisterCommands<Admin.DailyPatCmds>();
        Log.Information("[Commands.Slash.Admin] DailyPatCmds Registered");
        s.RegisterCommands<Admin.IrlQuoteCmds>();
        Log.Information("[Commands.Slash.Admin] IrlQuoteCmds Registered");

        s.RegisterCommands<Color>();
        Log.Information("[Commands.Slash] Color Registered");
        s.RegisterCommands<Summon>();
        Log.Information("[Commands.Slash] Summon Registered");
        s.RegisterCommands<VeryBasic>();
        Log.Information("[Commands.Slash] VeryBasic Registered");
        s.RegisterCommands<Contributors.Contributors>();
        Log.Information("[Commands.Slash] Contributors Registered");
        s.RegisterCommands<ReplyApplication>();
        Log.Information("[Commands.Slash] ReplyApplication Registered");
        s.RegisterCommands<Leaderboards>();
        Log.Information("[Commands.Slash] Leaderboards Registered");
        s.RegisterCommands<LoveCommands>();
        Log.Information("[Commands.Slash] LoveCommands Registered");
    }
}