using Quartz;

namespace HeadPats.Managers.Loops.Jobs;

public class PatreonInfoJob : IJob {
    private int _numberOfPatreonErrored;
    public async Task Execute(IJobExecutionContext context) {
        try {
            await Program.Instance.PatreonClientInstance.GetPatreonInfo(true);
        }
        catch (Exception err) {
            if (_numberOfPatreonErrored >= 5) return;
            await DNetToConsole.SendErrorToLoggingChannelAsync($"Patreon:\n{err}");
            _numberOfPatreonErrored++;
        }
    }
}