using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Managers;
using Serilog;

namespace HeadPats.Commands.Legacy.Owner; 

public class UserControl : BaseCommandModule {
    [Command("MultiKick"), Aliases("mk"), Description("Kicks multiple users at once"), RequireOwner]
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
            .Where(u => u.UserId.Equals(c.User.Id)).ToList().FirstOrDefault();
        
        if (checkUser is null) {
            var newUser = new Users {
                UserId = discordUser.Id,
                UsernameWithNumber = $"{discordUser.Username}#{discordUser.Discriminator}",
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
        try
        {
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
                sb.Append($"<@{users[i].Id}>\r\n");
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
}