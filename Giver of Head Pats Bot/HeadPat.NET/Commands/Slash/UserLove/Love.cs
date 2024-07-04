using Discord;
using Discord.Interactions;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Utils;
using HeadPats.Utils.ExternalApis;
using Serilog;

namespace HeadPats.Commands.Slash.UserLove;

public class Love : InteractionModuleBase<SocketInteractionContext> {
    [Group("user", "User Actions"), CommandContextType(InteractionContextType.Guild)]
    public class Commands : InteractionModuleBase<SocketInteractionContext> {
        private static string? _tempPatGifUrl, _tempHugGifUrl, _tempSlapGifUrl, _tempCookieGifUrl, _tempKissGifUrl;

        [SlashCommand("pat", "Pat a user")]
        public async Task Pat([Summary("user", "User to pat")] IGuildUser user,
            [Summary("params", "Extra parameters (for the bot owner)")]
            string extraParams = "") {
            var logger = Log.ForContext("SourceContext", "Command - Pat");
            var canUseParams = Context.User.Id == 167335587488071682;
            var hasCommandBlacklist = Config.Base.FullBlacklistOfGuilds!.Contains(Context.Guild.Id);
            if (hasCommandBlacklist) {
                var isThisCommandBlacklisted = Config.GuildSettings(Context.Guild.Id)!.BlacklistedCommands!.Contains("pat");
                if (isThisCommandBlacklisted) {
                    await RespondAsync("This guild is not allowed to use this command. This was set by a bot developer.", ephemeral: true);
                    return;
                }
            }

            await using var db = new Context();
            var checkGuild = db.Guilds.AsQueryable()
                .Where(u => u.GuildId.Equals(Context.Guild.Id)).ToList().FirstOrDefault();

            if (checkGuild is null) {
                var newGuild = new Guilds {
                    GuildId = Context.Guild.Id,
                    HeadPatBlacklistedRoleId = 0,
                    PatCount = 0
                };
                logger.Information("Added guild to database from Pat Command");
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
                logger.Debug("Added user to database from Pat Command");
                db.Users.Add(newUser);
            }

            var isUserBlackListed = checkUser is not null && checkUser.IsUserBlacklisted == 1;

            if (isUserBlackListed) {
                await RespondAsync("You are not allowed to use this command. This was set by a bot developer.", ephemeral: true);
                return;
            }

            if (user.Id == Context.User.Id) {
                await RespondAsync("You cannot give yourself headpats.", ephemeral: true);
                return;
            }

            if (user.IsBot) {
                await RespondAsync("You cannot give bots headpats.", ephemeral: true);
                return;
            }

            var gaveToBot = false;
            if (user.Id == Vars.ClientId) {
                await RespondAsync("How dare you give me headpats! No, have some of your own~");
                gaveToBot = true;
                await Task.Delay(300);
            }

            var e = new EmbedBuilder();
            var outputs = new[] { "_pat pat_", "_Pats_", "_pet pet_", "_**mega pats**_" };
            var num = new Random().Next(0, outputs.Length);

            var numberOfPats = num == 3 ? 2 : 1;

            e.WithTitle(outputs[num]);
            var newNumber = checkUser!.PatCount + numberOfPats;

            if (Vars.UseLocalImages) {
                start:
                var image = LocalImages.GetRandomPat();
                // logger.Debug($"THE IMAGE IS: {image}");
                if (image.Equals(_tempPatGifUrl)) {
                    logger.Debug("Image is same as previous image");
                    goto start;
                }

                _tempPatGifUrl = image;

                e.WithImageUrl(image);
                e.WithFooter($"Powered by the community | You have {newNumber:N0} pats");
            }
            else {
                start2:
                var image = (await Program.Instance.FluxpointClient!.Gifs.GetPatAsync()).file;
                if (image.Equals(_tempPatGifUrl)) {
                    logger.Debug("Image is same as previous image");
                    goto start2;
                }

                _tempPatGifUrl = image;

                e.WithImageUrl(image);
                e.WithFooter($"Powered by Fluxpoint API | You have {newNumber:N0} pats");
            }

            var doingTheCutieSpecial = false;

            switch (canUseParams) {
                case true: {
                    if (!string.IsNullOrWhiteSpace(extraParams)) {
                        if (extraParams.Equals("mega"))
                            numberOfPats = 2;

                        if (extraParams.Contains('%'))
                            numberOfPats = int.Parse(extraParams.Split('%')[1]);

                        if (extraParams.Contains("elly", StringComparison.CurrentCultureIgnoreCase)) {
                            numberOfPats += 5;
                            doingTheCutieSpecial = true;
                        }
                    }

                    break;
                }
                case false when !string.IsNullOrWhiteSpace(extraParams):
                    await RespondAsync("You do not have permission to use extra parameters.", ephemeral: true);
                    return;
            }

            e.WithColor(Colors.Random);

            if (gaveToBot)
                e.WithDescription($"Gave headpats to {user.Mention}");
            else if (doingTheCutieSpecial)
                e.WithDescription($"{Context.User.Mention} gave **{numberOfPats}** headpats to the cutie {user.Mention}");
            else
                e.WithDescription(PatUtils.GetRandomPatMessageTemplate(Context.User.Mention, user.Mention));
            
            UserControl.AddPatToUser(user.Id, numberOfPats, true, Context.Guild.Id);
            await RespondAsync(embed: e.Build());
            logger.Debug($"Total Pat amount Given: {numberOfPats}");
        }

        [SlashCommand("hug", "Hug a user"), IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall), CommandContextType(InteractionContextType.Guild, InteractionContextType.PrivateChannel)]
        public async Task Hug([Summary("User", "User to hug")] IUser user) {
            var logger = Log.ForContext("SourceContext", "Command - Hug");
            if (Context.Guild is not null) {
                var hasCommandBlacklist = Config.Base.FullBlacklistOfGuilds!.Contains(Context.Guild.Id);
                if (hasCommandBlacklist) {
                    var isThisCommandBlacklisted = Config.GuildSettings(Context.Guild.Id)!.BlacklistedCommands!.Contains("hug");
                    if (isThisCommandBlacklisted) {
                        await RespondAsync("This guild is not allowed to use this command. This was set by a bot developer.", ephemeral: true);
                        return;
                    }
                }
            }

            if (user.Id == Context.User.Id) {
                await RespondAsync("You cannot give yourself hugs.", ephemeral: true);
                return;
            }

            if (user.IsBot || user.Id == Vars.ClientId) {
                await RespondAsync("You cannot give bots hugs.", ephemeral: true);
                return;
            }

            var e = new EmbedBuilder();
            var outputs = new[] { "_huggies_", "_huggle_", "_hugs_", "_ultra hugs_" };
            var num = new Random().Next(0, outputs.Length);

            e.WithTitle(outputs[num]);

            if (Vars.UseLocalImages) {
                start:
                var image = LocalImages.GetRandomHug();
                if (image.Equals(_tempHugGifUrl)) {
                    logger.Debug("Image is same as previous image");
                    goto start;
                }

                _tempHugGifUrl = image;

                e.WithImageUrl(image);
                e.WithFooter("Powered by the community");
            }
            else {
                start2:
                var image = (await Program.Instance.FluxpointClient!.Gifs.GetHugAsync()).file;
                if (image.Equals(_tempPatGifUrl)) {
                    logger.Debug("Image is same as previous image");
                    goto start2;
                }

                _tempHugGifUrl = image;

                e.WithImageUrl(image);
                e.WithFooter("Powered by Fluxpoint API");
            }

            e.WithColor(Colors.Random);
            e.WithDescription($"{Context.User.Mention} gave a hug to {user.Mention}");
            await RespondAsync(embed: e.Build());
        }

        [SlashCommand("kiss", "Kiss a user")]
        public async Task Kiss([Summary("kiss", "User to kiss")] IGuildUser user) {
            var logger = Log.ForContext("SourceContext", "Command - Kiss");
            var hasCommandBlacklist = Config.Base.FullBlacklistOfGuilds!.Contains(Context.Guild.Id);
            if (hasCommandBlacklist) {
                var isThisCommandBlacklisted = Config.GuildSettings(Context.Guild.Id)!.BlacklistedCommands!.Contains("kiss");
                if (isThisCommandBlacklisted) {
                    await RespondAsync("This guild is not allowed to use this command. This was set by a bot developer.", ephemeral: true);
                    return;
                }
            }

            if (user.Id == Context.User.Id) {
                await RespondAsync("You cannot give yourself kisses.", ephemeral: true);
                return;
            }

            if (user.IsBot || user.Id == Vars.ClientId) {
                await RespondAsync("You cannot give bots kisses.", ephemeral: true);
                return;
            }

            var e = new EmbedBuilder();
            var outputs = new[] { "_kisses_", "_kissies_", "_kissies_", "_kisses_", "_ultra kisses_" };
            var num = new Random().Next(0, outputs.Length);

            e.WithTitle(outputs[num]);

            if (Vars.UseLocalImages) {
                start:
                var image = LocalImages.GetRandomKiss();
                if (image.Equals(_tempKissGifUrl)) {
                    logger.Debug("Image is same as previous image");
                    goto start;
                }

                _tempKissGifUrl = image;

                e.WithImageUrl(image);
                e.WithFooter("Powered by the community");
            }
            else {
                start2:
                var image = (await Program.Instance.FluxpointClient!.Gifs.GetKissAsync()).file;
                if (image.Equals(_tempPatGifUrl)) {
                    logger.Debug("Image is same as previous image");
                    goto start2;
                }

                _tempKissGifUrl = image;

                e.WithImageUrl(image);
                e.WithFooter("Powered by Fluxpoint API");
            }

            e.WithColor(Colors.Random);
            e.WithDescription($"{Context.User.Mention} gave kisses to {user.Mention}");
            await RespondAsync(embed: e.Build());
        }

        [SlashCommand("slap", "Slap a user")]
        public async Task Slap([Summary("slap", "User to slap")] IGuildUser user) {
            var logger = Log.ForContext("SourceContext", "Command - Slap");
            var hasCommandBlacklist = Config.Base.FullBlacklistOfGuilds!.Contains(Context.Guild.Id);
            if (hasCommandBlacklist) {
                var isThisCommandBlacklisted = Config.GuildSettings(Context.Guild.Id)!.BlacklistedCommands!.Contains("slap");
                if (isThisCommandBlacklisted) {
                    await RespondAsync("This guild is not allowed to use this command. This was set by a bot developer.", ephemeral: true);
                    return;
                }
            }

            if (user.Id == Context.User.Id) {
                await RespondAsync("You cannot slap yourself.", ephemeral: true);
                return;
            }

            if (user.IsBot || user.Id == Vars.ClientId) {
                await RespondAsync("You cannot slap bots.", ephemeral: true);
                return;
            }

            var e = new EmbedBuilder();
            var outputs = new[] { "_slaps_", "_slaps_", "_slaps_", "_slaps_", "_ultra slaps_" };
            var num = new Random().Next(0, outputs.Length);

            e.WithTitle(outputs[num]);

            if (Vars.UseLocalImages) {
                start:
                var image = LocalImages.GetRandomSlap();
                if (image.Equals(_tempSlapGifUrl)) {
                    logger.Debug("Image is same as previous image");
                    goto start;
                }

                _tempSlapGifUrl = image;

                e.WithImageUrl(image);
                e.WithFooter("Powered by the community");
            }
            else {
                start2:
                var image = (await Program.Instance.FluxpointClient!.Gifs.GetSlapAsync()).file;
                if (image.Equals(_tempPatGifUrl)) {
                    logger.Debug("Image is same as previous image");
                    goto start2;
                }

                _tempSlapGifUrl = image;

                e.WithImageUrl(image);
                e.WithFooter("Powered by Fluxpoint API");
            }

            e.WithColor(Colors.Random);
            e.WithDescription($"{Context.User.Mention} slapped {user.Mention}");
            await RespondAsync(embed: e.Build());
        }

        [SlashCommand("cookie", "Give a user a cookie")]
        public async Task Cookie([Summary("Cookie", "User to give cookie")] IGuildUser user) {
            var logger = Log.ForContext("SourceContext", "Command - Cookie");
            var hasCommandBlacklist = Config.Base.FullBlacklistOfGuilds!.Contains(Context.Guild.Id);
            if (hasCommandBlacklist) {
                var isThisCommandBlacklisted = Config.GuildSettings(Context.Guild.Id)!.BlacklistedCommands!.Contains("cookie");
                if (isThisCommandBlacklisted) {
                    await RespondAsync("This guild is not allowed to use this command. This was set by a bot developer.", ephemeral: true);
                    return;
                }
            }

            await using var db = new Context();
            var checkGuild = db.Guilds.AsQueryable()
                .Where(u => u.GuildId.Equals(Context.Guild.Id)).ToList().FirstOrDefault();

            if (checkGuild is null) {
                var newGuild = new Guilds {
                    GuildId = Context.Guild.Id,
                    HeadPatBlacklistedRoleId = 0,
                    PatCount = 0
                };
                logger.Information("Added guild to database from Pat Command");
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
                logger.Debug("Added user to database from Pat Command");
                db.Users.Add(newUser);
            }

            if (user.Id == Context.User.Id) {
                await RespondAsync("You requested a cookie, so here you go!");
                UserControl.AddCookieToUser(Context.User.Id, 1);
                return;
            }

            if (user.IsBot || user.Id == Vars.ClientId) {
                await RespondAsync("I cannot do anything with a cookie, so here, you have it!");
                UserControl.AddCookieToUser(Context.User.Id, 1);
                return;
            }

            var e = new EmbedBuilder();
            var outputs = new[] { "C O O K I E S", "Cookies!", "nom nom" };
            var num = new Random().Next(0, outputs.Length);

            e.WithTitle(outputs[num]);
            var newNumber = checkUser!.CookieCount + 1;

            if (Vars.UseLocalImages) {
                start:
                var image = LocalImages.GetRandomCookie();
                if (image.Equals(_tempCookieGifUrl)) {
                    logger.Debug("Image is same as previous image");
                    goto start;
                }

                _tempCookieGifUrl = image;

                e.WithImageUrl(image);
                e.WithFooter($"Powered by the community | You have {newNumber:N0} cookies");
            }
            else 
                e.WithFooter($"You have {newNumber:N0} cookies");

            e.WithColor(Colors.GetRandomCookieColor());
            e.WithDescription($"{Context.User.Mention} gave a cookie to {user.Mention}");
            await RespondAsync(embed: e.Build());
            UserControl.AddCookieToUser(user.Id, 1);
        }
    }
}