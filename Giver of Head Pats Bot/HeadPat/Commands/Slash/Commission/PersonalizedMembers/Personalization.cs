using System.Text;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HeadPats.Configuration;
using HeadPats.Configuration.Classes;
using HeadPats.Handlers.CommandAttributes;
using HeadPats.Handlers.Events;
using HeadPats.Utils;

namespace HeadPats.Commands.Slash.Commission.PersonalizedMembers; 

public class Personalization : ApplicationCommandModule {

    [SlashCommandGroup("Personalization", "Personalization commands", false), SlashBangerCommand(false)]
    public class PersonalizedMembers : ApplicationCommandModule {
        
        #region Admin
        
        [SlashCommand("toggle", "(Admin) Toggles the Personalized Member system"), SlashBangerCommand(true)]
        public async Task TogglePersonalizedMember(InteractionContext ctx, 
            [Option("Enabled", "Enable or disable the Personalized Member system"),
             Choice("true", "true"), Choice("false", "false")] string enabled) {
            var result = enabled.ToLower().Contains('t');
            var personalData = Config.PersonalizedMember(ctx.Guild.Id);
            personalData.Enabled = result;
            Config.Save();
            await ctx.CreateResponseAsync($"Personalized Roles are now {(result ? "enabled" : "disabled")}.");
        }
        
        [SlashCommand("setchannel", "(Admin) Sets the channel where users can use the personalization commands"), SlashBangerCommand(true)]
        public async Task SetPersonalizationChannel(InteractionContext ctx, 
            [Option("Destination", "Destination Discord Channel (mention)", true)] DiscordChannel channel) {
            var personalData = Config.PersonalizedMember(ctx.Guild.Id);
            personalData.ChannelId = channel.Id;
            Config.Save();
            await ctx.CreateResponseAsync($"Set the personalization channel to {channel.Mention}.");
        }
        
        [SlashCommand("setdefaultrole", "(Admin) Sets the default role for users to be granted when they remove their personalized role"), SlashBangerCommand(true)]
        public async Task SetDefaultRole(InteractionContext ctx, 
            [Option("Role", "Role to set as default", true)] DiscordRole role) {
            var personalData = Config.PersonalizedMember(ctx.Guild.Id);
            personalData.DefaultRoleId = role.Id;
            Config.Save();
            await ctx.CreateResponseAsync($"Set the default role to {role.Mention}.");
        }
        
        [SlashCommand("setresettime", "(Admin) Sets the artificial cooldown for the role command"), SlashBangerCommand(true)]
        public async Task SetResetTime(InteractionContext ctx, 
            [Option("Time", "Time in seconds", true)] string time) {
            var personalData = Config.PersonalizedMember(ctx.Guild.Id);
            personalData.ResetTimer = time.AsInt();
            Config.Save();
            await ctx.CreateResponseAsync($"Set the reset time to {time} seconds.");
        }
        
        [SlashCommand("addroleto", "(Admin) Adds a personalized role to a user"), SlashBangerCommand(true)]
        public async Task AddRoleTo(InteractionContext ctx, 
            [Option("User", "Target user (mention)", true)] DiscordUser user,
            [Option("Role", "Role to add (ID)", true)] string roleId) {
            if (string.IsNullOrWhiteSpace(roleId)) {
                await ctx.CreateResponseAsync("Please specify a role ID.", true);
                return;
            }
            var personalData = Config.PersonalizedMember(ctx.Guild.Id);
            var ulongRoleId = ulong.Parse(roleId);
            var role = ctx.Guild.GetRole(ulongRoleId);
            if (role is null) {
                await ctx.CreateResponseAsync("The role does not exist is guild.", true);
                return;
            }
            if (personalData.Members!.Any(x => x.roleId == ulongRoleId)) {
                await ctx.CreateResponseAsync("The role is already in the system, it cannot be one more than one person.");
                return;
            }
            personalData.Members!.Add(new Member {
                userId = user.Id,
                roleId = ulongRoleId,
                roleName = role.Name,
                colorHex = role.Color.ToString().ValidateHexColor().ToLower(),
                epochTime = 1002
            });
            Config.Save();
            var discordMember = await ctx.Guild.GetMemberAsync(user.Id);
            await ctx.CreateResponseAsync($"Added **{role.Name}** to the personalized system for **{discordMember.DisplayName}**.");
        }
        
        [SlashCommand("removefrom", "(Admin) Removes a personalized role from a user"), SlashBangerCommand(true)]
        public async Task RemoveFrom(InteractionContext ctx, 
            [Option("User", "Target user (mention)", true)] DiscordUser user) {
            var personalData = Config.PersonalizedMember(ctx.Guild.Id);
    
            var memberData = personalData.Members!.FirstOrDefault(x => x.userId == user.Id);
            if (memberData is null) {
                await ctx.CreateResponseAsync("User data does not exist.", true);
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
            await ctx.CreateResponseAsync($"Removed {discordMember.DisplayName}'s personalized role.");
        }
        
        #endregion
        
        #region Anyone
        
        private static bool _IsInChannel(BaseContext ctx, ulong guildId) => ctx.Channel.Id == Config.PersonalizedMember(guildId).ChannelId;
        
        [SlashCommand("createrole", "Creates a role for you"), SlashBangerCommand(false)]
        public async Task CreateRole(InteractionContext ctx, 
            [Option("RoleName", "Name of the role to create", true)] string roleName,
            [Option("Color", "Color of the role to create", true)] string colorHex) {
            var personalData = Config.PersonalizedMember(ctx.Guild.Id);
            if (!personalData.Enabled) {
                await ctx.CreateResponseAsync("Personalized Roles is not enabled.", true);
                return;
            }
            if (!_IsInChannel(ctx, ctx.Guild.Id)) {
                await ctx.CreateResponseAsync($"You can only use this command in <#{personalData.ChannelId}>", true);
                return;
            }
            var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var personalizedMember = personalData.Members!.FirstOrDefault(x => x.userId == ctx.User.Id);
            if (personalizedMember is null) {
                var memberRole = await ctx.Guild.CreateRoleAsync(name: ctx.User.Username, reason: "Personalized Member - User");
                var newColorString = colorHex.ValidateHexColor().Left(6);
                await Task.Delay(TimeSpan.FromSeconds(1));
                var personalizedMemberData = new Member {
                    userId = ctx.User.Id,
                    roleId = memberRole.Id,
                    roleName = ctx.User.Username.Left(15).Trim(),
                    colorHex = newColorString,
                    epochTime = currentEpoch
                };
                personalData.Members!.Add(personalizedMemberData);
                Config.Save();
                await ctx.Member!.GrantRoleAsync(memberRole, "Personalized Member - User: " + ctx.User.Username);
                if (personalData.DefaultRoleId != 0) {
                    var defaultRole = ctx.Guild.GetRole(personalData.DefaultRoleId);
                    await ctx.Member!.RevokeRoleAsync(defaultRole, "Personalized Member - User: " + ctx.User.Username);
                }
                await ctx.CreateResponseAsync("Successfully created your personalized member role.");
                return;
            }

            await ctx.CreateResponseAsync("You already have a personalized role.", true);
        }
        
        [SlashCommand("deleterole", "Deletes your personalized role"), SlashBangerCommand(false)]
        public async Task DeleteRole(InteractionContext ctx) {
            var personalData = Config.PersonalizedMember(ctx.Guild.Id);
            if (!personalData.Enabled) {
                await ctx.CreateResponseAsync("Personalized Roles is not enabled.", true);
                return;
            }
            if (!_IsInChannel(ctx, ctx.Guild.Id)) {
                await ctx.CreateResponseAsync($"You can only use this command in <#{personalData.ChannelId}>", true);
                return;
            }
            var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var personalizedMember = personalData.Members!.FirstOrDefault(x => x.userId == ctx.User.Id);
            if (personalizedMember is null) {
                await ctx.CreateResponseAsync("You need to create a personalized role first.\nRun the following command to create one:\n`/personalization createrole`", true);
                return;
            }
            if (personalizedMember.epochTime + personalData.ResetTimer > currentEpoch) {
                await ctx.CreateResponseAsync($"You need to wait {personalizedMember.epochTime + personalData.ResetTimer - currentEpoch} seconds before you can use this command again.", true);
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
            await ctx.CreateResponseAsync("Successfully removed your personalized member role.");
        }
        
        [SlashCommand("updatecolor", "Changes your role color"), SlashBangerCommand(false)]
        public async Task UpdateColor(InteractionContext ctx, 
            [Option("Color", "Color of the role to create", true)] string colorHex) {
            var personalData = Config.PersonalizedMember(ctx.Guild.Id);
            if (!personalData.Enabled) {
                await ctx.CreateResponseAsync("Personalized Roles is not enabled.", true);
                return;
            }
            if (!_IsInChannel(ctx, ctx.Guild.Id)) {
                await ctx.CreateResponseAsync($"You can only use this command in <#{personalData.ChannelId}>", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(colorHex)) {
                await ctx.CreateResponseAsync("Color string cannot be empty.", true);
                return;
            }
            var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var personalizedMember = personalData.Members!.FirstOrDefault(x => x.userId == ctx.User.Id);
            if (personalizedMember is null) {
                await ctx.CreateResponseAsync("You need to create a personalized role first.\nRun the following command to create one:\n`/personalization createrole`", true);
                return;
            }
            if (personalizedMember!.epochTime + personalData.ResetTimer > currentEpoch) {
                await ctx.CreateResponseAsync($"You need to wait {personalizedMember.epochTime + personalData.ResetTimer - currentEpoch} seconds before you can use this command again.", true);
                return;
            }
            var memberRole = ctx.Guild.GetRole(personalizedMember.roleId);
            var newColorString = colorHex.ValidateHexColor().Left(6);
            if (personalizedMember.colorHex == newColorString) {
                await ctx.CreateResponseAsync("Your personalized member role color is already set to that.", true);
                return;
            }
            personalizedMember.colorHex = newColorString;
            Config.Save();
            await memberRole!.ModifyAsync(color: Colors.HexToColor(newColorString), reason: "Personalized Member - User: " + ctx.User.Username);
            await ctx.CreateResponseAsync("Successfully updated your personalized member role color.");
        }
        
        [SlashCommand("updatename", "Changes your role name"), SlashBangerCommand(false)]
        public async Task UpdateName(InteractionContext ctx, 
            [Option("Name", "Name of the role to create", true)] string name) {
            var personalData = Config.PersonalizedMember(ctx.Guild.Id);
            if (!personalData.Enabled) {
                await ctx.CreateResponseAsync("Personalized Roles is not enabled.", true);
                return;
            }
            if (!_IsInChannel(ctx, ctx.Guild.Id)) {
                await ctx.CreateResponseAsync($"You can only use this command in <#{personalData.ChannelId}>", true);
                return;
            }
            if (string.IsNullOrWhiteSpace(name)) {
                await ctx.CreateResponseAsync("Name string cannot be empty.", true);
                return;
            }
            if (name.Length > 15) {
                await ctx.CreateResponseAsync("Name string is longer than 15 characters, only the first 15 will be used.", true);
            }
            var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var personalizedMember = personalData.Members!.FirstOrDefault(x => x.userId == ctx.User.Id);
            if (personalizedMember is null) {
                await ctx.CreateResponseAsync("You need to create a personalized role first.\nRun the following command to create one:\n`/personalization createrole`", true);
                return;
            }
            if (personalizedMember.epochTime + personalData.ResetTimer > currentEpoch) {
                await ctx.CreateResponseAsync($"You need to wait {personalizedMember.epochTime + personalData.ResetTimer - currentEpoch} seconds before you can use this command again.", true);
                return;
            }
            var newRoleName = name.Sanitize().Left(15).Trim();
            if (personalizedMember.roleName == newRoleName) {
                await ctx.CreateResponseAsync("Your personalized member role name is already set to that.", true);
                return;
            }
            var memberRole = ctx.Guild.GetRole(personalizedMember.roleId);
            personalizedMember.roleName = newRoleName;
            Config.Save();
            await memberRole!.ModifyAsync(name: newRoleName, reason: "Personalized Member - User: " + ctx.User.Username);
            await ctx.CreateResponseAsync("Successfully updated your personalized member role name.");
        }
        
        #endregion
        
    }
}