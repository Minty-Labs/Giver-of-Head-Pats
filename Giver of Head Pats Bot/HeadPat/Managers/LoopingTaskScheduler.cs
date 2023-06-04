using HeadPats.Data;
using HeadPats.Managers.Loops;

namespace HeadPats.Managers; 

public static class LoopingTaskScheduler {
    public static void StartLoop() => new Thread(Loop).Start();
    
    private static void Loop() {
        var rnd = new Random();
        while (true) {
            using var db = new Context();
            var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            // Status
            try {
                StatusLoop.Update(db);
            }
            catch (Exception err) {
                DSharpToConsole.SendErrorToLoggingChannel($"Status:\n{err}");
            }
            
            // Daily Pats
            try {
                DailyPatLoop.DoDailyPat(db, currentEpoch);
            }
            catch (Exception err) {
                DSharpToConsole.SendErrorToLoggingChannel($"Daily Pats:\n{err}");
            }
            
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