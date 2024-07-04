namespace HeadPats.Managers.Loops;

public class DownloadUsersLoop {
    public static void DownloadUsers() {
        foreach (var guild in Program.Instance.Client.Guilds) {
            guild.DownloadUsersAsync().GetAwaiter().GetResult();
        }
    }
}