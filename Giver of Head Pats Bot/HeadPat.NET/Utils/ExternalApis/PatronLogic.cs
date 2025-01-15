using HeadPats.Configuration;
using HeadPats.Managers;
using Patreon.Net;
using Serilog;

namespace HeadPats.Utils.ExternalApis; 

public class PatronLogic {
    public static PatronLogic Instance { get; private set; }
    private PatreonClient PatreonClient { get; set; }
    public List<string>? CutieTier { get; private set; }
    public List<string>? MegaCutieTier { get; private set; }
    public List<string>? AdorableTier { get; private set; }
    public int MemberCount { get; private set; }
    private readonly ILogger Logger = Log.ForContext("SourceContext", "PatreonClient");
    
    private PatronLogic() {
        Instance = this;
        Init();
    }

    public async Task GetPatronInfo(bool reRun = false) {
        // if (reRun && OnBotJoinOrLeave.DoNotRunOnStart) return;
        PatreonClient = new PatreonClient(Config.Base.Api.PatreonClientData.PatreonAccessToken, Config.Base.Api.PatreonClientData.PatreonRefreshToken, Config.Base.Api.PatreonClientData.PatreonClientId);
        
        var campaigns = await PatreonClient.GetCampaignsAsync(Includes.All).ConfigureAwait(false);
        var cId = string.Empty;
        if (string.IsNullOrWhiteSpace(cId)) {
            Logger.Information("Total number of {0} campaigns found.", campaigns.Meta.Pagination.Total);
            await foreach (var c in campaigns) {
                Logger.Information("Campaign, {0} ({1}) has {2} patrons.", c.CreationName , c.Id, c.PatronCount);
                cId = c.Id;
                Config.Base.Api.PatreonClientData.CampaignId = c.Id;
            }
            if (!reRun)
                Config.Save();
        }
        else {
            Logger.Error("No campaigns found.");
            return;
        }
        
        var singleCampaign = await PatreonClient.GetCampaignAsync(cId, Includes.All).ConfigureAwait(false);
        Logger.Information("Campaign {0}: created at {1}, created by {2}", singleCampaign.PledgeUrl, singleCampaign.CreatedAt, singleCampaign.Relationships.Creator.FirstName);
        var tiers = singleCampaign.Relationships.Tiers;
        if (tiers is not null && tiers.Length > 0) {
            foreach (var tier in tiers) 
                Logger.Information("Tier {0}: titled {1}, worth {2} cents, has {3} patrons.", tier.Id, tier.Title, tier.AmountCents, tier.PatronCount);
        }
        
        var members = await PatreonClient.GetCampaignMembersAsync(cId, Includes.All).ConfigureAwait(false);
        var memberId = string.Empty;
        if (string.IsNullOrWhiteSpace(memberId)) {
            if (reRun && MemberCount != members.Meta.Pagination.Total) {
                await DNetToConsole.SendMessageToLoggingChannelAsync("Patron count changed!", line2: $"New count: {members.Meta.Pagination.Total}");
            }
            Logger.Information("Total number of {0} members found.", members.Meta.Pagination.Total);
            MemberCount = members.Meta.Pagination.Total;
            
            await foreach (var member in members) {
                Logger.Information("Member {0}: {1} ({2}) has pledged {3} cents total with status {4}.", member.Id, member.FullName, member.Email, member.LifetimeSupportCents, member.PatronStatus);
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
            Logger.Information("No members found.");
        }
        Logger.Information("Ran PatreonClient successfully.");
    }

    private void Init() {
        CutieTier = [];
        MegaCutieTier = [];
        AdorableTier = [];
    }
}