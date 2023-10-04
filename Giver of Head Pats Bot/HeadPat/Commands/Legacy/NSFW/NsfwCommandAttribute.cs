/*using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace HeadPats.Commands.Legacy.NSFW; 

public class CanUseNsfwCommandsFromSpecificRoleName : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) 
        => Task.FromResult(ctx.Member!.Roles.Any(x => x.Name.ToLower().Contains("nsfw")));
}*/