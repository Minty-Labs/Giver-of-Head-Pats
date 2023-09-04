using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;
using HeadPats.Configuration;

namespace HeadPats.Handlers;

public class RequireAnyOwner : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => Task.FromResult(Config.Base.OwnerIds!.Contains(ctx.User.Id));
}

public class LockCommandForOnlyMintyLabs : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => Task.FromResult(ctx.Guild.Id == 1083619886980403272);
}

public class LockCommandForLilysComfyCorner : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => Task.FromResult(ctx.Guild.Id == 805663181170802719/* && ctx.User.Id == 211681643235115008*/);
}
public class LockCommandForLilysComfyCornerAdmin : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => Task.FromResult(ctx.Guild.Id == 805663181170802719 && ctx.User.Id == 167335587488071682);
}

public class LockCommandForPennysGuildAdmin : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => Task.FromResult(ctx.Guild.Id == 977705960544014407 && ctx.User.Id is 875251523641294869 or 167335587488071682);
}
public class LockCommandForPennysGuild : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => Task.FromResult(ctx.Guild.Id == 977705960544014407);
}

}




 /* ================= SLASH ================= */
public class SlashRequireAnyOwner : SlashCheckBaseAttribute {
    public override Task<bool> ExecuteChecksAsync(InteractionContext c) => Task.FromResult(Config.Base.OwnerIds!.Contains(c.User.Id));
}