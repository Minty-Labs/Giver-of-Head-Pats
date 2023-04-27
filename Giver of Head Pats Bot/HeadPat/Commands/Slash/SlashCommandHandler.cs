using DSharpPlus.SlashCommands;
using HeadPats.Commands.Slash.Basic;
using HeadPats.Commands.Slash.Reply;
using HeadPats.Commands.Slash.UserLove;

namespace HeadPats.Commands.Slash; 

public static class SlashCommandHandler {
    public static void Register(SlashCommandsExtension s) {
        s.RegisterCommands<HeadPats.Commands.Slash.Admin.Admin>();
        Logger.Log("[Commands.Slash] Admin Registered");

        s.RegisterCommands<Color>();
        Logger.Log("[Commands.Slash] Color Registered");
        s.RegisterCommands<Summon>();
        Logger.Log("[Commands.Slash] Summon Registered");
        s.RegisterCommands<VeryBasic>();
        Logger.Log("[Commands.Slash] VeryBasic Registered");
        s.RegisterCommands<Contributors.Contributors>();
        Logger.Log("[Commands.Slash] Contributors Registered");
        s.RegisterCommands<ReplyApplication>();
        Logger.Log("[Commands.Slash] ReplyApplication Registered");
        s.RegisterCommands<Leaderboards>();
        Logger.Log("[Commands.Slash] Leaderboards Registered");
        s.RegisterCommands<LoveCommands>();
        Logger.Log("[Commands.Slash] Love Registered");
    }
}