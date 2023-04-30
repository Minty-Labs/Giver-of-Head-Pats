namespace HeadPats.Commands.Legacy.NSFW; 

public static class NsfwExtensions {
    public static async Task<string> GetImageUrlFromMediaType(string mediaType, string contentType, fluxpoint_sharp.FluxpointClient fluxpointClient) {
        if (mediaType.ToLower() is "image") {
            var image = contentType.ToLower() switch {
                "anal" => fluxpointClient.Nsfw.GetAnalAsync(),
                "ass" => fluxpointClient.Nsfw.GetAssAsync(),
                "azurlane" => fluxpointClient.Nsfw.GetAzurlaneAsync(),
                "bdsm" => fluxpointClient.Nsfw.GetBdsmAsync(),
                "blowjob" => fluxpointClient.Nsfw.GetBlowjobAsync(),
                "boobs" => fluxpointClient.Nsfw.GetBoobsAsync(),
                "cum" => fluxpointClient.Nsfw.GetCumAsync(),
                "futa" => fluxpointClient.Nsfw.GetFutaAsync(),
                "gasm" => fluxpointClient.Nsfw.GetGasmAsync(),
                "holo" => fluxpointClient.Nsfw.GetHoloAsync(),
                "kitsune" => fluxpointClient.Nsfw.GetKitsuneAsync(),
                "lewd" => fluxpointClient.Nsfw.GetLewdAsync(),
                "neko" => fluxpointClient.Nsfw.GetNekoAsync(),
                "nekopara" => fluxpointClient.Nsfw.GetNekoparaAsync(),
                "pantyhose" => fluxpointClient.Nsfw.GetPantyhoseAsync(),
                "petplay" => fluxpointClient.Nsfw.GetPetplayAsync(),
                "pussy" => fluxpointClient.Nsfw.GetPussyAsync(),
                "slimes" => fluxpointClient.Nsfw.GetSlimeAsync(),
                "solo" => fluxpointClient.Nsfw.GetSoloGirlAsync(),
                "swimsuit" => fluxpointClient.Nsfw.GetSwimsuitAsync(),
                "tentacle" => fluxpointClient.Nsfw.GetTentacleAsync(),
                "thighs" => fluxpointClient.Nsfw.GetThighsAsync(),
                "trap" => fluxpointClient.Nsfw.GetTrapAsync(),
                "yaoi" => fluxpointClient.Nsfw.GetYaoiAsync(),
                "yuri" => fluxpointClient.Nsfw.GetYuriAsync(),
                _ => throw new ArgumentOutOfRangeException()
            };

            return (await image).file ?? "https://totallywholeso.me/assets/img/team/null.jpg";
        }

        var gif = contentType.ToLower() switch {
            "anal" => fluxpointClient.Nsfw.GetAnalGifAsync(),
            "ass" => fluxpointClient.Nsfw.GetAssGifAsync(),
            "bdsm" => fluxpointClient.Nsfw.GetBdsmGifAsync(),
            "blowjob" => fluxpointClient.Nsfw.GetBlowjobGifAsync(),
            "boobjob" => fluxpointClient.Nsfw.GetBoobjobGifAsync(),
            "boobs" => fluxpointClient.Nsfw.GetBoobsGifAsync(),
            "cum" => fluxpointClient.Nsfw.GetCumGifAsync(),
            "futa" => fluxpointClient.Nsfw.GetFutaGifAsync(),
            "handjob" => fluxpointClient.Nsfw.GetHandjobGifAsync(),
            "hentai" => fluxpointClient.Nsfw.GetHentaiGifAsync(),
            // "kitsune" => fluxpointClient.Nsfw.GetKitsuneGifAsync(), // not yet on API
            "kuni" => fluxpointClient.Nsfw.GetKuniGifAsync(),
            "neko" => fluxpointClient.Nsfw.GetNekoAsync(),
            "pussy" => fluxpointClient.Nsfw.GetPussyGifAsync(),
            "solo" => fluxpointClient.Nsfw.GetSoloGirlGifAsync(),
            "spank" => fluxpointClient.Nsfw.GetSpankGifAsync(),
            "tentacle" => fluxpointClient.Nsfw.GetTentacleGifAsync(),
            "toys" => fluxpointClient.Nsfw.GetToysGifAsync(),
            "yuri" => fluxpointClient.Nsfw.GetYuriGifAsync(),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        return (await gif).file ?? "https://totallywholeso.me/assets/img/team/null.jpg";
    }
}