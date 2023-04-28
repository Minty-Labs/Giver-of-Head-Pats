using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Managers;

namespace HeadPats.Commands.Legacy.NSFW; 

public class FunnyCommands : BaseCommandModule {
    
    [Command("nsfw"), Description("NSFW Commands"), RequireNsfw, CanUseNsfwCommands, Hidden]
    public async Task Nsfw(CommandContext ctx, 
        [Description("Media Type (Image or GIF)")] string mediaType,
        [Description("Content Type / Endpoint (Do hp!nsfw <media> list)")] string contentType = "list") {

        if (!ctx.Channel.IsNSFW) {
            await ctx.RespondAsync("You cannot run NSFW commands in a non-NSFW channel.").DeleteAfter(5);
            return;
        }
        
        if (string.IsNullOrWhiteSpace(mediaType)) {
            await ctx.RespondAsync("Please specify a valid media type [`image` or `gif`]\n" +
                                   "If you are looking for a list of endpoints, please do `hp!nsfw <media> list`; media being `image` or `gif`.");
            return;
        }

        if (contentType.ToLower() is "list") {
            switch (mediaType.ToLower()) {
                case "image":
                    await ctx.RespondAsync("Available Endpoints:\n" +
                                           "`anal, ass, azurlane, bdsm, blowjob, boobs, cum, futa, gasm, holo, kitsune, lewd, neko, nekopara, pantyhose, petplay, " +
                                           "pussy, slimes, solo, swimsuit, tentacle, thighs, trap, yaoi, yuri`");
                    return;
                case "gif":
                    await ctx.RespondAsync("Available Endpoints:\n" +
                                           "`anal, ass, bdsm, blowjob, boobjob, boobs, cum, futa, handjob, hentai, kuni, neko, pussy, solo, spank, " +
                                           "tentacle, toys, yuri`");
                    return;
                default:
                    await ctx.RespondAsync("Please specify a valid media type [`image` or `gif`]");
                    return;
            }
        }

        if (string.IsNullOrWhiteSpace(Vars.Config.FluxpointApiKey!)) {
            await ctx.RespondAsync("The bot owner has not setup access to the API for this command; therefore, this command will not continue.");
            return;
        }
        
        await using var db = new Context();
        var check = db.Overall.AsQueryable()
            .Where(u => u.ApplicationId.Equals(Vars.ClientId)).ToList().FirstOrDefault();
        
        if (check is null) {
            var overall = new Overlord {
                ApplicationId = Vars.ClientId,
                PatCount = 0,
                NsfwCommandsUsed = 0
            };
            db.Overall.Add(overall);
        }
        else {
            check.NsfwCommandsUsed++;
            db.Overall.Update(check);
        }
        await db.SaveChangesAsync();

        var embed = new DiscordEmbedBuilder {
            Title = contentType,
            ImageUrl = await NsfwExtensions.GetImageUrlFromMediaType(mediaType, contentType, Program.FluxpointClient!),
            Footer = new DiscordEmbedBuilder.EmbedFooter {
                Text = "Powered by Fluxpoint API"
            }
        };

        await ctx.RespondAsync(embed);
    }
    
}