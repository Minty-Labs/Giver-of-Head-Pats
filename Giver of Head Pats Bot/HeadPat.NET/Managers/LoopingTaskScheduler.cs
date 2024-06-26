﻿using HeadPats.Managers.Loops.Jobs;
using HeadPats.Modules;
using Michiru.Managers.Jobs;
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
    
        var rotatingStatusLoop = JobBuilder.Create<RotatingStatusJob>().Build();
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
        
        var dailyPat = JobBuilder.Create<DailyPatJob>().Build();
        var dailyPatTrigger = TriggerBuilder.Create()
            .WithIdentity("DailyPat", Vars.Name)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(5)
                .RepeatForever())
            .Build();
        await scheduler.ScheduleJob(dailyPat, dailyPatTrigger);
        
        var patreonInfo = JobBuilder.Create<PatreonInfoJob>().Build();
        var patreonInfoTrigger = TriggerBuilder.Create()
            .WithIdentity("PatreonInfo", Vars.Name)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInHours(24)
                .RepeatForever())
            .Build();
        await scheduler.ScheduleJob(patreonInfo, patreonInfoTrigger);
        
        var configSaveLoopJob = JobBuilder.Create<ConfigSaveJob>().Build();
        var configSaveLoopTrigger = TriggerBuilder.Create()
            .WithIdentity("ConfigSaveLoop", Vars.Name)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(2)
                .RepeatForever())
            .Build();
        await scheduler.ScheduleJob(configSaveLoopJob, configSaveLoopTrigger);
        
        Logger.Information("Initialized!");
    }
}