using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HeadPats.Configuration;
using HeadPats.Configuration.Classes;
using HeadPats.Handlers;
using HeadPats.Utils;

namespace HeadPats.Commands.Legacy.Commission.PersonalizedMembers; 

public class PersonalizationAdmin : BaseCommandModule {
    
    [Command("RoleToggle"), Description("Toggles the personalized roles for the guild."), LockCommandForLilysOrPennysGuildAdmin]
    public async Task TogglePersonalization(CommandContext ctx, string boolean) {
        var result = boolean.ToLower().Contains('t');
        var personalData = Config.PersonalizedMember(ctx.Guild.Id);
        personalData.Enabled = result;
        Config.Save();
        await ctx.RespondAsync($"Personalized Roles are now {(result ? "enabled" : "disabled")}.");
    }
    
    [Command("RoleSetChannel"), Description("Sets the channel where users can use the personalization commands."), LockCommandForLilysOrPennysGuildAdmin]
    public async Task SetPersonalizationChannel(CommandContext ctx, [Description("Destination Discord Channel (mention)")] DiscordChannel? channel) {
        channel ??= ctx.Channel;
        var personalData = Config.PersonalizedMember(ctx.Guild.Id);
        personalData.ChannelId = channel.Id;
        Config.Save();
        await ctx.RespondAsync($"Set the personalization channel to {channel.Mention}.");
    }
    
    [Command("RoleResetTimer"), Description("Resets the artificial cooldown for the role command."), LockCommandForLilysOrPennysGuildAdmin]
    public async Task ResetRoleTimer(CommandContext ctx, [Description("Target user (mention)")] DiscordUser? user = null) {
        if (user is null) {
            await ctx.RespondAsync("Please specify a user.");
            return;
        }
        var personalData = Config.PersonalizedMember(ctx.Guild.Id);
        var memberData = personalData.Members!.FirstOrDefault(x => x.userId == user.Id);
        if (memberData is null) {
            await ctx.RespondAsync("User data does not exist.");
            return;
        }
        memberData.epochTime = 1001;
        Config.Save();
        var discordMember = await ctx.Guild.GetMemberAsync(user.Id);
        await ctx.RespondAsync($"Reset the modification timer for {discordMember.DisplayName}.");
    }

    [Command("RoleRemoveAdmin"), Description("Removes a personalized role from a user."), LockCommandForLilysOrPennysGuildAdmin]
    public async Task RemovePersonalRole(CommandContext ctx, [Description("Target user (mention)")] DiscordUser? user = null) {
        if (user is null) {
            await ctx.RespondAsync("Please specify a user.");
            return;
        }
        var personalData = Config.PersonalizedMember(ctx.Guild.Id);
        
        var memberData = personalData.Members!.FirstOrDefault(x => x.userId == user.Id);
        if (memberData is null) {
            await ctx.RespondAsync("User data does not exist.");
            return;
        }
        var memberRole = ctx.Guild.GetRole(memberData.roleId);
        await memberRole.DeleteAsync(reason: "Personalized Member - Admin: " + ctx.User.Username);
        personalData.Members!.Remove(memberData);
        Config.Save();
        var discordMember = await ctx.Guild.GetMemberAsync(user.Id);
        if (personalData.DefaultRoleId != 0) {
            var defaultRole = ctx.Guild.GetRole(personalData.DefaultRoleId);
            await discordMember.GrantRoleAsync(defaultRole, "Personalized Member - Admin: " + ctx.User.Username);
        }
        await ctx.RespondAsync($"Removed {discordMember.DisplayName}'s personalized role.");
    }

    [Command("RoleSetTimer"), Description("Sets the artificial cooldown for the role command."), LockCommandForLilysOrPennysGuildAdmin]
    public async Task SetRoleTimer(CommandContext ctx, string number = "") {
        var personalData = Config.PersonalizedMember(ctx.Guild.Id);
        if (string.IsNullOrWhiteSpace(number)) {
            await ctx.RespondAsync($"The current timer is set to {personalData.ResetTimer}. Please specify a number to change it.");
            return;
        }

        var newNumber = number.RemoveAllLetters() ?? 30;
        personalData.ResetTimer = newNumber;
        Config.Save();
        await ctx.RespondAsync($"Set the modification timer to {newNumber} seconds.");
    }

    [Command("RoleAddTo"), Description("Adds an existing role to the personalized system."), LockCommandForLilysOrPennysGuildAdmin]
    public async Task AddRoleToSystem(CommandContext ctx, string userId = "", string roleId = "") {
        if (string.IsNullOrWhiteSpace(userId)) {
            await ctx.RespondAsync("Please specify a user ID.");
            return;
        }
        if (string.IsNullOrWhiteSpace(roleId)) {
            await ctx.RespondAsync("Please specify a role ID.");
            return;
        }
        var personalData = Config.PersonalizedMember(ctx.Guild.Id);
        var ulongRoleId = ulong.Parse(roleId);
        var role = ctx.Guild.GetRole(ulongRoleId);
        if (role is null) {
            await ctx.RespondAsync("The role does not exist is guild.");
            return;
        }
        if (personalData.Members!.Any(x => x.roleId == ulongRoleId)) {
            await ctx.RespondAsync("The role is already in the system, it cannot be one more than one person.");
            return;
        }
        var discordUser = await ctx.Client.GetUserAsync(ulong.Parse(userId));
        personalData.Members!.Add(new Member {
            userId = discordUser.Id,
            roleId = ulongRoleId,
            roleName = role.Name,
            colorHex = role.Color.ToString().ValidateHexColor().ToLower(),
            epochTime = 1002
        });
        Config.Save();
        var discordMember = await ctx.Guild.GetMemberAsync(discordUser.Id);
        await ctx.RespondAsync($"Added **{role.Name}** to the personalized system for **{discordMember.DisplayName}**.");
    }
    
    [Command("RoleSetDefault"), Description("Sets the default role of the server"), LockCommandForLilysOrPennysGuildAdmin]
    public async Task SetDefaultRole(CommandContext ctx, string roleId = "") {
        if (string.IsNullOrWhiteSpace(roleId)) {
            await ctx.RespondAsync("Please specify a role ID.");
            return;
        }
        
        var personalData = Config.PersonalizedMember(ctx.Guild.Id);

        if (roleId.ToLower().Equals("none") || roleId.ToLower().Equals("null") || roleId.Equals("0")) {
            personalData.DefaultRoleId = 0;
            Config.Save();
            await ctx.RespondAsync("Successfully removed the default role.");
            return;
        }

        var ulongRoleId = ulong.Parse(roleId);
        personalData.DefaultRoleId = ulongRoleId;
        var role = ctx.Guild.GetRole(ulongRoleId);
        Config.Save();
        await ctx.RespondAsync($"Successfully set the default role to **{role.Name}**.");
    }
    
    [Command("RoleDataSetGuild"), Description("Sets the guild ID for the personalized system."), LockCommandForLilysOrPennysGuildAdmin]
    public async Task SetGuildId(CommandContext ctx, string guild = "") {
        if (string.IsNullOrWhiteSpace(guild)) {
            await ctx.RespondAsync("Please specify a target guild.");
            return;
        }

        var personalData = guild.ToLower().Equals("lily") ? Config.Base.PersonalizedMemberLily : Config.Base.PersonalizedMemberPenny;
        personalData.GuildId = ctx.Guild.Id;
        Config.Save();
        await ctx.RespondAsync("Set the guild ID.");
    }
    
}