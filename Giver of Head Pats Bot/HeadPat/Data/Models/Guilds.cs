using System.ComponentModel.DataAnnotations;

namespace HeadPats.Data.Models; 

public class Guilds {
    [Key] public ulong GuildId { get; set; }
    public int PatCount { get; set; }
}