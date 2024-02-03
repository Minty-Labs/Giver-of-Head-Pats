using System.Diagnostics;
using Discord.Commands;
using Michiru.Utils;

namespace Michiru.Commands;

[RequireContext(ContextType.Guild)]
public class BasicCommandsThatIDoNotWantAsSlashCommands : ModuleBase<SocketCommandContext> {
    [RequireOwner, Command("exec")]
    public async Task SetPennyGuildWatcher(string command) {
        var process = new Process {
            StartInfo = new ProcessStartInfo {
                FileName = "/bin/bash",
                Arguments = $"-c \"{command}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        if (command.Equals("pm2 stop 1"))
            await Context.Client.StopAsync();
        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();
        
        var weh = StringUtils.SplitMessage(output, 1900);
        foreach (var chuck in weh)
            await ReplyAsync($"```\n{chuck}```");
    }
}