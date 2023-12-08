using HeadPats.Managers.Loops.Jobs;
using HeadPats.Modules;
using Quartz;
using Serilog;

namespace HeadPats.Managers; 

public class LoopingTaskScheduler : BasicModule {
    protected override string ModuleName => "Task Scheduler";
    protected override string ModuleDescription => "Runs scheduler tasks";

    public override void Initialize() => Scheduler();

    private static async void Scheduler() {
        var scheduler = await SchedulerBuilder.Create()
            .UseDefaultThreadPool(x => x.MaxConcurrency = 6)
            .BuildScheduler();
        await scheduler.Start();
        
        var statusLoop = JobBuilder.Create<StatusLoopJob>().Build();
        var statusLoopTrigger = TriggerBuilder.Create()
            .WithIdentity("StatusLoop", Vars.Name)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(10)
                .RepeatForever())
            .Build();
        await scheduler.ScheduleJob(statusLoop, statusLoopTrigger);
    
        var rotatingStatusLoop = JobBuilder.Create<DataDeletionJob>().Build();
        var rotatingStatusLoopTrigger = TriggerBuilder.Create()
            .WithIdentity("RotatingStatusLoop", Vars.Name)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(10)
                .RepeatForever())
            .Build();
        await scheduler.ScheduleJob(rotatingStatusLoop, rotatingStatusLoopTrigger);
        
        var guildDataDeletion = JobBuilder.Create<DataDeletionJob>().Build();
        var guildDataDeletionTrigger = TriggerBuilder.Create()
            .WithIdentity("GuildDataDeletion", Vars.Name)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInHours(12)
                .RepeatForever())
            .Build();
        await scheduler.ScheduleJob(guildDataDeletion, guildDataDeletionTrigger);
        
        var dailyPat = JobBuilder.Create<DataDeletionJob>().Build();
        var dailyPatTrigger = TriggerBuilder.Create()
            .WithIdentity("DailyPat", Vars.Name)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(10)
                .RepeatForever())
            .Build();
        await scheduler.ScheduleJob(dailyPat, dailyPatTrigger);
        
        var patreonInfo = JobBuilder.Create<DataDeletionJob>().Build();
        var patreonInfoTrigger = TriggerBuilder.Create()
            .WithIdentity("PatreonInfo", Vars.Name)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInHours(24)
                .RepeatForever())
            .Build();
        await scheduler.ScheduleJob(patreonInfo, patreonInfoTrigger);
        Log.Information("[Scheduler] Scheduler initialized!");
    }
}