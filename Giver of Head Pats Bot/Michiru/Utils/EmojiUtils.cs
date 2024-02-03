using Discord;

namespace Michiru.Utils;

public static class EmojiUtils {
    public static Emote? GetCustomEmoji(string name, ulong emojiId) => Emote.TryParse($"<:{name}:{emojiId}>", out var emote) ? emote : null;
}