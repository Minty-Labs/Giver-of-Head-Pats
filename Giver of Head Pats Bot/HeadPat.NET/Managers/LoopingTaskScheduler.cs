using HeadPats.Data;
using HeadPats.Events;
using HeadPats.Managers.Loops;
using HeadPats.Modules;

namespace HeadPats.Managers; 

public class LoopingTaskScheduler : BasicModule {
    protected override string ModuleName => "Looping Task Scheduler";
    protected override string ModuleDescription => "Runs looping tasks";

    public override void Initialize() {
        // new Thread(Loop).Start();
        new Thread(LoopAsync).Start();
    }

    private static int _numberOfErrored;

    private static async void LoopAsync() {
        while (true) {
            await using var db = new Context();
            var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Thread.Sleep(TimeSpan.FromSeconds(30));
            
            // Status
            try {
                await StatusLoop.Update(db);
            }
            catch (Exception err) {
                await DNetToConsole.SendErrorToLoggingChannelAsync($"Status:\n{err}");
            }
            
            // Daily Pats
            // try {
            //     await DailyPatLoop.DoDailyPat(db, currentEpoch);
            // }
            // catch (Exception err) {
            //     if (_numberOfErrored >= 5) return;
            //     await DNetToConsole.SendErrorToLoggingChannelAsync($"Daily Pats:\n{err}");
            //     _numberOfErrored++;
            // }
            
            // Data Deletion
            try {
                DataDeletionFinderLoop.FindDataDeletion(db, currentEpoch);
            }
            catch (Exception err) {
                await DNetToConsole.SendErrorToLoggingChannelAsync($"Data Deletion:\n{err}");
            }
            
            // Rotating Status
            try {
                await RotatingStatus.Update(db);
            }
            catch (Exception err) {
                await DNetToConsole.SendErrorToLoggingChannelAsync($"Rotating Status:\n{err}");
            }
            
            // Patreon
            try {
                await Program.Instance.PatreonClientInstance.GetPatreonInfo(true);
            }
            catch (Exception err) {
                await DNetToConsole.SendErrorToLoggingChannelAsync($"Patreon:\n{err}");
            }
            
            // Misc
            var client = Program.Instance.Client;
            OnBotJoinOrLeave.GuildCount = client.Guilds.Count;
            if (OnBotJoinOrLeave.GuildIds is not null) {
                OnBotJoinOrLeave.GuildIds.Clear();
                foreach (var guild in client.Guilds) {
                    OnBotJoinOrLeave.GuildIds.Add(guild.Id);
                }
            }
            
            Thread.Sleep(TimeSpan.FromMinutes(10));
        }
        // ReSharper disable once FunctionNeverReturns
    }
    
    /*private static void Loop() {
        var rnd = new Random();
        while (true) {
            using var db = new Context();
            var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            // IRL Quotes
            // try {
            //     IrlQuotesLoop.SendQuote(currentEpoch, rnd);
            // }
            // catch (Exception err) {
            //     DNetToConsole.SendErrorToLoggingChannel($"IRL Quotes:\n{err}");
            // }
            
            // Data Deletion
            try {
                DataDeletionFinderLoop.FindDataDeletion(db, currentEpoch);
            }
            catch (Exception err) {
                DNetToConsole.SendErrorToLoggingChannel($"Data Deletion:\n{err}");
            }

            Thread.Sleep(TimeSpan.FromMinutes(10));
        }
        // ReSharper disable once FunctionNeverReturns
    }*/
}