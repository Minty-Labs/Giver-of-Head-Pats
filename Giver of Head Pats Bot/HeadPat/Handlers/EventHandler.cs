using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Net;
using Emzi0767.Utilities;
using Pastel;
using System.Reflection.Emit;
using HeadPats.Handlers.Events;

namespace HeadPats.Handlers;

internal class EventHandler {
    public EventHandler(DiscordClient c) {
        Logger.Log("Setting up Event Handler . . .");
        
        var mc = new MessageCreated(c);
        var jl = new OnBotJoinOrLeave(c);
    }
}