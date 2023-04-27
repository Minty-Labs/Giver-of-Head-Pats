using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace HeadPats.Commands.Legacy.NSFW; 

public class CanUseNsfwCommands : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) 
        => Task.FromResult(ctx.Member!.Roles.Any(x => x.Name.Contains("NSFW Access")));
}