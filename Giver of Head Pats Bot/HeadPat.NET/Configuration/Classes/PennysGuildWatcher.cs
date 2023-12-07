namespace HeadPats.Configuration.Classes; 

public class PennysGuildWatcher {
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
    public long LastUpdateTime { get; set; }
}