using DSharpPlus;
using Serilog;

namespace HeadPats.Modules;

public class EventModule {
    protected virtual string EventName { get; set; } = "MODULE NAME";
    protected virtual string Description { get; set; } = "MODULE DESCRIPTION";

    internal EventModule() {
        if (EventName == "MODULE NAME" || Description == "MODULE DESCRIPTION") return;
        Log.Information("Setting up {EventName} Event Handler :: {EventDescription}", EventName, Description);
    }
    
    public virtual void Initialize(DiscordClient client) { }
}