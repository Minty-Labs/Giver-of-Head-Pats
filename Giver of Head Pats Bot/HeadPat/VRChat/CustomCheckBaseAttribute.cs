using DSharpPlus.CommandsNext.Attributes;
using cc = DSharpPlus.CommandsNext.CommandContext;

namespace HeadPats.VRChat;

public class IsMod : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(cc c, bool yes) {
        var isAMod = ProtectStructure.Base.Users?.FirstOrDefault(x => x.UserId == c.User.Id)?.Role == Roles.Mod;
        return Task.FromResult(isAMod);
    }
}

public class IsAdmin : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(cc c, bool yes) {
        var isAnAdmin = ProtectStructure.Base.Users?.FirstOrDefault(x => x.UserId == c.User.Id)?.Role == Roles.Admin;
        return Task.FromResult(isAnAdmin);
    }
}

public class IsAdminOrMod : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(cc c, bool yes) {
        var isAnAdminOrMod = ProtectStructure.Base.Users?.FirstOrDefault(x => x.UserId == c.User.Id)?.Role == Roles.Admin ||
                             ProtectStructure.Base.Users?.FirstOrDefault(x => x.UserId == c.User.Id)?.Role == Roles.Mod;
        return Task.FromResult(isAnAdminOrMod);
    }
}

public class IsOnList : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(cc c, bool yes) {
        var IsOnList = ProtectStructure.Base.Users?.FirstOrDefault(x => x.UserId == c.User.Id)?.Role == Roles.None ||
                             ProtectStructure.Base.Users?.FirstOrDefault(x => x.UserId == c.User.Id)?.Role == Roles.None;
        return Task.FromResult(IsOnList);
    }
}