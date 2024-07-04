using Quartz;

namespace HeadPats.Managers.Loops.Jobs;

public class DownloadUsersJob : IJob {
    public async Task Execute(IJobExecutionContext context) {
        try {
            DownloadUsersLoop.DownloadUsers();
        }
        catch (Exception err) {
            await DNetToConsole.SendErrorToLoggingChannelAsync("DownloadUsersJob:", obj: err);
        }
    }
}