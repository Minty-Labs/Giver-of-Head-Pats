using DSharpPlus;
using HeadPats.Handlers.Events;

namespace HeadPats.Handlers;

internal class EventHandler {
    private bool _complete;
    public EventHandler(DiscordClient c) {
        Logger.Log("Setting up Event Handler . . .");
        
        var mc = new MessageCreated(c);
        var jl = new OnBotJoinOrLeave(c);
        _complete = true;
    }

    public void Complete() {
        if (_complete)
            Logger.Log("Event Handler setup complete.");
    }
}