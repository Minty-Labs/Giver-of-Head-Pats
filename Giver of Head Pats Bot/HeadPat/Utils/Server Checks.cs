using DSharpPlus.Entities;

namespace HeadPats.Utils; 

public static class ServerChecks {
    public static bool IsChannelNsfw(this DiscordChannel channel) => channel.IsNSFW;
}