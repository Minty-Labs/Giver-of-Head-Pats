using HeadPats.Configuration;
using HeadPats.Events;
using HeadPats.Managers;
using Patreon.Net;
using Serilog;

namespace HeadPats.Utils.ExternalApis; 

public class Patreon_Client {
    private PatreonClient PatreonClient { get; set; }
    public List<string>? CutieTier { get; set; }
    public List<string>? MegaCutieTier { get; set; }
    public List<string>? AdorableTier { get; set; }
    private int MemberCount { get; set; }

    public async Task GetPatreonInfo(bool reRun = false) {
        if (reRun && OnBotJoinOrLeave.DoNotRunOnStart) return;
        PatreonClient = new PatreonClient(Config.Base.Api.PatreonClientData.PatreonAccessToken, Config.Base.Api.PatreonClientData.PatreonRefreshToken, Config.Base.Api.PatreonClientData.PatreonClientId);
        
        var campaigns = await PatreonClient.GetCampaignsAsync(Includes.All).ConfigureAwait(false);
        var cId = string.Empty;
        if (string.IsNullOrWhiteSpace(cId)) {
            Log.Information("[{0}] Total number of {1} campaigns found.", "Patreon Client", campaigns.Meta.Pagination.Total);
            await foreach (var c in campaigns) {
                Log.Information("[{0}] Campaign, {1} ({2}) has {3} patrons.", "Patreon Client", c.CreationName , c.Id, c.PatronCount);
                cId = c.Id;
                Config.Base.Api.PatreonClientData.CampaignId = c.Id;
            }
            if (!reRun)
                Config.Save();
        }
        else {
            Log.Error("[{0}] No campaigns found.", "Patreon Client");
            return;
        }
        
        var singleCampaign = await PatreonClient.GetCampaignAsync(cId, Includes.All).ConfigureAwait(false);
        Log.Information("[{0}] Campaign {1}: created at {2}, created by {3}", "Patreon Client", singleCampaign.PledgeUrl, singleCampaign.CreatedAt, singleCampaign.Relationships.Creator.FirstName);
        var tiers = singleCampaign.Relationships.Tiers;
        if (tiers is not null && tiers.Length > 0) {
            foreach (var tier in tiers) 
                Log.Information("[{0}] Tier {1}: titled {2}, worth {3} cents, has {4} patrons.", "Patreon Client", tier.Id, tier.Title, tier.AmountCents, tier.PatronCount);
        }
        
        var members = await PatreonClient.GetCampaignMembersAsync(cId, Includes.All).ConfigureAwait(false);
        var memberId = string.Empty;
        if (string.IsNullOrWhiteSpace(memberId)) {
            if (reRun && MemberCount != members.Meta.Pagination.Total) {
                await DNetToConsole.SendMessageToLoggingChannelAsync($"Patron count changed! New count: {members.Meta.Pagination.Total}");
            }
            Log.Information("[{0}] Total number of {1} members found.", "Patreon Client", members.Meta.Pagination.Total);
            MemberCount = members.Meta.Pagination.Total;
            
            await foreach (var member in members) {
                Log.Information("[{0}] Member {1}: {2} ({3}) has pledged {4} cents total with status {5}.", "Patreon Client", member.Id, member.FullName, member.Email, member.LifetimeSupportCents, member.PatronStatus);
                memberId = member.Id;
                var tier = member.Relationships.Tiers.FirstOrDefault(t => t.Id.Equals(member.Id));
                switch (tier!.Title.ToLower()) {
                    case "cutie":
                        if (CutieTier is null)
                            CutieTier.Add(member.Relationships.User.FirstName);
                        else if (!CutieTier.Contains(member.Relationships.User.FirstName))
                            CutieTier.Add(member.Relationships.User.FirstName);
                        break;
                    case "mega cutie":
                        if (MegaCutieTier is null)
                            MegaCutieTier.Add(member.Relationships.User.FirstName);
                        else if (!MegaCutieTier.Contains(member.Relationships.User.FirstName))
                            MegaCutieTier.Add(member.Relationships.User.FirstName);
                        break;
                    case "adorable":
                        if (AdorableTier is null)
                            AdorableTier.Add(member.Relationships.User.FirstName);
                        else if (!AdorableTier.Contains(member.Relationships.User.FirstName))
                            AdorableTier.Add(member.Relationships.User.FirstName);
                        break;
                    default: throw new Exception("Invalid tier.");
                }
            }
        }
        else {
            Log.Information("[{0}] No members found.", "Patreon Client");
        }
    }
}