using DSharpPlus.Entities;
using HeadPats.Data;

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
            Name = "lots of cuties | hp!help",
            ActivityType = ActivityType.Watching
        }, UserStatus.Online).GetAwaiter().GetResult();;
    }

    private static void LoopStatus() {
        while (true) {
            using var db = new Context();
            var tempPatCount = db.Overall.AsQueryable().ToList().First().PatCount;
            
            Program.Client!.UpdateStatusAsync(new DiscordActivity {
                Name = $"{tempPatCount} head pats | hp!help",
                ActivityType = ActivityType.Watching
            }, UserStatus.Online).GetAwaiter().GetResult();
            // Logger.Log("Updated Status");
            db.Dispose();
            
            Thread.Sleep(TimeSpan.FromMinutes(10));
        }
    }
}