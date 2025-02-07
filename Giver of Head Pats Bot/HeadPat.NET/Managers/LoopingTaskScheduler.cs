using HeadPats.Managers.Loops.Jobs;
using HeadPats.Modules;
using Quartz;
using Serilog;

namespace HeadPats.Managers; 

public class LoopingTaskScheduler : BasicModule {
    protected override string ModuleName => "Task Scheduler";
    protected override string ModuleDescription => "Runs scheduler tasks";
    private static readonly ILogger Logger = Log.ForContext(typeof(LoopingTaskScheduler));

    public override async Task InitializeAsync() => await Scheduler();

    private static async Task Scheduler() {
        Logger.Information("Creating and Building...");
        var scheduler = await SchedulerBuilder.Create()
            .UseDefaultThreadPool(x => x.MaxConcurrency = 5)
            .BuildScheduler();
        await scheduler.Start();
        
        // 1
        var rotatingStatusLoop = JobBuilder.Create<RotatingStatusJob>().Build();
        var rotatingStatusLoopTrigger = TriggerBuilder.Create()
            .WithIdentity("RotatingStatusLoop", Vars.Name)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(10)
                .RepeatForever())
            .Build();
        await scheduler.ScheduleJob(rotatingStatusLoop, rotatingStatusLoopTrigger);
        
        // 2
        var guildDataDeletion = JobBuilder.Create<DataDeletionJob>().Build();
        var guildDataDeletionTrigger = TriggerBuilder.Create()
            .WithIdentity("GuildDataDeletion", Vars.Name)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInHours(12)
                .RepeatForever())
            .Build();
        await scheduler.ScheduleJob(guildDataDeletion, guildDataDeletionTrigger);
        
        // 3
        /*var dailyPat = JobBuilder.Create<DailyPatJob>().Build();
        var dailyPatTrigger = TriggerBuilder.Create()
            .WithIdentity("DailyPat", Vars.Name)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(5)
                .RepeatForever())
            .Build();
        await scheduler.ScheduleJob(dailyPat, dailyPatTrigger);*/
        
        // 4
        /*var patreonInfo = JobBuilder.Create<PatreonInfoJob>().Build();
        var patreonInfoTrigger = TriggerBuilder.Create()
            .WithIdentity("PatreonInfo", Vars.Name)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInHours(24)
                .RepeatForever())
            .Build();
        await scheduler.ScheduleJob(patreonInfo, patreonInfoTrigger);*/
        
        // 5
        var configSaveLoopJob = JobBuilder.Create<ConfigSaveJob>().Build();
        var configSaveLoopTrigger = TriggerBuilder.Create()
            .WithIdentity("ConfigSaveLoop", Vars.Name)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(5)
                .RepeatForever())
            .Build();
        await scheduler.ScheduleJob(configSaveLoopJob, configSaveLoopTrigger);
        
        // 6
        var downloadUserLoopJob = JobBuilder.Create<DownloadUsersJob>().Build();
        var downloadUserTrigger = TriggerBuilder.Create()
            .WithIdentity("DownloadUsers", Vars.Name)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInHours(24)
                .RepeatForever())
            .Build();
        await scheduler.ScheduleJob(downloadUserLoopJob, downloadUserTrigger);
        
        // 7
        var processLocalImagesJob = JobBuilder.Create<ProcessLocalImages>().Build();
        var processLocalImagesTrigger = TriggerBuilder.Create()
            .WithIdentity("ProcessLocalImages", Vars.Name)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInHours(24)
                .RepeatForever())
            .Build();
        await scheduler.ScheduleJob(processLocalImagesJob, processLocalImagesTrigger);
        
        Logger.Information("Initialized!");
    }
}