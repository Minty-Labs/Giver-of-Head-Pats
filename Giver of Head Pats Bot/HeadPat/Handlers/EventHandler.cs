using DSharpPlus;
using HeadPats.Handlers.Events;
using Serilog;

namespace HeadPats.Handlers;

internal class EventHandler {
    private bool _complete;
    public EventHandler(DiscordClient c) {
        Log.Debug("Setting up Event Handler . . .");
        
        var mc = new MessageCreated(c);
        var jl = new OnBotJoinOrLeave(c);
        var bl = new BangerEventListener(c);
        _complete = true;
    }

    public void Complete() {
        if (_complete)
            Log.Information("Event Handler setup complete.");
    }
}