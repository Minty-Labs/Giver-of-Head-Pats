using HeadPats.Configuration;
using Quartz;

namespace HeadPats.Managers.Loops.Jobs;

public class ConfigSaveJob : IJob {
    public async Task Execute(IJobExecutionContext context) {
        try {
            if (Config.ShouldUpdateConfigFile) {
                Config.SaveFile();
            }
        }
        catch (Exception err) {
            await DNetToConsole.SendErrorToLoggingChannelAsync("ConfigSaveJob:", obj: err);
        }
    }
}