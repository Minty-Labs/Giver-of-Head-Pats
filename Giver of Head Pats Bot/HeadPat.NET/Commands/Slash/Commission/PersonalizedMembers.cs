﻿using Discord;
using Discord.Interactions;
using HeadPats.Commands.Preexecution;
using HeadPats.Configuration;
using HeadPats.Configuration.Classes;
using HeadPats.Utils;

namespace HeadPats.Commands.Slash.Commission; 

public class PersonalizedMembers : InteractionModuleBase<SocketInteractionContext> {
    private static bool _IsInChannel(SocketInteractionContext context, ulong guildId) => context.Channel.Id == Config.PersonalizedMember(guildId).ChannelId;

    [Group("personalization", "Personalized Members Commands"), EnabledInDm(false)]
    public class Commands : InteractionModuleBase<SocketInteractionContext> {

        [SlashCommand("createrole", "Creates a personalized role for you")]
        public async Task CreateRole([Summary("color", "Role Color (hex)")] string colorHex) {
            var personalData = Config.PersonalizedMember(Context.Guild.Id);
            if (!personalData.Enabled) {
                await RespondAsync("Personalized Roles is not enabled.", ephemeral: true);
                return;
            }
            if (!_IsInChannel(Context, Context.Guild.Id)) {
                await RespondAsync($"You can only use this command in <#{personalData.ChannelId}>", ephemeral: true);
                return;
            }
            var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var personalizedMember = personalData.Members!.FirstOrDefault(x => x.userId == Context.User.Id);
            
            if (personalizedMember is null) {
                var memberRole = await Context.Guild.CreateRoleAsync(Context.User.Username.Left(15).Trim(), options: new RequestOptions {AuditLogReason = "Personalized Member - User"});
                var newColorString = colorHex.ValidateHexColor().Left(6);
                await Task.Delay(TimeSpan.FromSeconds(0.5f));
                var personalizedMemberData = new Member {
                    userId = Context.User.Id,
                    roleId = memberRole.Id,
                    roleName = Context.User.Username.Left(15).Trim(),
                    colorHex = newColorString,
                    epochTime = currentEpoch
                };
                personalData.Members!.Add(personalizedMemberData);
                Config.Save();
                var discordMember = Context.User as IGuildUser;
                await discordMember!.AddRoleAsync(memberRole, new RequestOptions {AuditLogReason = "Personalized Member - User: " + Context.User.Username});
                if (personalData.DefaultRoleId != 0) {
                    var defaultRole = Context.Guild.GetRole(personalData.DefaultRoleId);
                    await discordMember!.RemoveRoleAsync(defaultRole, new RequestOptions {AuditLogReason = "Personalized Member - User: " + Context.User.Username});
                }
                await RespondAsync("Successfully created your personalized member role.");
                return;
            }

            await RespondAsync("You already have a personalized role.", ephemeral: true);
        }

        [SlashCommand("deleterole", "Removes your personalized role")]
        public async Task DeleteRole() {
            var personalData = Config.PersonalizedMember(Context.Guild.Id);
            if (!personalData.Enabled) {
                await RespondAsync("Personalized Roles is not enabled.", ephemeral: true);
                return;
            }
            if (!_IsInChannel(Context, Context.Guild.Id)) {
                await RespondAsync($"You can only use this command in <#{personalData.ChannelId}>", ephemeral: true);
                return;
            }
            var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var personalizedMember = personalData.Members!.FirstOrDefault(x => x.userId == Context.User.Id);
            if (personalizedMember is null) {
                await RespondAsync("You need to create a personalized role first.\nRun the following command to create one:\n`/personalization createrole`", ephemeral: true);
                return;
            }
            if (personalizedMember.epochTime + personalData.ResetTimer > currentEpoch) {
                await RespondAsync($"You need to wait {personalizedMember.epochTime + personalData.ResetTimer - currentEpoch} seconds before you can use this command again.", ephemeral: true);
                return;
            }
            var memberRole = Context.Guild.GetRole(personalizedMember!.roleId);
            await memberRole!.DeleteAsync(new RequestOptions {AuditLogReason = "Personalized Member - User: " + Context.User.Username});
            personalData.Members!.Remove(personalizedMember);
            Config.Save();
            if (personalData.DefaultRoleId != 0) {
                var defaultRole = Context.Guild.GetRole(personalData.DefaultRoleId);
                var discordMember = Context.User as IGuildUser;
                await discordMember!.AddRoleAsync(defaultRole, new RequestOptions { AuditLogReason = "Personalized Member - User: " + Context.User.Username });
            }
            await RespondAsync("Successfully removed your personalized member role.");
        }

        [SlashCommand("updatecolor", "Updates your personalized role's color")]
        public async Task UpdateColor([Summary("color", "Role Color (hex)")] string colorHex) {
            var personalData = Config.PersonalizedMember(Context.Guild.Id);
            if (!personalData.Enabled) {
                await RespondAsync("Personalized Roles is not enabled.", ephemeral: true);
                return;
            }
            if (!_IsInChannel(Context, Context.Guild.Id)) {
                await RespondAsync($"You can only use this command in <#{personalData.ChannelId}>", ephemeral: true);
                return;
            }

            if (string.IsNullOrWhiteSpace(colorHex)) {
                await RespondAsync("Color string cannot be empty.", ephemeral: true);
                return;
            }
            var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var personalizedMember = personalData.Members!.FirstOrDefault(x => x.userId == Context.User.Id);
            if (personalizedMember is null) {
                await RespondAsync("You need to create a personalized role first.\nRun the following command to create one:\n`/personalization createrole`", ephemeral: true);
                return;
            }
            if (personalizedMember!.epochTime + personalData.ResetTimer > currentEpoch) {
                await RespondAsync($"You need to wait {personalizedMember.epochTime + personalData.ResetTimer - currentEpoch} seconds before you can use this command again.", ephemeral: true);
                return;
            }
            var memberRole = Context.Guild.GetRole(personalizedMember.roleId);
            var newColorString = colorHex.ValidateHexColor().Left(6);
            if (personalizedMember.colorHex == newColorString) {
                await RespondAsync("Your personalized member role color is already set to that.", ephemeral: true);
                return;
            }
            personalizedMember.colorHex = newColorString;
            Config.Save();
            await memberRole!.ModifyAsync(x => x.Color = Colors.HexToColor(newColorString), new RequestOptions {AuditLogReason = "Personalized Member - User: " + Context.User.Username});
            await RespondAsync("Successfully updated your personalized member role color.");
        }

        [SlashCommand("updatename", "Updates your personalized role's name")]
        public async Task UpdateName([Summary("name", "Name of your role")] string name) {
            var personalData = Config.PersonalizedMember(Context.Guild.Id);
            if (!personalData.Enabled) {
                await RespondAsync("Personalized Roles is not enabled.", ephemeral: true);
                return;
            }
            if (!_IsInChannel(Context, Context.Guild.Id)) {
                await RespondAsync($"You can only use this command in <#{personalData.ChannelId}>", ephemeral: true);
                return;
            }

            if (string.IsNullOrWhiteSpace(name)) {
                await RespondAsync("Name string cannot be empty.", ephemeral: true);
                return;
            }
            if (name.Length > 15) {
                await RespondAsync("Name string is longer than 15 characters, only the first 15 will be used.", ephemeral: true);
            }
            var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var personalizedMember = personalData.Members!.FirstOrDefault(x => x.userId == Context.User.Id);
            if (personalizedMember is null) {
                await RespondAsync("You need to create a personalized role first.\nRun the following command to create one:\n`/personalization createrole`", ephemeral: true);
                return;
            }
            if (personalizedMember!.epochTime + personalData.ResetTimer > currentEpoch) {
                await RespondAsync($"You need to wait {personalizedMember.epochTime + personalData.ResetTimer - currentEpoch} seconds before you can use this command again.", ephemeral: true);
                return;
            }
            var newRoleName = name.Sanitize().Left(15).Trim();
            if (personalizedMember.roleName == newRoleName) {
                await RespondAsync("Your personalized member role name is already set to that.", ephemeral: true);
                return;
            }
            var memberRole = Context.Guild.GetRole(personalizedMember.roleId);
            personalizedMember.roleName = newRoleName;
            Config.Save();
            await memberRole!.ModifyAsync(x => x.Name = newRoleName, new RequestOptions {AuditLogReason = "Personalized Member - User: " + Context.User.Username});
            await RespondAsync("Successfully updated your personalized member role name.");
        }
    }

    [Group("personalizationadmin", "Personalized Members Admin Commands"), EnabledInDm(false)]
    public class AdminCommands : InteractionModuleBase<SocketInteractionContext> {
        
        [SlashCommand("toggle", "Toggles the personalized members system"), RequireUser(875251523641294869, 167335587488071682)]
        public async Task TogglePersonalizedMembersSystem([Summary("toggle", "Enable or disable the personalized members system")] bool enabled) {
            var personalData = Config.PersonalizedMember(Context.Guild.Id);
            personalData.Enabled = enabled;
            Config.Save();
            await RespondAsync($"Personalized Members are now {(enabled ? "enabled" : "disabled")}.");
        }
        
        [SlashCommand("setchannel", "Sets the channel to only personalized members"), RequireUser(875251523641294869, 167335587488071682)]
        public async Task SetPersonalizedMembersChannel([Summary("channel", "Destination Discord Channel")] ITextChannel channel) {
            var personalData = Config.PersonalizedMember(Context.Guild.Id);
            personalData.ChannelId = channel.Id;
            Config.Save();
            await RespondAsync($"Set Personalized Members channel to {channel.Mention}.");
        }
        
        [SlashCommand("setdefaultrole", "Sets the default role for users to be granted when they remove their personalized role"), RequireUser(875251523641294869, 167335587488071682)]
        public async Task SetDefaultRole([Summary("role", "Role to set as the default role")] IRole role) {
            var personalData = Config.PersonalizedMember(Context.Guild.Id);
            personalData.DefaultRoleId = role.Id;
            Config.Save();
            await RespondAsync($"Set default role to {role.Mention}.");
        }
        
        [SlashCommand("setresettime", "Sets the time in seconds for when a user's personalized role is reset"), RequireUser(875251523641294869, 167335587488071682)]
        public async Task SetResetTime([Summary("time", "Time in seconds")] int time) {
            var personalData = Config.PersonalizedMember(Context.Guild.Id);
            personalData.ResetTimer = time;
            Config.Save();
            await RespondAsync($"Set reset time to {time} seconds.");
        }
        
        [SlashCommand("addroleto", "Adds a role to the personalized members list"), RequireUser(875251523641294869, 167335587488071682)]
        public async Task AddRoleToPersonalizedMembers(
            [Summary("user", "User to add the role to")] IUser user,
            [Summary("role", "Role to add to the personalized members list")] IRole role) {
            var personalData = Config.PersonalizedMember(Context.Guild.Id);
            if (personalData.Members!.Any(x => x.roleId == role.Id)) {
                await RespondAsync("The role is already in the system, it cannot be one more than one person.", ephemeral: true);
                return;
            }
            personalData.Members!.Add(new Member {
                userId = user.Id,
                roleId = role.Id,
                roleName = role.Name,
                colorHex = (role.Color.ToString() ?? string.Empty).ValidateHexColor().ToLower(),
                epochTime = 1002
            });
            Config.Save();
            var discordMember = (IGuildUser)user;
            await RespondAsync($"Added **{role.Name}** to the personalized system for **{discordMember.DisplayName}**.");
        }
        
        [SlashCommand("removerolefrom", "Removes a role from the personalized members list"), RequireUser(875251523641294869, 167335587488071682)]
        public async Task RemoveRoleFromPersonalizedMembers(
            [Summary("user", "User to remove the role from")] IUser user) {
            var personalData = Config.PersonalizedMember(Context.Guild.Id);
            var memberData = personalData.Members!.FirstOrDefault(x => x.userId == user.Id);
            if (memberData is null) {
                await RespondAsync("User data does not exist.", ephemeral: true);
                return;
            }

            var memberRole = Context.Guild.GetRole(memberData.roleId);
            await memberRole.DeleteAsync(new RequestOptions {AuditLogReason = "Personalized Member - Admin: " + Context.User.Username});
            personalData.Members!.Remove(memberData);
            Config.Save();
            var discordMember = (IGuildUser)user;
            if (personalData.DefaultRoleId != 0) {
                var defaultRole = Context.Guild.GetRole(personalData.DefaultRoleId);
                await discordMember.AddRoleAsync(defaultRole, new RequestOptions {AuditLogReason = "Personalized Member - Admin: " + Context.User.Username});
            }
            await RespondAsync($"Removed {discordMember.DisplayName}'s personalized role.");
        }
        
    }
}