/*using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using HeadPats.Data;
using HeadPats.Handlers;
using static System.UInt64;
using ic = DSharpPlus.SlashCommands.InteractionContext;

namespace HeadPats.Commands.Modules; 

public class Moderation : ApplicationCommandModule {

    [SlashCommand("EnableModerationModule", "Enables the moderation module.")]
    [SlashRequirePermissions(Permissions.Administrator)]
    public async Task EnableModeration(ic c,
        [Choice("false", "0")]
        [Choice("true", "1")]
        [Option("EnableModeration", "Enables the moderation module.")] string enable) {
        var enabled = enable == "1";
        /*await using var moderationDb = new ModerationModuleContext();
        var check = moderationDb.Moderation.AsQueryable().Where(m => m.GuildId == BuildInfo.TestGuildId).ToList().FirstOrDefault();
        if (check == null) {
            moderationDb.Moderation.Add(new HeadPats.Data.Models.Moderation {
                GuildId = c.Guild.Id,
                Enabled = enabled
            });
        } else 
            check.Enabled = enabled;

        await moderationDb.SaveChangesAsync();#1#
        
        Program.GuildCommandCheckList.FirstOrDefault(t => t.GuildId == c.Guild.Id)!.Enabled = enabled;
        
        await c.CreateResponseAsync($"Moderation module has been {(enabled ? "enabled" : "disabled")}.", true);
    }
    
    [SlashCommand("Kick", "Kick a user from the server.")]
    [SlashRequirePermissions(Permissions.KickMembers)]
    [HasModerationModuleEnabled]
    public async Task Kick(ic ctx, 
        [Option("user", "The user to kick.")] DiscordUser user, 
        [Option("reason", "The reason for the kick.")] string reason = "No reason given.") {
        var member = await ctx.Guild.GetMemberAsync(user.Id);
        await ctx.CreateResponseAsync($"{member.Mention} has been kicked from the server for {reason}");
        await member.RemoveAsync(reason);
    }
    
    [SlashCommand("Ban", "Ban a user from the server.")]
    [SlashRequirePermissions(Permissions.BanMembers)]
    [HasModerationModuleEnabled]
    public async Task Ban(ic ctx, 
        [Option("user", "The user to ban.")] DiscordUser user, 
        [Option("reason", "The reason for the ban.")] string reason = "No reason given.",
        [Option("days", "Days of messages to delete.")] string days = "0") {
        var member = await ctx.Guild.GetMemberAsync(user.Id);
        await ctx.CreateResponseAsync($"{member.Mention} has been banned from the server for {reason}");
        int _days;
        try { _days = int.Parse(days); } catch { _days = 0; }
        await ctx.Guild.BanMemberAsync(member, _days, reason);
    }
    
    [SlashCommand("Softban", "Bans a user, deletes all of their messages, and then unbans them.")]
    [SlashRequirePermissions(Permissions.BanMembers)]
    [HasModerationModuleEnabled]
    public async Task Softban(ic ctx, 
        [Option("user", "The user to softban.")] DiscordUser user, 
        [Option("reason", "The reason for the softban.")] string reason = "No reason given.") {
        var member = await ctx.Guild.GetMemberAsync(user.Id);
        await ctx.CreateResponseAsync($"{member.Mention} has been softbanned from the server for {reason}");
        await ctx.Guild.BanMemberAsync(member, 7, reason);
        await Task.Delay(2000);
        await member.RemoveAsync(reason);
    }
    
    [SlashCommand("Unban", "Unban a user from the server.")]
    [SlashRequirePermissions(Permissions.BanMembers)]
    [HasModerationModuleEnabled]
    public async Task Unban(ic ctx, 
        [Option("UserID", "The user to unban via User ID.")] string userId, 
        [Option("reason", "The reason for the unban.")] string reason = "No reason given.") {
        var id = Parse(userId);
        var member = await ctx.Guild.GetMemberAsync(id);
        DiscordBan? discordBan;
        try {
            discordBan = await ctx.Guild.GetBanAsync(id);
        }
        catch {
            await ctx.CreateResponseAsync($"{member.Mention} ({id}) is not banned from the server.");
            return;
        }
        await ctx.CreateResponseAsync($"{discordBan.User} has been unbanned from the server for {reason}");
        await ctx.Guild.UnbanMemberAsync(id, reason);
    }

    [SlashCommand("Slowmode", "Set the slowmode for the channel.")]
    [SlashRequirePermissions(Permissions.ManageChannels)]
    [HasModerationModuleEnabled]
    public async Task SlowMode(ic ic,
        [Choice("off", "0")]
        [Choice("5 seconds", "1")]
        [Choice("10 seconds", "2")]
        [Choice("15 seconds", "3")]
        [Choice("30 seconds", "4")]
        [Choice("1 minute", "5")]
        [Choice("2 minutes", "6")]
        [Choice("5 minutes", "7")]
        [Choice("10 minutes", "8")]
        [Choice("15 minutes", "9")]
        [Choice("30 minutes", "10")]
        [Choice("1 hour", "11")]
        [Choice("2 hours", "12")]
        [Choice("6 hours", "13")]
        [Option("Time", "The time to set the slowmode to.")] string time = "0") {
        var channel = ic.Channel;
        var timeValue = int.Parse(time);
        await channel.ModifyAsync(c => c.PerUserRateLimit = timeValue);
        await ic.CreateResponseAsync($"Slowmode has been set to {SlowModeType(timeValue)}.", true);
    }

    private string SlowModeType(int input)
        => input switch {
            0 => "off", 1 => "5 seconds", 2 => "10 seconds", 3 => "15 seconds", 4 => "30 seconds",
            5 => "1 minute", 6 => "2 minutes", 7 => "5 minutes", 8 => "10 minutes", 9 => "15 minutes",
            10 => "30 minutes", 11 => "1 hour", 12 => "2 hours", 13 => "6 hours", _ => "off"
        };
}*/