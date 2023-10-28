using DSharpPlus.SlashCommands;

namespace HeadPats.Handlers.CommandAttributes; 

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class SlashPersonalizationCommand : SlashCheckBaseAttribute {
    public bool IsAdminCommand { get; }

    public SlashPersonalizationCommand(bool isAdminCommand) {
        this.IsAdminCommand = isAdminCommand;
    }
    
    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) {
        if (ctx.Guild.Id is not (977705960544014407 or 805663181170802719)) {
            await ctx.CreateResponseAsync("This command is only available in a specific Guild.", true);
            return false;
        }

        if (!IsAdminCommand) return true;
        var adminBool = ctx.User.Id is 875251523641294869 or 167335587488071682;
        if (adminBool) return adminBool;
        await ctx.CreateResponseAsync("You do not have permission to use this command.", true);
        return false;
    }
}