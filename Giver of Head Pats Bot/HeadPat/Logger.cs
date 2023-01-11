using Pastel;
using System;
using System.Diagnostics;
using DSharpPlus;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using System.Text.RegularExpressions;
using DSharpPlus.Entities;
using HeadPats.Utils;
using EventHandler = HeadPats.Handlers.EventHandler;

namespace HeadPats;
internal class Logger {
    private static StreamWriter? _log;
    
    public static void ConsoleLogger() {
        if (_log == null) {
            var file = Path.Combine(Environment.CurrentDirectory, "Data", "Logs",
                $"{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.log");
                
            var info = new FileInfo(file);
            if (info.DirectoryName != null) {
                var directoryInfo = new DirectoryInfo(info.DirectoryName);
                
                if (!directoryInfo.Exists) directoryInfo.Create();
                else CleanOld(directoryInfo);
            }

            var stream = !info.Exists ? info.Create() : new FileStream(file, FileMode.Open, FileAccess.Write, FileShare.Read);

            _log = new StreamWriter(stream);
            _log.AutoFlush = true;
            //log.Close();
        }
        Log("Console Output Logging has Initialized");
    }
    
    private static void CleanOld(DirectoryInfo logDirInfo) {
        var files = logDirInfo.GetFiles("*");
        if (files.Length == 0) return;
        var list = (from x in files.ToList()
            orderby x.LastWriteTime
            select x).ToList();
        for (var i = list.Count - 25; i > -1; i--)
            list[i].Delete();
    }

    public static void SendLog(object message) {
        var e = new DiscordEmbedBuilder();
        e.WithColor(Colors.HexToColor("FF2525"));
        e.WithDescription(message.ToString());
        e.WithTimestamp(DateTime.Now);

        Program.Client?.SendMessageAsync(Program.ErrorLogChannel, e.Build()).GetAwaiter().GetResult();
    }

    public static void LogEvent(object message) {
        var e = new DiscordEmbedBuilder();
        e.WithColor(Colors.HexToColor("4F87F5"));
        e.WithDescription(message.ToString());
        e.WithTimestamp(DateTime.Now);
        
        Program.Client?.SendMessageAsync(Program.GeneralLogChannel, e.Build()).GetAwaiter().GetResult();
    }
    
    private static void Stop() => _log?.Close();

    private static string GetTimestamp() => DateTime.Now.ToString("HH:mm:ss.fff");
    
    public static void Log(string message) {
        Console.WriteLine($"[{GetTimestamp()}]".Pastel("00F8FF") + " HeadPat".Pastel("47c687") + $" > {message}");
        _log?.WriteLine($"[{GetTimestamp()}] HeadPat > {message}");
    }

    public static void Log() => Console.WriteLine();

    public static void Error(object @object) {
        Console.WriteLine($"[{GetTimestamp()}]".Pastel("00F8FF") + " HeadPat".Pastel("47c687") + $" > {@object}".Pastel("ff0000"));
        _log?.WriteLine($"[{GetTimestamp()}] [ERROR] HeadPat > {@object}");
        SendLog(@object);
    }
    
    public static void CommandNull(string username, string result) {
        Console.WriteLine($"[{GetTimestamp()}]".Pastel("00F8FF") + " HeadPat".Pastel("FEF600") + $" > Command failed to execute for [" + $"{username}".Pastel("EECCE0") + "] <-> [" + 
                          $"{result}".Pastel("FFD766") + "]!");
        _log?.WriteLine($"[{GetTimestamp()}] [CMDNULL] HeadPat > Command failed to execute for [{username}] <-> [{result}]!");
    }
    
    public static void CommandExecuted(string cmd, string username, string guild) {
        Console.WriteLine($"[{GetTimestamp()}]".Pastel("00F8FF") + " HeadPat".Pastel("FEF600") + $" > Command [" + $"{cmd}".Pastel("FFD766") + "] was executed by [" + 
                          $"{username}".Pastel("EECCE0") + "] in guild [" + $"{guild}".Pastel("91D7FD") + "]");
        _log?.WriteLine($"[{GetTimestamp()}] [CMDEXEC] HeadPat > Command [{cmd}] was executed by [{username}] in guild [{guild}]");
    }

    public static void CommandErrored(string cmd, string username, string guild, object message, object exception) {
        Console.WriteLine($"[{GetTimestamp()}]".Pastel("00F8FF") + " HeadPat".Pastel("ff0000") + $" > Command [" + $"{cmd}".Pastel("FFD766") + "] was executed by [" + 
                          $"{username}".Pastel("EECCE0") + "] in guild [" + $"{guild}".Pastel("91D7FD") + "] \n" + $"{exception}".Pastel("CF284D"));
        _log?.WriteLine($"[{GetTimestamp()}] [CMDERROR] HeadPat > Command [{cmd}] was executed by [{username}] in guild [{guild}] \n {exception}");
        SendLog(@$"[CMDERROR] HeadPat > Command [{cmd}] was executed by [{username}] in guild [{guild}] Message:\n```{message}```\nStackTrace\n```{exception}```");
    }
    
    public static void SlashCommandErrored(string cmd, string username, string guild, object exception, bool isSlashCmd = false) {
        Console.WriteLine($"[{GetTimestamp()}]".Pastel("00F8FF") + " HeadPat".Pastel("ff0000") + $" > {(isSlashCmd ? "Slash" : "")}Command [" + $"{cmd}".Pastel("FFD766") + "] was executed by [" + 
                          $"{username}".Pastel("EECCE0") + "] in guild [" + $"{guild}".Pastel("91D7FD") + "] \n" + $"{exception}".Pastel("CF284D"));
        _log?.WriteLine($"[{GetTimestamp()}] [CMDERROR] HeadPat > {(isSlashCmd ? "Slash" : "")}Command [{cmd}] was executed by [{username}] in guild [{guild}] \n {exception}");
        SendLog(@$"[CMDERROR] HeadPat > {(isSlashCmd ? "Slash" : "")}Command [{cmd}] was executed by [{username}] in guild [{guild}] StackTrace\n```{exception}```");
    }
        
    public static void LoadModule(string message) {
        Console.WriteLine($"[{GetTimestamp()}]".Pastel("00F8FF") + " HeadPat".Pastel("47c687") + $" > Loading " + $"{message}".Pastel("FFC366") + " Module");
        _log?.WriteLine($"[{GetTimestamp()}] [LOAD] HeadPat > {message}");
    }
        
    public static void WriteSeparator(string pastelHexTextColor = "ffffff") {
        var pad = "".PadLeft(Console.WindowWidth, '=');
        Console.WriteLine(pad.Pastel(pastelHexTextColor));
        _log?.WriteLine(pad);
    }

    public static string? DoInput(string message) {
        Console.Write($"[{GetTimestamp()}]".Pastel("00F8FF") + " HeadPat".Pastel("47c687") + " > Input " + $"{message}".Pastel("FFC366"));
        return Console.ReadLine();
    }
    
    #region DSharpPlus Logs
    
    public static bool IsInErrorState;

    public static void ReplaceDSharpLogs(DiscordClient c) {
        c.ClientErrored += DiscordClientOnSocketErrored;
        c.SocketErrored += OnSocketErrored;
        //c.Heartbeated += OnHeartbeated;
        c.Resumed += OnResumed;
        c.Zombied += OnZombied;
        c.SocketClosed += OnSocketClosed;
        //c.SocketOpened += OnSocketOpened;
        c.UnknownEvent += OnUnknownEvent;
    }

    private static async Task OnUnknownEvent(DiscordClient sender, UnknownEventArgs e)
        => await Task.Run(() => Console.Write(""));

    private static async Task OnSocketOpened(DiscordClient sender, SocketEventArgs e) {
        Console.WriteLine($"[{GetTimestamp()}]".Pastel("00F8FF") + $" SOCKET   ".Pastel("#9fffa2") + $" > {e}".Pastel("ffffff"));
        await _log?.WriteLineAsync($"[{GetTimestamp()}] SOCKET    > {e}")!;
    }
    
    private static async Task OnSocketClosed(DiscordClient sender, SocketCloseEventArgs e) {
        Console.WriteLine($"[{GetTimestamp()}]".Pastel("00F8FF") + $" SOCKET   ".Pastel("#9fc4ff") + $" > Code: {e.CloseCode} > {e.CloseMessage}".Pastel("ffffff"));
        await _log?.WriteLineAsync($"[{GetTimestamp()}] SOCKET    > Code: {e.CloseCode} > {e.CloseMessage}")!;
        IsInErrorState = true;
    }
    
    private static async Task OnZombied(DiscordClient sender, ZombiedEventArgs e) {
        Console.WriteLine($"[{GetTimestamp()}]".Pastel("00F8FF") + $" ZOMBIED  ".Pastel("#9fffa2") + $" > {e.Failures}".Pastel("ffffff"));
        await _log?.WriteLineAsync($"[{GetTimestamp()}] ZOMBIED   > {e.Failures}")!;
    }

    private static async Task OnResumed(DiscordClient sender, ReadyEventArgs e) {
        Console.WriteLine($"[{GetTimestamp()}]".Pastel("00F8FF") + $" CLIENT   ".Pastel("#f1ff9f") + $" > Client Resumed".Pastel("ffffff"));
        await _log?.WriteLineAsync($"[{GetTimestamp()}] CLIENT    > Client Resumed")!;
        IsInErrorState = false;
    }
    
    private static async Task OnHeartbeated(DiscordClient sender, HeartbeatEventArgs e) 
        => await Task.Run(() => Console.WriteLine($"[{GetTimestamp()}]".Pastel("00F8FF") + $" HEARTBEAT".Pastel("#f1ff9f") + $" > {e.Ping}".Pastel("ffffff")));

    private static async Task OnSocketErrored(DiscordClient sender, SocketErrorEventArgs e) {
        Console.WriteLine($"[{GetTimestamp()}]".Pastel("00F8FF") + $" SOCKET   ".Pastel("#ff9f9f") + $" > {e.Exception}".Pastel("ff0000"));
        await _log?.WriteLineAsync($"[{GetTimestamp()}] SOCKET    > {e.Exception}")!;
        IsInErrorState = true;
        // if (e.Exception.ToString().Contains("Could not connect to Discord") || e.Exception.ToString().Contains("No such host is known.")) {
        //     Stop();
        //     Process.Start("HeadPatDS.exe");
        //     Process.GetCurrentProcess().Kill();
        // }
    }
    
    private static async Task DiscordClientOnSocketErrored(DiscordClient sender, ClientErrorEventArgs e) {
        Console.WriteLine($"[{GetTimestamp()}]".Pastel("00F8FF") + $" CLIENT   ".Pastel("#ff9f9f") + $" > {e.Exception}".Pastel("ff0000"));
        await _log?.WriteLineAsync($"[{GetTimestamp()}] CLIENT    > {e.Exception}")!;
        IsInErrorState = true;
    }

    #endregion

    // public static async Task ReadConsoleOutput() {
    //     var p = GetProcessAfterStarting("");
    //     p!.OutputDataReceived += P_OutputDataReceived;
    //     p.EnableRaisingEvents = true;
    //     p.BeginOutputReadLine();
    //     await p.WaitForExitAsync();
    //     p.CancelOutputRead();
    // }
    //
    // private static string[]? patterns = new[] { "Could not connect to Discord", "No such host is known", "discord.com:443", "discord.gg:443" };
    //
    // private static void P_OutputDataReceived(object sender, DataReceivedEventArgs e) {
    //     if (e.Data == null || patterns == null) return;
    //     if (e.Data.Contains(patterns[0]) || e.Data.Contains(patterns[1]) || e.Data.Contains(patterns[2]) || e.Data.Contains(patterns[3])) {
    //         Stop();
    //         Process.Start("HeadPat.exe");
    //         try   { BuildInfo.ThisProcess?.Kill(); }
    //         catch { Process.GetCurrentProcess().Kill(); }
    //     }
    // }
    //
    // private static Process? GetProcessAfterStarting(string pathToExe) {
    //     var process = Process.Start(new ProcessStartInfo {
    //         FileName = pathToExe,
    //         RedirectStandardInput = true,
    //         RedirectStandardOutput = true,
    //         CreateNoWindow = false,
    //         UseShellExecute = false,
    //     });
    //     return process;
    // }
}
