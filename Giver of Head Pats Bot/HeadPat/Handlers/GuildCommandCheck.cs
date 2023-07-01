using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;
using HeadPats.Configuration;
using HeadPats.Managers;

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

public class InPennysServerAdmin : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => Task.FromResult(ctx.Guild.Id == 977705960544014407 && ctx.User.Id is 875251523641294869 or 167335587488071682);
}

public class InPennysServerAnyone : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => Task.FromResult(ctx.Guild.Id == 977705960544014407);
}





 /* ================= SLASH ================= */
public class SlashRequireAnyOwner : SlashCheckBaseAttribute {
    public override Task<bool> ExecuteChecksAsync(InteractionContext c) => Task.FromResult(Config.Base.OwnerIds!.Contains(c.User.Id));
}