using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HeadPats.Configuration;
using HeadPats.Configuration.Classes;
using HeadPats.Handlers;
using HeadPats.Managers;
using HeadPats.Utils;

namespace HeadPats.Commands.Legacy.Commission.PersonalizedMembers; 

public class PersonalizationAnyone : BaseCommandModule {
    
    private static bool _IsInChannel(CommandContext ctx, ulong guildId) => ctx.Channel.Id == Config.PersonalizedMember(guildId).ChannelId;

    [Command("RoleCreate"), Description("Create a personalized role for this server"), Aliases("RoleAdd", "RoleMake", "mkrole"), LockCommandForLilysOrPennysGuild]
    public async Task CreateRole(CommandContext ctx) {
        var personalData = Config.PersonalizedMember(ctx.Guild.Id);
        if (!personalData.Enabled) {
            var msg = await ctx.RespondAsync("Personalized Roles is not enabled.");
            await Task.Delay(TimeSpan.FromSeconds(5));
            await msg.DeleteAsync();
            await ctx.Message.DeleteAsync();
            return;
        }
        if (!_IsInChannel(ctx, ctx.Guild.Id)) {
            var msg = await ctx.RespondAsync($"You can only use this command in <#{personalData.ChannelId}>");
            await Task.Delay(TimeSpan.FromSeconds(5));
            await msg.DeleteAsync();
            await ctx.Message.DeleteAsync();
            return;
        }
        var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var personalizedMember = personalData.Members!.FirstOrDefault(x => x.userId == ctx.User.Id);
        if (personalizedMember is null) {
            var memberRole = await ctx.Guild.CreateRoleAsync(name: ctx.User.Username, reason: "Personalized Member - User");
            await Task.Delay(TimeSpan.FromSeconds(1));
            var personalizedMemberData = new Member {
                userId = ctx.User.Id,
                roleId = memberRole.Id,
                roleName = ctx.User.Username.Left(15).Trim(),
                colorHex = "",
                epochTime = currentEpoch
            };
            personalData.Members!.Add(personalizedMemberData);
            Config.Save();
            await ctx.Member!.GrantRoleAsync(memberRole, "Personalized Member - User: " + ctx.User.Username);
            if (personalData.DefaultRoleId != 0) {
                var defaultRole = ctx.Guild.GetRole(personalData.DefaultRoleId);
                await ctx.Member!.RevokeRoleAsync(defaultRole, "Personalized Member - User: " + ctx.User.Username);
            }
            await ctx.RespondAsync("Successfully created your personalized member role.");
            return;
        }

        await ctx.RespondAsync("You already have a personalized role.");
    }

    [Command("RoleColor"), Description("Add or update the color of your personalized role for this server"), Aliases("RoleColour"), LockCommandForLilysOrPennysGuild]
    public async Task ColorRole(CommandContext ctx, [Description("Color value as HEX (#rrggbb)")] string colorHex = "") {
        var personalData = Config.PersonalizedMember(ctx.Guild.Id);
        if (!personalData.Enabled) {
            var msg = await ctx.RespondAsync("Personalized Roles is not enabled.");
            await Task.Delay(TimeSpan.FromSeconds(5));
            await msg.DeleteAsync();
            await ctx.Message.DeleteAsync();
            return;
        }
        if (!_IsInChannel(ctx, ctx.Guild.Id)) {
            var msg = await ctx.RespondAsync($"You can only use this command in <#{personalData.ChannelId}>");
            await Task.Delay(TimeSpan.FromSeconds(5));
            await msg.DeleteAsync();
            await ctx.Message.DeleteAsync();
            return;
        }

        if (string.IsNullOrWhiteSpace(colorHex)) {
            await ctx.RespondAsync("Color string cannot be empty.");
            return;
        }
        var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var personalizedMember = personalData.Members!.FirstOrDefault(x => x.userId == ctx.User.Id);
        if (personalizedMember is null) {
            await ctx.RespondAsync($"You need to create a personalized role first.\nRun the following command to create one:\n`{Config.Base.Prefix}rolecreate`");
            return;
        }
        if (personalizedMember!.epochTime + personalData.ResetTimer > currentEpoch) {
            await ctx.RespondAsync($"You need to wait {personalizedMember.epochTime + personalData.ResetTimer - currentEpoch} seconds before you can use this command again.");
            return;
        }
        var memberRole = ctx.Guild.GetRole(personalizedMember.roleId);
        var newColorString = colorHex.ValidateHexColor().Left(6);
        if (personalizedMember.colorHex == newColorString) {
            await ctx.RespondAsync("Your personalized member role color is already set to that.");
            return;
        }
        personalizedMember.colorHex = newColorString;
        Config.Save();
        await memberRole!.ModifyAsync(color: Colors.HexToColor(newColorString), reason: "Personalized Member - User: " + ctx.User.Username);
        await ctx.RespondAsync("Successfully updated your personalized member role color.");
    }
    
    [Command("RoleName"), Description("Update the name of your personalized role for this server"), LockCommandForLilysOrPennysGuild]
    public async Task NameRole(CommandContext ctx, [Description("Desired Role Name (15 characters)"), RemainingText] string name = "") {
        var personalData = Config.PersonalizedMember(ctx.Guild.Id);
        if (!personalData.Enabled) {
            var msg = await ctx.RespondAsync("Personalized Roles is not enabled.");
            await Task.Delay(TimeSpan.FromSeconds(5));
            await msg.DeleteAsync();
            await ctx.Message.DeleteAsync();
            return;
        }
        if (!_IsInChannel(ctx, ctx.Guild.Id)) {
            var msg = await ctx.RespondAsync($"You can only use this command in <#{personalData.ChannelId}>");
            await Task.Delay(TimeSpan.FromSeconds(5));
            await msg.DeleteAsync();
            await ctx.Message.DeleteAsync();
            return;
        }
        if (string.IsNullOrWhiteSpace(name)) {
            await ctx.RespondAsync("Name string cannot be empty.");
            return;
        }
        if (name.Length > 15) {
            await ctx.RespondAsync("Name string is longer than 15 characters, only the first 15 will be used.").DeleteAfter(5);
        }
        var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var personalizedMember = personalData.Members!.FirstOrDefault(x => x.userId == ctx.User.Id);
        if (personalizedMember is null) {
            await ctx.RespondAsync($"You need to create a personalized role first.\nRun the following command to create one:\n`{Config.Base.Prefix}rolecreate`");
            return;
        }
        if (personalizedMember.epochTime + personalData.ResetTimer > currentEpoch) {
            await ctx.RespondAsync($"You need to wait {personalizedMember.epochTime + personalData.ResetTimer - currentEpoch} seconds before you can use this command again.");
            return;
        }
        var newRoleName = name.Left(15).Trim();
        if (personalizedMember.roleName == newRoleName) {
            await ctx.RespondAsync("Your personalized member role name is already set to that.");
            return;
        }
        var memberRole = ctx.Guild.GetRole(personalizedMember.roleId);
        personalizedMember.roleName = newRoleName;
        Config.Save();
        await memberRole!.ModifyAsync(name: newRoleName, reason: "Personalized Member - User: " + ctx.User.Username);
        await ctx.RespondAsync("Successfully updated your personalized member role name.");
    }
    
    // TODO: (Only if people abuse the current feature set) public static Dictionary<ulong, bool> RoleRemovalConfirmation = new Dictionary<ulong, bool>();
    [Command("RoleRemove"), Description("Remove your personalized role for this server"), Aliases("RoleDel", "RoleDelete", "rolerm"), LockCommandForLilysOrPennysGuild]
    public async Task RemoveRole(CommandContext ctx) {
        var personalData = Config.PersonalizedMember(ctx.Guild.Id);
        if (!personalData.Enabled) {
            var msg = await ctx.RespondAsync("Personalized Roles is not enabled.");
            await Task.Delay(TimeSpan.FromSeconds(5));
            await msg.DeleteAsync();
            await ctx.Message.DeleteAsync();
            return;
        }
        if (!_IsInChannel(ctx, ctx.Guild.Id)) {
            var msg = await ctx.RespondAsync($"You can only use this command in <#{personalData.ChannelId}>");
            await Task.Delay(TimeSpan.FromSeconds(5));
            await msg.DeleteAsync();
            await ctx.Message.DeleteAsync();
            return;
        }
        var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var personalizedMember = personalData.Members!.FirstOrDefault(x => x.userId == ctx.User.Id);
        if (personalizedMember is null) {
            await ctx.RespondAsync($"You need to create a personalized role first.\nRun the following command to create one:\n`{Config.Base.Prefix}rolecreate`");
            return;
        }
        if (personalizedMember.epochTime + personalData.ResetTimer > currentEpoch) {
            await ctx.RespondAsync($"You need to wait {personalizedMember.epochTime + personalData.ResetTimer - currentEpoch} seconds before you can use this command again.");
            return;
        }
        var memberRole = ctx.Guild.GetRole(personalizedMember!.roleId);
        await memberRole!.DeleteAsync(reason: "Personalized Member - User: " + ctx.User.Username);
        personalData.Members!.Remove(personalizedMember);
        Config.Save();
        if (personalData.DefaultRoleId != 0) {
            var defaultRole = ctx.Guild.GetRole(personalData.DefaultRoleId);
            await ctx.Member!.GrantRoleAsync(defaultRole, "Personalized Member - User: " + ctx.User.Username);
        }
        await ctx.RespondAsync("Successfully removed your personalized member role.");
    }
    
}