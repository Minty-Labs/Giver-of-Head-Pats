namespace HeadPats.Utils; 

public class TaskScheduler {
    private static List<Timer> _timers = new();

    public static void ScheduleTask(double intervalInSeconds, Action task) {
        var timer = new Timer(x => {
            try {
                if (!Logger.IsInErrorState)
                    task.Invoke();
            } 
            catch (Exception e) { Logger.Error(e); }
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(intervalInSeconds));
        _timers.Add(timer);
    }
}