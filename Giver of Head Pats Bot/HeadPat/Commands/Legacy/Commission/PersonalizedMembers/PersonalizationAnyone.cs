using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HeadPats.Configuration;
using HeadPats.Configuration.Classes;
using HeadPats.Handlers;
using HeadPats.Managers;
using HeadPats.Utils;

namespace HeadPats.Commands.Legacy.Commission.PersonalizedMembers; 

public class PersonalizationAnyone : BaseCommandModule {
    
    private static bool _IsInChannel(CommandContext ctx) => ctx.Channel.Id == Config.Base.PersonalizedMember.ChannelId;

    [Command("RoleCreate"), Description("Create a personalized role for this server"), Aliases("RoleAdd", "RoleMake", "mkrole"), LockCommandForLilysComfyCorner]
    public async Task CreateRole(CommandContext ctx) {
        if (!Config.Base.PersonalizedMember.Enabled) {
            var msg = await ctx.RespondAsync("Personalized Roles is not enabled.");
            await Task.Delay(TimeSpan.FromSeconds(5));
            await msg.DeleteAsync();
            await ctx.Message.DeleteAsync();
            return;
        }
        if (!_IsInChannel(ctx)) {
            var msg = await ctx.RespondAsync($"You can only use this command in <#{Config.Base.PersonalizedMember.ChannelId}>");
            await Task.Delay(TimeSpan.FromSeconds(5));
            await msg.DeleteAsync();
            await ctx.Message.DeleteAsync();
            return;
        }
        var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var personalizedMember = Config.Base.PersonalizedMember.Members!.FirstOrDefault(x => x.userId == ctx.User.Id);
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
            Config.Base.PersonalizedMember.Members!.Add(personalizedMemberData);
            Config.Save();
            await ctx.Member!.GrantRoleAsync(memberRole, "Personalized Member - User");
            await ctx.RespondAsync("Successfully created your personalized member role.");
            return;
        }

        await ctx.RespondAsync("You already have a personalized role.");
    }

    [Command("RoleColor"), Description("Add or update the color of your personalized role for this server"), Aliases("RoleColour"), LockCommandForLilysComfyCorner]
    public async Task ColorRole(CommandContext ctx, [Description("Color value as HEX (#rrggbb)")] string colorHex = "") {
        if (!Config.Base.PersonalizedMember.Enabled) {
            var msg = await ctx.RespondAsync("Personalized Roles is not enabled.");
            await Task.Delay(TimeSpan.FromSeconds(5));
            await msg.DeleteAsync();
            await ctx.Message.DeleteAsync();
            return;
        }
        if (!_IsInChannel(ctx)) {
            var msg = await ctx.RespondAsync($"You can only use this command in <#{Config.Base.PersonalizedMember.ChannelId}>");
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
        var personalizedMember = Config.Base.PersonalizedMember.Members!.FirstOrDefault(x => x.userId == ctx.User.Id);
        if (personalizedMember is null) {
            await ctx.RespondAsync($"You need to create a personalized role first.\nRun the following command to create one:\n`{Config.Base.Prefix}rolecreate`");
            return;
        }
        if (personalizedMember!.epochTime + 60 > currentEpoch) {
            await ctx.RespondAsync($"You need to wait {personalizedMember.epochTime + 60 - currentEpoch} seconds before you can use this command again.");
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
        await memberRole!.ModifyAsync(color: Colors.HexToColor(newColorString), reason: "Personalized Member - User");
        await ctx.RespondAsync("Successfully updated your personalized member role color.");
    }
    
    [Command("RoleName"), Description("Update the name of your personalized role for this server"), LockCommandForLilysComfyCorner]
    public async Task NameRole(CommandContext ctx, [Description("Desired Role Name (15 characters)"), RemainingText] string name = "") {
        if (!Config.Base.PersonalizedMember.Enabled) {
            var msg = await ctx.RespondAsync("Personalized Roles is not enabled.");
            await Task.Delay(TimeSpan.FromSeconds(5));
            await msg.DeleteAsync();
            await ctx.Message.DeleteAsync();
            return;
        }
        if (!_IsInChannel(ctx)) {
            var msg = await ctx.RespondAsync($"You can only use this command in <#{Config.Base.PersonalizedMember.ChannelId}>");
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
        var personalizedMember = Config.Base.PersonalizedMember.Members!.FirstOrDefault(x => x.userId == ctx.User.Id);
        if (personalizedMember is null) {
            await ctx.RespondAsync($"You need to create a personalized role first.\nRun the following command to create one:\n`{Config.Base.Prefix}rolecreate`");
            return;
        }
        if (personalizedMember.epochTime + 60 > currentEpoch) {
            await ctx.RespondAsync($"You need to wait {personalizedMember.epochTime + 60 - currentEpoch} seconds before you can use this command again.");
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
        await memberRole!.ModifyAsync(name: newRoleName, reason: "Personalized Member - User");
        await ctx.RespondAsync("Successfully updated your personalized member role name.");
    }
    
    // TODO: (Only if people abuse the current feature set) public static Dictionary<ulong, bool> RoleRemovalConfirmation = new Dictionary<ulong, bool>();
    [Command("RoleRemove"), Description("Remove your personalized role for this server"), Aliases("RoleDel", "RoleDelete", "rolerm"), LockCommandForLilysComfyCorner]
    public async Task RemoveRole(CommandContext ctx) {
        if (!Config.Base.PersonalizedMember.Enabled) {
            var msg = await ctx.RespondAsync("Personalized Roles is not enabled.");
            await Task.Delay(TimeSpan.FromSeconds(5));
            await msg.DeleteAsync();
            await ctx.Message.DeleteAsync();
            return;
        }
        if (!_IsInChannel(ctx)) {
            var msg = await ctx.RespondAsync($"You can only use this command in <#{Config.Base.PersonalizedMember.ChannelId}>");
            await Task.Delay(TimeSpan.FromSeconds(5));
            await msg.DeleteAsync();
            await ctx.Message.DeleteAsync();
            return;
        }
        var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var personalizedMember = Config.Base.PersonalizedMember.Members!.FirstOrDefault(x => x.userId == ctx.User.Id);
        if (personalizedMember is null) {
            await ctx.RespondAsync($"You need to create a personalized role first.\nRun the following command to create one:\n`{Config.Base.Prefix}rolecreate`");
            return;
        }
        if (personalizedMember.epochTime + 60 > currentEpoch) {
            await ctx.RespondAsync($"You need to wait {personalizedMember.epochTime + 60 - currentEpoch} seconds before you can use this command again.");
            return;
        }
        var memberRole = ctx.Guild.GetRole(personalizedMember!.roleId);
        await memberRole!.DeleteAsync(reason: "Personalized Member - User");
        Config.Base.PersonalizedMember.Members!.Remove(personalizedMember);
        Config.Save();
        await ctx.RespondAsync("Successfully removed your personalized member role.");
    }

    /*[Command("role"), Description("Create a personalized role for this server"), LockCommandForOnlyLilysComfyCorner]
    public async Task Role(CommandContext ctx, 
        [Description("Command action (set, update, remove)")]    string action = "",
        [Description("Role Color as Hex (#rrggbb)")]             string color = "",
        [Description("Role Name (if empty, will be username)")]  string name = "") {
        
        if (ctx.Channel.Id != Config.Base.PersonalizedMember.ChannelId) {
            await ctx.RespondAsync($"You can only use this command in <#{Config.Base.PersonalizedMember.ChannelId}>");
            return;
        }
        
        if (string.IsNullOrWhiteSpace(action)) {
            await ctx.RespondAsync("You need to specify an action. (`set`, `update`, `remove`)");
            return;
        }

        if (string.IsNullOrWhiteSpace(color)) {
            await ctx.RespondAsync($"Please use the correct format: `{Config.Base.Prefix}role <action> <color> <name>`");
            return;
        }

        if (string.IsNullOrWhiteSpace(name)) {
            name = ctx.User.Username;
        }
        
        var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var member = ctx.Member;
        var personalizedMember = Config.Base.PersonalizedMember.Members!.FirstOrDefault(x => x.userId == member?.Id);
        var newColorString = color.ValidateHexColor().Left(6);
        if (newColorString.Length < 6) {
            await ctx.RespondAsync("You need to specify a __valid__ color. (`#000000`)");
            return;
        }
        var memberRole = 
            personalizedMember is null ?
                // if null, create role
                await ctx.Guild.CreateRoleAsync(name:name.Left(12), color:Colors.HexToColor(newColorString), mentionable:false, hoist:false, reason:"Personalized Member - User") :
                // if not null, get role
                ctx.Guild.GetRole(personalizedMember!.roleId);
        
        if (personalizedMember!.epochTime + 60 > currentEpoch) {
            await ctx.RespondAsync($"You need to wait {personalizedMember.epochTime + 60 - currentEpoch} seconds before you can use this command again.");
            return;
        }

        string newActionString;

        switch (action) {
            case "set":
                newActionString = "set";
                // already created the role above
                break;
            
            case "update":
                newActionString = "updated";
                await memberRole!.ModifyAsync(name:name.Left(12), color:Colors.HexToColor(newColorString), reason:"Personalized Member - User");
                break;
            
            case "remove":
                newActionString = "removed";
                await memberRole!.DeleteAsync(reason:"Personalized Member - User");
                break;
            
            default:
                await ctx.RespondAsync("You need to specify a __valid__ action. (`set`, `update`, `remove`)");
                return;
        }

        var personalizedMemberData = new Member {
            userId = ctx.User.Id,
            roleId = memberRole.Id,
            roleName = name.Left(12),
            colorHex = newColorString,
            epochTime = currentEpoch
        };
        // TODO: Add update if already exists
        Config.Base.PersonalizedMember.Members!.Add(personalizedMemberData);
        Config.Save();
        await ctx.RespondAsync($"Successfully {newActionString} your personalized member role.");
    }*/
    
}