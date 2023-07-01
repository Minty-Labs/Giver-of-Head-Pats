using DSharpPlus.SlashCommands;

namespace HeadPats.Handlers.CommandAttributes; 

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class CustomSlashRequireOwnerAttribute : SlashCheckBaseAttribute {
    public CustomSlashRequireOwnerAttribute() { }
    
    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) {
        var app = ctx.Client.CurrentApplication;
        var me = ctx.Client.CurrentUser;

        var finalBool = app != null ? app.Owners.Any(x => x.Id == ctx.User.Id) : ctx.User.Id == me.Id;
        if (finalBool) return finalBool;
        await ctx.CreateResponseAsync("You do not have permission to use this command.", true);
        return false;
    }
}