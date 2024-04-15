using System.Collections.Concurrent;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using HeadPats.Utils;

namespace HeadPats.Commands.Preexecution;

public class RateLimit(
    int seconds = 4,
    int requests = 1,
    RateLimit.RateLimitType context = RateLimit.RateLimitType.User,
    RateLimit.RateLimitBaseType baseType = RateLimit.RateLimitBaseType.BaseOnCommandInfo ) : PreconditionAttribute {
    private static readonly ConcurrentDictionary<ulong, List<RateLimitItem>> Items = new();
    private static DateTime _removeExpiredCommandsTime = DateTime.MinValue;
    private readonly RateLimitType? _context = context;

    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services) {
        // clear old expired commands every 30m
        if (DateTime.UtcNow > _removeExpiredCommandsTime) {
            _ = Task.Run(async () => {
                await ClearExpiredCommands();
                _removeExpiredCommandsTime = DateTime.UtcNow.AddMinutes(30);
            });
        }

        ulong id = _context!.Value switch {
            RateLimitType.User => context.User.Id,
            RateLimitType.Channel => context.Channel.Id,
            RateLimitType.Guild => context.Guild.Id,
            RateLimitType.Global => 0,
            _ => 0
        };

        var contextId = baseType switch {
            RateLimitBaseType.BaseOnCommandInfo => commandInfo.Module.Name + "//" + commandInfo.Name + "//" + commandInfo.MethodName,
            RateLimitBaseType.BaseOnCommandInfoHashCode => commandInfo.GetHashCode().ToString(),
            RateLimitBaseType.BaseOnSlashCommandName => (context.Interaction as SocketSlashCommand)?.CommandName,
            RateLimitBaseType.BaseOnMessageComponentCustomId => (context.Interaction as SocketMessageComponent)?.Data.CustomId,
            RateLimitBaseType.BaseOnAutocompleteCommandName => (context.Interaction as SocketAutocompleteInteraction)?.Data.CommandName,
            RateLimitBaseType.BaseOnApplicationCommandName => (context.Interaction as SocketApplicationCommand).Name,
            _ => "unknown"
        };

        var dateTime = DateTime.UtcNow;

        var target = Items.GetOrAdd(id, new List<RateLimitItem>());

        var commands = target.Where(
            a =>
                a.Command == contextId
        );

        var rateLimitItems = commands.ToList();
        foreach (var c in rateLimitItems.ToList().Where(c => dateTime >= c.ExpireAt))
            target.Remove(c);

        if (rateLimitItems.Count >= requests) return Task.FromResult(PreconditionResult.FromError($"This command is usable at {target.Last().ExpireAt.ConvertToDiscordTimestamp(TimestampFormat.RelativeTime)}."));
        target.Add(new RateLimitItem {
            Command = contextId,
            ExpireAt = DateTime.UtcNow + TimeSpan.FromSeconds(seconds)
        });
        return Task.FromResult(PreconditionResult.FromSuccess());

    }

    private static Task ClearExpiredCommands() {
        foreach (var doc in Items) {
            var utcTime = DateTime.UtcNow;
            foreach (var command in doc.Value.Where(a => utcTime > a.ExpireAt).ToList())
                doc.Value.Remove(command);
        }

        return Task.CompletedTask;
    }

    public static List<RateLimitItem> GetCommandsByIdAsync(ulong id) => Items.GetOrAdd(id, []).ToList();

    public static void ExpireCommandsById(ulong id) {
        var target = Items.GetOrAdd(id, []);
        target.Clear();
    }

    public static void ExpireCommands() => Items.Clear();

    public class RateLimitItem {
        public string? Command { get; init; }
        public DateTime ExpireAt { get; init; }
    }

    public enum RateLimitType {
        User,
        Channel,
        Guild,
        Global
    }

    public enum RateLimitBaseType {
        BaseOnCommandInfo,
        BaseOnCommandInfoHashCode,
        BaseOnSlashCommandName,
        BaseOnMessageComponentCustomId,
        BaseOnAutocompleteCommandName,
        BaseOnApplicationCommandName,
        BaseOnApplicationCommandId
    }
}