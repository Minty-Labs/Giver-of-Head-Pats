using Discord;
using Discord.Interactions;

namespace HeadPats.Commands.Preexecution; 

public class RequireUserAttribute : PreconditionAttribute {
    private readonly ulong[] _userIds;
    
    public RequireUserAttribute(params ulong[] userIds) => _userIds = userIds;

    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo cmdInfo, IServiceProvider services) 
        => Task.FromResult(_userIds.Contains(context.User.Id) ? PreconditionResult.FromSuccess() : PreconditionResult.FromError("You are not allowed to use this command."));
}