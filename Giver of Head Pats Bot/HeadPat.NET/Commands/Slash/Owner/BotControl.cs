using System.Diagnostics;
using System.Text;
using Discord;
using Discord.Interactions;
using HeadPats.Commands.Preexecution;
using HeadPats.Utils;
using Serilog;

namespace HeadPats.Commands.Slash.Owner; 

public class BotControl : InteractionModuleBase<SocketInteractionContext> {

    [Group("bot", "Owner only commands - Controls the bot"), RequireUser(167335587488071682)]
    public class Commands : InteractionModuleBase<SocketInteractionContext> {

        [SlashCommand("listguilds", "Lists all the guilds the bot is in")]
        public async Task ListGuilds() {
            var guilds = Program.Instance.Client.Guilds;
            var sb = new StringBuilder();
            sb.AppendLine($"Guild Count: {guilds.Count}");
            foreach (var g in guilds) {
                sb.AppendLine($"{g.Name} - {g.Id}");
                // sb.AppendLine();
            }

            var overLimit = sb.ToString().Length > 2000;
            var f = sb.ToString();

            if (overLimit) {
                using var ms = new MemoryStream();
                await using var sw = new StreamWriter(ms);
                await sw.WriteAsync(f);
                await sw.FlushAsync();
                ms.Seek(0, SeekOrigin.Begin);
                await Context.Channel.SendFileAsync(new FileAttachment(ms, "Guilds.txt", "File Sent below"));
                return;
            }
            await RespondAsync(f);
        }

        [SlashCommand("leaveguild", "Leaves a guild")]
        public async Task LeaveGuild([Summary(description: "Guild ID")] string guildId) {
            var @ulong = ulong.Parse(guildId);
            var guild = Program.Instance.Client.GetGuild(@ulong);
            await guild.LeaveAsync();
            await RespondAsync($"Left the server: {guild.Name}");
        }

        [SlashCommand("swapapis", "Swaps the API used for image commands")]
        public async Task SwapApis() {
            Vars.UseCookieApi = !Vars.UseCookieApi;
            await RespondAsync($"API changed to: {(Vars.UseCookieApi ? "Cookie" : "Fluxpoint")}");
        }

        [SlashCommand("shutdown", "Shuts down the bot")]
        public async Task ShutdownBot() {
            var m = await Context.Channel.SendMessageAsync("Shutting down...");
            await Task.Delay(TimeSpan.FromSeconds(2));
            await m.ModifyAsync(properties => properties.Content = "Fully shut down...", options: new RequestOptions {AuditLogReason = "Updated Shutdown Message"});
            await Log.CloseAndFlushAsync();
            await Context.Client.StopAsync();
            if (!Vars.IsWindows) {
                var process = new Process {
                    StartInfo = new ProcessStartInfo {
                        FileName = "/bin/bash",
                        Arguments = "-c \"pm2 stop 1\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
            }
            Environment.Exit(0);
        }
        
        [SlashCommand("exec", "Runs a linux command on the server")]
        public async Task Execute([Summary(description: "The command to run")] string command) {
            var process = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
        
            var weh = StringUtils.SplitMessage(output, 1900);
            foreach (var chuck in weh)
                await RespondAsync($"```\n{chuck}```");
            if (command.Equals("pm2 stop 1"))
                await Context.Client.StopAsync();
        }
        
    }
    
}