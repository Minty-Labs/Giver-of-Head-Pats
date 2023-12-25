using Discord;
using HeadPats.Configuration;
using HeadPats.Data;

namespace HeadPats.Managers.Loops;

public static class StatusLoop {    
    private static int _tempPatCount;
    
    public static async Task Update(Context db) {
        if (Config.Base.RotatingStatus.Enabled || Vars.IsDebug) return;
        var tempPatCount = Convert.ToInt32(db.GlobalVariables.AsQueryable().ToList().FirstOrDefault(x => x.Name.Equals("PatCount")).Value);

        if (tempPatCount == _tempPatCount) return;
        
        await Program.Instance.Client.SetStatusAsync(UserStatus.Online);
        await Program.Instance.Client.SetActivityAsync(new CustomStatusGame($"{tempPatCount} head pats given | hp!help"));
        // Log.Debug("Updated Status");
    }
}