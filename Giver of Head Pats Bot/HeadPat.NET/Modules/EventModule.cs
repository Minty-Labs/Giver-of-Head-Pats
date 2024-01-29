using Discord.WebSocket;
using Serilog;

namespace HeadPats.Modules;

public class EventModule {
    protected virtual string EventName { get; set; } = "MODULE NAME";
    protected virtual string Description { get; set; } = "MODULE DESCRIPTION";

    internal EventModule() {
        if (EventName == "MODULE NAME" || Description == "MODULE DESCRIPTION") return;
        var logger = Log.ForContext("SourceContext", "EVENT");
        logger.Information("Setting up {EventName} Event Handler :: {EventDescription}", EventName, Description);
    }
    
    public virtual void Initialize(DiscordSocketClient client) { }
    public virtual void OnSessionCreated() { }
    public virtual async Task OnSessionCreatedTask() { }
}