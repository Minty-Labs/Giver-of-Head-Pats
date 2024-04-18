using Discord;
using Discord.Interactions;

namespace HeadPats.Commands.Preexecution; 

public class RequireUserAttribute(params ulong[] userIds) : PreconditionAttribute {
    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo cmdInfo, IServiceProvider services) 
        => Task.FromResult(userIds.Contains(context.User.Id) ? PreconditionResult.FromSuccess() : PreconditionResult.FromError("You are not allowed to use this command."));
}