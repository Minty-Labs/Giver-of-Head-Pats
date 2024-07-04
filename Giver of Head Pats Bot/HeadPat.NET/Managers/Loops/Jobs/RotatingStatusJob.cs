using HeadPats.Data;
using Quartz;

namespace HeadPats.Managers.Loops.Jobs;

public class RotatingStatusJob : IJob {
    public async Task Execute(IJobExecutionContext context) {
        await using var db = new Context();
        try {
            await RotatingStatus.Update(db);
        }
        catch (Exception err) {
            await DNetToConsole.SendErrorToLoggingChannelAsync("Rotating Status:", obj: err);
        }
    }
}