/*using System.Text;
using Discord;
using Discord.WebSocket;
using HeadPats.Configuration;
using HeadPats.Modules;
using HeadPats.Utils;

namespace HeadPats.Events; 

public class MessageReceived : EventModule {
    protected override string EventName => "MessageReceived";
    protected override string Description => "Handles the Bot DMs & Message Trigger events.";
    
    public override void Initialize(DiscordSocketClient client) {
        // client.MessageReceived += GetAndMaybeRespondToTrigger;
        client.MessageReceived += GetUserBotDm;
        client.MessageReceived += RespondToDmFromChannel;
    }

    public override void OnSessionCreated() => Program.Instance.DmCategory = Program.Instance.GetCategory(Vars.SupportServerId, Config.Base.DmCategory);

    private static async Task GetUserBotDm(SocketMessage e) {
        if (e.Channel.GetChannelType() != ChannelType.DM) return;
        if (e.Author.IsBot) return;
        var message = e.Content;
        // format: userId-userName

        if (message.StartsWith('.'))
            return;
        
        var author = e.Author;
        ITextChannel? serverChannelFromDm;
        var supportGuild = Program.Instance.GetGuild(Vars.SupportServerId);
        var dmCategory = Program.Instance.DmCategory;

        if (dmCategory.GetChannelType() != ChannelType.Category) return;

        if (!dmCategory!.Channels.Any(c => c.Name.Contains(author.Id.ToString()))) {
            serverChannelFromDm = await supportGuild!.CreateTextChannelAsync(
                $"{author.Id}-{author.Username.ReplaceAll("[ǃ@#$%^`~&*()+=,./<>?;:'\"\\|{}]")}",
                delegate(TextChannelProperties properties) {
                    properties.Topic = $"DM from: {author.Username} ({author.Id})";
                    properties.CategoryId = dmCategory.Id;
                }, new RequestOptions {
                    AuditLogReason = $"DM from: {author.Username} ({author.Id})"
                });
        }
        else 
            serverChannelFromDm = (ITextChannel?)dmCategory.Channels.Single(c => c.Name.Contains(author.Id.ToString()));

        var count = e.Attachments.Count;
        var att = count != 0;
        var attsb = new StringBuilder();
        if (att) {
            attsb.AppendLine("attachment(s): ");
            foreach (var a in e.Attachments) {
                attsb.AppendLine($"{a.ProxyUrl}");
            }
        }
        await serverChannelFromDm!.SendMessageAsync($"{(att ? $"Has `{count}` {attsb}\n" : "")}\n{message}");
    }

    private static async Task RespondToDmFromChannel(SocketMessage e) {
        if (e.Channel.GetChannelType() == ChannelType.DM) return;
        if (e.Author.IsBot) return;
        ulong originalAuthorId;
        try {
            originalAuthorId = ulong.Parse(e.Channel.Name.Split('-')[0]);
        }
        catch {
            return; // stop if failed to parse
        }
        if (originalAuthorId.ToString().Length != 18) return;
        if (!e.Channel.Name.Contains(originalAuthorId.ToString())) return;
        
        var user = e.Author;
        var channel = await user.CreateDMChannelAsync();
        try {
            if (!string.IsNullOrWhiteSpace(e.Content)) await channel.SendMessageAsync(e.Content);
        }
        catch {
            await e.Channel.SendMessageAsync("I cannot send messages to this user. They have DMs closed.");
        }
    }

    // private static async Task GetAndMaybeRespondToTrigger(SocketMessage e) {
    //     var contents = e.Content;
    //     if (e.Author.IsBot) return;
    //
    //     var guildSettings = Config.GuildSettings(e.Id);
    //     
    //     if (guildSettings is null) return;
    //     
    //     foreach (var t in guildSettings.Replies!.Where(t => t.Trigger != null)) {
    //         if (e.Channel.GetChannelType() == ChannelType.DM) return;
    //
    //         // if (t.Trigger!.ToLower().Equals("salad") && e.Content.Contains("hp!salad") && e.Id == 805663181170802719)
    //         //     break; // for the Minty Labs salad command
    //
    //         if (contents.Equals(t.Trigger) && t.OnlyTrigger)
    //             await e.Channel.SendMessageAsync(t.Response?.Replace("<br>", "\n"));
    //             
    //         else if (t.Trigger != null && contents.Contains(t.Trigger) && !t.OnlyTrigger) 
    //             await e.Channel.SendMessageAsync(t.Response?.Replace("<br>", "\n"));
    //             
    //         else if (t.Trigger != null && contents.Equals(t.Trigger) && !t.OnlyTrigger && t.DeleteTriggerIfIsOnlyInMessage) {
    //             await e.DeleteAsync(new RequestOptions {AuditLogReason = "Auto delete by bot response."});
    //             await e.Channel.SendMessageAsync(t.Response?.Replace("<br>", "\n"));
    //         }
    //
    //         if (contents.Equals(t.Trigger?.ToLower()) && t.DeleteTrigger)
    //             await e.DeleteAsync(new RequestOptions {AuditLogReason = "Auto delete by bot response."});
    //     }
    // }
}*/