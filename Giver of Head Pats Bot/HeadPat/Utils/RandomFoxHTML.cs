namespace HeadPats.Utils; 

public static class RandomFoxHtml {
    public static async Task<string> GetFoxCount() {
        var httpClient = new HttpClient();
        var content = await httpClient.GetStringAsync("https://randomfox.ca/");
        var _ = content.Split("<br />");
        return _[1].Split("Fox Count: ")[1];
    }
}