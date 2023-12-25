using Discord.WebSocket;
using Serilog;

namespace HeadPats.Modules; 

public class BasicModule {
    protected virtual string ModuleName { get; set; } = "MODULE NAME";
    protected virtual string ModuleDescription { get; set; } = "MODULE DESCRIPTION";

    internal BasicModule() {
        if (ModuleName == "MODULE NAME" || ModuleDescription == "MODULE DESCRIPTION") return;
        var logger = Log.ForContext("SourceContext", "MODULE");
        logger.Information("Setting up the {Name} Module :: {Description}", ModuleName, ModuleDescription);
    }
    
    public virtual void Initialize() { }
    public virtual void InitializeClient(DiscordSocketClient client) { }
}