using System.ComponentModel.DataAnnotations;

namespace HeadPats.Data.Models; 

public class Overlord {
    [Key] public ulong ApplicationId { get; set; }
    public int PatCount { get; set; }
}