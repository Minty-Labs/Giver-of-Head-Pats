using HeadPats.Data;
using Quartz;

namespace HeadPats.Managers.Loops.Jobs;

public class DataDeletionJob : IJob {
    public async Task Execute(IJobExecutionContext context) {
        await using var db = new Context();
        var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        try {
            DataDeletionFinderLoop.FindDataDeletion(db, currentEpoch);
        }
        catch (Exception err) {
            await DNetToConsole.SendErrorToLoggingChannelAsync($"Data Deletion:\n{err}");
        }
    }
}