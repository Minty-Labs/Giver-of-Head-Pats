using System.Text.Json.Serialization;
using HeadPats.Commands.Slash.Contributors;
namespace HeadPats.Configuration.Classes; 

public class BotContributor {
    public string? UserName { get; set; }
    public string? Info { get; set; }
}