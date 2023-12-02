using System.Net.Mime;
using System.Text;
using Discord;
using Discord.WebSocket;
using HeadPats.Configuration;
using HeadPats.Managers;
using HeadPats.Configuration.Classes;
using HeadPats.Modules;

namespace HeadPats.Events; 

public class BangerEventListener : EventModule {
    protected override string EventName => "Banger";
    protected override string Description => "Handles the Banger events.";

    public override void Initialize(DiscordSocketClient client) {
        client.MessageReceived += WatchForBangerChannel;
    }

    private static List<string>? WhitelistedUrls;
    private static List<string>? WhitelistedFileExtensions;

    public override void OnSessionCreated() {
        if (Config.Base.Banger is null) {
            var banger = new Banger {
                Enabled = false,
                GuildId = 0,
                ChannelId = 0,
                WhitelistedUrls = new List<string> { "open.spotify.com", "youtube.com", "youtube.com", "music.youtube.com", "youtu.be", "deezer.com", "tidal.com", "bandcamp.com", "music.apple.com", "soundcloud.com" },
                WhitelistedFileExtensions = new List<string> { "mp3", "flac", "wav", "ogg", "m4a", "alac", "aac", "aiff", "wma" },
                UrlErrorResponseMessage = "This URL is not whitelisted.",
                FileErrorResponseMessage = "This file type is not whitelisted."
            };
            Config.Base.Banger = banger;
            Config.Save();
        }
        WhitelistedUrls = Config.Base.Banger.WhitelistedUrls!.ToList();
        WhitelistedFileExtensions = Config.Base.Banger.WhitelistedFileExtensions!.ToList();
    }

    private static bool IsUrlWhitelisted(string url, ICollection<string> list) {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;
        return list?.Contains(uri.Host) ?? throw new ArgumentNullException(nameof(list));
    }
    
    private static bool IsFileExtWhitelisted(string extension, ICollection<string> list) 
        => list?.Contains(extension) ?? throw new ArgumentNullException(nameof(list));

    private static async Task WatchForBangerChannel(SocketMessage args) {
        if (args.Channel.GetChannelType() != ChannelType.DM) return;
        if (!Config.Base.Banger!.Enabled) return;
        if (args.Id != Config.Base.Banger.GuildId) return;
        if (args.Author.Id == 875251523641294869) return; // Penny can talk in channel just fine
        if (args.Channel.Id != Config.Base.Banger.ChannelId) return;
        if (args.Author.IsBot) return; // no bot
        if (args.Content.StartsWith(".")) return; // can technically be exploited but whatever
        
        var messageContent = args.Content;
        var attachments = args.Attachments;
        var stickers = args.Stickers;

        if (string.IsNullOrEmpty(messageContent) && (attachments.Count != 0 || stickers.Count != 0)) {
            var extGood = IsFileExtWhitelisted(attachments.First().Filename.Split('.').Last(), WhitelistedFileExtensions!);
            if (extGood) return;
            await args.Channel.SendMessageAsync(Config.Base.Banger.FileErrorResponseMessage, messageReference: new MessageReference(messageId: args.Id, channelId: args.Channel.Id)).DeleteAfter(5);
            await args.DeleteAsync();
        }
        
        var urlGood = IsUrlWhitelisted(messageContent, WhitelistedUrls!);
        if (urlGood) return;
        await args.Channel.SendMessageAsync(Config.Base.Banger.UrlErrorResponseMessage, messageReference: new MessageReference(messageId: args.Id, channelId: args.Channel.Id)).DeleteAfter(5);
        await args.DeleteAsync();
    }
}