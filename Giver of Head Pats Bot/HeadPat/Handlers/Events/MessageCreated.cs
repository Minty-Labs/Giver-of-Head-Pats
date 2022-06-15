using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using HeadPats.Data;
using HeadPats.Utils;

namespace HeadPats.Handlers.Events; 

public class MessageCreated {
    public MessageCreated(DiscordClient c) {
        Logger.Log("Setting up MessageCreated Event Handler . . .");
        
        c.MessageCreated += GetAndMaybeRespondToTrigger;
        c.MessageCreated += GetUserBotDm;
        c.MessageCreated += RespondToDmFromChannel;
    }

    internal static DiscordChannel? DmCategory;

    private static async Task GetUserBotDm(DiscordClient sender, MessageCreateEventArgs e) {
        if (!e.Channel.IsPrivate) return;
        if (e.Author.IsBot) return;
        var builder = new DiscordMessageBuilder();
        // format: userId-userName
        var author = e.Message.Author;
        DiscordChannel? serverChannelFromDm;
        var supportGuild = await sender.GetGuildAsync(BuildInfo.Config.SupportGuildId);

        if (!DmCategory!.IsCategory) return;

        if (!DmCategory.Children.Any(c => c.Name.Contains(author.Id.ToString()))) {
            serverChannelFromDm = await supportGuild.CreateChannelAsync($"{author.Id}-{author.Username.ReplaceAll("[ǃ@#$%^`~&*()+=,./<>?;:'\"\\|{}]", "")}",
                ChannelType.Text, DmCategory, $"DM from: {author.Username}#{author.Discriminator} ({author.Id})");
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
        builder.WithContent($"From {author.Username}#{author.Discriminator} ({author.Id}):\n{(att ? $"Has `{count}` {attsb.ToString()}\n" : "")}\n" +
                            $"{e.Message.Content}");

        /*if (att) {
            if (e.Message.Content.Contains($"From {author.Username}#{author.Discriminator}")) return;
            await supportGuild.GetChannel(serverChannelFromDm.Id).SendMessageAsync(builder);
        }
        else {
            if (e.Message.Content.Contains($"From {author.Username}#{author.Discriminator}")) return;
            await builder.SendAsync(serverChannelFromDm);
        }*/
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
            foreach (var g in sender.Guilds.Values) {
                foreach (var u in await g.GetAllMembersAsync()) {
                    if (u.Id != originalAuthorId) continue;
                    member = u;
                }
            }
        }
        catch {
            await e.Channel.SendMessageAsync("I cannot send a message to this user, I do not share a guild with them.");
            return;
        }
        
        var channel = await member!.CreateDmChannelAsync();

        try {
            await sender.SendMessageAsync(channel, m?.Content);
        }
        catch {
            // Respond if DMs are closed
            await sender.SendMessageAsync(e.Channel, "I cannot send messages to this user. They have DMs closed.");
        }
    }

    private static async Task GetAndMaybeRespondToTrigger(DiscordClient sender, MessageCreateEventArgs e) {
        var contents = e.Message.Content;
        if (e.Author.IsBot) return;

        if (ReplyStructure.Base.Replies != null) {
            foreach (var t in ReplyStructure.Base.Replies.Where(t => t.Trigger != null)) {
                if (e.Channel.IsPrivate) return;
                if (t.GuildId != e.Guild.Id) continue;
                
                if (contents.Equals(t.Trigger) && t.OnlyTrigger) 
                    await sender.SendMessageAsync(e.Channel, t.Response?.Replace("<br>", "\n"));
                
                else if (t.Trigger != null && contents.Contains(t.Trigger) && !t.OnlyTrigger) 
                    await sender.SendMessageAsync(e.Channel, t.Response?.Replace("<br>", "\n"));
            }
        }
    }
}