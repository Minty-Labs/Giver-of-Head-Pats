using DSharpPlus.Entities;

namespace HeadPats.Managers; 

public static class MessageTasks {
    /// <summary>
    /// Meant to be used with a DiscordMessage Task to delete the message after a given amount of seconds
    /// </summary>
    /// <param name="message">this</param>
    /// <param name="seconds">time until deletion</param>
    /// <param name="reason">deletion reason (for Audit Log)</param>
    /// <returns>an Asynchronous Task</returns>
    /// <exception cref="ArgumentNullException">message is null</exception>
    public static Task<DiscordMessage> DeleteAfter(this Task<DiscordMessage> message, int seconds, string reason = "") {
        Task.Delay(TimeSpan.FromSeconds(seconds)).ContinueWith(async _ => await message.GetAwaiter().GetResult().DeleteAsync(reason));
        return Task.FromResult(message.GetAwaiter().GetResult());
    }
    
    /// <summary>
    /// Sends a webhook message to the desired channel
    /// </summary>
    /// <param name="channel">Target Channel</param>
    /// <param name="embed">Embed to send</param>
    /// <param name="content">Content (text-only) to send</param>
    /// <param name="username">Username the "User" will appear as</param>
    /// <param name="avatarUrl">Avatar URL of which the "User" will have</param>
    public static async Task SendWebhook(DiscordChannel channel, 
        DiscordEmbed embed,
        string content = "",
        string username = "Giver of Head Pats",
        string avatarUrl = "https://cdn.discordapp.com/avatars/489144212911030304/191154c9857f3c998b6cd13af9210551.png?size=1024") {
        // =========================================================================
        try {
            var webhookListOnChannel = await channel.GetWebhooksAsync();
            if (webhookListOnChannel.Count == 0 || !webhookListOnChannel.FirstOrDefault()!.Name.ToLower().Contains("head pat")) {
                await channel.CreateWebhookAsync(username, reason: "Auto Creation since one did not exist");
                webhookListOnChannel = await channel.GetWebhooksAsync();
            }
            var webhook = webhookListOnChannel.FirstOrDefault(n => n.Name.ToLower().Contains("head pat"));

            var wc = new DiscordWebhookBuilder {
                Username = username,
                AvatarUrl = avatarUrl,
                Content = content,
                IsTTS = false
            };
            
            wc.AddEmbed(embed);
            await wc.SendAsync(webhook);
        } catch {
            await channel.SendMessageAsync(new DiscordMessageBuilder().WithContent(content).WithEmbed(embed));
        }
    }

    /// <summary>
    /// Sends a webhook message to the desired channel
    /// </summary>
    /// <param name="channel">Target Channel</param>
    /// <param name="builder">Message builder for various components</param>
    /// <param name="content">Content (text-only) to send</param>
    /// <param name="username">Username the "User" will appear as</param>
    /// <param name="avatarUrl">Avatar URL of which the "User" will have</param>
    public static async Task SendWebhook(DiscordChannel channel,
        DiscordMessageBuilder builder,
        string content = "",
        string username = "Giver of Head Pats",
        string avatarUrl = "https://cdn.discordapp.com/avatars/489144212911030304/191154c9857f3c998b6cd13af9210551.png?size=1024") {
        // =========================================================================
        try {
            var webhookListOnChannel = await channel.GetWebhooksAsync();
            if (webhookListOnChannel.Count == 0 || !webhookListOnChannel.FirstOrDefault()!.Name.ToLower().Contains("head pat")) {
                await channel.CreateWebhookAsync(username, reason: "Auto Creation since one did not exist");
                webhookListOnChannel = await channel.GetWebhooksAsync();
            }
            var webhook = webhookListOnChannel.FirstOrDefault(n => n.Name.ToLower().Contains("head pat"));

            var wc = new DiscordWebhookBuilder {
                Username = username,
                AvatarUrl = avatarUrl,
                Content = content,
                IsTTS = false
            };
            
            wc.AddComponents(builder.Components);
            await wc.SendAsync(webhook);
        }
        catch {
            await channel.SendMessageAsync(new DiscordMessageBuilder(builder).WithContent(content));
        }
    }

    /// <summary>
    /// Sends a webhook message to the desired channel
    /// </summary>
    /// <param name="channel">Target Channel</param>
    /// <param name="content">Content (text-only) to send</param>
    /// <param name="username">Username the "User" will appear as</param>
    /// <param name="avatarUrl">Avatar URL of which the "User" will have</param>
    public static async Task SendWebhook(DiscordChannel channel,
        string content,
        string username = "Giver of Head Pats",
        string avatarUrl = "https://cdn.discordapp.com/avatars/489144212911030304/191154c9857f3c998b6cd13af9210551.png?size=1024") {
        // =========================================================================
        try {
            var webhookListOnChannel = await channel.GetWebhooksAsync();
            if (webhookListOnChannel.Count == 0 || !webhookListOnChannel.FirstOrDefault()!.Name.ToLower().Contains("head pat")) {
                await channel.CreateWebhookAsync(username, reason: "Auto Creation since one did not exist");
                webhookListOnChannel = await channel.GetWebhooksAsync();
            }

            var webhook = webhookListOnChannel.FirstOrDefault(n => n.Name.ToLower().Contains("head pat"));

            var wc = new DiscordWebhookBuilder {
                Username = username,
                AvatarUrl = avatarUrl,
                Content = content,
                IsTTS = false
            };

            await wc.SendAsync(webhook);
        } catch {
            await channel.SendMessageAsync(new DiscordMessageBuilder().WithContent(content));
        }
    }
}