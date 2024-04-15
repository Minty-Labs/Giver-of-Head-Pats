using HeadPats.Configuration;
using HeadPats.Managers;
using Quartz;

namespace Michiru.Managers.Jobs;

public class ConfigSaveJob : IJob {
    public async Task Execute(IJobExecutionContext context) {
        try {
            if (Config.ShouldUpdateConfigFile) {
                Config.SaveFile();
            }
        }
        catch (Exception err) {
            await DNetToConsole.SendErrorToLoggingChannelAsync(err);
        }
    }
}