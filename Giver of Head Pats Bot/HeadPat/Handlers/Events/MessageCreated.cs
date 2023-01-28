using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using HeadPats.Data;
// using HeadPats.MelonLoaderBlacklist;
using HeadPats.Utils;
using Newtonsoft.Json;

namespace HeadPats.Handlers.Events; 

public class MessageCreated {
    public MessageCreated(DiscordClient c) {
        Logger.Log("Setting up MessageCreated Event Handler . . .");
        
        c.MessageCreated += GetAndMaybeRespondToTrigger;
        c.MessageCreated += GetUserBotDm;
        c.MessageCreated += RespondToDmFromChannel;
        // c.MessageCreated += LookForResetListCommandResponse;
        // c.MessageCreated += HiddenMinecraftCommand;
    }

    // private static async Task HiddenMinecraftCommand(DiscordClient sender, MessageCreateEventArgs e) {
    //     if (e.Channel.IsPrivate) return;
    //     if (e.Author.IsBot) return;
    //     if (e.Message.Content.ToLower().StartsWith("hp!minecraft")) {
    //         await e.Message.RespondAsync("Did you now the bot creator has a minecraft server?\nIP: `mintlily.lgbt` - Vanilla 1.19.x+");
    //     }
    // }

    internal static DiscordChannel? DmCategory;

    private static async Task GetUserBotDm(DiscordClient sender, MessageCreateEventArgs e) {
        if (!e.Channel.IsPrivate) return;
        if (e.Author.IsBot) return;
        var builder = new DiscordMessageBuilder();
        // format: userId-userName

        if (e.Message.ToString().StartsWith("hp!")) return;
        
        var author = e.Message.Author;
        DiscordChannel? serverChannelFromDm;
        var supportGuild = await sender.GetGuildAsync(BuildInfo.Config.SupportGuildId);

        if (!DmCategory!.IsCategory) return;

        if (!DmCategory.Children.Any(c => c.Name.Contains(author.Id.ToString()))) {
            serverChannelFromDm = await supportGuild.CreateChannelAsync($"{author.Id}-{author.Username.ReplaceAll("[ǃ@#$%^`~&*()+=,./<>?;:'\"\\|{}]")}",
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
        builder.WithContent($"From {author.Username}#{author.Discriminator} ({author.Id}):\n{(att ? $"Has `{count}` {attsb}\n" : "")}\n" +
                            $"{e.Message.Content}");
        
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

                if (contents.Equals(t.Trigger?.ToLower()) && t.DeleteTrigger)
                    await e.Message.DeleteAsync("Auto delete by bot response.");
            }
        }
    }

    /*private static async Task LookForResetListCommandResponse(DiscordClient sender, MessageCreateEventArgs e) {
        if (e.Channel.IsPrivate) return;
        if (e.Author.IsBot) return;
        if (!ProtectCommands.LookingForAnswer) return;
        if (e.Author.Id == 167335587488071682 && e.Message.Content.ToLower().Contains('y')) { /* ID of Lily #1#
            var path = BuildInfo.IsWindows ? 
                $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}MelonLoaderBlacklist{Path.DirectorySeparatorChar}Protection.json" :
                $"{Path.DirectorySeparatorChar}MelonLoaderBlacklist{Path.DirectorySeparatorChar}Protection.json";
            ProtectStructure.GetAllModsAsList()?.Clear();
            ProtectStructure.GetAllAuthorsAsList()?.Clear();
            ProtectStructure.GetAllPluginsAsList()?.Clear();
            var users = ProtectStructure.GetListOfUsers();
            
            var protectBase = new BaseProtection {
                Users = users,
                ModNames = new List<string> { "___Test" },
                PluginNames = new List<string> { "___Test" },
                AuthorNames = new List<string> { "___Test" }
            };
            
            await File.WriteAllTextAsync(path,  JsonConvert.SerializeObject(protectBase, Formatting.Indented));
            
            ProtectStructure.Save();
            ProtectCommands.LookingForAnswer = false;
        }
    }*/
}