namespace HeadPats.Commands.Legacy.NSFW; 

public static class NsfwExtensions {
    public static async Task<string> GetImageUrlFromMediaType(string mediaType, string contentType, fluxpoint_sharp.FluxpointClient fluxpointClient) {
        if (mediaType.ToLower() is "image") {
            switch (contentType.ToLower()) {
                case "anal":
                    var anal = await fluxpointClient.Nsfw.GetAnalAsync();
                    return anal.file;
                case "ass":
                    var ass = await fluxpointClient.Nsfw.GetAnalAsync();
                    return ass.file;
                case "azurlane":
                    var al = await fluxpointClient.Nsfw.GetAzurlaneAsync();
                    return al.file;
                case "bdsm":
                    var bdsm = await fluxpointClient.Nsfw.GetBdsmAsync();
                    return bdsm.file;
                case "blowjob":
                    var bj = await fluxpointClient.Nsfw.GetBlowjobAsync();
                    return bj.file;
                case "boobs":
                    var boobs = await fluxpointClient.Nsfw.GetBoobsAsync();
                    return boobs.file;
                case "cum":
                    var cum = await fluxpointClient.Nsfw.GetCumAsync();
                    return cum.file;
                case "futa":
                    var futa = await fluxpointClient.Nsfw.GetFutaAsync();
                    return futa.file;
                case "gasm":
                    var gasm = await fluxpointClient.Nsfw.GetGasmAsync();
                    return gasm.file;
                case "holo":
                    var holo = await fluxpointClient.Nsfw.GetHoloAsync();
                    return holo.file;
                case "kitsune":
                    var theBestOne = await fluxpointClient.Nsfw.GetKitsuneAsync();
                    return theBestOne.file;
                case "lewd":
                    var lewd = await fluxpointClient.Nsfw.GetLewdAsync();
                    return lewd.file;
                case "neko":
                    var neko = await fluxpointClient.Nsfw.GetNekoAsync();
                    return neko.file;
                case "nekopara":
                    var nekopara = await fluxpointClient.Nsfw.GetNekoparaAsync();
                    return nekopara.file;
                case "pantyhose":
                    var pantyhose = await fluxpointClient.Nsfw.GetPantyhoseAsync();
                    return pantyhose.file;
                case "petplay":
                    var petplay = await fluxpointClient.Nsfw.GetPetplayAsync();
                    return petplay.file;
                case "pussy":
                    var pussy = await fluxpointClient.Nsfw.GetPussyAsync();
                    return pussy.file;
                case "slimes":
                    var slimes = await fluxpointClient.Nsfw.GetSlimeAsync();
                    return slimes.file;
                case "solo":
                    var solo = await fluxpointClient.Nsfw.GetSoloGirlAsync();
                    return solo.file;
                case "swimsuit":
                    var swimsuit = await fluxpointClient.Nsfw.GetSwimsuitAsync();
                    return swimsuit.file;
                case "tentacle":
                    var tentacle = await fluxpointClient.Nsfw.GetTentacleAsync();
                    return tentacle.file;
                case "thighs":
                    var thighs = await fluxpointClient.Nsfw.GetThighsAsync();
                    return thighs.file;
                case "trap":
                    var trap = await fluxpointClient.Nsfw.GetTrapAsync();
                    return trap.file;
                case "yaoi":
                    var yaoi = await fluxpointClient.Nsfw.GetYaoiAsync();
                    return yaoi.file;
                case "yuri":
                    var yuri = await fluxpointClient.Nsfw.GetYuriAsync();
                    return yuri.file;
            }
        }
        switch (contentType.ToLower()) {
            case "anal":
                var anal = await fluxpointClient.Nsfw.GetAnalGifAsync();
                return anal.file;
            case "ass":
                var ass = await fluxpointClient.Nsfw.GetAssGifAsync();
                return ass.file;
            case "bdsm":
                var bdsm = await fluxpointClient.Nsfw.GetBdsmGifAsync();
                return bdsm.file;
            case "blowjob":
                var bj = await fluxpointClient.Nsfw.GetBlowjobGifAsync();
                return bj.file;
            case "boobjob":
                var bbj = await fluxpointClient.Nsfw.GetBoobjobGifAsync();
                return bbj.file;
            case "boobs":
                var boobs = await fluxpointClient.Nsfw.GetBoobsGifAsync();
                return boobs.file;
            case "cum":
                var cum = await fluxpointClient.Nsfw.GetCumGifAsync();
                return cum.file;
            case "futa":
                var futa = await fluxpointClient.Nsfw.GetFutaGifAsync();
                return futa.file;
            case "handjob":
                var hj = await fluxpointClient.Nsfw.GetHandjobGifAsync();
                return hj.file;
            case "hentai":
                var hentai = await fluxpointClient.Nsfw.GetHentaiGifAsync();
                return hentai.file;
            // case "kitsune":
            //     var theBestOne = await fluxpointClient.Nsfw.GetKitsuneGifAsync();
            //     return theBestOne.file;
            case "kuni":
                var kuni = await fluxpointClient.Nsfw.GetKuniGifAsync();
                return kuni.file;
            case "neko":
                var neko = await fluxpointClient.Nsfw.GetNekoGifAsync();
                return neko.file;
            case "pussy":
                var pussy = await fluxpointClient.Nsfw.GetPussyGifAsync();
                return pussy.file;
            case "solo":
                var solo = await fluxpointClient.Nsfw.GetSoloGirlGifAsync();
                return solo.file;
            case "spank":
                var spank = await fluxpointClient.Nsfw.GetSpankGifAsync();
                return spank.file;
            case "tentacle":
                var tentacle = await fluxpointClient.Nsfw.GetTentacleGifAsync();
                return tentacle.file;
            case "toys":
                var toys = await fluxpointClient.Nsfw.GetToysGifAsync();
                return toys.file;
            case "yuri":
                var yuri = await fluxpointClient.Nsfw.GetYuriGifAsync();
                return yuri.file;
        }

        return "https://totallywholeso.me/assets/img/team/null.jpg";
    }
}