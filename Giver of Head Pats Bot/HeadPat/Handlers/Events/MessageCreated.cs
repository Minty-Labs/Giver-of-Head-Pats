using DSharpPlus;
using DSharpPlus.EventArgs;
using HeadPats.Data;

namespace HeadPats.Handlers.Events; 

public class MessageCreated {
    public MessageCreated(DiscordClient c) {
        Logger.Log("Setting up MessageCreated Event Handler . . .");
        
        c.MessageCreated += GetAndMaybeRespondToTrigger;
    }

    private async Task GetAndMaybeRespondToTrigger(DiscordClient sender, MessageCreateEventArgs e) {
        var contents = e.Message.Content;

        if (ReplyStructure.Base.Replies != null) {
            foreach (var t in ReplyStructure.Base.Replies.Where(t => t.Trigger != null)) {
                if (t.GuildId != e.Guild.Id) continue;
                
                if (contents.Equals(t.Trigger) && t.OnlyTrigger) 
                    await sender.SendMessageAsync(e.Channel, t.Response);
                
                else if (t.Trigger != null && contents.Contains(t.Trigger) && !t.OnlyTrigger) 
                    await sender.SendMessageAsync(e.Channel, t.Response);
            }
        }
    }
}