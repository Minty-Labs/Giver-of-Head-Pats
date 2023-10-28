using DSharpPlus.SlashCommands;

namespace HeadPats.Handlers.CommandAttributes; 

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class SlashBangerCommand : SlashCheckBaseAttribute {
    public bool IsAdminCommand { get; }

    public SlashBangerCommand(bool isAdminCommand) {
        this.IsAdminCommand = isAdminCommand;
    }
    
    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) {
        if (ctx.Guild.Id != 977705960544014407) {
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