using Discord;
using Discord.Interactions;

namespace Michiru.Commands.Preexecution; 

public class RequireToBeSpecial : PreconditionAttribute {

    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo cmdInfo, IServiceProvider services) {
        if (context.Guild.OwnerId == context.User.Id)
            return Task.FromResult(PreconditionResult.FromSuccess());
        return context.Guild.Id switch {
            977705960544014407 when context.User.Id is 875251523641294869 or 167335587488071682 => Task.FromResult(PreconditionResult.FromSuccess()),
            1149332156313768007 when context.User.Id is 723217987774971975 or 927059361514291260 or 167335587488071682 => Task.FromResult(PreconditionResult.FromSuccess()),
            _ => Task.FromResult(PreconditionResult.FromError("You are not allowed to use this command."))
        };
    }
}