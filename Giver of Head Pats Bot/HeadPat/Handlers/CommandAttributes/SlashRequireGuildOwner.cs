using DSharpPlus.SlashCommands;
using HeadPats.Configuration;

namespace HeadPats.Handlers.CommandAttributes; 

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class SlashRequireGuildOwner : SlashCheckBaseAttribute {
    public bool IsBeingUsedForDailyPats { get; }

    public SlashRequireGuildOwner(bool isBeingUsedForDailyPats = false) {
        this.IsBeingUsedForDailyPats = isBeingUsedForDailyPats;
    }
    
    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) {
        var final = ctx.User.Id == ctx.Guild.OwnerId;
        var guildSettings = Config.GuildSettings(ctx.Guild.Id);
        if (this.IsBeingUsedForDailyPats && guildSettings!.DailyPatChannelId == 0) {
            await ctx.CreateResponseAsync($"{(final ? "You need" : "The guild owner needs")} to setup a daily pat channel: `/dailypat setpatchannel <channel>`", true);
            return false;
        }
        if (final) return final;
        await ctx.CreateResponseAsync("You do not have permission to use this command. Only the guild owner can use this command.", true);
        return false;
    }
}