using System.Text.Json;

namespace HeadPats.Cookie;

public class CookieClient {
    private const string BaseUrl = "https://cookie.ellyvr.dev/api/v1/";
    private const string StaticUrl = "https://cookie.ellyvr.dev/images/";
    public string? CookieApiKey { get; internal set; }

    public CookieClient(String key) => CookieApiKey = key;

    private CookieRes MakeRequest(String endpoint) {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("authorization", CookieApiKey);
        var resultString = httpClient.GetStringAsync(endpoint);
        // Logger.Log("res: " + resultString.GetAwaiter().GetResult());
        var data = JsonSerializer.Deserialize<CookieRes>(resultString.GetAwaiter().GetResult());
        return data!;
    }

    /// <summary>
    /// Gets a URL to a random cookie image of GIF
    /// </summary>
    /// <returns>URL as string of a cookie</returns>
    public string GetCookie() => $"{StaticUrl}cookie/{MakeRequest($"{BaseUrl}images/cookie").Path}";

    // public string GetCuddle() => $"{staticURL}cuddle/{MakeRequest($"{baseURL}images/cuddle").path}";

    /// <summary>
    /// Gets a URL to a random hug image or GIF
    /// </summary>
    /// <returns>URL as string of a Hug</returns>
    public string GetHug() => $"{StaticUrl}hug/{MakeRequest($"{BaseUrl}images/hug").Path}";

    /// <summary>
    /// Gets a URL to a random kiss image or GIF
    /// </summary>
    /// <returns>URL as string of Kiss</returns>
    public string GetKiss() => $"{StaticUrl}kiss/{MakeRequest($"{BaseUrl}images/kiss").Path}";

    
    /// <summary>
    /// Gets a URL to a random poke image or GIF
    /// </summary>
    /// <returns>URL as string of a Poke</returns>
    public string GetPoke() => $"{StaticUrl}poke/{MakeRequest($"{BaseUrl}images/poke").Path}";

    /// <summary>
    /// Gets a URL to a random slap image or GIF
    /// </summary>
    /// <returns>URL as string of a Slap</returns>
    public string GetSlap() => $"{StaticUrl}slap/{MakeRequest($"{BaseUrl}images/slap").Path}";
}