using DSharpPlus.Entities;
using HeadPats.Data;

namespace HeadPats.Managers; 

public class StatusUpdater {
    private static List<Timer> _timers = new();
    private static int _tempPatCount;

    private static void ScheduleTask(double intervalInSeconds, Action task) {
        var timer = new Timer(x => {
            try {
                if (!Logger.IsInErrorState)
                    task.Invoke();
            } 
            catch (Exception e) { Logger.Error(e); }
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(intervalInSeconds));
        _timers.Add(timer);
    }

    public static void Start(bool clearAndInvokeNew, bool disableAndSetStaticStatus = false) {
        if (disableAndSetStaticStatus) {
            Program.Client!.UpdateStatusAsync(new DiscordActivity {
                Name = "lots of cuties | hp!help",
                ActivityType = ActivityType.Watching
            }, UserStatus.Online);
            return;
        }
        
        if (clearAndInvokeNew) _timers.Clear();
        
        using var db = new Context();
        var globalPats = db.Overall.AsQueryable().ToList();
        _tempPatCount = globalPats.First().PatCount;

        ScheduleTask(600, () => {
            var tempGlobalPats = globalPats.First().PatCount;
            if (tempGlobalPats == _tempPatCount) 
                return; // Don't run update if number is the same
            
            Program.Client!.UpdateStatusAsync(new DiscordActivity {
                Name = $"{tempGlobalPats} head pats | hp!help",
                ActivityType = ActivityType.Watching
            }, UserStatus.Online);
        });
    }
}