using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;
using HeadPats.Configuration;

namespace HeadPats.Handlers;

public class LockCommandForOnlyMintyLabs : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => Task.FromResult(ctx.Guild.Id == 1083619886980403272);
}

public class LockCommandForOnlyLilysComfiCorner : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => Task.FromResult(ctx.Guild.Id == 805663181170802719);
}

public class RequireAnyOwner : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => Task.FromResult(Config.Base.OwnerIds!.Contains(ctx.User.Id));
}

public class SlashRequireAnyOwner : SlashCheckBaseAttribute {
    public override Task<bool> ExecuteChecksAsync(InteractionContext c) => Task.FromResult(Config.Base.OwnerIds!.Contains(c.User.Id));
}