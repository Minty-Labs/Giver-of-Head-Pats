using DSharpPlus.Entities;
using HeadPats.Commands.Legacy.Owner;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Utils;
using Serilog;

namespace HeadPats.Managers;

public static class TaskScheduler {
    public static void StartStatusLoop() => new Thread(LoopStatus).Start();

    public static void StopStatusLoop() {
        _tempPatCount = 0;
        new Thread(LoopStatus).Suspend();
    }
    
    private static int _tempPatCount;

    public static void NormalDiscordActivity() {
        Program.Client!.UpdateStatusAsync(new DiscordActivity {
            Name = Config.Base.Activity!,
            ActivityType = Program.GetActivityType(Config.Base.ActivityType)
        }, UserStatus.Online).GetAwaiter().GetResult();
    }

    private static void LoopStatus() {
        while (true) {
            using var db = new Context();
            var tempPatCount = db.Overall.AsQueryable().ToList().First().PatCount;

            if (tempPatCount != _tempPatCount) {
                Program.Client!.UpdateStatusAsync(new DiscordActivity {
                    Name = $"{tempPatCount} head pats | hp!help",
                    ActivityType = ActivityType.Watching
                }, UserStatus.Online).GetAwaiter().GetResult();
                // Log.Debug("Updated Status");
            }
            
            // Daily Pats
            var updated = false;
            var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var channel = Program.Client!.GetChannelAsync(Config.Base.DailyPatNotifyChannelId).GetAwaiter().GetResult();
            foreach (var user in Config.Base.DailyPats!) {
                if (user.SetEpochTime > currentEpoch)
                    continue;
                if (channel is null)
                    continue;
                
                var userPatCount = db.Users.AsQueryable().ToList().FirstOrDefault(u => u.UserId.Equals(user.UserId))!.PatCount;

                var embed = new DiscordEmbedBuilder {
                    Title = "Daily Pats!",
                    Description = $"{user.UserName.ReplaceName(user.UserId)}, You have received your daily pats! You now have {userPatCount} pats!",
                    Color = Colors.HexToColor("ffff00"),
                    ImageUrl = Program.FluxpointClient!.Gifs.GetPatAsync().GetAwaiter().GetResult().file,
                    Footer = new DiscordEmbedBuilder.EmbedFooter {
                        Text = "Powered by Fluxpoint API"
                    }
                }.Build();
                
                channel.SendMessageAsync(embed).GetAwaiter().GetResult();
                
                Data.Models.UserControl.AddPatToUser(user.UserId, 1, false);
                user.SetEpochTime += 86400;
                updated = true;
            }

            if (updated) {
                Config.Save();
            }

            Thread.Sleep(TimeSpan.FromMinutes(10));
        }
        // ReSharper disable once FunctionNeverReturns
    }
}