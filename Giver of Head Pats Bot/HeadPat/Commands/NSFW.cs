using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HeadPats.Data.Models;
using HeadPats.Utils;
using cc = DSharpPlus.CommandsNext.CommandContext;

namespace HeadPats.Commands; 

public class Nsfw : BaseCommandModule {
    public Nsfw() => Logger.Loadodule("NSFWCommands");
    
    private string FooterText(string extra = "")
        => $"{BuildInfo.Name} (v{BuildInfo.Version}) • {BuildInfo.BuildDate}{(string.IsNullOrWhiteSpace(extra) ? "" : $" • {extra}")}";

    // public override Task BeforeExecutionAsync(cc c) {
    //     if (c.Message.Channel.IsChannelNsfw()) return Task.CompletedTask;
    //     var m = c.Client.SendMessageAsync(c.Message.Channel, "You cannot run this command in non-NSFW channels.");
    //     Task.Delay(10 * 1000);
    //     m!.Result.DeleteAsync();
    //     return Task.CompletedTask;
    // }

    private async Task OutputBaseCommand(cc c, string? imageUrl, string embedTitle = "") {
        if (!c.Message.Channel.IsChannelNsfw()) {
            var m = await c.RespondAsync("You cannot run this command in non-NSFW channels.");
            await Task.Delay(10 * 1000);
            await c.Message.DeleteAsync();
            await m.DeleteAsync();
            return;
        }
        
        var m1 = await c.RespondAsync("NSFW commands are not yet ready to use.");
        await Task.Delay(10 * 1000);
        await c.Message.DeleteAsync();
        await m1.DeleteAsync();
        OverlordControl.AddToCommandCounter();
        
        // var e = new DiscordEmbedBuilder();
        // e.WithTitle(embedTitle);
        // e.WithImageUrl(imageUrl);
        // e.WithColor(Colors.HexToColor("ff00ff"));
        // e.WithFooter(FooterText("Powered by nekos.life"));
        // await c.RespondAsync(e.Build());
    }
    
    [Command("Anal"), Description("(NSFW) Show a picture or GIF of anal")]
    public async Task Anal(cc c) {
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 1);

        var neko = num1 == 0 ? Program.NekoClient?.Nsfw.AnalGif() : Program.NekoClient?.Nsfw_v3.Anal();

        await OutputBaseCommand(c, neko?.Result.ImageUrl, "Booty");
    }
    
    [Command("Blowjob"), Aliases("bj"), Description("(NSFW) Show a picture or GIF of blowjob")]
    public async Task Blowjob(cc c) {
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 1);

        var neko = num1 == 0 ? Program.NekoClient?.Nsfw.Blowjob() : Program.NekoClient?.Nsfw.BlowjobGif();

        await OutputBaseCommand(c, neko?.Result.ImageUrl, "Blowjob");
    }

    [Command("Boobs"), Aliases("boob"), Description("(NSFW) Show a picture or GIF of boobs")]
    public async Task Boob(cc c) {
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 1);

        var neko = num1 == 0 ? Program.NekoClient?.Nsfw.Boobs() : Program.NekoClient?.Nsfw.BoobsGif();

        Logger.Log($"{num1} : {neko?.Result.ImageUrl}");
        await OutputBaseCommand(c, neko?.Result.ImageUrl, "Booba");
    }
    
    [Command("Cum"), Description("(NSFW) Show a picture or GIF of cum")]
    public async Task Cum(cc c) {
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 1);

        var neko = num1 == 0 ? Program.NekoClient?.Nsfw.Cum() : Program.NekoClient?.Nsfw.CumGif();

        await OutputBaseCommand(c, neko?.Result.ImageUrl, "Cummy");
    }
    
    [Command("Feet"), Description("(NSFW) Show a picture or GIF of feet")]
    public async Task Feet(cc c) {
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 2);

        var neko = num1 switch {
            1 => Program.NekoClient?.Nsfw.FeetGif(),
            2 => Program.NekoClient?.Nsfw.LewdFeet(),
            _ => Program.NekoClient?.Nsfw.Feet()
        };
        Logger.Log($"{num1} : {neko?.Result.ImageUrl}");
        await OutputBaseCommand(c, neko?.Result.ImageUrl, "Feet");
    }
    
    [Command("Futanari"), Aliases("Futa"), Description("(NSFW) Show a picture or GIF of futanari")]
    public async Task Futanari(cc c) {
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 1);
        
        var neko = num1 == 0 ? Program.NekoClient?.Nsfw.Futanari() : Program.NekoClient?.Nsfw_v3.Futanari();

        await OutputBaseCommand(c, neko?.Result.ImageUrl, "Futa");
    }
    
    [Command("Gasm"), Description("(NSFW) Show a picture or GIF of gasm")]
    public async Task Gasm(cc c) {
        var neko = Program.NekoClient?.Nsfw.GasmAvatar();

        await OutputBaseCommand(c, neko?.Result.ImageUrl, "Gasm");
    }
    
    [Command("Hentai"), Description("(NSFW) Show a picture or GIF of hentai")]
    public async Task Hentai(cc c) {
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 3);

        var neko = num1 switch {
            1 => Program.NekoClient?.Nsfw_v3.Hentai(),
            2 => Program.NekoClient?.Nsfw.HentaiGif(),
            3 => Program.NekoClient?.Nsfw_v3.HentaiGif(),
            _ => Program.NekoClient?.Nsfw.Hentai()
        };

        await OutputBaseCommand(c, neko?.Result.ImageUrl, "...and it\'s art.");
    }
    
    [Command("Lewd"), Description("(NSFW) Show a picture or GIF of all things lewd")]
    public async Task Lewd(cc c) {
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 6);

        var neko = num1 switch {
            1 => Program.NekoClient?.Nsfw_v3.Lewd(),
            2 => Program.NekoClient?.Nsfw.LewdFeet(),
            3 => Program.NekoClient?.Nsfw.LewdFox(),
            4 => Program.NekoClient?.Nsfw.LewdHolo(),
            5 => Program.NekoClient?.Nsfw.LewdNeko(),
            6 => Program.NekoClient?.Nsfw.LewdYuri(),
            _ => Program.NekoClient?.Nsfw.Lewd()
        };

        await OutputBaseCommand(c, neko?.Result.ImageUrl, "Oh my, how lewd~");
    }
    
    [Command("Pussy"), Description("(NSFW) Show a picture or GIF of pussy")]
    public async Task Pussy(cc c) {
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 3);

        var neko = num1 switch {
            1 => Program.NekoClient?.Nsfw_v3.Pussy(),
            2 => Program.NekoClient?.Nsfw_v3.PussyGif(),
            3 => Program.NekoClient?.Nsfw.PussyGif(),
            _ => Program.NekoClient?.Nsfw.Pussy()
        };

        await OutputBaseCommand(c, neko?.Result.ImageUrl, "Meow");
    }
    
    [Command("Solo"), Description("(NSFW) Show a picture or GIF of solo content")]
    public async Task Solo(cc c) {
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 3);

        var neko = num1 switch {
            1 => Program.NekoClient?.Nsfw_v3.Solo(),
            2 => Program.NekoClient?.Nsfw_v3.SoloGif(),
            3 => Program.NekoClient?.Nsfw.SoloGif(),
            _ => Program.NekoClient?.Nsfw.Solo()
        };

        await OutputBaseCommand(c, neko?.Result.ImageUrl, "Gotta go solo");
    }
    
    [Command("Spank"), Description("(NSFW) Show a picture or GIF of spanking")]
    public async Task Spank(cc c) {
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 1);

        var neko = num1 switch {
            1 => Program.NekoClient?.Nsfw_v3.SpankGif(),
            _ => Program.NekoClient?.Nsfw.Spank()
        };

        await OutputBaseCommand(c, neko?.Result.ImageUrl, "_Ouch_");
    }
    
    [Command("Trap"), Description("(NSFW) Show a picture or GIF of traps")]
    public async Task Trap(cc c) {
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 1);

        var neko = num1 switch {
            1 => Program.NekoClient?.Nsfw_v3.Trap(),
            _ => Program.NekoClient?.Nsfw.Trap()
        };

        await OutputBaseCommand(c, neko?.Result.ImageUrl, "Are you sure that\'s a boy?");
    }
    
    [Command("Yuri"), Description("(NSFW) Show a picture or GIF of yuri")]
    public async Task Yuri(cc c) {
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 3);

        var neko = num1 switch {
            1 => Program.NekoClient?.Nsfw_v3.Yuri(),
            2 => Program.NekoClient?.Nsfw_v3.YuriGif(),
            3 => Program.NekoClient?.Nsfw.YuriGif(),
            _ => Program.NekoClient?.Nsfw.Yuri()
        };

        await OutputBaseCommand(c, neko?.Result.ImageUrl, "Yuri");
    }
}