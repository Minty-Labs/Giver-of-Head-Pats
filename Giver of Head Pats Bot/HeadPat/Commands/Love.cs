﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HeadPats.Data;
using HeadPats.Utils;
using HeadPats.Data.Models;
using cc = DSharpPlus.CommandsNext.CommandContext;
using ic = DSharpPlus.SlashCommands.InteractionContext;

namespace HeadPats.Commands; 

public class Love : BaseCommandModule {
    public Love() => Logger.LoadModule("LoveCommands");

    public static string tempPatGifUrl, tempHugGifUrl, tempSlapGifUrl, tempCuddleGifUrl, tempPokeGifUrl;

    private async Task OutputBaseCommand(cc c, string? mentionedUser, string? imageUrlFromApi, string embedTitle, string embedDesc, string action, int pats, string embedColorHex = "ffff00", bool deleteMessage = true) {
        if (deleteMessage)
            await c.Message.DeleteAsync("Auto Delete from command");
        ulong number = 0;
        var getUserIdFromMention = mentionedUser?.Replace("<@", "").Replace(">", "");
        if (getUserIdFromMention != null) {
            try {
                number = ulong.Parse(getUserIdFromMention);
            }
            catch (Exception ex) {
                await c.RespondAsync("Incorrect format, please only mention a user; this command does not support mentioning a role or plain text.");
                Logger.Error($"Failed to parse data \"{mentionedUser}\" into a ulong.\nGuild: {c.Guild.Name} ({c.Guild.Id}) -> #{c.Channel.Name}\nFull Error:\n{ex}");
                return;
            }
        }
        var user = c.Client.GetUserAsync(number).GetAwaiter().GetResult();
        
        if (c.Message.Author.Id == user?.Id) {
            await c.RespondAsync($"You cannot give yourself {action}.");
            return;
        }
        
        if (user!.IsBot) {
            await c.RespondAsync($"You cannot give bots {action}.");
            return;
        }

        var gaveToBot = false;
        if (user.Id == BuildInfo.ClientId) {
            await c.RespondAsync($"How dare you give me {action}! No, have some of your own~");
            gaveToBot = true;
            await Task.Delay(300);
        }

        var guild = c.Guild;
        
        var e = new DiscordEmbedBuilder();
        e.WithTitle(embedTitle);
        e.WithImageUrl(imageUrlFromApi);
        e.WithColor(Colors.HexToColor(embedColorHex));
        e.WithTimestamp(DateTime.Now);
        e.WithFooter($"{(string.IsNullOrEmpty(imageUrlFromApi) ? "" : "Powered by nekos.life")}");
        e.WithDescription(gaveToBot ? $"Gave {action} to <@{c.Message.Author.Id}>" : embedDesc);
        UserControl.AddPatToUser(user.Id, pats, true, guild.Id);
        await c.Client.SendMessageAsync(c.Message.Channel, e.Build());
    }

    [Command("pat"), Aliases("pet", "p", "pets", "pats", "headpat"), Description("Give headpats to a specified user.")]
    public async Task GivePat(cc c, [Description("Looks for User ID or mention")] string? mentionedUser = null, [RemainingText, Description("Add text to command message")] string? extraText = null) {
        if (string.IsNullOrWhiteSpace(mentionedUser)) {
            await c.RespondAsync($"Incorrect command format! Please use the command like this:\n`{BuildInfo.Config.Prefix}pat [@user]`");
            return;
        }
        
        await using var db = new Context();
        var checkGuild = db.Guilds.AsQueryable()
            .Where(u => u.GuildId.Equals(c.Guild.Id)).ToList().FirstOrDefault();
        
        var checkUser = db.Users.AsQueryable()
            .Where(u => u.UserId.Equals(c.User.Id)).ToList().FirstOrDefault();

        var isRoleBlackListed = c.Member!.Roles.Any(x => x.Id == checkGuild!.HeadPatBlacklistedRoleId && checkGuild.HeadPatBlacklistedRoleId != 0);

        if (isRoleBlackListed) {
            await c.RespondAsync("This role is not allowed to use this command. This was set by a server administrator.");
            return;
        }
        
        var isUserBlackListed = checkUser!.IsUserBlacklisted == 1;
        
        if (isUserBlackListed) {
            await c.RespondAsync("You are not allowed to use this command. This was set by a bot developer.");
            return;
        }
        
        var isOwner = c.Message.Author.Id == BuildInfo.Config.OwnerUserId;
        
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 1);

        var neko = num1 == 0 ? Program.NekoClient?.Action.Pat() : Program.NekoClient?.Action_v3.PatGif();
        var getUserIdFromMention = mentionedUser.Replace("<@", "").Replace(">", "");
        
        var rnd = new Random();
        var num = rnd.Next(0, 3);
        var outputs = new[] { "_pat pat_", "_Pats_", "_pet pet_", "_**mega pats**_" };

        var yes = !string.IsNullOrWhiteSpace(extraText) && extraText.Contains('%');
        var doNotDelete = !(yes && extraText!.Contains('>'));
        var special = num == 3 ? 2 : 1;
        var patAmount = yes && isOwner ? int.Parse(extraText!.Split('%')[1].Split('>')[0]) : special;
        
        start:
        var image = neko?.Result.ImageUrl;
        if (image == null) {
            Logger.Log("Image is null, restarting to get new image");
            goto start;
        }
        
        if (BlacklistedNekosLifeGifs.BlacklistedGifs.Urls!.Any(i => i.Equals(image))) {
            Logger.Log("Hit a blacklisted GIF URL");
            goto start;
        }

        if (image.Equals(tempPatGifUrl)) {
            Logger.Log("Image is same as previous image");
            goto start;
        }

        tempPatGifUrl = image;
        
        await OutputBaseCommand(c, mentionedUser, image, outputs[num],
            $"{c.Message.Author.Mention} gave {(patAmount != 1 ? $"**{patAmount}** headpats" : "a headpat")} to <@{getUserIdFromMention}>", "headpats", patAmount, "ffff00", doNotDelete);
        Logger.Log($"Total Pat amount Given: {patAmount}");
    }
    
    [Command("cuddle"), Aliases("c"), Description("Give cuddles to a specified user.")]
    public async Task GiveCuddles(cc c, [Description("Looks for User ID or mention")] string? mentionedUser = null) {
        if (string.IsNullOrWhiteSpace(mentionedUser)) {
            await c.RespondAsync($"Incorrect command format! Please use the command like this:\n`{BuildInfo.Config.Prefix}cuddle [@user]`");
            return;
        }
        
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 1);

        var neko = num1 == 0 ? Program.NekoClient?.Action.Cuddle() : Program.NekoClient?.Action_v3.CuddleGif();
        var getUserIdFromMention = mentionedUser.Replace("<@", "").Replace(">", "");
        
        var rnd = new Random();
        var num = rnd.Next(0, 4);
        var outputs = new[] { "_snuggies_", "_snuggles_", "_snugs_", "_cuddles_", "_ultra cuddles_" };
        
        start:
        var image = neko?.Result.ImageUrl;
        if (image == null) {
            Logger.Log("Image is null, restarting to get new image");
            goto start;
        }
        if (image.Equals(tempCuddleGifUrl)) {
            Logger.Log("Image is same as previous image");
            goto start;
        }

        tempCuddleGifUrl = image;

        await OutputBaseCommand(c, mentionedUser, image, outputs[num],
            $"{c.Message.Author.Mention} gave cuddles to <@{getUserIdFromMention}>", "cuddles", 0, "3498DB");
    }
    
    [Command("hug"), Aliases("h"), Description("Give hugs to a specified user.")]
    public async Task GiveHugs(cc c, [Description("Looks for User ID or mention")] string? mentionedUser = null) {
        if (string.IsNullOrWhiteSpace(mentionedUser)) {
            await c.RespondAsync($"Incorrect command format! Please use the command like this:\n`{BuildInfo.Config.Prefix}hug [@user]`");
            return;
        }
        
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 1);

        var neko = num1 == 0 ? Program.NekoClient?.Action.Hug() : Program.NekoClient?.Action_v3.HugGif();
        var getUserIdFromMention = mentionedUser.Replace("<@", "").Replace(">", "");

        var rnd = new Random();
        var num = rnd.Next(0, 3);
        var outputs = new[] { "_huggies_", "_huggle_", "_hugs_", "_ultra hugs_" };
        
        start:
        var image = neko?.Result.ImageUrl;
        if (image == null) {
            Logger.Log("Image is null, restarting to get new image");
            goto start;
        }
        if (image.Equals(tempHugGifUrl)) {
            Logger.Log("Image is same as previous image");
            goto start;
        }

        tempHugGifUrl = image;

        await OutputBaseCommand(c, mentionedUser, image, outputs[num],
            $"{c.Message.Author.Mention} hugged <@{getUserIdFromMention}>", "hugs", 0, "6F41B6");
        // e.WithDescription($"{ctx.User.Mention} gave {(special != 1 ? $"**{special}** hugs" : "a hug")} to <@{user.Id}>");
    }
    
    [Command("kiss"), Aliases("k"), Description("Give kisses to a specified user.")]
    public async Task GiveKiss(cc c, [Description("Looks for User ID or mention")] string? mentionedUser = null) {
        if (string.IsNullOrWhiteSpace(mentionedUser)) {
            await c.RespondAsync($"Incorrect command format! Please use the command like this:\n`{BuildInfo.Config.Prefix}kiss [@user]`");
            return;
        }
        
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 1);

        var neko = num1 == 0 ? Program.NekoClient?.Action.Kiss() : Program.NekoClient?.Action_v3.KissGif();
        var getUserIdFromMention = mentionedUser.Replace("<@", "").Replace(">", "");

        var rnd = new Random();
        var num = rnd.Next(0, 1);
        var outputs = new[] { "_kissies_", "_kisses_" };
        
        start:
        var image = neko?.Result.ImageUrl;
        if (image == null) {
            Logger.Log("Image is null, restarting to get new image");
            goto start;
        }

        await OutputBaseCommand(c, mentionedUser, image, outputs[num],
            $"{c.Message.Author.Mention} kissed <@{getUserIdFromMention}>", "kisses", 0, "F771A3");
    }
    
    [Command("slap"), Description("Slap a specified user.")]
    public async Task Slap(cc c, [Description("Looks for User ID or mention")] string? mentionedUser = null) {
        if (string.IsNullOrWhiteSpace(mentionedUser)) {
            await c.RespondAsync($"Incorrect command format! Please use the command like this:\n`{BuildInfo.Config.Prefix}slap [@user]`");
            return;
        }
        
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 1);

        var neko = num1 == 0 ? Program.NekoClient?.Action.Slap() : Program.NekoClient?.Action_v3.SlapGif();
        var getUserIdFromMention = mentionedUser.Replace("<@", "").Replace(">", "");

        var rnd = new Random();
        var num = rnd.Next(0, 2);
        var outputs = new[] { "_bap_", "_crack_", "_slap_" };
        
        start:
        var image = neko?.Result.ImageUrl;
        if (image == null) {
            Logger.Log("Image is null, restarting to get new image");
            goto start;
        }
        if (image.Equals(tempSlapGifUrl)) {
            Logger.Log("Image is same as previous image");
            goto start;
        }

        tempSlapGifUrl = image;

        await OutputBaseCommand(c, mentionedUser, image, outputs[num],
            $"{c.Message.Author.Mention} slapped <@{getUserIdFromMention}>", "slapped", 0, "E74C3C");
    }

    [Command("lick"), Description("Lick a specified user.")]
    public async Task Lick(cc c, [Description("Looks for User ID or mention")] string? mentionedUser = null) {
        if (string.IsNullOrWhiteSpace(mentionedUser)) {
            await c.RespondAsync($"Incorrect command format! Please use the command like this:\n`{BuildInfo.Config.Prefix}lick [@user]`");
            return;
        }
        await c.Message.DeleteAsync("Auto Delete from command");
        
        var getUserIdFromMention = mentionedUser.Replace("<@", "").Replace(">", "");
        
        var rnd = new Random();
        var num = rnd.Next(0, 4);
        var outputs = new[] { "_lick_", "_lick lick_", "_lickies_", "_**juicy** lick_", "_licks_" };
        
        var e = new DiscordEmbedBuilder();
        e.WithTitle(outputs[num]);
        e.WithColor(Colors.HexToColor("FF00FF"));
        e.WithTimestamp(DateTime.Now);
        e.WithDescription($"{c.Message.Author.Mention} licked <@{getUserIdFromMention}>");
        await c.Client.SendMessageAsync(c.Message.Channel, e.Build());
    }
    
    [Command("poke"), Aliases("boop", "pokes"), Description("Poke a specified user.")]
    public async Task Poke(cc c, [Description("Looks for User ID or mention")] string? mentionedUser = null) {
        if (string.IsNullOrWhiteSpace(mentionedUser)) {
            await c.RespondAsync($"Incorrect command format! Please use the command like this:\n`{BuildInfo.Config.Prefix}poke [@user]`");
            return;
        }
        
        var rnd1 = new Random();
        var num1 = rnd1.Next(0, 1);

        var neko = num1 == 0 ? Program.NekoClient?.Action.Poke() : Program.NekoClient?.Action_v3.PokeGif();
        var getUserIdFromMention = mentionedUser.Replace("<@", "").Replace(">", "");

        var rnd = new Random();
        var num = rnd.Next(0, 1);
        var outputs = new[] { "_poke_", "_boop_" };
        
        start:
        var image = neko?.Result.ImageUrl;
        if (image == null) {
            Logger.Log("Image is null, restarting to get new image");
            goto start;
        }
        if (image.Equals(tempPokeGifUrl)) {
            Logger.Log("Image is same as previous image");
            goto start;
        }

        tempPokeGifUrl = image;

        await OutputBaseCommand(c, mentionedUser, neko?.Result.ImageUrl, outputs[num],
            $"{c.Message.Author.Mention} poked <@{getUserIdFromMention}>", "poked", 0, "0E4730");
    }
}

public class LoveSlash : ApplicationCommandModule {

    [ContextMenu(ApplicationCommandType.UserContextMenu, "Hug")]
    public async Task Hug(ContextMenuContext ctx) {
        if (ctx.TargetMember.Id == BuildInfo.ClientId)
            await ctx.CreateResponseAsync($"I got hugs from {ctx.User.Username}?! Thankies~");
        else if (ctx.TargetMember.Id == ctx.User.Id)
            await ctx.CreateResponseAsync("You cant give yourself hugs, but I'll gladly give you some!");
        else 
            await ctx.CreateResponseAsync($"{ctx.User.Username} hugged {ctx.TargetMember.DisplayName}!");
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
            await c.CreateResponseAsync("You cannot give yourself headpats.");
            return;
        }

        if (c.TargetUser.IsBot) {
            await c.CreateResponseAsync("You cannot give bots headpats.");
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
        
        if (image!.Equals(Love.tempPatGifUrl)) {
            Logger.Log("Image is same as previous image");
            goto start;
        }

        Love.tempPatGifUrl = image;

        var e = new DiscordEmbedBuilder();
        e.WithTitle(outputs[num]);
        e.WithImageUrl(image);
        e.WithColor(Colors.HexToColor("ffff00"));
        e.WithFooter("Powered by nekos.life");
        e.WithDescription(gaveToBot ? $"Gave headpats to <@{c.TargetUser.Id}>" : $"{c.User.Mention} gave {(special != 1 ? $"**{special}** headpats" : "a headpat")} to <@{c.TargetUser.Id}>");
        UserControl.AddPatToUser(c.TargetUser.Id, special, true, c.Guild.Id);
        // await c.Client.SendMessageAsync(c.Channel, e.Build());
        await c.CreateResponseAsync(e.Build());
        Logger.Log($"Total Pat amount Given: {special}");
    }

    /*[SlashCommandGroup("user", "User Actions")]
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
                await c.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("You cannot give yourself headpats."));
                return;
            }

            if (user.IsBot) {
                await c.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("You cannot give bots headpats."));
                return;
            }
            
            if (user.Id == BuildInfo.ClientId) {
                await c.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("How dare you give me headpats! No, have some of your own~"));
                return;
            }

            var num1 = new Random().Next(0, 1);

            var neko = num1 == 0 ? Program.NekoClient?.Action.Pat() : Program.NekoClient?.Action_v3.PatGif();

            var num = new Random().Next(0, 3);
            var outputs = new[] { "_pat pat_", "_Pats_", "_pet pet_", "_**mega pats**_" };

            var special = num == 3 ? 2 : 1;

            start:
            var image = neko?.Result.ImageUrl;
            if (BlacklistedNekosLifeGifs.BlacklistedGifs.Urls!.Any(i => i.Equals(image!))) {
                Logger.Log("Hit a blacklisted GIF URL");
                goto start;
            }

            var e = new DiscordEmbedBuilder();
            e.WithTitle(outputs[num]);
            e.WithImageUrl(image);
            e.WithColor(Colors.HexToColor("ffff00"));
            e.WithFooter("Powered by nekos.life");
            e.WithDescription($"{c.User.Mention} gave {(special != 1 ? $"**{special}** headpats" : "a headpat")} to <@{user.Id}>");
            UserControl.AddPatToUser(user.Id, special, true, c.Guild.Id);
            await c.CreateResponseAsync(e.Build());
            Logger.Log($"Total Pat amount Given: {special}");
        }
        
        [SlashCommand("hug", "Hug a specified user.")]
        public async Task Hug(ic c, [Option("user", "The user to hug")] DiscordUser user) {
            if (user.Id == c.User.Id) {
                await c.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("You cannot give yourself hugs."));
                return;
            }

            if (user.IsBot) {
                await c.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("You cannot give bots hugs."));
                return;
            }
            if (user.Id == BuildInfo.ClientId) {
                await c.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("How dare you give me hugs! No, have some of your own~"));
                return;
            }

            var num1 = new Random().Next(0, 1);

            var neko = num1 == 0 ? Program.NekoClient?.Action.Hug() : Program.NekoClient?.Action_v3.HugGif();

            var num = new Random().Next(0, 3);
            var outputs = new[] { "_huggies_", "_huggle_", "_hugs_", "_ultra hugs_" };

            start:
            var image = neko?.Result.ImageUrl;
            if (BlacklistedNekosLifeGifs.BlacklistedGifs.Urls!.Any(i => i.Equals(image!))) {
                Logger.Log("Hit a blacklisted GIF URL");
                goto start;
            }

            var e = new DiscordEmbedBuilder();
            e.WithTitle(outputs[num]);
            e.WithImageUrl(image);
            e.WithColor(Colors.HexToColor("6F41B6"));
            e.WithFooter("Powered by nekos.life");
            e.WithDescription($"{c.User.Mention} gave a hug to <@{user.Id}>");
            await c.CreateResponseAsync(e.Build());
        }
    }*/
}