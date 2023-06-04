using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Utils;
using Serilog;

namespace HeadPats.Commands.Slash.UserLove;

public class LoveCommands : ApplicationCommandModule {
    
    private static string? _tempPatGifUrl, _tempHugGifUrl, _tempSlapGifUrl, _tempCookieGifUrl, _tempPokeGifUrl, _tempKissGifUrl;

     [SlashCommandGroup("user", "User Actions")]
     public class LoveUser : ApplicationCommandModule {
     
         [SlashCommand("pat", "Pat a specified user.")]
         public async Task Pat(InteractionContext c, [Option("user", "The user to pat", true)] DiscordUser user, 
             [Option("params", "Extra parameters (for the bot owner)")] string extraParams = "") {
             var canUseParams = c.User.Id == 167335587488071682;
             var hasCommandBlacklist = Config.Base.FullBlacklistOfGuilds!.Contains(c.Guild.Id);
             if (hasCommandBlacklist) {
                 var isThisCommandBlacklisted = Config.GuildSettings(c.Guild.Id)!.BlacklistedCommands!.Contains("pat");
                 if (isThisCommandBlacklisted) {
                     await c.CreateResponseAsync("This guild is not allowed to use this command. This was set by a bot developer.", true);
                     return;
                 }
             }
             
             await using var db = new Context();
             var checkGuild = db.Guilds.AsQueryable()
                 .Where(u => u.GuildId.Equals(c.Guild.Id)).ToList().FirstOrDefault();
             
             if (checkGuild is null) {
                 var newGuild = new Guilds {
                     GuildId = c.Guild.Id,
                     HeadPatBlacklistedRoleId = 0,
                     PatCount = 0
                 };
                 Log.Information("Added guild to database from Pat Command");
                 db.Guilds.Add(newGuild);
             }
         
             var checkUser = db.Users.AsQueryable()
                 .Where(u => u.UserId.Equals(user.Id)).ToList().FirstOrDefault();

             if (checkUser is null) {
                 var newUser = new Users {
                     UserId = user.Id,
                     UsernameWithNumber = $"{user.Username}",
                     PatCount = 0,
                     CookieCount = 0,
                     IsUserBlacklisted = 0
                 };
                 Log.Debug("Added user to database from Pat Command");
                 db.Users.Add(newUser);
             }
             
             // if (addedUser || addedGuild)
             //     await db.SaveChangesAsync();
         
             var isUserBlackListed = checkUser is not null && checkUser.IsUserBlacklisted == 1;
         
             if (isUserBlackListed) {
                 await c.CreateResponseAsync("You are not allowed to use this command. This was set by a bot developer.", true);
                 return;
             }
             
             var isRoleBlackListed = c.Member!.Roles.Any(x => x.Id == checkGuild!.HeadPatBlacklistedRoleId);

             if (isRoleBlackListed) {
                 await c.CreateResponseAsync("This role is not allowed to use this command. This was set by a server administrator.", true);
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

             var e = new DiscordEmbedBuilder();
             var num = new Random().Next(0, 3);
             var outputs = new[] { "_pat pat_", "_Pats_", "_pet pet_", "_**mega pats**_" };

             var special = num == 3 ? 2 : 1;
             
             e.WithTitle(outputs[num]);
             
             if (Vars.UseCookieApi) {
                 start:
                 var image = Program.CookieClient!.GetPat();
                 // Log.Debug($"THE IMAGE IS: {image}");
                 if (image.Equals(_tempPatGifUrl)) {
                     Log.Debug("Image is same as previous image");
                     goto start;
                 }

                 _tempPatGifUrl = image;
                 
                 e.WithImageUrl(image);
                 e.WithFooter("Powered by CookieAPI");
             }
             else {
                 start2:
                 var image = (await Program.FluxpointClient!.Gifs.GetPatAsync()).file;
                 if (image.Equals(_tempPatGifUrl)) {
                     Log.Debug("Image is same as previous image");
                     goto start2;
                 }

                 _tempPatGifUrl = image;
                 
                 e.WithImageUrl(image);
                 e.WithFooter("Powered by Fluxpoint API");
             }

             var doingTheCutieSpecial = false;
             
             switch (canUseParams) {
                 case true: {
                     if (!string.IsNullOrWhiteSpace(extraParams)) {
                         if (extraParams.Equals("mega"))
                             special = 2;

                         if (extraParams.Contains('%'))
                             special = int.Parse(extraParams.Split('%')[1]);

                         if (extraParams.ToLower().Contains("elly") || extraParams.ToLower().Contains("ahri")) {
                             special = 5;
                             doingTheCutieSpecial = true;
                         }
                     }

                     break;
                 }
                 case false when !string.IsNullOrWhiteSpace(extraParams):
                     await c.CreateResponseAsync("You do not have permission to use extra parameters.", true);
                     return;
             }

             e.WithColor(Colors.HexToColor("ffff00"));
             if (doingTheCutieSpecial)
                 e.WithDescription(gaveToBot ? $"Gave headpats to {user.Mention}" : $"{c.User.Mention} gave **{special}** headpats to {user.Mention}");
             else 
                 e.WithDescription(gaveToBot ? $"Gave headpats to {user.Mention}" : $"{c.User.Mention} gave {(special != 1 ? $"**{special}** headpats" : "a headpat")} to {user.Mention}");
             UserControl.AddPatToUser(user.Id, special, true, c.Guild.Id);
             // await c.CreateResponseAsync(e.Build());
             await c.Channel.SendMessageAsync(e.Build());
             Log.Debug($"Total Pat amount Given: {special}");
         }
         
         [SlashCommand("hug", "Hug a specified user.")]
         public async Task Hug(InteractionContext c, [Option("user", "The user to hug", true)] DiscordUser user) {
             var hasCommandBlacklist = Config.Base.FullBlacklistOfGuilds!.Contains(c.Guild.Id);
             if (hasCommandBlacklist) {
                 var isThisCommandBlacklisted = Config.GuildSettings(c.Guild.Id)!.BlacklistedCommands!.Contains("hug");
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
             
             var e = new DiscordEmbedBuilder();
             var num = new Random().Next(0, 3);
             var outputs = new[] { "_huggies_", "_huggle_", "_hugs_", "_ultra hugs_" };
             
             e.WithTitle(outputs[num]);
             
             if (Vars.UseCookieApi) {
                 start:
                 var image = Program.CookieClient!.GetHug();
                 if (image.Equals(_tempHugGifUrl)) {
                     Log.Debug("Image is same as previous image");
                     goto start;
                 }

                 _tempHugGifUrl = image;
                 
                 e.WithImageUrl(image);
                 e.WithFooter("Powered by CookieAPI");
             }
             else {
                 start2:
                 var image = (await Program.FluxpointClient!.Gifs.GetHugAsync()).file;
                 if (image.Equals(_tempPatGifUrl)) {
                     Log.Debug("Image is same as previous image");
                     goto start2;
                 }

                 _tempHugGifUrl = image;
                 
                 e.WithImageUrl(image);
                 e.WithFooter("Powered by Fluxpoint API");
             }
             
             e.WithColor(Colors.HexToColor("6F41B6"));
             e.WithDescription($"{c.User.Mention} gave a hug to {user.Mention}");
             e.Build();
             // await c.CreateResponseAsync(e.Build());
             await c.Channel.SendMessageAsync(e.Build());
         }

         [SlashCommand("Cuddle", "Cuddle a specified user.")]
         public async Task Cuddle(InteractionContext c, [Option("user", "The user to cuddle", true)] DiscordUser user) {
             var hasCommandBlacklist = Config.Base.FullBlacklistOfGuilds!.Contains(c.Guild.Id);
             if (hasCommandBlacklist) {
                 var isThisCommandBlacklisted = Config.GuildSettings(c.Guild.Id)!.BlacklistedCommands!.Contains("cuddle");
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
             
             var e = new DiscordEmbedBuilder();
             var num = new Random().Next(0, 4);
             var outputs = new[] { "_snuggies_", "_snuggles_", "_snugs_", "_cuddles_", "_ultra cuddles_" };
             
             e.WithTitle(outputs[num]);
             
             if (Vars.UseCookieApi) {
                 start:
                 var image = Program.CookieClient!.GetHug();
                 if (image.Equals(_tempHugGifUrl)) {
                     Log.Debug("Image is same as previous image");
                     goto start;
                 }

                 _tempHugGifUrl = image;
                 
                 e.WithImageUrl(image);
                 e.WithFooter("Powered by CookieAPI");
             }
             else {
                 start2:
                 var image = (await Program.FluxpointClient!.Gifs.GetHugAsync()).file;
                 if (image.Equals(_tempPatGifUrl)) {
                     Log.Debug("Image is same as previous image");
                     goto start2;
                 }

                 _tempHugGifUrl = image;
                 
                 e.WithImageUrl(image);
                 e.WithFooter("Powered by Fluxpoint API");
             }
             
             e.WithColor(Colors.HexToColor("3498DB"));
             e.WithDescription($"{c.User.Mention} gave cuddles to {user.Mention}");
             // await c.CreateResponseAsync(e.Build());
             await c.Channel.SendMessageAsync(e.Build());
         }

         [SlashCommand("kiss", "Kiss a specified user.")]
         public async Task Kiss(InteractionContext c, [Option("user", "The user to kiss", true)] DiscordUser user) {
             var hasCommandBlacklist = Config.Base.FullBlacklistOfGuilds!.Contains(c.Guild.Id);
             if (hasCommandBlacklist) {
                 var isThisCommandBlacklisted = Config.GuildSettings(c.Guild.Id)!.BlacklistedCommands!.Contains("kiss");
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
             
             var e = new DiscordEmbedBuilder();
             var num = new Random().Next(0, 4);
             var outputs = new[] { "_kisses_", "_kissies_", "_kissies_", "_kisses_", "_ultra kisses_" };
             
             e.WithTitle(outputs[num]);
             
             if (Vars.UseCookieApi) {
                 start:
                 var image = Program.CookieClient!.GetKiss();
                 if (image.Equals(_tempKissGifUrl)) {
                     Log.Debug("Image is same as previous image");
                     goto start;
                 }

                 _tempKissGifUrl = image;
                 
                 e.WithImageUrl(image);
                 e.WithFooter("Powered by CookieAPI");
             }
             else {
                 start2:
                 var image = (await Program.FluxpointClient!.Gifs.GetKissAsync()).file;
                 if (image.Equals(_tempPatGifUrl)) {
                     Log.Debug("Image is same as previous image");
                     goto start2;
                 }

                 _tempKissGifUrl = image;
                 
                 e.WithImageUrl(image);
                 e.WithFooter("Powered by Fluxpoint API");
             }
             
             e.WithColor(Colors.HexToColor("F771A3"));
             e.WithDescription($"{c.User.Mention} gave kisses to {user.Mention}");
             // await c.CreateResponseAsync(e.Build());
             await c.Channel.SendMessageAsync(e.Build());
         }

         [SlashCommand("slap", "Slap a specified user.")]
         public async Task Slap(InteractionContext c, [Option("user", "The user to slap", true)] DiscordUser user) {
             var hasCommandBlacklist = Config.Base.FullBlacklistOfGuilds!.Contains(c.Guild.Id);
             if (hasCommandBlacklist) {
                 var isThisCommandBlacklisted = Config.GuildSettings(c.Guild.Id)!.BlacklistedCommands!.Contains("slap");
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
             
             var e = new DiscordEmbedBuilder();
             var num = new Random().Next(0, 4);
             var outputs = new[] { "_slaps_", "_slaps_", "_slaps_", "_slaps_", "_ultra slaps_" };
             
             e.WithTitle(outputs[num]);

             if (Vars.UseCookieApi) {
                 start:
                 var image = Program.CookieClient!.GetSlap();
                 if (image.Equals(_tempSlapGifUrl)) {
                     Log.Debug("Image is same as previous image");
                     goto start;
                 }

                 _tempSlapGifUrl = image;
                 
                 e.WithImageUrl(image);
                 e.WithFooter("Powered by CookieAPI");
             }
             else {
                 start2:
                 var image = (await Program.FluxpointClient!.Gifs.GetSlapAsync()).file;
                 if (image.Equals(_tempPatGifUrl)) {
                     Log.Debug("Image is same as previous image");
                     goto start2;
                 }

                 _tempSlapGifUrl = image;
                 
                 e.WithImageUrl(image);
                 e.WithFooter("Powered by Fluxpoint API");
             }

             e.WithColor(Colors.HexToColor("E74C3C"));
             e.WithDescription($"{c.User.Mention} slapped {user.Mention}");
             await c.CreateResponseAsync(e.Build());
             // await c.Channel.SendMessageAsync(e.Build());
         }
         
         [SlashCommand("Poke", "Poke or boop a user.")]
         public async Task Poke(InteractionContext c, [Option("user", "The user to poke", true)] DiscordUser user) {
             var hasCommandBlacklist = Config.Base.FullBlacklistOfGuilds!.Contains(c.Guild.Id);
             if (hasCommandBlacklist) {
                 var isThisCommandBlacklisted = Config.GuildSettings(c.Guild.Id)!.BlacklistedCommands!.Contains("poke");
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
             
             var e = new DiscordEmbedBuilder();
             var num = new Random().Next(0, 4);
             var outputs = new[] { "_pokes_", "_pokes_", "_pokes_", "_pokes_", "_ultra pokes_" };
             
             e.WithTitle(outputs[num]);
             
             if (Vars.UseCookieApi) {
                 start:
                 var image = Program.CookieClient!.GetPoke();
                 if (image.Equals(_tempPokeGifUrl)) {
                     Log.Debug("Image is same as previous image");
                     goto start;
                 }

                 _tempPokeGifUrl = image;
                 
                 e.WithImageUrl(image);
                 e.WithFooter("Powered by CookieAPI");
             }
             else {
                 start2:
                 var image = (await Program.FluxpointClient!.Gifs.GetPokeAsync()).file;
                 if (image.Equals(_tempPatGifUrl)) {
                     Log.Debug("Image is same as previous image");
                     goto start2;
                 }

                 _tempPokeGifUrl = image;
                 
                 e.WithImageUrl(image);
                 e.WithFooter("Powered by Fluxpoint API");
             }
             
             e.WithColor(Colors.HexToColor("0E4730"));
             e.WithDescription($"{c.User.Mention} poked {user.Mention}");
             // await c.CreateResponseAsync(e.Build());
             await c.Channel.SendMessageAsync(e.Build());
         }

         [SlashCommand("Cookie", "Give a user a cookie.")]
         public async Task Cookie(InteractionContext c, [Option("user", "The user to give a cookie to", true)] DiscordUser user) {
             var hasCommandBlacklist = Config.Base.FullBlacklistOfGuilds!.Contains(c.Guild.Id);
             if (hasCommandBlacklist) {
                 var isThisCommandBlacklisted = Config.GuildSettings(c.Guild.Id)!.BlacklistedCommands!.Contains("cookie");
                 if (isThisCommandBlacklisted) {
                     await c.CreateResponseAsync("This guild is not allowed to use this command. This was set by a bot developer.", true);
                     return;
                 }
             }
             
             await using var db = new Context();
             var checkGuild = db.Guilds.AsQueryable()
                 .Where(u => u.GuildId.Equals(c.Guild.Id)).ToList().FirstOrDefault();
             
             if (checkGuild is null) {
                 var newGuild = new Guilds {
                     GuildId = c.Guild.Id,
                     HeadPatBlacklistedRoleId = 0,
                     PatCount = 0
                 };
                 Log.Information("Added guild to database from Pat Command");
                 db.Guilds.Add(newGuild);
             }
         
             var checkUser = db.Users.AsQueryable()
                 .Where(u => u.UserId.Equals(user.Id)).ToList().FirstOrDefault();

             if (checkUser is null) {
                 var newUser = new Users {
                     UserId = user.Id,
                     UsernameWithNumber = $"{user.Username}",
                     PatCount = 0,
                     CookieCount = 0,
                     IsUserBlacklisted = 0
                 };
                 Log.Debug("Added user to database from Pat Command");
                 db.Users.Add(newUser);
             }
             
             // if (addedUser || addedGuild)
             //     await db.SaveChangesAsync();

             if (user.Id == c.User.Id) {
                 await c.CreateResponseAsync("You requested a cookie, so here you go!");
                 UserControl.AddCookieToUser(c.User.Id, 1);
                 return;
             }
             if (user.IsBot || user.Id == Vars.ClientId) {
                 await c.CreateResponseAsync("I cannot do anything with a cookie, so here, you have it!");
                 UserControl.AddCookieToUser(c.User.Id, 1);
                 return;
             }
             
             var e = new DiscordEmbedBuilder();
             var num = new Random().Next(0, 2);
             var outputs = new[] { "C O O K I E S", "Cookies!", "nom nom" };
             
             e.WithTitle(outputs[num]);

             if (Vars.UseCookieApi) {
                 start:
                 var image = Program.CookieClient!.GetCookie();
                 if (image.Equals(_tempCookieGifUrl)) {
                     Log.Debug("Image is same as previous image");
                     goto start;
                 }

                 _tempCookieGifUrl = image;
                 
                 e.WithImageUrl(image);
                 e.WithFooter("Powered by CookieAPI");
             }
             
             e.WithColor(Colors.GetRandomCookieColor());
             e.WithDescription($"{c.User.Mention} gave a cookie to {user.Mention}");
             // await c.CreateResponseAsync(e.Build());
             await c.Channel.SendMessageAsync(e.Build());
             UserControl.AddCookieToUser(user.Id, 1);
         }
     }
}