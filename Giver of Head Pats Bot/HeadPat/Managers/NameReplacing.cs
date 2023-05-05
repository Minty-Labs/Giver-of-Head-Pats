using HeadPats.Configuration;
using HeadPats.Utils;

namespace HeadPats.Managers; 

public static class NameReplacing {
    public static string? ReplaceName(this string? beforeName, ulong userId) {
        var d = Config.Base.NameReplacements!.Find(u => u.UserId == userId);
        return d is null ? beforeName : d.Replacement;
    }
}