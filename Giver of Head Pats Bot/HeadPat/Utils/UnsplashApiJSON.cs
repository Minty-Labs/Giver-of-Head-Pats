using System.Net;
using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace HeadPats.Utils; 

public class Exif {
    public string make { get; set; }
    public string model { get; set; }
    public string name { get; set; }
    public string exposure_time { get; set; }
    public string aperture { get; set; }
    public string focal_length { get; set; }
    public int? iso { get; set; }
}

public class FoodDrink {
    public string status { get; set; }
    public DateTime approved_on { get; set; }
}

public class Health {
    public string status { get; set; }
    public DateTime? approved_on { get; set; }
}

public class Links {
    public string self { get; set; }
    public string html { get; set; }
    public string download { get; set; }
    public string download_location { get; set; }
    public string photos { get; set; }
    public string likes { get; set; }
    public string portfolio { get; set; }
    public string following { get; set; }
    public string followers { get; set; }
}

public class Location {
    public string name { get; set; }
    public string city { get; set; }
    public string country { get; set; }
    public Position position { get; set; }
}

public class Position {
    public double? latitude { get; set; }
    public double? longitude { get; set; }
}

public class ProfileImage {
    public string small { get; set; }
    public string medium { get; set; }
    public string large { get; set; }
}

public class Social {
    public string instagram_username { get; set; }
    public string portfolio_url { get; set; }
    public string twitter_username { get; set; }
    public object paypal_email { get; set; }
}

public class TopicSubmissions {
    [JsonProperty("food-drink")]
    public FoodDrink fooddrink { get; set; }
    public Health health { get; set; }
}

public class Urls {
    public string raw { get; set; }
    public string full { get; set; }
    public string regular { get; set; }
    public string small { get; set; }
    public string thumb { get; set; }
    public string small_s3 { get; set; }
}

public class User {
    public string id { get; set; }
    public DateTime updated_at { get; set; }
    public string username { get; set; }
    public string name { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string twitter_username { get; set; }
    public string portfolio_url { get; set; }
    public string bio { get; set; }
    public string location { get; set; }
    public Links links { get; set; }
    public ProfileImage profile_image { get; set; }
    public string instagram_username { get; set; }
    public int total_collections { get; set; }
    public int total_likes { get; set; }
    public int total_photos { get; set; }
    public bool accepted_tos { get; set; }
    public bool for_hire { get; set; }
    public Social social { get; set; }
}

public class UnsplashApi {
    public string id { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public DateTime? promoted_at { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public string color { get; set; }
    public string blur_hash { get; set; }
    public string? description { get; set; }
    public string? alt_description { get; set; }
    public Urls urls { get; set; }
    public Links links { get; set; }
    public int likes { get; set; }
    public bool liked_by_user { get; set; }
    public List<object> current_user_collections { get; set; }
    public object sponsorship { get; set; }
    public TopicSubmissions topic_submissions { get; set; }
    public User user { get; set; }
    public Exif exif { get; set; }
    public Location location { get; set; }
    public int views { get; set; }
    public int downloads { get; set; }
}

/*public class UnsplashApi_Download {
    public string url { get; set; }
}*/

public class UnsplashApiJson {
    public static List<UnsplashApi>? unsplashApi;
    //public static UnsplashApi_Download? DownloadImage;
    public static void GetData(string data) => unsplashApi = JsonConvert.DeserializeObject<List<UnsplashApi>>(data);

    //public static void DownloadImageMethod(string data) => DownloadImage = JsonConvert.DeserializeObject<UnsplashApi_Download>(data);
    
    public static string GetImage() => unsplashApi![0].urls.regular;
    public static DateTime GetCreatedAt() => unsplashApi?[0].created_at ?? DateTime.Now;
    public static string GetAuthorName() => unsplashApi![0].user.name;
    public static string GetAuthorProfileLink() => unsplashApi![0].user.links.html;
    public static string GetAuthorProfileImage() => unsplashApi![0].user.profile_image.small;
    public static int GetLikes() => unsplashApi?[0].likes ?? 0;
    public static int GetDownloadCount() => unsplashApi?[0].downloads ?? 0;
    public static string GetImageId() => unsplashApi![0].id;
    public static DiscordColor GetColor() => Colors.HexToColor(unsplashApi![0].color);
    public static string? GetPostDescription() => unsplashApi![0].description;
    public static string? GetPostAltDescription() => unsplashApi![0].alt_description;
    public static string GetDownloadImageLink() => unsplashApi![0].links.download_location;

    // public static bool tempMethod;
    public static bool LikeImage(string imageId) {
        var url = $"https://api.unsplash.com/photos/{imageId}/like?client_id={BuildInfo.Config.UnsplashAccessKey}";
        var content = new StringContent(imageId, Encoding.UTF8);
        try {
            var likeAction = new HttpClient().PostAsync(url, content).GetAwaiter().GetResult();
            return likeAction.StatusCode is HttpStatusCode.Created or HttpStatusCode.OK;
        }
        catch (Exception e) {
            Logger.Error(e);
            return false;
        }
    }

    /*public static void LikeImageAction(string imageId, CommandContext c) {
        var url = $"https://api.unsplash.com/photos/{imageId}/like?client_id={BuildInfo.Config.UnsplashAccessKey}";
        var hit = 0;
        try {
            hit++;
            var content = new StringContent(imageId, Encoding.UTF8, "application/json");
            new HttpClient().PostAsync(url, content).GetAwaiter().GetResult();
        }
        catch {
            try {
                hit++;
                var content = new StringContent(imageId, Encoding.UTF8, "x-www-form-urlencoded");
                new HttpClient().PostAsync(url, content).GetAwaiter().GetResult();
            }
            catch {
                try {
                    hit++;
                    var content = new StringContent(imageId, Encoding.UTF8);
                    new HttpClient().PostAsync(url, content).GetAwaiter().GetResult();
                }
                catch {
                    c.RespondAsync($"Things really did not work. Hit {hit} times").GetAwaiter().GetResult();
                }
            }
        }
    }*/
    
    public static bool DislikeImage(string imageId) {
        var url = $"https://api.unsplash.com/photos/{imageId}/like?client_id={BuildInfo.Config.UnsplashAccessKey}";
        try {
            var likeAction = new HttpClient().DeleteAsync(url).GetAwaiter().GetResult();
            return likeAction.StatusCode is HttpStatusCode.NoContent or HttpStatusCode.OK;
        }
        catch (Exception e) {
            Logger.Error(e);
            return false;
        }
    }
}