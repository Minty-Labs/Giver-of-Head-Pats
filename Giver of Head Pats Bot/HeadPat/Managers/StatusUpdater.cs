/*using DSharpPlus.Entities;

namespace HeadPats.Managers; 

public class StatusUpdater {
    private static List<Timer> Timers = new();

    private static void ScheduleTask(double intervalInSeconds, Action task) {
        var timer = new Timer(x => {
            try {
                if (!Logger.IsInErrorState)
                    task.Invoke();
            } 
            catch (Exception e) { Logger.Error(e); }
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(intervalInSeconds));
        Timers.Add(timer);
    }

    public static void Start(bool clearAndInvokeNew) {
        if (clearAndInvokeNew) Timers.Clear();
        // 
        var statues = Configuration.Statuses();

        foreach (var s in statues) {
            ScheduleTask(300, () => {
                Program.Client!.UpdateStatusAsync(new DiscordActivity {
                    Name = s.Game,
                    ActivityType = Program.GetActivityType(s.ActivityType!)
                }, Program.GetUserStatus(s.OnlineType!)).GetAwaiter().GetResult();
            });
        }
    }
}*/