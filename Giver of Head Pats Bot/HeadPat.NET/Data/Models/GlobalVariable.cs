using System.ComponentModel.DataAnnotations;

namespace HeadPats.Data.Models;

public class GlobalVariable {
    [Key] public string Name { get; set; }
    public string Value { get; set; }
    public ulong ApplicationId { get; set; }
}