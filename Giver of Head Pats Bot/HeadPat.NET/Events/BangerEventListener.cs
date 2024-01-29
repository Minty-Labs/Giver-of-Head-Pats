using System.Net.Mime;
using System.Text;
using Discord;
using Discord.WebSocket;
using HeadPats.Configuration;
using HeadPats.Managers;
using HeadPats.Configuration.Classes;
using HeadPats.Modules;
using HeadPats.Utils;

namespace HeadPats.Events; 

public class BangerEventListener : EventModule {
    protected override string EventName => "Banger";
    protected override string Description => "Handles the Banger events.";

    public override void Initialize(DiscordSocketClient client) {
        client.MessageReceived += WatchForBangerChannel;
        // client.MessageUpdated += WatchForReactionChanges;
    }

    public static List<string>? WhitelistedUrls;
    public static List<string>? WhitelistedFileExtensions;

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
        if (args.Channel.GetChannelType() == ChannelType.DM) return; // if dm
        if (!Config.Base.Banger!.Enabled) return;                    // if disabled
        if (args.Id != Config.Base.Banger.GuildId) return;           // if not target guild
        if (args.Author.Id == 875251523641294869) return;            // Penny can talk in channel freely
        if (args.Channel.Id != Config.Base.Banger.ChannelId) return; // if not target channel
        if (args.Author.IsBot) return;                               // if bot
        if (args.Content.StartsWith('.')) return;                    // can technically be exploited but whatever
        
        var messageContent = args.Content;
        var attachments = args.Attachments;
        var stickers = args.Stickers;

        if (string.IsNullOrEmpty(messageContent) && (attachments.Count != 0 || stickers.Count != 0)) {
            var extGood = IsFileExtWhitelisted(attachments.First().Filename.Split('.').Last(), WhitelistedFileExtensions!);
            if (extGood) {
                await args.AddReactionAsync(Upvote);
                // await args.AddReactionAsync(Downvote);
                return;
            }
            await args.Channel.SendMessageAsync(Config.Base.Banger.FileErrorResponseMessage).DeleteAfter(5);
            await args.DeleteAsync();
        }
        
        var urlGood = IsUrlWhitelisted(messageContent, WhitelistedUrls!);
        if (urlGood) {
            await args.AddReactionAsync(Upvote);
            // await args.AddReactionAsync(Downvote);
            return;
        }
        await args.Channel.SendMessageAsync(Config.Base.Banger.UrlErrorResponseMessage).DeleteAfter(5);
        await args.DeleteAsync();
    }

    // private const float RemovalPercent = 0.2f;
    // private const int MinVotes = 5;
    private static readonly Emote Upvote = EmojiUtils.GetCustomEmoji("upvote", 1201639290048872529)!;
    // private static readonly Emote Downvote = EmojiUtils.GetCustomEmoji("downvote", 1201639287972696166)!;
    
    // private static async Task WatchForReactionChanges(Cacheable<IMessage, ulong> oldMessage, SocketMessage newMessage, ISocketMessageChannel messageChannel) {
    //     var emotes = newMessage.Reactions;
    //     var upvote = emotes.FirstOrDefault(e => e.Key.Name == "upvote").Value;
    //     var downvote = emotes.FirstOrDefault(e => e.Key.Name == "downvote").Value;
    //     
    //     if (upvote.ReactionCount + downvote.ReactionCount < MinVotes) return;
    //     var reactionVotes = _getPercentage(upvote.ReactionCount, downvote.ReactionCount);
    //
    //     if (reactionVotes < RemovalPercent) {
    //         var sb = new StringBuilder();
    //     }
    // }
    //
    // private static float _getPercentage(int up, int down) => (float)up / (up + down);
}