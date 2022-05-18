using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;

namespace HeadPats.Managers; 

public class HelpFormatter : DefaultHelpFormatter {
    public HelpFormatter(CommandContext ctx) : base(ctx) { }

    public override CommandHelpMessage Build() {
        EmbedBuilder.Color = DiscordColor.SpringGreen;
        var f = new DiscordEmbedBuilder.EmbedFooter {
            Text = $"{BuildInfo.Name} (v{BuildInfo.Version}) • {BuildInfo.BuildDate}"
        };
        EmbedBuilder.Footer = f;

        return base.Build();
    }
}