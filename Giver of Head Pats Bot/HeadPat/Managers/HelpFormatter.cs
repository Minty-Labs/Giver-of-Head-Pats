using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;

namespace HeadPats.Managers; 

public class HelpFormatter : DefaultHelpFormatter {
    public HelpFormatter(CommandContext ctx) : base(ctx) { }

    public override CommandHelpMessage Build() {
        EmbedBuilder.Color = DiscordColor.SpringGreen;
        EmbedBuilder.Footer = new DiscordEmbedBuilder.EmbedFooter {
            Text = $"{Vars.Name} (v{Vars.Version}) • {Vars.BuildDate}"
        };
        EmbedBuilder.AddField("Addition Information", 
            "There are also slash commands available for this bot. " +
            $"Don't see them? Keep the bot in your server and [re-invite it with this link]({Vars.InviteLink}) to update it's permissions.");
        return base.Build();
    }
}