using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeadPats.Data.Models;

public class DailyPats { 
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public ulong UserId { get; set; }
    public ulong GuildId { get; set; }
    public long SetEpochTime { get; set; }
}