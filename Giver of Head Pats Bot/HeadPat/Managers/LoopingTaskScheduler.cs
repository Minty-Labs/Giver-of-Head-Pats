using HeadPats.Data;
using HeadPats.Managers.Loops;

namespace HeadPats.Managers; 

public static class LoopingTaskScheduler {
    public static void StartLoop() {
        DailyPatLoop.DailyPatted = new Dictionary<ulong, bool>();
        // DailyPatLoop.PreviousPatUrl = new Dictionary<ulong, string>();
        new Thread(Loop).Start();
        new Thread(LoopAsync).Start();
    }

    private static async void LoopAsync() {
        while (true) {
            await using var db = new Context();
            var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            // Status
            try {
                await StatusLoop.Update(db);
            }
            catch (Exception err) {
                await DSharpToConsole.SendErrorToLoggingChannelAsync($"Status:\n{err}");
            }
            
            // Daily Pats
            try {
                await DailyPatLoop.DoDailyPat(db, currentEpoch);
            }
            catch (Exception err) {
                await DSharpToConsole.SendErrorToLoggingChannelAsync($"Daily Pats:\n{err}");
            }
            
            Thread.Sleep(TimeSpan.FromMinutes(10));
        }
        // ReSharper disable once FunctionNeverReturns
    }
    
    private static void Loop() {
        var rnd = new Random();
        while (true) {
            using var db = new Context();
            var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            // IRL Quotes
            try {
                IrlQuotesLoop.SendQuote(currentEpoch, rnd);
            }
            catch (Exception err) {
                DSharpToConsole.SendErrorToLoggingChannel($"IRL Quotes:\n{err}");
            }

            Thread.Sleep(TimeSpan.FromMinutes(10));
        }
        // ReSharper disable once FunctionNeverReturns
    }
}