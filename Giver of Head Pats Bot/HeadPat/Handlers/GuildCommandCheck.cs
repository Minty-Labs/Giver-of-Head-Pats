using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
// using DSharpPlus.SlashCommands;

namespace HeadPats.Handlers;

public class LockCommandForOnlyMintyLabs : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => Task.FromResult(ctx.Guild.Id == 1083619886980403272);
}

public class LockCommandForOnlyLilysComfiCorner : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => Task.FromResult(ctx.Guild.Id == 805663181170802719);
}

/*public class GuildCommandCheck {
    public ulong GuildId { get; set; }
    public bool Enabled { get; set; }
}

public class HasModerationModuleEnabled : SlashCheckBaseAttribute {
    
    public override Task<bool> ExecuteChecksAsync(InteractionContext c) {
        return Task.FromResult(Program.GuildCommandCheckList.FirstOrDefault(t => c.Guild.Id == t.GuildId)!.Enabled);
    }
}*/