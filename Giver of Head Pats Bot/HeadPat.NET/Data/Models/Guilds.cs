using System.ComponentModel.DataAnnotations;

namespace HeadPats.Data.Models; 

public class Guilds {
    [Key] public ulong GuildId { get; set; }
    public string Name { get; set; }
    public long DataDeletionTime { get; set; }
    public long PatCount { get; set; }
    public ulong DailyPatChannelId { get; set; }
}