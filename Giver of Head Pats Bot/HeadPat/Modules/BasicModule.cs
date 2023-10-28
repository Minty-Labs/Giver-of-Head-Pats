using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using Serilog;

namespace HeadPats.Modules; 

public class BasicModule {
    protected virtual string ModuleName { get; set; } = "MODULE NAME";
    protected virtual string ModuleDescription { get; set; } = "MODULE DESCRIPTION";

    internal BasicModule() {
        if (ModuleName == "MODULE NAME" || ModuleDescription == "MODULE DESCRIPTION") return;
        Log.Information("Setting up the {Name} Module :: {Description}", ModuleName, ModuleDescription);
    }
    
    public virtual void Initialize() { }
    public virtual void InitializeClient(DiscordClient client) { }
    // public virtual void RegisterLegacyCommands(CommandsNextExtension commandsNextExtension) { }
    // public virtual void RegisterSlashCommands(SlashCommandsExtension slashCommandsExtension) { }
}