using HeadPats.Utils.ExternalApis;
using Quartz;

namespace HeadPats.Managers.Loops.Jobs;

public class ProcessLocalImages : IJob {
    public async Task Execute(IJobExecutionContext context) {
        try {
            LocalImages.ReadFromLocalStorage();
        }
        catch (Exception err) {
            await DNetToConsole.SendErrorToLoggingChannelAsync("ProcessLocalImagesJob:", obj: err);
        }
    }
}