using Michiru.Managers.Jobs;
using Quartz;
using Serilog;

namespace Michiru.Managers;

public class Scheduler {
    private static readonly ILogger Logger = Log.ForContext(typeof(Scheduler));

    public static async Task Initialize() {
        Logger.Information("Creating and Building...");
        var scheduler = await SchedulerBuilder.Create()
            .UseDefaultThreadPool(x => x.MaxConcurrency = 1)
            .BuildScheduler();
        await scheduler.Start();

        var statusLoop = JobBuilder.Create<RotatingStatusJob>().Build();
        var statusLoopTrigger = TriggerBuilder.Create()
            .WithIdentity("StatusLoop", Vars.Name)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(10)
                .RepeatForever())
            .Build();
        await scheduler.ScheduleJob(statusLoop, statusLoopTrigger);
        
        Logger.Information("Initialized!");
    }
}