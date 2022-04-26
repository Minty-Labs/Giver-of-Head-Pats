using Newtonsoft.Json;

namespace HeadPats.Utils;

public static class TheData {
    public static Welcome4[]? RedditData;

    public static void GetData(string data) => RedditData = JsonConvert.DeserializeObject<Welcome4[]>(data);

    public static string? GetImageUrl() => RedditData?[0].Data.Children[0].Data.Url.ToString();
    public static string? GetTitle() => RedditData?[0].Data.Children[0].Data.Title;
    public static string? GetPostUrl() => RedditData?[0].Data.Children[0].Data.Permalink.ToString();
    public static bool IsNsfw() {
        var dataOver18 = RedditData?[0].Data.Children[0].Data.Over18;
        return dataOver18 != null && (bool)dataOver18;
    }

    public static string[] MemeSubreddits = { "memes", "me_irl", "dankmemes", "comedyheaven", "historymemes", "memes", "me_irl", "comedyheaven", "historymemes" };
}

public partial class Welcome4
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("data")]
        public Welcome4Data Data { get; set; }
    }

    public partial class Welcome4Data
    {
        [JsonProperty("after")]
        public object After { get; set; }

        [JsonProperty("dist")]
        public long? Dist { get; set; }

        [JsonProperty("modhash")]
        public string Modhash { get; set; }

        [JsonProperty("geo_filter")]
        public string GeoFilter { get; set; }

        [JsonProperty("children")]
        public Child[] Children { get; set; }

        [JsonProperty("before")]
        public object Before { get; set; }
    }

    public partial class Child
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("data")]
        public ChildData Data { get; set; }
    }

    public partial class ChildData
    {
        [JsonProperty("approved_at_utc")]
        public object ApprovedAtUtc { get; set; }

        [JsonProperty("subreddit")]
        public string Subreddit { get; set; }

        [JsonProperty("selftext", NullValueHandling = NullValueHandling.Ignore)]
        public string Selftext { get; set; }

        [JsonProperty("user_reports")]
        public object[] UserReports { get; set; }

        [JsonProperty("saved")]
        public bool Saved { get; set; }

        [JsonProperty("mod_reason_title")]
        public object ModReasonTitle { get; set; }

        [JsonProperty("gilded")]
        public long Gilded { get; set; }

        [JsonProperty("clicked", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Clicked { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("link_flair_richtext", NullValueHandling = NullValueHandling.Ignore)]
        public object[] LinkFlairRichtext { get; set; }

        [JsonProperty("subreddit_name_prefixed")]
        public string SubredditNamePrefixed { get; set; }

        [JsonProperty("hidden", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Hidden { get; set; }

        [JsonProperty("pwls", NullValueHandling = NullValueHandling.Ignore)]
        public long? Pwls { get; set; }

        [JsonProperty("link_flair_css_class")]
        public object LinkFlairCssClass { get; set; }

        [JsonProperty("downs")]
        public long Downs { get; set; }

        [JsonProperty("thumbnail_height", NullValueHandling = NullValueHandling.Ignore)]
        public long? ThumbnailHeight { get; set; }

        [JsonProperty("top_awarded_type")]
        public object TopAwardedType { get; set; }

        [JsonProperty("parent_whitelist_status", NullValueHandling = NullValueHandling.Ignore)]
        public string ParentWhitelistStatus { get; set; }

        [JsonProperty("hide_score", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HideScore { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("quarantine", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Quarantine { get; set; }

        [JsonProperty("link_flair_text_color", NullValueHandling = NullValueHandling.Ignore)]
        public string LinkFlairTextColor { get; set; }

        [JsonProperty("upvote_ratio", NullValueHandling = NullValueHandling.Ignore)]
        public double? UpvoteRatio { get; set; }

        [JsonProperty("author_flair_background_color")]
        public string AuthorFlairBackgroundColor { get; set; }

        [JsonProperty("subreddit_type")]
        public string SubredditType { get; set; }

        [JsonProperty("ups")]
        public long Ups { get; set; }

        [JsonProperty("total_awards_received")]
        public long TotalAwardsReceived { get; set; }

        [JsonProperty("media_embed", NullValueHandling = NullValueHandling.Ignore)]
        public Gildings MediaEmbed { get; set; }

        [JsonProperty("thumbnail_width", NullValueHandling = NullValueHandling.Ignore)]
        public long? ThumbnailWidth { get; set; }

        [JsonProperty("author_flair_template_id")]
        public object AuthorFlairTemplateId { get; set; }

        [JsonProperty("is_original_content", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsOriginalContent { get; set; }

        [JsonProperty("author_fullname")]
        public string AuthorFullname { get; set; }

        [JsonProperty("secure_media")]
        public object SecureMedia { get; set; }

        [JsonProperty("is_reddit_media_domain", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsRedditMediaDomain { get; set; }

        [JsonProperty("is_meta", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsMeta { get; set; }

        [JsonProperty("category")]
        public object Category { get; set; }

        [JsonProperty("secure_media_embed", NullValueHandling = NullValueHandling.Ignore)]
        public Gildings SecureMediaEmbed { get; set; }

        [JsonProperty("link_flair_text")]
        public object LinkFlairText { get; set; }

        [JsonProperty("can_mod_post")]
        public bool CanModPost { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("approved_by")]
        public object ApprovedBy { get; set; }

        [JsonProperty("is_created_from_ads_ui", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsCreatedFromAdsUi { get; set; }

        [JsonProperty("author_premium")]
        public bool AuthorPremium { get; set; }

        [JsonProperty("thumbnail", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Thumbnail { get; set; }

        [JsonProperty("edited")]
        public bool Edited { get; set; }

        [JsonProperty("author_flair_css_class")]
        public string AuthorFlairCssClass { get; set; }

        [JsonProperty("author_flair_richtext")]
        public object[] AuthorFlairRichtext { get; set; }

        [JsonProperty("gildings")]
        public Gildings Gildings { get; set; }

        [JsonProperty("post_hint", NullValueHandling = NullValueHandling.Ignore)]
        public string PostHint { get; set; }

        [JsonProperty("content_categories")]
        public object ContentCategories { get; set; }

        [JsonProperty("is_self", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsSelf { get; set; }

        [JsonProperty("mod_note")]
        public object ModNote { get; set; }

        [JsonProperty("created")]
        public double Created { get; set; }

        [JsonProperty("link_flair_type", NullValueHandling = NullValueHandling.Ignore)]
        public string LinkFlairType { get; set; }

        [JsonProperty("wls", NullValueHandling = NullValueHandling.Ignore)]
        public long? Wls { get; set; }

        [JsonProperty("removed_by_category")]
        public object RemovedByCategory { get; set; }

        [JsonProperty("banned_by")]
        public object BannedBy { get; set; }

        [JsonProperty("author_flair_type")]
        public string AuthorFlairType { get; set; }

        [JsonProperty("domain", NullValueHandling = NullValueHandling.Ignore)]
        public string Domain { get; set; }

        [JsonProperty("allow_live_comments", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AllowLiveComments { get; set; }

        [JsonProperty("selftext_html")]
        public object SelftextHtml { get; set; }

        [JsonProperty("likes")]
        public object Likes { get; set; }

        [JsonProperty("suggested_sort")]
        public object SuggestedSort { get; set; }

        [JsonProperty("banned_at_utc")]
        public object BannedAtUtc { get; set; }

        [JsonProperty("url_overridden_by_dest", NullValueHandling = NullValueHandling.Ignore)]
        public Uri UrlOverriddenByDest { get; set; }

        [JsonProperty("view_count")]
        public object ViewCount { get; set; }

        [JsonProperty("archived")]
        public bool Archived { get; set; }

        [JsonProperty("no_follow")]
        public bool NoFollow { get; set; }

        [JsonProperty("is_crosspostable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsCrosspostable { get; set; }

        [JsonProperty("pinned", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Pinned { get; set; }

        [JsonProperty("over_18", NullValueHandling = NullValueHandling.Ignore)]
        public bool Over18 { get; set; }

        [JsonProperty("preview", NullValueHandling = NullValueHandling.Ignore)]
        public Preview Preview { get; set; }

        [JsonProperty("all_awardings")]
        public object[] AllAwardings { get; set; }

        [JsonProperty("awarders")]
        public object[] Awarders { get; set; }

        [JsonProperty("media_only", NullValueHandling = NullValueHandling.Ignore)]
        public bool? MediaOnly { get; set; }

        [JsonProperty("can_gild")]
        public bool CanGild { get; set; }

        [JsonProperty("spoiler", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Spoiler { get; set; }

        [JsonProperty("locked")]
        public bool Locked { get; set; }

        [JsonProperty("author_flair_text")]
        public string AuthorFlairText { get; set; }

        [JsonProperty("treatment_tags")]
        public object[] TreatmentTags { get; set; }

        [JsonProperty("visited", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Visited { get; set; }

        [JsonProperty("removed_by")]
        public object RemovedBy { get; set; }

        [JsonProperty("num_reports")]
        public object NumReports { get; set; }

        [JsonProperty("distinguished")]
        public object Distinguished { get; set; }

        [JsonProperty("subreddit_id")]
        public string SubredditId { get; set; }

        [JsonProperty("author_is_blocked")]
        public bool AuthorIsBlocked { get; set; }

        [JsonProperty("mod_reason_by")]
        public object ModReasonBy { get; set; }

        [JsonProperty("removal_reason")]
        public object RemovalReason { get; set; }

        [JsonProperty("link_flair_background_color", NullValueHandling = NullValueHandling.Ignore)]
        public string LinkFlairBackgroundColor { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("is_robot_indexable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsRobotIndexable { get; set; }

        [JsonProperty("num_duplicates", NullValueHandling = NullValueHandling.Ignore)]
        public long? NumDuplicates { get; set; }

        [JsonProperty("report_reasons")]
        public object ReportReasons { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("discussion_type")]
        public object DiscussionType { get; set; }

        [JsonProperty("num_comments", NullValueHandling = NullValueHandling.Ignore)]
        public long? NumComments { get; set; }

        [JsonProperty("send_replies")]
        public bool SendReplies { get; set; }

        [JsonProperty("media")]
        public object Media { get; set; }

        [JsonProperty("contest_mode", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ContestMode { get; set; }

        [JsonProperty("author_patreon_flair")]
        public bool AuthorPatreonFlair { get; set; }

        [JsonProperty("author_flair_text_color")]
        public string AuthorFlairTextColor { get; set; }

        [JsonProperty("permalink", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Permalink { get; set; }

        [JsonProperty("whitelist_status", NullValueHandling = NullValueHandling.Ignore)]
        public string WhitelistStatus { get; set; }

        [JsonProperty("stickied")]
        public bool Stickied { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Url { get; set; }

        [JsonProperty("subreddit_subscribers", NullValueHandling = NullValueHandling.Ignore)]
        public long? SubredditSubscribers { get; set; }

        [JsonProperty("created_utc")]
        public double CreatedUtc { get; set; }

        [JsonProperty("num_crossposts", NullValueHandling = NullValueHandling.Ignore)]
        public long? NumCrossposts { get; set; }

        [JsonProperty("mod_reports")]
        public object[] ModReports { get; set; }

        [JsonProperty("is_video", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsVideo { get; set; }

        [JsonProperty("comment_type")]
        public object CommentType { get; set; }

        [JsonProperty("replies", NullValueHandling = NullValueHandling.Ignore)]
        public string Replies { get; set; }

        [JsonProperty("collapsed_reason_code")]
        public object CollapsedReasonCode { get; set; }

        [JsonProperty("parent_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ParentId { get; set; }

        [JsonProperty("collapsed", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Collapsed { get; set; }

        [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
        public string Body { get; set; }

        [JsonProperty("is_submitter", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsSubmitter { get; set; }

        [JsonProperty("body_html", NullValueHandling = NullValueHandling.Ignore)]
        public string BodyHtml { get; set; }

        [JsonProperty("collapsed_reason")]
        public object CollapsedReason { get; set; }

        [JsonProperty("associated_award")]
        public object AssociatedAward { get; set; }

        [JsonProperty("unrepliable_reason")]
        public object UnrepliableReason { get; set; }

        [JsonProperty("score_hidden", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ScoreHidden { get; set; }

        [JsonProperty("link_id", NullValueHandling = NullValueHandling.Ignore)]
        public string LinkId { get; set; }

        [JsonProperty("controversiality", NullValueHandling = NullValueHandling.Ignore)]
        public long? Controversiality { get; set; }

        [JsonProperty("depth", NullValueHandling = NullValueHandling.Ignore)]
        public long? Depth { get; set; }

        [JsonProperty("collapsed_because_crowd_control")]
        public object CollapsedBecauseCrowdControl { get; set; }
    }

    public partial class Gildings
    {
    }

    public partial class Preview
    {
        [JsonProperty("images")]
        public Image[] Images { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
    }

    public partial class Image
    {
        [JsonProperty("source")]
        public Source Source { get; set; }

        [JsonProperty("resolutions")]
        public Source[] Resolutions { get; set; }

        [JsonProperty("variants")]
        public Gildings Variants { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public partial class Source
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }
    }