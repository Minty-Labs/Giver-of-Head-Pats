using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using HeadPats.Configuration;
using HeadPats.Managers;
using Serilog;

namespace HeadPats.Handlers.Events; 

public class BangerEventListener {
    public BangerEventListener(DiscordClient c) {
        Log.Information("Setting up MessageCreated Event Handler . . .");
        
        c.MessageCreated += WatchForBangerChannel;
    }

    public static List<string>? WhitelistedUrls;
    public static List<string>? WhitelistedFileExtensions;

    public static void OnStartup() {
        if (Config.Base.Banger is null) {
            var banger = new Banger {
                Enabled = false,
                GuildId = 0,
                ChannelId = 0,
                WhitelistedUrls = new List<string> { "open.spotify.com", "youtube.com", "youtu.be", "deezer.com", "tidal.com", "bandcamp.com", "music.apple.com", "soundcloud.com" },
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
        if (list == null) throw new ArgumentNullException(nameof(list));
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;
        var domain = uri.Host;
        return list.Contains(domain);
    }
    
    private static bool IsFileExtWhitelisted(string extension, ICollection<string> list) {
        if (list == null) throw new ArgumentNullException(nameof(list));
        return list.Contains(extension);
    }

    private static async Task WatchForBangerChannel(DiscordClient sender, MessageCreateEventArgs args) {
        if (args.Channel.IsPrivate) return;
        if (args.Author.Id == 875251523641294869) return; // Penny can talk in channel just fine
        if (!Config.Base.Banger.Enabled) return;
        if (args.Guild.Id != Config.Base.Banger.GuildId) return;
        if (args.Channel.Id != Config.Base.Banger.ChannelId) return;
        if (args.Author.IsBot) return; // no bot
        if (/*args.Message.Content.StartsWith(".") || */args.Message.Content.Contains("hp!")) return; // ignore command prefix (can technically be exploited but whatever)
        
        var messageContent = args.Message.Content;
        var attachments = args.Message.Attachments;
        var stickers = args.Message.Stickers;

        if (string.IsNullOrEmpty(messageContent) && (attachments.Count != 0 || stickers.Count != 0)) {
            var extGood = IsFileExtWhitelisted(attachments[0].FileName.Split('.').Last(), WhitelistedFileExtensions!);
            if (extGood) return;
            await args.Message.RespondAsync(new DiscordMessageBuilder().WithReply(args.Message.Id, true).WithContent(Config.Base.Banger.FileErrorResponseMessage)).DeleteAfter(5);
            await args.Message.DeleteAsync();
        }
        
        var urlGood = IsUrlWhitelisted(messageContent, WhitelistedUrls!);
        if (urlGood) return;
        await args.Message.RespondAsync(new DiscordMessageBuilder().WithReply(args.Message.Id, true).WithContent(Config.Base.Banger.UrlErrorResponseMessage)).DeleteAfter(5);
        await args.Message.DeleteAsync();
    }
}