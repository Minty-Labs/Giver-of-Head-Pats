using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Managers;
using Serilog;

namespace HeadPats.Commands.Legacy.Owner; 

public class UserControl : BaseCommandModule {
    [Command("MultiKick"), Aliases("mk"), Description("Kicks multiple users at once"), RequireOwner, RequirePermissions(Permissions.KickMembers)]
    public async Task MultiKick(CommandContext c, [Description("List of users separated by commas")] string userIds,
        [Description("Reason for kick"), RemainingText] string reason = "No reason provided.") {
        if (string.IsNullOrWhiteSpace(userIds)) {
            await c.RespondAsync("Please provide one or more user IDs, separated by commas.");
            return;
        }
        
        var ulongList = userIds.Split(',').Select(ulong.Parse).ToList();
        var num = 0;
        var usernameList = new List<string>();
        var message = await c.RespondAsync("Kicking users: ");
        foreach (var id in ulongList) {
            if (num != 0) // Instantly do the first, delay the rest
                await Task.Delay(TimeSpan.FromSeconds(2));

            var user = await c.Guild.GetMemberAsync(id);
            usernameList.Add(user.Username);
            await message.ModifyAsync($"Kicking users: {string.Join(", ", usernameList)}");
            await user.RemoveAsync(reason);
            num++;
        }
        
        await c.RespondAsync($"Finished kicking {num} users.");
        await message.ModifyAsync($"Kicked users: {string.Join(", ", usernameList)}");
    }
    
    [Command("ForceAddUserToDatabase"), Aliases("fautd"), RequireOwner]
    public async Task ForceAddUserToDatabase(CommandContext c, [RemainingText] string userIdStr) {
        await using var db = new Context();
        var userId = ulong.Parse(userIdStr);
        var discordUser = await c.Client.GetUserAsync(userId);
        
        var checkUser = db.Users.AsQueryable()
            .Where(u => u.UserId.Equals(userId)).ToList().FirstOrDefault();
        
        if (checkUser is null) {
            var newUser = new Users {
                UserId = discordUser.Id,
                UsernameWithNumber = $"{discordUser.Username}",
                PatCount = 0,
                CookieCount = 0,
                IsUserBlacklisted = 0
            };
            Log.Debug("Added user to database from a force command");
            db.Users.Add(newUser);
            await c.RespondAsync("Done.");
        }
        
        await db.SaveChangesAsync();
    }
    
    [Command("GetUserPresence"), Aliases("gup"), Description("Gets users with the given presence"), RequireOwner]
    public async Task GetPresence(CommandContext c, [RemainingText, Description("Text to search presences with")] string activity) {
        if (string.IsNullOrWhiteSpace(activity)) {
            await c.RespondAsync("Activity search field was empty").DeleteAfter(3);
            return;
        }
        try {
            await c.Guild.RequestMembersAsync(presences: true);
            
            List<DiscordMember> users = 
                (
                    from member in c.Guild.Members 
                    where member.Value?.Presence?.Activities != null 
                    from discordActivity in member.Value.Presence.Activities 
                    where discordActivity?.Name != null 
                    where discordActivity.Name.Contains(activity, StringComparison.CurrentCultureIgnoreCase) 
                    select member.Value
                ).ToList();
            
            var sb = new StringBuilder();
            for (var i = 0; i < users.Count; i++) {
                sb.AppendLine($"<@{users[i].Id}>");
                if (i == 0 || 1 % 20 != 0) continue;
                await c.RespondAsync(sb.ToString());
                sb.Clear();
            }
            if (sb.Length > 0)
                await c.RespondAsync(sb.ToString());
            else
                await c.RespondAsync($"No user had the presence of {activity}");
        }
        catch (Exception ex) {
            await c.RespondAsync($"```\n{ex.Message}\n```");
        }
    }
    
    private static bool _doesItExist(SnowflakeObject user) => Config.Base.DailyPats!.Any(u => u.UserId == user.Id); 
    
    [Command("SetDailyPat"), Aliases("sdp"), Description("Sets the daily pat to user"), RequireOwner]
    public async Task SetDailyPat(CommandContext c, DiscordUser user, int manualSetEpochTime = 0) {
        if (_doesItExist(user)) {
            await c.RespondAsync("User already has a daily pat set.");
            return;
        }
        
        var dailyPat = new DailyPat {
            UserId = user.Id,
            UserName = user.Username,
            SetEpochTime = manualSetEpochTime == 0 ? DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 86400 : manualSetEpochTime + 86400
        };
        
        Config.Base.DailyPats!.Add(dailyPat);
        Config.Save();
        await c.RespondAsync($"Set daily pat to {user.Username}.");
    }
    
    [Command("RemoveDailyPat"), Aliases("rdp"), Description("Removes the daily pat from user"), RequireOwner]
    public async Task RemoveDailyPat(CommandContext c, DiscordUser user) {
        if (!_doesItExist(user)) {
            await c.RespondAsync("User does not have a daily pat set.");
            return;
        }
        
        var dailyPat = Config.Base.DailyPats!.Single(u => u.UserId == user.Id);
        Config.Base.DailyPats!.Remove(dailyPat);
        Config.Save();
        await c.RespondAsync($"Removed daily pat from {user.Username}.");
    }
    
    [Command("ListDailyPats"), Aliases("ldp"), Description("Lists all daily pats"), RequireOwner]
    public async Task ListDailyPats(CommandContext c) {
        var sb = new StringBuilder();
        foreach (var dailyPat in Config.Base.DailyPats!) {
            sb.AppendLine($"{dailyPat.UserName.ReplaceName(dailyPat.UserId)} ({dailyPat.UserId}) - {dailyPat.SetEpochTime}");
        }
        await c.RespondAsync(sb.ToString());
    }
}