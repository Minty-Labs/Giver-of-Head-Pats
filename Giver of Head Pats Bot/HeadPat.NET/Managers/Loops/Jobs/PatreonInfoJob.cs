﻿/*using HeadPats.Utils.ExternalApis;
using Quartz;

namespace HeadPats.Managers.Loops.Jobs;

public class PatreonInfoJob : IJob {
    private int _numberOfPatreonErrored;
    public async Task Execute(IJobExecutionContext context) {
        try {
            await PatronLogic.Instance.GetPatronInfo(true);
        }
        catch (Exception err) {
            if (_numberOfPatreonErrored >= 5) return;
            await DNetToConsole.SendErrorToLoggingChannelAsync("Patreon:", obj: err);
            _numberOfPatreonErrored++;
        }
    }
}*/