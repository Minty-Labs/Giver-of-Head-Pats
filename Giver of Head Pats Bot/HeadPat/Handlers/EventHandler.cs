using DSharpPlus;
using HeadPats.Handlers.Events;

namespace HeadPats.Handlers;

internal class EventHandler {
    public EventHandler(DiscordClient c) {
        Logger.Log("Setting up Event Handler . . .");
        
        var mc = new MessageCreated(c);
        var jl = new OnBotJoinOrLeave(c);
    }
}