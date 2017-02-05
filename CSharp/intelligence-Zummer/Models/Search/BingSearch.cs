using System;

namespace Zummer.Models.Search
{
    public class BingSearch
    {
        public string _type { get; set; }
        public Instrumentation instrumentation { get; set; }
        public Webpages webPages { get; set; }
        public Entities entities { get; set; }
        public Images images { get; set; }
        public News news { get; set; }
        public Relatedsearches relatedSearches { get; set; }
        public Rankingresponse rankingResponse { get; set; }
    }

    public class Instrumentation
    {
        public string pingUrlBase { get; set; }
        public string pageLoadPingUrl { get; set; }
    }

    public class Webpages
    {
        public string webSearchUrl { get; set; }
        public string webSearchUrlPingSuffix { get; set; }
        public int totalEstimatedMatches { get; set; }
        public Value[] value { get; set; }
    }

    public class Value
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string urlPingSuffix { get; set; }
        public string displayUrl { get; set; }
        public string snippet { get; set; }
        public Deeplink[] deepLinks { get; set; }
        public DateTime dateLastCrawled { get; set; }
        public string cachedPageUrl { get; set; }
        public string cachedPageUrlPingSuffix { get; set; }
    }

    public class Deeplink
    {
        public string name { get; set; }
        public string url { get; set; }
        public string urlPingSuffix { get; set; }
    }

    public class Entities
    {
        public string readLink { get; set; }
        public string queryScenario { get; set; }
        public Value1[] value { get; set; }
    }

    public class Value1
    {
        public string id { get; set; }
        public string readLink { get; set; }
        public string name { get; set; }
        public Image image { get; set; }
        public string description { get; set; }
        public Descriptionattribution descriptionAttribution { get; set; }
        public string webSearchUrl { get; set; }
        public string webSearchUrlPingSuffix { get; set; }
        public Entitypresentationinfo entityPresentationInfo { get; set; }
    }

    public class Image
    {
        public string name { get; set; }
        public string thumbnailUrl { get; set; }
        public Provider[] provider { get; set; }
        public string hostPageUrl { get; set; }
        public string hostPageUrlPingSuffix { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Provider
    {
        public string _type { get; set; }
        public string url { get; set; }
        public string urlPingSuffix { get; set; }
    }

    public class Descriptionattribution
    {
        public License license { get; set; }
        public string licenseNotice { get; set; }
        public string seeMoreUrl { get; set; }
        public string seeMoreUrlPingSuffix { get; set; }
        public string seeMoreDisplayUrl { get; set; }
    }

    public class License
    {
        public string name { get; set; }
        public string url { get; set; }
        public string urlPingSuffix { get; set; }
    }

    public class Entitypresentationinfo
    {
        public string entityScenario { get; set; }
        public string[] entityTypeHints { get; set; }
        public Formattedfact[] formattedFacts { get; set; }
        public Attribution[] attributions { get; set; }
        public Related[] related { get; set; }
        public string entityTypeDisplayHint { get; set; }
    }

    public class Formattedfact
    {
        public string label { get; set; }
        public Item[] items { get; set; }
    }

    public class Item
    {
        public string text { get; set; }
        public string _type { get; set; }
        public string url { get; set; }
        public string urlPingSuffix { get; set; }
    }

    public class Attribution
    {
        public Provider1 provider { get; set; }
    }

    public class Provider1
    {
        public string name { get; set; }
        public string url { get; set; }
        public string urlPingSuffix { get; set; }
    }

    public class Related
    {
        public string displayName { get; set; }
        public string id { get; set; }
        public string webSearchUrl { get; set; }
        public string webSearchUrlPingSuffix { get; set; }
        public Relationship[] relationships { get; set; }
    }

    public class Relationship
    {
        public Relatedthing relatedThing { get; set; }
        public string description { get; set; }
    }

    public class Relatedthing
    {
        public string id { get; set; }
        public string readLink { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string urlPingSuffix { get; set; }
        public Image1 image { get; set; }
    }

    public class Image1
    {
        public string name { get; set; }
        public string thumbnailUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Images
    {
        public string id { get; set; }
        public string readLink { get; set; }
        public string webSearchUrl { get; set; }
        public string webSearchUrlPingSuffix { get; set; }
        public bool isFamilyFriendly { get; set; }
        public Value2[] value { get; set; }
        public bool displayShoppingSourcesBadges { get; set; }
        public bool displayRecipeSourcesBadges { get; set; }
    }

    public class Value2
    {
        public string name { get; set; }
        public string webSearchUrl { get; set; }
        public string webSearchUrlPingSuffix { get; set; }
        public string thumbnailUrl { get; set; }
        public object datePublished { get; set; }
        public string contentUrl { get; set; }
        public string hostPageUrl { get; set; }
        public string hostPageUrlPingSuffix { get; set; }
        public string contentSize { get; set; }
        public string encodingFormat { get; set; }
        public string hostPageDisplayUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public Thumbnail thumbnail { get; set; }
    }

    public class Thumbnail
    {
        public int width { get; set; }
        public int height { get; set; }
    }

    public class News
    {
        public string id { get; set; }
        public string readLink { get; set; }
        public Value3[] value { get; set; }
    }

    public class Value3
    {
        public string name { get; set; }
        public string url { get; set; }
        public string urlPingSuffix { get; set; }
        public Image2 image { get; set; }
        public string description { get; set; }
        public About[] about { get; set; }
        public Provider2[] provider { get; set; }
        public DateTime datePublished { get; set; }
        public string category { get; set; }
        public Clusteredarticle[] clusteredArticles { get; set; }
    }

    public class Image2
    {
        public string contentUrl { get; set; }
        public Thumbnail1 thumbnail { get; set; }
    }

    public class Thumbnail1
    {
        public string contentUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class About
    {
        public string readLink { get; set; }
        public string name { get; set; }
    }

    public class Provider2
    {
        public string _type { get; set; }
        public string name { get; set; }
    }

    public class Clusteredarticle
    {
        public string name { get; set; }
        public string url { get; set; }
        public string urlPingSuffix { get; set; }
        public string description { get; set; }
        public About1[] about { get; set; }
        public Provider3[] provider { get; set; }
        public DateTime datePublished { get; set; }
        public string category { get; set; }
    }

    public class About1
    {
        public string readLink { get; set; }
        public string name { get; set; }
    }

    public class Provider3
    {
        public string _type { get; set; }
        public string name { get; set; }
    }

    public class Relatedsearches
    {
        public string id { get; set; }
        public Value4[] value { get; set; }
    }

    public class Value4
    {
        public string text { get; set; }
        public string displayText { get; set; }
        public string webSearchUrl { get; set; }
        public string webSearchUrlPingSuffix { get; set; }
    }

    public class Rankingresponse
    {
        public Mainline mainline { get; set; }
        public Sidebar sidebar { get; set; }
    }

    public class Mainline
    {
        public Item1[] items { get; set; }
    }

    public class Item1
    {
        public string answerType { get; set; }
        public int resultIndex { get; set; }
        public Value5 value { get; set; }
    }

    public class Value5
    {
        public string id { get; set; }
    }

    public class Sidebar
    {
        public Item2[] items { get; set; }
    }

    public class Item2
    {
        public string answerType { get; set; }
        public Value6 value { get; set; }
        public int resultIndex { get; set; }
    }

    public class Value6
    {
        public string id { get; set; }
    }
}