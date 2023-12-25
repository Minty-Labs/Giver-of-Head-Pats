using System.Diagnostics;

namespace HeadPats.Utils.Execution;

public class PM2 {
    internal static void Stop() {
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
}