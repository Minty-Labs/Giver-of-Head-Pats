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
                FileErrorResponseMessage = "This file type is not whitelisted.",
                AddUpvoteEmoji = true,
                AddDownvoteEmoji = false,
                UseCustomUpvoteEmoji = true,
                CustomUpvoteEmojiName = "upvote",
                CustomUpvoteEmojiId = 1201639290048872529,
                UseCustomDownvoteEmoji = false,
                CustomDownvoteEmojiName = "downvote",
                CustomDownvoteEmojiId = 1201639287972696166,
                NoticeComment = "Having a custom emoji ID of zero will logically mean that you are using a Discord default emoji."
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
        var conf = Config.Base.Banger;                               // shorten config to variable
        if (!conf!.Enabled) return;                                  // if disabled
        if (args.Id != conf.GuildId) return;                         // if not target guild
        if (args.Author.Id == 875251523641294869) return;            // Penny can talk in channel freely
        if (args.Channel.Id != conf.ChannelId) return;               // if not target channel
        if (args.Author.IsBot) return;                               // if bot
        if (args.Content.StartsWith('.')) return;                    // can technically be exploited but whatever

        var messageContent = args.Content;
        var attachments = args.Attachments;
        var stickers = args.Stickers;
        var socketUserMessage = (SocketUserMessage)args; // i think D.Net is being picky about that type of message it is
        var upvote = conf.CustomUpvoteEmojiId != 0 ? EmojiUtils.GetCustomEmoji(conf.CustomUpvoteEmojiName, conf.CustomUpvoteEmojiId) : Emote.Parse(conf.CustomUpvoteEmojiName) ?? Emote.Parse(":thumbsup:");
        var downvote = conf.CustomDownvoteEmojiId != 0 ? EmojiUtils.GetCustomEmoji(conf.CustomDownvoteEmojiName, conf.CustomDownvoteEmojiId) : Emote.Parse(conf.CustomDownvoteEmojiName) ?? Emote.Parse(":thumbsdown:");
        var extGood = IsFileExtWhitelisted(attachments.First().Filename.Split('.').Last(), WhitelistedFileExtensions!);
        var urlGood = IsUrlWhitelisted(messageContent, WhitelistedUrls!);

        if (string.IsNullOrEmpty(messageContent) && (attachments.Count != 0 || stickers.Count != 0)) {
            if (extGood || (urlGood && extGood)) {
                if (conf.AddUpvoteEmoji && conf.UseCustomUpvoteEmoji)
                    await socketUserMessage.AddReactionAsync(upvote);
                if (conf.AddDownvoteEmoji && conf.UseCustomDownvoteEmoji)
                    await socketUserMessage.AddReactionAsync(downvote);
                return;
            }

            await args.Channel.SendMessageAsync(conf.FileErrorResponseMessage).DeleteAfter(5);
            await args.DeleteAsync();
        }

        if (urlGood) {
            if (conf.AddUpvoteEmoji && conf.UseCustomUpvoteEmoji)
                await socketUserMessage.AddReactionAsync(upvote);
            if (conf.AddDownvoteEmoji && conf.UseCustomDownvoteEmoji)
                await socketUserMessage.AddReactionAsync(downvote);
            return;
        }

        await args.Channel.SendMessageAsync(conf.UrlErrorResponseMessage).DeleteAfter(5);
        await args.DeleteAsync();
    }

    // private const float RemovalPercent = 0.2f;
    // private const int MinVotes = 5;

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