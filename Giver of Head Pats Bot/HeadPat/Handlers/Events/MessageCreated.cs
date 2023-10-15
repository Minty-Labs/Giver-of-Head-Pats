using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using HeadPats.Configuration;
using HeadPats.Modules;
using HeadPats.Utils;

namespace HeadPats.Handlers.Events; 

public class MessageCreated : EventModule {
    protected override string EventName => "MessageCreated";
    protected override string Description => "Handles the Bot DMs & Message Trigger events.";
    
    public override void Initialize(DiscordClient client) {
        client.MessageCreated += GetAndMaybeRespondToTrigger;
        client.MessageCreated += GetUserBotDm;
        client.MessageCreated += RespondToDmFromChannel;
    }

    internal static DiscordChannel? DmCategory { get; set; }

    public override async Task OnSessionCreatedTask() {
        DmCategory = await Program.Client!.GetChannelAsync(Config.Base.DmCategory);
    }

    private static async Task GetUserBotDm(DiscordClient sender, MessageCreateEventArgs e) {
        if (!e.Channel.IsPrivate) return;
        if (e.Author.IsBot) return;
        var builder = new DiscordMessageBuilder();
        var message = e.Message.Content;
        // format: userId-userName

        if (message.Contains("hp!")) return;
        if (message.StartsWith("."))
            return;
        
        var author = e.Message.Author;
        DiscordChannel? serverChannelFromDm;
        var supportGuild = await sender.GetGuildAsync(Vars.SupportServerId);

        if (!DmCategory!.IsCategory) return;

        if (!DmCategory.Children.Any(c => c.Name.Contains(author.Id.ToString()))) {
            serverChannelFromDm = await supportGuild.CreateChannelAsync($"{author.Id}-{author.Username.ReplaceAll("[ǃ@#$%^`~&*()+=,./<>?;:'\"\\|{}]")}",
                ChannelType.Text, DmCategory, $"DM from: {author.Username} ({author.Id})");
        }
        else 
            serverChannelFromDm = DmCategory.Children.Single(c => c.Name.Contains(author.Id.ToString()));

        var count = e.Message.Attachments.Count;
        var att = count != 0;
        var attsb = new StringBuilder();
        if (att) {
            attsb.AppendLine("attachment(s): ");
            foreach (var a in e.Message.Attachments) {
                attsb.AppendLine($"{a.ProxyUrl}");
            }
        }
        builder.WithContent($"{(att ? $"Has `{count}` {attsb}\n" : "")}\n{message}");
        
        await builder.SendAsync(serverChannelFromDm);
    }

    private static async Task RespondToDmFromChannel(DiscordClient sender, MessageCreateEventArgs e) {
        if (e.Channel.IsPrivate) return;
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
        
        var m = e.Message;
        DiscordMember? member = null;
        try {
            /*foreach (var g in sender.Guilds.Values) {
                foreach (var u in await g.GetAllMembersAsync()) {
                    if (u.Id != originalAuthorId) continue;
                    member = u;
                }
            }*/
            member = await e.Guild.GetMemberAsync(originalAuthorId);
        }
        catch {
            await e.Channel.SendMessageAsync("I cannot send a message to this user, I do not share a guild with them.");
            return;
        }
        
        var channel = await member!.CreateDmChannelAsync();

        try {
            if (m?.Content != null) await sender.SendMessageAsync(channel, m.Content);
        }
        catch {
            // Respond if DMs are closed
            await sender.SendMessageAsync(e.Channel, "I cannot send messages to this user. They have DMs closed.");
        }
    }

    private static async Task GetAndMaybeRespondToTrigger(DiscordClient sender, MessageCreateEventArgs e) {
        var contents = e.Message.Content;
        if (e.Author.IsBot) return;

        var guildSettings = Config.GuildSettings(e.Guild.Id);
        
        if (guildSettings is null)
            return;
        
        foreach (var t in guildSettings.Replies!.Where(t => t.Trigger != null)) {
            if (e.Channel.IsPrivate) return;

            if (t.Trigger!.ToLower().Equals("salad") && e.Message.Content.Contains("hp!salad") && e.Guild.Id == 805663181170802719)
                break; // for the Minty Labs salad command
                
            if (contents.Equals(t.Trigger) && t.OnlyTrigger) 
                await sender.SendMessageAsync(e.Channel, t.Response?.Replace("<br>", "\n"));
                
            else if (t.Trigger != null && contents.Contains(t.Trigger) && !t.OnlyTrigger) 
                await sender.SendMessageAsync(e.Channel, t.Response?.Replace("<br>", "\n"));
                
            else if (t.Trigger != null && contents.Equals(t.Trigger) && !t.OnlyTrigger && t.DeleteTriggerIfIsOnlyInMessage) {
                await e.Message.DeleteAsync("Auto delete by bot response.");
                await sender.SendMessageAsync(e.Channel, t.Response?.Replace("<br>", "\n"));
            }

            if (contents.Equals(t.Trigger?.ToLower()) && t.DeleteTrigger)
                await e.Message.DeleteAsync("Auto delete by bot response.");
        }
    }
}