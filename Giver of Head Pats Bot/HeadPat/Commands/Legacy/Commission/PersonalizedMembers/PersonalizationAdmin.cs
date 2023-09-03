using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HeadPats.Configuration;
using HeadPats.Configuration.Classes;
using HeadPats.Handlers;
using HeadPats.Utils;

namespace HeadPats.Commands.Legacy.Commission.PersonalizedMembers; 

public class PersonalizationAdmin : BaseCommandModule {
    
    [Command("RoleToggle"), Description("Toggles the personalized roles for the guild."), LockCommandForLilysComfyCornerAdmin]
    public async Task TogglePersonalization(CommandContext ctx, string boolean) {
        var result = boolean.ToLower().Contains('t');
        Config.Base.PersonalizedMember.Enabled = result;
        Config.Save();
        await ctx.RespondAsync($"Personalized Roles are now {(result ? "enabled" : "disabled")}.");
    }
    
    [Command("RoleSetChannel"), Description("Sets the channel where users can use the personalization commands."), LockCommandForLilysComfyCornerAdmin, DisallowDirectMessage]
    public async Task SetPersonalizationChannel(CommandContext ctx, [Description("Destination Discord Channel (mention)")] DiscordChannel? channel) {
        channel ??= ctx.Channel;
        Config.Base.PersonalizedMember.ChannelId = channel.Id;
        Config.Save();
        await ctx.RespondAsync($"Set the personalization channel to {channel.Mention}.");
    }
    
    [Command("RoleResetTimer"), Description("Resets the artificial cooldown for the role command."), LockCommandForLilysComfyCornerAdmin, DisallowDirectMessage]
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

    [Command("RoleRemoveAdmin"), Description("Removes a personalized role from a user."), LockCommandForLilysComfyCornerAdmin, DisallowDirectMessage]
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

    [Command("RoleSetTimer"), Description("Sets the artificial cooldown for the role command."), LockCommandForLilysComfyCornerAdmin]
    public async Task SetRoleTimer(CommandContext ctx, string number = "") {
        if (string.IsNullOrWhiteSpace(number)) {
            await ctx.RespondAsync($"The current timer is set to {PersonalizedMemberLogic.ResetTimer}. Please specify a number to change it.");
            return;
        }

        var newNumber = number.RemoveAllLetters() ?? 30;
        Config.Base.PersonalizedMember.ResetTimer = newNumber;
        PersonalizedMemberLogic.ResetTimer = newNumber;
        Config.Save();
        await ctx.RespondAsync($"Set the modification timer to {newNumber} seconds.");
    }
    
}