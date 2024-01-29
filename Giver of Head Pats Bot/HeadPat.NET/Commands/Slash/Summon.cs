using System.Net.Http.Headers;
using Discord;
using Discord.Interactions;
using HeadPats.Utils;
using HeadPats.Utils.ExternalApis;

namespace HeadPats.Commands.Slash; 

public class Summon : InteractionModuleBase<SocketInteractionContext> {

    [Group("summon", "Summon various images")]
    public class Commands : InteractionModuleBase<SocketInteractionContext> {

        [SlashCommand("bunny", "Bunnies are adorable")]
        public async Task Bunny() {
            start:
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", Vars.FakeUserAgent);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var content = await httpClient.GetStringAsync("https://api.bunnies.io/v2/loop/random/?media=gif,png");
            // Logger.Log($"Data: {content}");
            var id = content.Split("\"id\":\"")[1].Split("\"")[0];
            var url = content.Split("\"gif\":\"")[1].Split("\"")[0];
            var source = content.Split("\"source\":\"")[1].Split("\"")[0];

            if (string.IsNullOrWhiteSpace(url)) {
                httpClient.Dispose();
                goto start;
            }

            var embed = new EmbedBuilder {
                Title = $"Bunny #{id}",
                Color = Colors.HexToColor("#B88F64"),
                ImageUrl = url,
                Footer = new EmbedFooterBuilder {
                    Text = "Powered by Bunnies.io"
                }
            };
            if (source != "unknown") {
                embed.WithAuthor("Source", source);
            }
            httpClient.Dispose();
            await RespondAsync(embed: embed.Build());
            // BunnyJson.BunnyData = null;
        }
        
        [SlashCommand("fox", "Foxes are best")]
        public async Task Fox() {
            var doingCustomFox = new Random().Next(0, 1).Equals(0);
            if (!doingCustomFox) {
                RandomFoxJson.FoxData = null;
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", Vars.FakeUserAgent);
                var content = await httpClient.GetStringAsync("https://randomfox.ca/floof/");
                RandomFoxJson.GetData(content);

                var foxCount = await RandomFoxHtml.GetFoxCount();

                var embed = new EmbedBuilder {
                    Author = new EmbedAuthorBuilder {
                        Name = "Source", Url = "https://randomfox.ca/"
                    },
                    Title = $"{RandomFoxJson.GetImageNumber()} / {foxCount}",
                    Color = Colors.HexToColor("AC5F25"),
                    ImageUrl = RandomFoxJson.GetImage(),
                    Footer = new EmbedFooterBuilder {
                        Text = "Powered by randomfox.ca"
                    }
                };
                httpClient.Dispose();
                await RespondAsync(embed: embed.Build());
                RandomFoxJson.FoxData = null;
            }
            else {
                var getFoxCount = ExtraFoxo.MoreFoxes.Count;
                var randomFox = new Random().Next(0, getFoxCount);
                var embed = new EmbedBuilder {
                    Title = $"{randomFox} / {getFoxCount}",
                    Color = Colors.HexToColor("AC5F25"),
                    ImageUrl = ExtraFoxo.MoreFoxes[randomFox],
                    Footer = new EmbedFooterBuilder {
                        Text = "Powered by the community"
                    }
                };
                await RespondAsync(embed: embed.Build());
            }
        }
        
        [SlashCommand("neko", "Nekos are cute")]
        public async Task Neko() {
            var embed = new EmbedBuilder {
                Title = "Neko",
                Color = Colors.Random,
                Footer = new EmbedFooterBuilder {
                    Text = "Powered by FluxpointAPI"
                },
                ImageUrl = (await Program.Instance.FluxpointClient!.Sfw.GetNekoAsync()).file
            };
            await RespondAsync(embed: embed.Build());
        }
        
        [SlashCommand("cat", "Cats are fluffy")]
        public async Task Cat() {
            var embed = new EmbedBuilder {
                Title = "Kitty",
                Color = Colors.Random,
                Footer = new EmbedFooterBuilder {
                    Text = "Powered by FluxpointAPI"
                },
                ImageUrl = (await Program.Instance.FluxpointClient!.Animal.GetCatAsync()).file
            };
            await RespondAsync(embed: embed.Build());
        }
        
        [SlashCommand("redpanda", "Red Pandas are adorable")]
        public async Task RedPanda() {
            var getRedPandaCount = ExtraRedPando.MoreRedPandas.Count;
            var randomRedPanda = new Random().Next(0, getRedPandaCount);
            var embed = new EmbedBuilder {
                Title = $"{randomRedPanda} / {getRedPandaCount}",
                Color = Colors.HexToColor("935824"),
                ImageUrl = ExtraRedPando.MoreRedPandas[randomRedPanda],
                Footer = new EmbedFooterBuilder {
                    Text = "Powered by the community"
                }
            };
            await RespondAsync(embed: embed.Build());
        }
        
        [SlashCommand("aiquote", "Lets an inspirobot AI create an inspirational quote with image")]
        public async Task InspiroBot() {
            var httpClient = new HttpClient();
            var content = await httpClient.GetStringAsync("https://inspirobot.me/api?generate=true&oy=vey"); // out puts an image URL link
            if (string.IsNullOrWhiteSpace(content)) {
                httpClient.Dispose();
                await RespondAsync("Failed to get an image.", ephemeral: true);
                return;
            }
            var embed = new EmbedBuilder {
                Author = new EmbedAuthorBuilder {
                    Name = "InspiroBot", Url = "https://inspirobot.me/"
                },
                Title = "Got your image!",
                Color = Colors.Random,
                ImageUrl = content,
                Footer = new EmbedFooterBuilder {
                    Text = "Powered by inspirobot.me"
                }
            };
            httpClient.Dispose();
            await RespondAsync(embed: embed.Build());
        }
    }
}