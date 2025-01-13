using HeadPats.Configuration;
using Serilog;

namespace HeadPats.Utils.ExternalApis;

public class LocalImages {
    private static readonly ILogger Logger = Log.ForContext<LocalImages>();
    private static List<LocalImageData> Images { get; } = [];
    
    public static void ReadFromLocalStorage() {
        Images.Clear();
        Logger.Information("Processing local images...");
        if (string.IsNullOrWhiteSpace(Config.Base.LocalImagePath)) {
            Logger.Error("LocalImagePath is not set in the config.");
            return;
        }
        var dirs = Directory.EnumerateDirectories(Config.Base.LocalImagePath);
        foreach (var dir in dirs) {
            var category = dir.Split('/').Last();
            if (!Enum.TryParse<Category>(category, true, out var finalCategory)) {
                Logger.Warning("Failed to parse category {category}", category);
                continue;
            }
            
            var files = Directory.EnumerateFiles(dir);
            foreach (var file in files) {
                var fileName = file.Split('/').Last();
                Images.Add(new LocalImageData {
                    Category = finalCategory,
                    URL = $"https://img.mili.lgbt/headpat-images/{category}/{fileName}",
                    FileName = fileName,
                    FilePath = file
                });
            }
        }
    }
    
    public static int CurrentRedPandaEntryNumber, CurrentFoxEntryNumber;
    
    public static string GetRandomImage(Category category) {
        var list = Images.Where(x => x.Category == category).ToList();
        var random = new Random().Next(0, list.Count);
        switch (category) {
            case Category.RedPanda:
                CurrentRedPandaEntryNumber = random;
                break;
            case Category.Foxes:
                CurrentFoxEntryNumber = random;
                break;
            case Category.Cookie: case Category.Cuddle: case Category.Hug: case Category.Kiss:
            case Category.Lick: case Category.Pat: case Category.Poke: case Category.Slap: break;
            default: throw new ArgumentOutOfRangeException(nameof(category), category, null);
        }
        
        return list[random].URL;
    }
    
    public static int GetCategoryCount(Category category) => Images.Count(x => x.Category == category);
}

public class LocalImageData {
    public Category Category { get; init; }
    public string URL { get; init; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
}

public enum Category {
    Cookie = 0,
    Cuddle = 1,
    Hug = 2,
    Kiss = 3,
    Lick = 4,
    Pat = 5,
    Poke = 6,
    Slap = 7,
    RedPanda = 8,
    Foxes = 9,
}