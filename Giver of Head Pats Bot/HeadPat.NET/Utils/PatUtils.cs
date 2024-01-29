namespace HeadPats.Utils;

public class PatUtils {
    public static string GetRandomPatMessageTemplate(string? sender, string? receiver)
        => new Random().Next(0, 6) switch {
            0 => $"Head pats for {receiver}!",
            1 => $"Pat Pat Pat Pat Pat Pat Pat get head patted, {receiver}!",
            2 => $"HEAD PATS!!! ALL FOR {receiver}!!!",
            3 => $"{receiver} got head pats!",
            4 => $"{sender} gave pats to {receiver}!",
            5 => $"{receiver} got some head pats!",
            _ => $"{sender} gave head pats to {receiver}!"
        };
}