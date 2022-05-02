﻿using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using HeadPats.Commands;

namespace HeadPats.Managers;
internal static class Commands {
    public static void Register(CommandsNextExtension? c) {
        c?.RegisterCommands<Basic>();
        c?.RegisterCommands<LoveCmds>();
    }

    public static void Register(SlashCommandsExtension? s) {
        s?.RegisterCommands<BasicSlashCommands>();
    }
}
