using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HeadPats.Configuration;
using HeadPats.Handlers;
using HeadPats.Handlers.Events;
using HeadPats.Utils;

namespace HeadPats.Commands.Legacy.Commission.Banger; 

public class BangerAdmin : BaseCommandModule {

    [Command("ToggleBanger"), Description("Toggles the banger feature on or off"), InPennysServerAdmin]
    public async Task ToggleBanger(CommandContext ctx, [Description("(Optional) Set Bool")] string? boolSet) {
        var currentValue = Config.Base.Banger.Enabled;
        var newSet = string.IsNullOrWhiteSpace(boolSet) ? !currentValue : boolSet.AsBool();
        Config.Save();
        await ctx.RespondAsync($"Bangers are now **{(newSet ? "enabled" : "disabled")}**.");
    }

    [Command("SetBangerChannel"), Aliases("BangerChannel", "sbc"), Description("Sets the channel to only accept bangers."), InPennysServerAdmin]
    public async Task SetBangerChannel(CommandContext ctx, [Description("Destination Discord Channel (mention)")] DiscordChannel? channel) {
        channel ??= ctx.Channel;
        if (Config.Base.Banger.GuildId == 0)
            Config.Base.Banger.GuildId = ctx.Guild.Id;
        Config.Base.Banger.ChannelId = channel.Id;
        Config.Save();
        await ctx.RespondAsync($"Set Banger channel to {channel.Mention}.");
    }

    [Command("ChangeBangerUrlErrorMessage"), Aliases("changeurlerror", "cue", "cuem", "cbue", "cbuem"), Description("Changes the error message for when a non-whitelisted URL is posted."), InPennysServerAdmin]
    public static async Task ChangeBangerUrlErrorMessage(CommandContext ctx, [Description("Admin defined error message"), RemainingText] string? text) {
        var newText = string.IsNullOrWhiteSpace(text) ? "This URL is not whitelisted." : text;
        Config.Base.Banger.UrlErrorResponseMessage = newText;
        Config.Save();
        await ctx.RespondAsync($"Set Banger URL Error Message to: {newText}");
    }
    
    [Command("ChangeBangerExtErrorMessage"), Aliases("changeexterror", "cee", "ceem", "cbee", "cbeem"), Description("Changes the error message for when a non-whitelisted file extension is posted."), InPennysServerAdmin]
    public static async Task ChangeBangerExtErrorMessage(CommandContext ctx, [Description("Admin defined error message"), RemainingText] string? text) {
        var newText = string.IsNullOrWhiteSpace(text) ? "This file extension is not whitelisted." : text;
        Config.Base.Banger.FileErrorResponseMessage = newText;
        Config.Save();
        await ctx.RespondAsync($"Set Banger File Extension Error Message to: {newText}");
    }
    
    private static bool _doesItExist(string value, IEnumerable<string> list) => list.Any(x => x.Equals(value, StringComparison.OrdinalIgnoreCase)); 

    [Command("SetBangerValue"), Description("Sets various values to the banger system (SetBangerValue help)"), InPennysServerAdmin]
    public async Task SetBangerValue(CommandContext ctx, 
        [Description("Type (help | url | ext)")] string? arg1,
        [Description("Action (list | add | remove)")] string? arg2,
        [Description("Value ( \"(*.com|*.net|...etc)\" | \"(mp3|wav|...etc)\" )")] string? arg3)
    {
        // arg2 will have type [ url | ext ]
        if (string.IsNullOrWhiteSpace(arg1) || arg1.ToLower().Equals("help")) goto help;
        var isUrl = arg1.ToLower().Equals("url");
        var isExt = arg1.ToLower().Equals("ext");
        
        // arg3 will have action [ list | add | remove ]
        if (string.IsNullOrWhiteSpace(arg2)) goto help;
        var fArg3 = arg2.ToLower();
        
        // arg3 will have value [ (*.com|*.net|...etc) | (mp3|wav|...etc) ]
        if (string.IsNullOrWhiteSpace(arg3)) goto help;
        var isInList = isUrl ? _doesItExist(arg3.ToLower(), BangerEventListener.WhitelistedUrls!) : _doesItExist(arg3.ToLower(), BangerEventListener.WhitelistedFileExtensions!);
        switch (fArg3) {
            case "add": {
                if (isInList) {
                    await ctx.RespondAsync($"`{arg3}` is already in the list.");
                    return;
                }
                
                if (isUrl) {
                    BangerEventListener.WhitelistedUrls!.Add(arg3);
                    Config.Base.Banger.WhitelistedUrls!.Add(arg3);
                    await ctx.RespondAsync($"Added `{arg3}` to the list.");
                    Config.Save();
                    return;
                }
                // if (isExt)
                BangerEventListener.WhitelistedFileExtensions!.Add(arg3);
                Config.Base.Banger.WhitelistedFileExtensions!.Add(arg3);
                await ctx.RespondAsync($"Added `{arg3}` to the list.");
                Config.Save();
                return;
            }
            case "remove": {
                if (!isInList) {
                    await ctx.RespondAsync($"`{arg3}` is not in the list.");
                    return;
                }
                
                if (isUrl) {
                    BangerEventListener.WhitelistedUrls!.Remove(arg3);
                    Config.Base.Banger.WhitelistedUrls!.Remove(arg3);
                    await ctx.RespondAsync($"Removed `{arg3}` from the list.");
                    Config.Save();
                    return;
                }
                // if (isExt)
                BangerEventListener.WhitelistedFileExtensions!.Remove(arg3);
                Config.Base.Banger.WhitelistedFileExtensions!.Remove(arg3);
                await ctx.RespondAsync($"Removed `{arg3}` from the list.");
                Config.Save();
                return;
            }
            case "list": {
                if (isUrl) {
                    await ctx.RespondAsync($"Whitelisted URLs: {string.Join(", ", BangerEventListener.WhitelistedUrls!)}");
                    return;
                }
                if (isExt) {
                    await ctx.RespondAsync($"Whitelisted File Extensions: {string.Join(", ", BangerEventListener.WhitelistedFileExtensions!)}");
                }
                return;
            }
            default: goto help;
        }
        
        help:
        await ctx.RespondAsync("Expected format: `hp!SetBangerValue <type> <action> <value>`\nType: `url` or `etx`\nAction: `list` or `add` or `remove`\nValue: `domain.com` or file extension `mp3` or `wav` etc.\n" +
                               "*Note: Types and actions are exact text, the example above must be those options.*");
    }
}