/*using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;
using HeadPats.Data;

namespace HeadPats.Handlers;

public class GuildCommandCheck {
    public ulong GuildId { get; set; }
    public bool Enabled { get; set; }
}

public class HasModerationModuleEnabled : SlashCheckBaseAttribute {
    
    public override Task<bool> ExecuteChecksAsync(InteractionContext c) {
        return Task.FromResult(Program.GuildCommandCheckList.FirstOrDefault(t => c.Guild.Id == t.GuildId)!.Enabled);
    }
}*/