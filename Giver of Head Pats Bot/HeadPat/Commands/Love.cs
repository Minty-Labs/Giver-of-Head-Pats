using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HeadPats.Data;
using HeadPats.Utils;
using HeadPats.Data.Models;
using HeadPats.Managers;
using ic = DSharpPlus.SlashCommands.InteractionContext;

namespace HeadPats.Commands;

public class LoveSlash : ApplicationCommandModule {
    
    private static string _tempPatGifUrl, _tempHugGifUrl, _tempSlapGifUrl, _tempCuddleGifUrl, _tempPokeGifUrl, _tempKissGifUrl;

    [ContextMenu(ApplicationCommandType.UserContextMenu, "Hug")]
    public async Task Hug(ContextMenuContext ctx) {
        var target = ctx.TargetMember.Username + "#" + ctx.TargetMember.Discriminator;
        var author = ctx.User.Username;
        if (ctx.TargetMember.Id == Vars.ClientId)
            await ctx.CreateResponseAsync($"I got hugs from {author.ReplaceTheNames()}?! Thankies~");
        else if (ctx.TargetMember.Id == ctx.User.Id)
            await ctx.CreateResponseAsync("You cant give yourself hugs, but I'll gladly give you some!");
        else 
            await ctx.CreateResponseAsync($"{author.ReplaceTheNames()} hugged {target.ReplaceTheNamesWithTags()}!");
    }
    
    [ContextMenu(ApplicationCommandType.UserContextMenu, "Pat")]
    public async Task Pat(ContextMenuContext c) {
        await using var db = new Context();
        var checkGuild = db.Guilds.AsQueryable()
            .Where(u => u.GuildId.Equals(c.Guild.Id)).ToList().FirstOrDefault();
        
        var checkUser = db.Users.AsQueryable()
            .Where(u => u.UserId.Equals(c.User.Id)).ToList().FirstOrDefault();

        var isRoleBlackListed = c.Member!.Roles.Any(x => x.Id == checkGuild!.HeadPatBlacklistedRoleId && checkGuild.HeadPatBlacklistedRoleId != 0);

        if (isRoleBlackListed) {
            await c.CreateResponseAsync("This role is not allowed to use this command. This was set by a server administrator.", true);
            return;
        }
        
        var isUserBlackListed = checkUser!.IsUserBlacklisted == 1;
        
        if (isUserBlackListed) {
            await c.CreateResponseAsync("You are not allowed to use this command. This was set by a bot developer.", true);
            return;
        }
        
        if (c.TargetUser.Id == c.User.Id) {
            await c.CreateResponseAsync("You cannot give yourself headpats.", true);
            return;
        }

        if (c.TargetUser.IsBot) {
            await c.CreateResponseAsync("You cannot give bots headpats.", true);
            return;
        }

        var gaveToBot = false;
        if (c.TargetUser.Id == Vars.ClientId) {
            await c.CreateResponseAsync("How dare you give me headpats! No, have some of your own~");
            gaveToBot = true;
            await Task.Delay(300);
        }

        var neko = new Random().Next(0, 1) == 0 ? Program.NekoClient?.Action.Pat() : Program.NekoClient?.Action_v3.PatGif();
        
        var num = new Random().Next(0, 3);
        var outputs = new[] { "_pat pat_", "_Pats_", "_pet pet_", "_**mega pats**_" };
        
        var special = num == 3 ? 2 : 1;
        
        start:
        var image = neko?.Result.ImageUrl;
        if (BlacklistedNekosLifeGifs.BlacklistedGifs.Urls!.Any(i => i.Equals(image!))) {
            Logger.Log("Hit a blacklisted GIF URL");
            goto start;
        }
        
        if (image!.Equals(_tempPatGifUrl)) {
            Logger.Log("Image is same as previous image");
            goto start;
        }

        _tempPatGifUrl = image;

        var e = new DiscordEmbedBuilder();
        e.WithTitle(outputs[num]);
        e.WithImageUrl(image);
        e.WithColor(Colors.HexToColor("ffff00"));
        e.WithFooter("Powered by nekos.life");
        e.WithDescription(gaveToBot ? $"Gave headpats to {c.TargetUser.Mention}" : $"{c.User.Mention} gave {(special != 1 ? $"**{special}** headpats" : "a headpat")} to {c.TargetUser.Mention}");
        UserControl.AddPatToUser(c.TargetUser.Id, special, true, c.Guild.Id);
        await c.CreateResponseAsync(e.Build());
        Logger.Log($"Total Pat amount Given: {special}");
    }

     [SlashCommandGroup("user", "User Actions")]
     public class LoveUser : ApplicationCommandModule {
     
         [SlashCommand("pat", "Pat a specified user.")]
         public async Task Pat(ic c, [Option("user", "The user to pat", true)] DiscordUser user, 
             [Option("params", "Extra parameters (for the bot owner)")] string extraParams = "") {
             var canUseParams = c.User.Id == 167335587488071682;
             var hasCommandBlacklist = BlacklistedCmdsGuilds.BlacklistedGuilds.Guilds!.Any(g => g.Id.Equals(c.Guild.Id));
             if (hasCommandBlacklist) {
                 var isThisCommandBlacklisted = BlacklistedCmdsGuilds.BlacklistedGuilds.Guilds!.Where(g => g.Id.Equals(c.Guild.Id))
                     .ToList().FirstOrDefault()!.CommandsToBlock.Any(c => c.Equals("pat"));
                 if (isThisCommandBlacklisted) {
                     await c.CreateResponseAsync("This guild is not allowed to use this command. This was set by a bot developer.", true);
                     return;
                 }
             }
             
             await using var db = new Context();
             var checkGuild = db.Guilds.AsQueryable()
                 .Where(u => u.GuildId.Equals(c.Guild.Id)).ToList().FirstOrDefault();
         
             var checkUser = db.Users.AsQueryable()
                 .Where(u => u.UserId.Equals(c.User.Id)).ToList().FirstOrDefault();

             bool isRoleBlackListed, failed = false;
             try {
                 isRoleBlackListed = c.Member!.Roles.Any(x => x.Id == checkGuild!.HeadPatBlacklistedRoleId);
             }
             catch (Exception ex) {
                 try {
                     isRoleBlackListed = c.Member!.Roles.FirstOrDefault()!.Id == checkGuild!.HeadPatBlacklistedRoleId;
                 }
                 catch (Exception ex2) {
                     failed = true;
                     if (!Vars.IsDebug) return;
                     Logger.SendLog(ex, true);
                     Logger.SendLog(ex2, true);
                     return;
                 }
             }
             if (failed) return;

             if (isRoleBlackListed) {
                 await c.CreateResponseAsync("This role is not allowed to use this command. This was set by a server administrator.", true);
                 return;
             }

             if (checkUser is null) {
                 var newUser = new Users {
                     UserId = user.Id,
                     UsernameWithNumber = $"{user.Username}#{user.Discriminator}",
                     PatCount = 0,
                     CookieCount = 0,
                     IsUserBlacklisted = 0
                 };
                 Logger.Log("Added user to database");
                 db.Users.Add(newUser);
             }
         
             var isUserBlackListed = checkUser is not null && checkUser.IsUserBlacklisted == 1;
         
             if (isUserBlackListed) {
                 await c.CreateResponseAsync("You are not allowed to use this command. This was set by a bot developer.", true);
                 return;
             }
             
             if (user.Id == c.User.Id) {
                 await c.CreateResponseAsync("You cannot give yourself headpats.", true);
                 return;
             }

             if (user.IsBot) {
                 await c.CreateResponseAsync("You cannot give bots headpats.", true);
                 return;
             }
             
             var gaveToBot = false;
             if (user.Id == Vars.ClientId) {
                 await c.CreateResponseAsync("How dare you give me headpats! No, have some of your own~");
                 gaveToBot = true;
                 await Task.Delay(300);
             }

             var neko = new Random().Next(0, 1) == 0 ? Program.NekoClient?.Action.Pat() : Program.NekoClient?.Action_v3.PatGif();

             var num = new Random().Next(0, 3);
             var outputs = new[] { "_pat pat_", "_Pats_", "_pet pet_", "_**mega pats**_" };

             var special = num == 3 ? 2 : 1;
             var doingTheEllySpecial = false;
             
             switch (canUseParams) {
                 case true: {
                     if (!string.IsNullOrWhiteSpace(extraParams)) {
                         if (extraParams.Equals("mega"))
                             special = 2;

                         if (extraParams.Contains('%'))
                             special = int.Parse(extraParams.Split('%')[1]);

                         if (extraParams.ToLower().Contains("elly")) {
                             special = 5;
                             doingTheEllySpecial = true;
                         }
                     }

                     break;
                 }
                 case false when !string.IsNullOrWhiteSpace(extraParams):
                     await c.CreateResponseAsync("You do not have permission to use extra parameters.", true);
                     return;
             }

             start:
             var image = neko?.Result.ImageUrl;
             if (BlacklistedNekosLifeGifs.BlacklistedGifs.Urls!.Any(i => i.Equals(image!))) {
                 Logger.Log("Hit a blacklisted GIF URL");
                 goto start;
             }
             
             if (image!.Equals(_tempPatGifUrl)) {
                 Logger.Log("Image is same as previous image");
                 goto start;
             }

             _tempPatGifUrl = image;

             var e = new DiscordEmbedBuilder();
             e.WithTitle(outputs[num]);
             e.WithImageUrl(image);
             e.WithColor(Colors.HexToColor("ffff00"));
             e.WithFooter("Powered by nekos.life");
             if (doingTheEllySpecial)
                 e.WithDescription(gaveToBot ? $"Gave headpats to {user.Mention}" : $"{c.User.Mention} gave **{special}** headpats to {user.Mention}");
             else 
                 e.WithDescription(gaveToBot ? $"Gave headpats to {user.Mention}" : $"{c.User.Mention} gave {(special != 1 ? $"**{special}** headpats" : "a headpat")} to {user.Mention}");
             UserControl.AddPatToUser(user.Id, special, true, c.Guild.Id);
             await c.CreateResponseAsync(e.Build());
             Logger.Log($"Total Pat amount Given: {special}");
         }
         
         [SlashCommand("hug", "Hug a specified user.")]
         public async Task Hug(ic c, [Option("user", "The user to hug", true)] DiscordUser user) {
             var hasCommandBlacklist = BlacklistedCmdsGuilds.BlacklistedGuilds.Guilds!.Any(g => g.Id.Equals(c.Guild.Id));
             if (hasCommandBlacklist) {
                 var isThisCommandBlacklisted = BlacklistedCmdsGuilds.BlacklistedGuilds.Guilds!.Where(g => g.Id.Equals(c.Guild.Id))
                     .ToList().FirstOrDefault()!.CommandsToBlock.Any(c => c.Equals("hug"));
                 if (isThisCommandBlacklisted) {
                     await c.CreateResponseAsync("This guild is not allowed to use this command. This was set by a bot developer.", true);
                     return;
                 }
             }
             
             if (user.Id == c.User.Id) {
                 await c.CreateResponseAsync("You cannot give yourself hugs.", true);
                 return;
             }

             if (user.IsBot || user.Id == Vars.ClientId) {
                 await c.CreateResponseAsync("You cannot give bots hugs.", true);
                 return;
             }

             var neko = new Random().Next(0, 1) == 0 ? Program.NekoClient?.Action.Hug() : Program.NekoClient?.Action_v3.HugGif();

             var num = new Random().Next(0, 3);
             var outputs = new[] { "_huggies_", "_huggle_", "_hugs_", "_ultra hugs_" };

             start:
             var image = neko?.Result.ImageUrl;
             if (image == null) {
                 Logger.Log("Image is null, restarting to get new image");
                 goto start;
             }
             if (BlacklistedNekosLifeGifs.BlacklistedGifs.Urls!.Any(i => i.Equals(image!))) {
                 Logger.Log("Hit a blacklisted GIF URL");
                 goto start;
             }
             if (image!.Equals(_tempHugGifUrl)) {
                 Logger.Log("Image is same as previous image");
                 goto start;
             }

             _tempHugGifUrl = image;

             var e = new DiscordEmbedBuilder();
             e.WithTitle(outputs[num]);
             e.WithImageUrl(image);
             e.WithColor(Colors.HexToColor("6F41B6"));
             e.WithFooter("Powered by nekos.life");
             e.WithDescription($"{c.User.Mention} gave a hug to {user.Mention}");
             await c.CreateResponseAsync(e.Build());
         }

         [SlashCommand("Cuddle", "Cuddle a specified user.")]
         public async Task Cuddle(ic c, [Option("user", "The user to cuddle", true)] DiscordUser user) {
             var hasCommandBlacklist = BlacklistedCmdsGuilds.BlacklistedGuilds.Guilds!.Any(g => g.Id.Equals(c.Guild.Id));
             if (hasCommandBlacklist) {
                 var isThisCommandBlacklisted = BlacklistedCmdsGuilds.BlacklistedGuilds.Guilds!.Where(g => g.Id.Equals(c.Guild.Id))
                     .ToList().FirstOrDefault()!.CommandsToBlock.Any(c => c.Equals("cuddle"));
                 if (isThisCommandBlacklisted) {
                     await c.CreateResponseAsync("This guild is not allowed to use this command. This was set by a bot developer.", true);
                     return;
                 }
             }
             
             if (user.Id == c.User.Id) {
                 await c.CreateResponseAsync("You cannot give yourself cuddles.", true);
                 return;
             }

             if (user.IsBot || user.Id == Vars.ClientId) {
                 await c.CreateResponseAsync("You cannot give bots cuddles.", true);
                 return;
             }

             var neko = new Random().Next(0, 1) == 0 ? Program.NekoClient?.Action.Cuddle() : Program.NekoClient?.Action_v3.CuddleGif();
        
             var num = new Random().Next(0, 4);
             var outputs = new[] { "_snuggies_", "_snuggles_", "_snugs_", "_cuddles_", "_ultra cuddles_" };
        
             start:
             var image = neko?.Result.ImageUrl;
             if (image == null) {
                 Logger.Log("Image is null, restarting to get new image");
                 goto start;
             }
             if (image.Equals(_tempCuddleGifUrl)) {
                 Logger.Log("Image is same as previous image");
                 goto start;
             }

             _tempCuddleGifUrl = image;
             
             var e = new DiscordEmbedBuilder();
             e.WithTitle(outputs[num]);
             e.WithImageUrl(image);
             e.WithColor(Colors.HexToColor("3498DB"));
             e.WithFooter("Powered by nekos.life");
             e.WithDescription($"{c.User.Mention} gave cuddles to {user.Mention}");
             await c.CreateResponseAsync(e.Build());
         }

         [SlashCommand("kiss", "Kiss a specified user.")]
         public async Task Kiss(ic c, [Option("user", "The user to kiss", true)] DiscordUser user) {
             var hasCommandBlacklist = BlacklistedCmdsGuilds.BlacklistedGuilds.Guilds!.Any(g => g.Id.Equals(c.Guild.Id));
             if (hasCommandBlacklist) {
                 var isThisCommandBlacklisted = BlacklistedCmdsGuilds.BlacklistedGuilds.Guilds!.Where(g => g.Id.Equals(c.Guild.Id))
                     .ToList().FirstOrDefault()!.CommandsToBlock.Any(c => c.Equals("kiss"));
                 if (isThisCommandBlacklisted) {
                     await c.CreateResponseAsync("This guild is not allowed to use this command. This was set by a bot developer.", true);
                     return;
                 }
             }
             
             if (user.Id == c.User.Id) {
                 await c.CreateResponseAsync("You cannot give yourself kisses.", true);
                 return;
             }

             if (user.IsBot || user.Id == Vars.ClientId) {
                 await c.CreateResponseAsync("You cannot give bots kisses.", true);
                 return;
             }

             var neko = new Random().Next(0, 1) == 0 ? Program.NekoClient?.Action.Kiss() : Program.NekoClient?.Action_v3.KissGif();

             var num = new Random().Next(0, 4);
             var outputs = new[] { "_kisses_", "_kissies_", "_kissies_", "_kisses_", "_ultra kisses_" };

             start:
             var image = neko?.Result.ImageUrl;
             if (image == null) {
                 Logger.Log("Image is null, restarting to get new image");
                 goto start;
             }

             if (image.Equals(_tempKissGifUrl)) {
                 Logger.Log("Image is same as previous image");
                 goto start;
             }

             _tempKissGifUrl = image;

             var e = new DiscordEmbedBuilder();
             e.WithTitle(outputs[num]);
             e.WithImageUrl(image);
             e.WithColor(Colors.HexToColor("F771A3"));
             e.WithFooter("Powered by nekos.life");
             e.WithDescription($"{c.User.Mention} gave kisses to {user.Mention}");
             await c.CreateResponseAsync(e.Build());
         }

         [SlashCommand("slap", "Slap a specified user.")]
         public async Task Slap(ic c, [Option("user", "The user to slap", true)] DiscordUser user) {
             var hasCommandBlacklist = BlacklistedCmdsGuilds.BlacklistedGuilds.Guilds!.Any(g => g.Id.Equals(c.Guild.Id));
             if (hasCommandBlacklist) {
                 var isThisCommandBlacklisted = BlacklistedCmdsGuilds.BlacklistedGuilds.Guilds!.Where(g => g.Id.Equals(c.Guild.Id))
                     .ToList().FirstOrDefault()!.CommandsToBlock.Any(c => c.Equals("slap"));
                 if (isThisCommandBlacklisted) {
                     await c.CreateResponseAsync("This guild is not allowed to use this command. This was set by a bot developer.", true);
                     return;
                 }
             }
             
             if (user.Id == c.User.Id) {
                 await c.CreateResponseAsync("You cannot slap yourself.", true);
                 return;
             }

             if (user.IsBot || user.Id == Vars.ClientId) {
                 await c.CreateResponseAsync("You cannot slap bots.", true);
                 return;
             }

             var neko = new Random().Next(0, 1) == 0 ? Program.NekoClient?.Action.Slap() : Program.NekoClient?.Action_v3.SlapGif();

             var num = new Random().Next(0, 4);
             var outputs = new[] { "_slaps_", "_slaps_", "_slaps_", "_slaps_", "_ultra slaps_" };

             start:
             var image = neko?.Result.ImageUrl;
             if (image == null) {
                 Logger.Log("Image is null, restarting to get new image");
                 goto start;
             }

             if (image.Equals(_tempSlapGifUrl)) {
                 Logger.Log("Image is same as previous image");
                 goto start;
             }

             _tempSlapGifUrl = image;

             var e = new DiscordEmbedBuilder();
             e.WithTitle(outputs[num]);
             e.WithImageUrl(image);
             e.WithColor(Colors.HexToColor("E74C3C"));
             e.WithFooter("Powered by nekos.life");
             e.WithDescription($"{c.User.Mention} slapped {user.Mention}");
             await c.CreateResponseAsync(e.Build());
         }
         
         [SlashCommand("Poke", "Poke a user.")]
         public async Task Poke(ic c, [Option("user", "The user to poke", true)] DiscordUser user) {
             var hasCommandBlacklist = BlacklistedCmdsGuilds.BlacklistedGuilds.Guilds!.Any(g => g.Id.Equals(c.Guild.Id));
             if (hasCommandBlacklist) {
                 var isThisCommandBlacklisted = BlacklistedCmdsGuilds.BlacklistedGuilds.Guilds!.Where(g => g.Id.Equals(c.Guild.Id))
                     .ToList().FirstOrDefault()!.CommandsToBlock.Any(c => c.Equals("poke"));
                 if (isThisCommandBlacklisted) {
                     await c.CreateResponseAsync("This guild is not allowed to use this command. This was set by a bot developer.", true);
                     return;
                 }
             }

             if (user.Id == c.User.Id) {
                 await c.CreateResponseAsync("You cannot poke yourself.", true);
                 return;
             }

             if (user.IsBot || user.Id == Vars.ClientId) {
                 await c.CreateResponseAsync("You cannot poke bots.", true);
                 return;
             }

             var neko = new Random().Next(0, 1) == 0 ? Program.NekoClient?.Action.Poke() : Program.NekoClient?.Action_v3.PokeGif();

             var num = new Random().Next(0, 4);
             var outputs = new[] { "_pokes_", "_pokes_", "_pokes_", "_pokes_", "_ultra pokes_" };

             start:
             var image = neko?.Result.ImageUrl;
             if (image == null) {
                 Logger.Log("Image is null, restarting to get new image");
                 goto start;
             }

             if (image.Equals(_tempPokeGifUrl)) {
                 Logger.Log("Image is same as previous image");
                 goto start;
             }

             _tempPokeGifUrl = image;

             var e = new DiscordEmbedBuilder();
             e.WithTitle(outputs[num]);
             e.WithImageUrl(image);
             e.WithColor(Colors.HexToColor("0E4730"));
             e.WithFooter("Powered by nekos.life");
             e.WithDescription($"{c.User.Mention} poked {user.Mention}");
             await c.CreateResponseAsync(e.Build());
         }

         [SlashCommand("Cookie", "Give a user a cookie.")]
         public async Task Cookie(ic c, [Option("user", "The user to give a cookie to", true)] DiscordUser user) {
             var hasCommandBlacklist = BlacklistedCmdsGuilds.BlacklistedGuilds.Guilds!.Any(g => g.Id.Equals(c.Guild.Id));
             if (hasCommandBlacklist) {
                 var isThisCommandBlacklisted = BlacklistedCmdsGuilds.BlacklistedGuilds.Guilds!.Where(g => g.Id.Equals(c.Guild.Id))
                     .ToList().FirstOrDefault()!.CommandsToBlock.Any(c => c.Equals("cookie"));
                 if (isThisCommandBlacklisted) {
                     await c.CreateResponseAsync("This guild is not allowed to use this command. This was set by a bot developer.", true);
                     return;
                 }
             }

             if (user.Id == c.User.Id) {
                 await c.CreateResponseAsync("You requested a cookie, so here you go!");
                 UserControl.AddCookieToUser(c.User.Id, 1);
             } else if (user.IsBot || user.Id == Vars.ClientId) {
                 await c.CreateResponseAsync("I cannot do anything with a cookie, so here, you have it!");
                 UserControl.AddCookieToUser(c.User.Id, 1);
             } else {
                 await c.CreateResponseAsync($"{c.User.Mention} gave {user.Mention} a cookie.");
                 UserControl.AddCookieToUser(user.Id, 1);
             }
         }
     }
}