﻿// using Discord;
// using HeadPats.Configuration;
// using HeadPats.Utils;
//
// namespace HeadPats.Managers.Loops; 
//
// public static class IrlQuotesLoop {
//     private static List<string> _quotes = new() { 
//         "Tired? Try sleeping.", 
//         "Fire is hot, avoid touching it.", 
//         "Remember to eat and drink, its important.", 
//         "If the door doesn't open when you push it, try pulling it!", 
//         "Money can be exchanged for goods and services.", 
//         "Thirsty? Try drinking water.", 
//         "Don't drink orange juice after you brush your teeth.", 
//         "When in doubt, flip the USB over at least twice before trying again.", 
//         "No matter who it is, they are a person too, treat them nicely.", 
//         "When you are not sure, ask Google.", 
//         "If you find the game too hard, try lowering the difficulty.", 
//         "A fart cannot always be trusted, be careful when dealing with farts.", 
//         "Praise the sun! \\o/", 
//         "Sanding wood elves, now 20%.... oh wait... wrong game.", 
//         "First to hit gets the xp and the loot.", 
//         "Tables at restaurants can be looted for gold.", 
//         "If its too dark you can adjust the exposure in graphics settings to lighten things up a bit.", 
//         "Wolves will chase you forever, like you have a bratwurst in your pocket... Or are you just happy to see me?", 
//         "Vaccines provide a buff against known diseases.", 
//         "Always say please and thank you.", 
//         "Thanks for listening to my TED Talk.", 
//         "Downloading more RAM...", 
//         "Updating Updater...", 
//         "Downloading Downloader...", 
//         "Debugging Debugger...", 
//         "Filtering Filters...", 
//         "How did I get here?", 
//         "Warning: Don't set yourself on fire.", 
//         "Follow the white rabbit", 
//         "People respond differently to you if you are naked and/or have a weapon drawn. Bear in mind when developing relationships.", 
//         "Little toe's hitbox is bigger than the model, beware.", 
//         "There are no checkpoints, so be careful.", 
//         "The 'Charisma' statistic may not seem to affect gameplay mechanics outside of dialogue, but in fact informs overall difficulty settings.", 
//         "sv_permadeath 1", 
//         "Objects in mirror are closer than they appear.", 
//         "Regularly drinking water replenishes most stats over time.", 
//         "Regularly drinking alcohol reduces resistance over time.", 
//         "Chose your starting class carefully because each gets vastly different gear at the end of the tutorial.", 
//         "IRS stands for Insane Running Speed", 
//         "Toasters are not a good substitute for bath bombs.", 
//         "If your character is not physically appealing to NPC'S, try investing points in the humour skill tree.", 
//         "You may want to avoid the yellow snow.", 
//         "Ensure finding a companion while still low level, as it gets harder as you level up.", 
//         "Your 18 year tutorial has ended.", 
//         "Improve your intelligence stats by using your phone less during class.", 
//         "Answer carefully when talking to NPCs or other players. They could be on 1HP and really need support.", 
//         "Be careful when starting conversations. They cannot be skipped.", 
//         "Don't like your spouse? You can cancel your marriage in exchange for half of your inventory.", 
//         "Don't forget to shower. People will react differently to you if you don't.", 
//         "Looking for a fight, your local Waffle House may be the place to go.", 
//         "You can lift up slightly when weighing your produce to pay less at the self-checkout.", 
//         "Lesbians have 99% resistance to common disease when romancing other lesbians.", 
//         "Certain types of bumper stickers may indicate what vehicles can be looted.", 
//         "Remember to touch grass every now and then.", 
//         "Tattoos may indicate hostile NPCs.", 
//         "Plastic bags are not to be used as parachutes."
//     };
//     
//     public static void SendQuote(long currentEpoch, Random random) {
//         if (Vars.IsDebug) return;
//         var updated = false;
//         var quote = _quotes[random.Next(0, _quotes.Count)];
//
//         foreach (var guildParam in Config.Base.GuildSettings!) {
//             // if (guildParam.IrlQuotes is null) continue;
//             if (!guildParam.IrlQuotes.Enabled) continue;
//             if (guildParam.IrlQuotes.ChannelId is 0) continue;
//             if (guildParam.IrlQuotes.SetEpochTime > currentEpoch)
//                 continue;
//
//             var embed = new EmbedBuilder {
//                 Description = quote,
//                 Color = Colors.Random
//             }.Build();
//             
//             Program.Instance.GetChannel(guildParam.GuildId, guildParam.IrlQuotes.ChannelId)!.SendMessageAsync(embed: embed);
//             
//             guildParam.IrlQuotes.SetEpochTime += 86400; // 24 hours
//             updated = true;
//         }
//         
//         if (updated) {
//             Config.Save();
//         }
//     }
// }