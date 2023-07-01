using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace HeadPats.Handlers.CommandAttributes; 

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class CustomSlashRequirePermissionsAttribute : SlashCheckBaseAttribute {
    public Permissions Permissions { get; }
    public bool IgnoreDms { get; } = true;
    
    public CustomSlashRequirePermissionsAttribute(Permissions permissions, bool ignoreDms = true) {
        this.Permissions = permissions;
        this.IgnoreDms = ignoreDms;
    }

    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) {
        if (ctx.Guild == null)
            return this.IgnoreDms;

        var usr = ctx.Member;
        if (usr == null)
            return false;
        var pusr = ctx.Channel.PermissionsFor(usr);

        var bot = await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id);
        if (bot == null)
            return false;
        var pbot = ctx.Channel.PermissionsFor(bot);

        var usrok = ctx.Guild.OwnerId == usr.Id;
        var botok = ctx.Guild.OwnerId == bot.Id;

        if (!usrok)
            usrok = (pusr & Permissions.Administrator) != 0 || (pusr & this.Permissions) == this.Permissions;

        if (!botok)
            botok = (pbot & Permissions.Administrator) != 0 || (pbot & this.Permissions) == this.Permissions;
        
        var bothOk = usrok && botok;
        if (bothOk) return bothOk;
        await ctx.CreateResponseAsync("You do not have permission to use this command.", true);
        return false;
    }
}