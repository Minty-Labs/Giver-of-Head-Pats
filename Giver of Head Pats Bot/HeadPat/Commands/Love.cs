﻿using DSharpPlus;
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
        if (ctx.TargetMember.Id == BuildInfo.ClientId)
            await ctx.CreateResponseAsync($"I got hugs from {ctx.User.Username}?! Thankies~");
        else if (ctx.TargetMember.Id == ctx.User.Id)
            await ctx.CreateResponseAsync("You cant give yourself hugs, but I'll gladly give you some!");
        else 
            await ctx.CreateResponseAsync($"{ctx.User.Username} hugged {target.ReplaceTheNamesWithTags()}!");
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
        if (c.TargetUser.Id == BuildInfo.ClientId) {
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
         public async Task Pat(ic c, [Option("user", "The user to pat")] DiscordUser user) {
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
             
             if (user.Id == c.User.Id) {
                 await c.CreateResponseAsync("You cannot give yourself headpats.", true);
                 return;
             }

             if (user.IsBot) {
                 await c.CreateResponseAsync("You cannot give bots headpats.", true);
                 return;
             }
             
             var gaveToBot = false;
             if (user.Id == BuildInfo.ClientId) {
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
             //e.WithDescription($"{c.User.Mention} gave {(special != 1 ? $"**{special}** headpats" : "a headpat")} to {user.Mention}");
             e.WithDescription(gaveToBot ? $"Gave headpats to {user.Mention}" : $"{c.User.Mention} gave {(special != 1 ? $"**{special}** headpats" : "a headpat")} to {user.Mention}");
             UserControl.AddPatToUser(user.Id, special, true, c.Guild.Id);
             await c.CreateResponseAsync(e.Build());
             Logger.Log($"Total Pat amount Given: {special}");
         }
         
         [SlashCommand("hug", "Hug a specified user.")]
         public async Task Hug(ic c, [Option("user", "The user to hug")] DiscordUser user) {
             if (user.Id == c.User.Id) {
                 await c.CreateResponseAsync("You cannot give yourself hugs.", true);
                 return;
             }

             if (user.IsBot || user.Id == BuildInfo.ClientId) {
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
         public async Task Cuddle(ic c, [Option("user", "The user to cuddle")] DiscordUser user) {
             if (user.Id == c.User.Id) {
                 await c.CreateResponseAsync("You cannot give yourself cuddles.", true);
                 return;
             }

             if (user.IsBot || user.Id == BuildInfo.ClientId) {
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
         public async Task Kiss(ic c, [Option("user", "The user to kiss")] DiscordUser user) {
             if (user.Id == c.User.Id) {
                 await c.CreateResponseAsync("You cannot give yourself kisses.", true);
                 return;
             }

             if (user.IsBot || user.Id == BuildInfo.ClientId) {
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
         public async Task Slap(ic c, [Option("user", "The user to slap")] DiscordUser user) {
             if (user.Id == c.User.Id) {
                 await c.CreateResponseAsync("You cannot slap yourself.", true);
                 return;
             }

             if (user.IsBot || user.Id == BuildInfo.ClientId) {
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
         public async Task Poke(ic c, [Option("user", "The user to poke")] DiscordUser user) {
             if (user.Id == c.User.Id) {
                 await c.CreateResponseAsync("You cannot poke yourself.", true);
                 return;
             }

             if (user.IsBot || user.Id == BuildInfo.ClientId) {
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
     }
}