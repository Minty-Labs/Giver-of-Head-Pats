using DSharpPlus;
using DSharpPlus.EventArgs;

namespace HeadPats.Handlers.Events; 

public class OnBotJoinOrLeave {
    public OnBotJoinOrLeave(DiscordClient c) {
        Logger.Log("Setting up OnBotJoinOrLeave Event Handler . . .");
        
        c.GuildCreated += OnJoinGuild;
        c.GuildDeleted += OnLeaveGuild;
    }

    private Task OnLeaveGuild(DiscordClient sender, GuildDeleteEventArgs e) {
        return Task.CompletedTask;
    }

    private Task OnJoinGuild(DiscordClient sender, GuildCreateEventArgs e) {
        return Task.CompletedTask;
    }
}