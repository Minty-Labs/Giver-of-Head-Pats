// using Discord;
// using Discord.Interactions;
// using HeadPats.Managers;
//
// namespace HeadPats.Commands.Preexecution; 
//
// public class RequireGuildAttribute : PreconditionAttribute {
//     private readonly ulong[] _guildIds;
//     
//     public RequireGuildAttribute(params ulong[] guildIds) => _guildIds = guildIds;
//
//     public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo cmdInfo, IServiceProvider services) {
//         if (_guildIds.Contains(context.Guild.Id)) return Task.FromResult(PreconditionResult.FromSuccess());
//         // Program.Instance.GetChannel(context.Interaction.GuildId ?? 0, context.Interaction.ChannelId ?? 0)?.SendMessageAsync("This command is not available in this guild.").DeleteAfter(5);
//         return Task.FromResult(PreconditionResult.FromError("This command is not available in this guild."));
//     }
// }