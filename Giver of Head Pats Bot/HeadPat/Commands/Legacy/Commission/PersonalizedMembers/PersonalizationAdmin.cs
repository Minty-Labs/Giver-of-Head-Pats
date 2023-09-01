using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HeadPats.Configuration;
using HeadPats.Handlers;
using Serilog;

namespace HeadPats.Commands.Legacy.Commission.PersonalizedMembers; 

public class PersonalizationAdmin : BaseCommandModule {
    
    [Command("RoleToggle"), Description("Toggles the personalized roles for the guild."), LockCommandForLilysComfyCornerAdmin]
    public async Task TogglePersonalization(CommandContext ctx, string boolean) {
        var result = boolean.ToLower().Contains('t');
        Config.Base.PersonalizedMember.Enabled = result;
        Config.Save();
        await ctx.RespondAsync($"Personalized Roles are now {(result ? "enabled" : "disabled")}.");
    }
    
    [Command("RoleSetChannel"), Description("Sets the channel where users can use the personalization commands."), LockCommandForLilysComfyCornerAdmin]
    public async Task SetPersonalizationChannel(CommandContext ctx, [Description("Destination Discord Channel (mention)")] DiscordChannel? channel) {
        channel ??= ctx.Channel;
        Config.Base.PersonalizedMember.ChannelId = channel.Id;
        Config.Save();
        await ctx.RespondAsync($"Set the personalization channel to {channel.Mention}.");
    }
    
    [Command("RoleResetTimer"), Description("Resets the artificial cooldown for the role command."), LockCommandForLilysComfyCornerAdmin]
    public async Task ResetRoleTimer(CommandContext ctx, [Description("Target user (mention)")] DiscordUser? user = null) {
        if (user is null) {
            await ctx.RespondAsync("Please specify a user.");
            return;
        }
        var memberData = Config.Base.PersonalizedMember.Members!.FirstOrDefault(x => x.userId == user.Id);
        if (memberData is null) {
            await ctx.RespondAsync("User data does not exist.");
            return;
        }
        memberData.epochTime = 1001;
        Config.Save();
        var discordMember = await ctx.Guild.GetMemberAsync(user.Id);
        await ctx.RespondAsync($"Reset the modification timer for {discordMember.DisplayName}.");
    }

    [Command("RoleRemoveAdmin"), Description("Removes a personalized role from a user."), LockCommandForLilysComfyCornerAdmin]
    public async Task RemovePersonalRole(CommandContext ctx, [Description("Target user (mention)")] DiscordUser? user = null) {
        if (user is null) {
            await ctx.RespondAsync("Please specify a user.");
            return;
        }
        
        var memberData = Config.Base.PersonalizedMember.Members!.FirstOrDefault(x => x.userId == user.Id);
        if (memberData is null) {
            await ctx.RespondAsync("User data does not exist.");
            return;
        }
        var memberRole = ctx.Guild.GetRole(memberData.roleId);
        await memberRole.DeleteAsync(reason: "Personalized Member - Admin");
        Config.Base.PersonalizedMember.Members!.Remove(memberData);
        Config.Save();
        var discordMember = await ctx.Guild.GetMemberAsync(user.Id);
        await ctx.RespondAsync($"Removed {discordMember.DisplayName}'s personalized role.");
    }
    
}